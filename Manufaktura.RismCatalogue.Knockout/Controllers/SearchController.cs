using Manufaktura.RismCatalogue.Knockout.Extensions;
using Manufaktura.RismCatalogue.Model;
using Manufaktura.RismCatalogue.Shared.Algorithms;
using Manufaktura.RismCatalogue.Shared.Services;
using Manufaktura.RismCatalogue.Shared.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Manufaktura.RismCatalogue.Knockout.Controllers
{
    [Route("api/[controller]")]
    public class SearchController
    {
        private readonly RismDbContext context;
        private readonly PlaineAndEasieService plaineAndEasieService;
        private readonly ScoreRendererService scoreRendererService;

        public SearchController([FromServices] RismDbContext context, PlaineAndEasieService plaineAndEasieService, ScoreRendererService scoreRendererService)
        {
            this.context = context;
            this.scoreRendererService = scoreRendererService;
            this.plaineAndEasieService = plaineAndEasieService;
        }

        [HttpPost("[action]")]
        public SearchResultsViewModel Search([FromBody] SearchQuery searchQuery)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            var intervals = searchQuery.Intervals.Take(Constants.MaxNumberOfDimensions).Select(i => (double)i).ToArray();
            var numberOfDimensions = intervals.Length;

            var basicQuery = context.Incipits.AsQueryable();
            if (!string.IsNullOrWhiteSpace(searchQuery.Rhythm))
            {
                if (searchQuery.IsRhythmRelative)
                    basicQuery = basicQuery.Where(i => i.RhythmRelativeDigest.StartsWith(searchQuery.Rhythm));
                else
                    basicQuery = basicQuery.Where(i => i.RhythmDigest.StartsWith(searchQuery.Rhythm));
            }

            IOrderedQueryable<SearchResultViewModel> query;
            if (numberOfDimensions == 0)
            {
                query = (from i in basicQuery
                         join ms in context.MusicalSources on i.MusicalSourceId equals ms.Id
                         select new SearchResultViewModel
                         {
                             Id = i.Id.ToString(),
                             RecordId = ms.Id,
                             IncipitSvg = string.IsNullOrWhiteSpace(i.MusicalNotation) ? null : scoreRendererService.RenderScore(plaineAndEasieService.Parse(i)),
                             CaptionOrHeading = i.CaptionOrHeading,
                             TextIncipit = i.TextIncipit,
                             Voice = i.VoiceOrInstrument,
                             Title = ms.Title,
                             ComposerName = ms.ComposerName,
                             Relevance = 1,
                             ShowRelevance = false
                         }).OrderBy(rm => rm.ComposerName);
            }
            else
            {
                var queryDictionary = new Dictionary<int, int>();
                for (var i = 1; i < 11; i++)
                {
                    var planes = context.Planes.Where(p => p.GroupNumber == i && p.NumberOfDimensions == numberOfDimensions).ToArray();
                    var lshAlgorithm = new LSHAlgorithm(planes.Select(p => new Vector<double>(new double[] {
                    p.Coordinate1, p.Coordinate2, p.Coordinate3, p.Coordinate4,
                    p.Coordinate5, p.Coordinate6, p.Coordinate7, p.Coordinate8,
                    p.Coordinate9, p.Coordinate10, p.Coordinate11, p.Coordinate12}.Take(numberOfDimensions))).ToArray());
                    queryDictionary.Add(i, lshAlgorithm.ComputeHash(new Vector<double>(intervals)));
                }

                query = (from i in basicQuery
                         join ms in context.MusicalSources on i.MusicalSourceId equals ms.Id
                         join sh in (from s in context.SpatialHashes where s.NumberOfDimensions == numberOfDimensions && (
                         (s.PlaneGroupNumber == 1 && s.Hash == queryDictionary[1]) ||
                         (s.PlaneGroupNumber == 2 && s.Hash == queryDictionary[2]) ||
                         (s.PlaneGroupNumber == 3 && s.Hash == queryDictionary[3]) ||
                         (s.PlaneGroupNumber == 4 && s.Hash == queryDictionary[4]) ||
                         (s.PlaneGroupNumber == 5 && s.Hash == queryDictionary[5]) ||
                         (s.PlaneGroupNumber == 6 && s.Hash == queryDictionary[6]) ||
                         (s.PlaneGroupNumber == 7 && s.Hash == queryDictionary[7]) ||
                         (s.PlaneGroupNumber == 8 && s.Hash == queryDictionary[8]) ||
                         (s.PlaneGroupNumber == 9 && s.Hash == queryDictionary[9]) ||
                         (s.PlaneGroupNumber == 10 && s.Hash == queryDictionary[10]))
                                     select new { IncipitId = s.IncipitId }) on i.Id equals sh.IncipitId into shh
                         
                          //TODO: Create this query dynamically
                         select new SearchResultViewModel
                         {
                             Id = i.Id.ToString(),
                             RecordId = ms.Id,
                             IncipitSvg = string.IsNullOrWhiteSpace(i.MusicalNotation) ? null : scoreRendererService.RenderScore(plaineAndEasieService.Parse(i)),
                             CaptionOrHeading = i.CaptionOrHeading,
                             TextIncipit = i.TextIncipit,
                             Voice = i.VoiceOrInstrument,
                             Title = ms.Title,
                             ComposerName = ms.ComposerName,
                             Relevance = (double)shh.Count() / 10,
                             ShowRelevance = true
                         }).OrderByDescending(rm => rm.Relevance);
            }



            var sql = query.ToSql();
            var incipits = query
                .Distinct()
                .Skip(searchQuery.Skip)
                .Take(searchQuery.Take)
                .ToArray();

            stopwatch.Stop();
            Debug.WriteLine($"Search finished in {stopwatch.Elapsed}.");

            return new SearchResultsViewModel { Results = incipits, QueryTime = stopwatch.ElapsedMilliseconds };
        }
    }
}