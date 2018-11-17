using Manufaktura.RismCatalogue.Knockout.Extensions;
using Manufaktura.RismCatalogue.Knockout.Services.Search;
using Manufaktura.RismCatalogue.Model;
using Manufaktura.RismCatalogue.Shared.Algorithms;
using Manufaktura.RismCatalogue.Shared.Services;
using Manufaktura.RismCatalogue.Shared.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

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

        private string GetRelevanceExpression(int[] intervals)
        {
            var sb = new StringBuilder("100 - (SQRT(");
            for (var i = 0; i < intervals.Length; i++)
            {
                if (i != 0) sb.Append(" + ");
                sb.Append($"POW(i.Interval{i + 1} - {intervals[i]}, 2)");
            }
            sb.Append(")) * (100 / 12) as Relevance");
            return sb.ToString();
        }

        private const int hashGroupsToInclude = 3;

        [HttpPost("[action]")]
        public SearchResultsViewModel Search([FromBody] SearchQuery searchQuery)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            if (!searchQuery.Intervals.Any())
            {
                var basicQuery = context.Incipits.AsQueryable();
                if (!string.IsNullOrWhiteSpace(searchQuery.Rhythm))
                {
                    if (searchQuery.IsRhythmRelative)
                        basicQuery = basicQuery.Where(i => i.RhythmRelativeDigest.StartsWith(searchQuery.Rhythm));
                    else
                        basicQuery = basicQuery.Where(i => i.RhythmDigest.StartsWith(searchQuery.Rhythm));
                }
                var query = (from i in basicQuery
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

                var res = query
                    .Skip(searchQuery.Skip)
                    .Take(searchQuery.Take)
                    .ToArray();

                stopwatch.Stop();
                return new SearchResultsViewModel { Results = res, QueryTime = stopwatch.ElapsedMilliseconds };
            }

            var intervals = searchQuery.Intervals.Take(Constants.MaxNumberOfDimensions).ToArray();
            var numberOfDimensions = intervals.Length;

            var queryDictionary = new Dictionary<int, int>();
            for (var groupNumber = 1; groupNumber <= hashGroupsToInclude; groupNumber++)
            {
                var planes = context.Planes.Where(p => p.GroupNumber == groupNumber && p.NumberOfDimensions == numberOfDimensions).ToArray();
                var lshAlgorithm = new LSHAlgorithm(planes.Select(p => new TranslatedVector(new double[] {
                    p.Coordinate1, p.Coordinate2, p.Coordinate3, p.Coordinate4,
                    p.Coordinate5, p.Coordinate6, p.Coordinate7, p.Coordinate8,
                    p.Coordinate9, p.Coordinate10, p.Coordinate11, p.Coordinate12}.Take(numberOfDimensions).ToArray(),
                    new double[] {
                    p.Translation1, p.Translation2, p.Translation3, p.Translation4,
                    p.Translation5, p.Translation6, p.Translation7, p.Translation8,
                    p.Translation9, p.Translation10, p.Translation11, p.Translation12}.Take(numberOfDimensions).ToArray()
                    )).ToArray());
                queryDictionary.Add(groupNumber, lshAlgorithm.ComputeHash(new Vector(intervals.Select(ii => (double)ii).ToArray())));
            }

            var sb = new StringBuilder();
            sb.Append($"SELECT DISTINCT i.{nameof(Incipit.MusicalNotation)}, i.{nameof(Incipit.Clef)}, i.{nameof(Incipit.KeySignature)}, i.{nameof(Incipit.TimeSignature)}, " +
                $"i.{nameof(Incipit.CaptionOrHeading)}, ms.{nameof(MusicalSource.ComposerName)}, ms.{nameof(MusicalSource.Id)}, i.{nameof(Incipit.TextIncipit)}, " +
                $"ms.{nameof(MusicalSource.Title)}, i.{nameof(Incipit.VoiceOrInstrument)}, ");
            sb.Append(GetRelevanceExpression(intervals));
            sb.Append(" from incipits i inner join musicalsources ms on ms.id = i.MusicalSourceId ");
            for (var groupNumber = 1; groupNumber <= hashGroupsToInclude; groupNumber++)
            {
                sb.Append($" left join spatialhashes sh{groupNumber} on sh{groupNumber}.IncipitId = i.Id and sh1.NumberOfDimensions = {intervals.Length} " +
                    $"and sh{groupNumber}.PlaneGroupNumber = {groupNumber}  and sh{groupNumber}.hash = {queryDictionary[groupNumber]}");
            }
            sb.Append(" WHERE ");
            for (var groupNumber = 1; groupNumber <= hashGroupsToInclude; groupNumber++)
            {
                if (groupNumber > 1) sb.Append(" OR ");
                sb.Append($"sh{groupNumber}.Id IS NOT NULL");
            }
            sb.Append($" order by Relevance desc LIMIT {searchQuery.Take} OFFSET {searchQuery.Skip}");

            var sql = sb.ToString();
            var incipits = context.RawSqlQuery(sql);
            var results = incipits.Select(r => new SearchResultViewModel
            {
                IncipitSvg = scoreRendererService.RenderScore(plaineAndEasieService.ParseAndColorMatchingIntervals(r[0] as string, r[1] as string, r[2] as string, r[3] as string, intervals)),
                CaptionOrHeading = r[4] as string,
                ComposerName = r[5] as string,
                RecordId = r[6] as string,
                TextIncipit = r[7] as string,
                Title = r[8] as string,
                Voice = r[9] as string,
                Relevance = (double)r[10],
                ShowRelevance = true
            }).ToArray();

            stopwatch.Stop();

            return new SearchResultsViewModel { Results = results, QueryTime = stopwatch.ElapsedMilliseconds };
        }
    }
}