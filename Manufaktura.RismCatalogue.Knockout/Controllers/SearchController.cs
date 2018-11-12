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

            var query = (from i in context.Incipits
                         join ms in context.MusicalSources on i.MusicalSourceId equals ms.Id
                         where context.SpatialHashes.Any(sh => sh.IncipitId == i.Id && sh.NumberOfDimensions == numberOfDimensions && (
                         (sh.PlaneGroupNumber == 1 && sh.Hash == queryDictionary[1]) ||
                         (sh.PlaneGroupNumber == 2 && sh.Hash == queryDictionary[2]) ||
                         (sh.PlaneGroupNumber == 3 && sh.Hash == queryDictionary[3]) ||
                         (sh.PlaneGroupNumber == 4 && sh.Hash == queryDictionary[4]) ||
                         (sh.PlaneGroupNumber == 5 && sh.Hash == queryDictionary[5]) ||
                         (sh.PlaneGroupNumber == 6 && sh.Hash == queryDictionary[6]) ||
                         (sh.PlaneGroupNumber == 7 && sh.Hash == queryDictionary[7]) ||
                         (sh.PlaneGroupNumber == 8 && sh.Hash == queryDictionary[8]) ||
                         (sh.PlaneGroupNumber == 9 && sh.Hash == queryDictionary[9]) ||
                         (sh.PlaneGroupNumber == 10 && sh.Hash == queryDictionary[10])
                         )) //TODO: Create this query dynamically
                         select new SearchResultViewModel
                         {
                             Id = i.Id.ToString(),
                             IncipitSvg = string.IsNullOrWhiteSpace(i.MusicalNotation) ? null : scoreRendererService.RenderScore(plaineAndEasieService.Parse(i)),
                             CaptionOrHeading = i.CaptionOrHeading,
                             TextIncipit = i.TextIncipit,
                             Voice = i.VoiceOrInstrument,
                             Title = ms.Title,
                             ComposerName = ms.ComposerName,
                             Relevance = (double)context.SpatialHashes.Count(sh => sh.IncipitId == i.Id && sh.NumberOfDimensions == numberOfDimensions && (
                                 (sh.PlaneGroupNumber == 1 && sh.Hash == queryDictionary[1]) ||
                                 (sh.PlaneGroupNumber == 2 && sh.Hash == queryDictionary[2]) ||
                                 (sh.PlaneGroupNumber == 3 && sh.Hash == queryDictionary[3]) ||
                                 (sh.PlaneGroupNumber == 4 && sh.Hash == queryDictionary[4]) ||
                                 (sh.PlaneGroupNumber == 5 && sh.Hash == queryDictionary[5]) ||
                                 (sh.PlaneGroupNumber == 6 && sh.Hash == queryDictionary[6]) ||
                                 (sh.PlaneGroupNumber == 7 && sh.Hash == queryDictionary[7]) ||
                                 (sh.PlaneGroupNumber == 8 && sh.Hash == queryDictionary[8]) ||
                                 (sh.PlaneGroupNumber == 9 && sh.Hash == queryDictionary[9]) ||
                                 (sh.PlaneGroupNumber == 10 && sh.Hash == queryDictionary[10])
                                 )) / (double)context.SpatialHashes.Count(sh => sh.IncipitId == i.Id && sh.NumberOfDimensions == numberOfDimensions)
                         }).OrderByDescending(rm => rm.Relevance);
            //var sql = query.ToSql();
            var incipits = query
                .Skip(searchQuery.Skip)
                .Take(searchQuery.Take)
                .ToArray();

            stopwatch.Stop();
            Debug.WriteLine($"Search finished in {stopwatch.Elapsed}.");

            return new SearchResultsViewModel { Results = incipits, QueryTime = stopwatch.ElapsedMilliseconds };
        }
    }
}