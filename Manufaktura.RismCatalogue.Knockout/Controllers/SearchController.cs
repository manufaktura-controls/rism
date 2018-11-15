using Manufaktura.RismCatalogue.Knockout.Extensions;
using Manufaktura.RismCatalogue.Knockout.Services.Search;
using Manufaktura.RismCatalogue.Model;
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
        private readonly MelodicQueryStrategy[] melodicQueryStrategies;

        public SearchController([FromServices] RismDbContext context, PlaineAndEasieService plaineAndEasieService, ScoreRendererService scoreRendererService, IEnumerable<MelodicQueryStrategy> melodicQueryStrategies)
        {
            this.context = context;
            this.scoreRendererService = scoreRendererService;
            this.plaineAndEasieService = plaineAndEasieService;
            this.melodicQueryStrategies = melodicQueryStrategies.ToArray();
        }

        [HttpPost("[action]")]
        public SearchResultsViewModel Search([FromBody] SearchQuery searchQuery)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            var basicQuery = context.Incipits.AsQueryable();
            if (!string.IsNullOrWhiteSpace(searchQuery.Rhythm))
            {
                if (searchQuery.IsRhythmRelative)
                    basicQuery = basicQuery.Where(i => i.RhythmRelativeDigest.StartsWith(searchQuery.Rhythm));
                else
                    basicQuery = basicQuery.Where(i => i.RhythmDigest.StartsWith(searchQuery.Rhythm));
            }

            IOrderedQueryable<SearchResultViewModel> query;
            if (!searchQuery.Intervals.Any())
            {
                query = (from i in basicQuery
                         join ms in context.MusicalSources on i.MusicalSourceId equals ms.Id
                         select new SearchResultViewModel
                         {
                             Id = i.Id.ToString(),
                             RecordId = ms.Id,
                             IncipitSvg = string.IsNullOrWhiteSpace(i.MusicalNotation) ? null : scoreRendererService.RenderScore(plaineAndEasieService.ParseAndColorMatchingIntervals(i, searchQuery.Intervals)),
                             CaptionOrHeading = i.CaptionOrHeading,
                             TextIncipit = i.TextIncipit,
                             Voice = i.VoiceOrInstrument,
                             Title = ms.Title,
                             ComposerName = ms.ComposerName,
                             Relevance = 1,
                             ShowRelevance = false
                         }).OrderBy(rm => rm.ComposerName);
            }
            else query = melodicQueryStrategies.FirstOrDefault(mqs => mqs.Mode == searchQuery.Mode).GetQuery(basicQuery, searchQuery);

            //var sql = query.ToSql();
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