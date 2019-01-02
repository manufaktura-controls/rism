﻿using Manufaktura.RismCatalogue.Knockout.Extensions;
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
        private const bool isAdminMode = false;

        private readonly RismDbContext context;
        private readonly PlaineAndEasieService plaineAndEasieService;
        private readonly ScoreRendererService scoreRendererService;

        public SearchController([FromServices] RismDbContext context, PlaineAndEasieService plaineAndEasieService, ScoreRendererService scoreRendererService)
        {
            this.context = context;
            this.scoreRendererService = scoreRendererService;
            this.plaineAndEasieService = plaineAndEasieService;
        }

        private string GetRelevanceExpression(int[] intervals)
        {
            if (!intervals.Any()) return "1 as Relevance";

            var sb = new StringBuilder("100 - (SQRT(");
            for (var i = 0; i < intervals.Length; i++)
            {
                if (i != 0) sb.Append(" + ");
                sb.Append($"POW(i.Interval{i + 1} - {intervals[i]}, 2)");
            }
            sb.Append(")) * (100 / 12) as Relevance");
            return sb.ToString();
        }

        private const int hashGroupsToInclude = 1;

        [HttpPost("[action]")]
        public SearchResultsViewModel Search([FromBody] SearchQuery searchQuery)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            var intervals = searchQuery.Intervals.Take(12).ToArray();
            var intervalsForLsh = searchQuery.Intervals.Take(Constants.MaxNumberOfDimensionsForLsh).ToArray();
            var lshQueryDictionary = new Dictionary<int, long>();

            if (searchQuery.UseSpatialHashes && intervals.Any())
            {
                for (var groupNumber = 1; groupNumber <= hashGroupsToInclude; groupNumber++)
                {
                    var planes = context.Planes.Where(p => p.GroupNumber == groupNumber && p.NumberOfDimensions == intervalsForLsh.Length).ToArray();
                    var lshAlgorithm = new LSHAlgorithm(planes.Select(p => new TranslatedVector(new double[] {
                    p.Coordinate1, p.Coordinate2, p.Coordinate3, p.Coordinate4,
                    p.Coordinate5, p.Coordinate6, p.Coordinate7, p.Coordinate8,
                    p.Coordinate9, p.Coordinate10, p.Coordinate11, p.Coordinate12}.Take(intervalsForLsh.Length).ToArray(),
                        new double[] {
                    p.Translation1, p.Translation2, p.Translation3, p.Translation4,
                    p.Translation5, p.Translation6, p.Translation7, p.Translation8,
                    p.Translation9, p.Translation10, p.Translation11, p.Translation12}.Take(intervalsForLsh.Length).ToArray()
                        )).ToArray());
                    lshQueryDictionary.Add(groupNumber, lshAlgorithm.ComputeHash(new Vector(intervalsForLsh.Select(ii => (double)ii).ToArray())));
                }
            }

            var sb = new StringBuilder();
            sb.AppendLine($"SELECT i.{nameof(Incipit.Id)}, i.{nameof(Incipit.MusicalNotation)}, i.{nameof(Incipit.Clef)}, i.{nameof(Incipit.KeySignature)}, i.{nameof(Incipit.TimeSignature)}, " +
                $"i.{nameof(Incipit.CaptionOrHeading)}, ms.{nameof(MusicalSource.ComposerName)}, ms.{nameof(MusicalSource.Id)}, i.{nameof(Incipit.TextIncipit)}, " +
                $"ms.{nameof(MusicalSource.Title)}, i.{nameof(Incipit.VoiceOrInstrument)}, ");
            sb.AppendLine(GetRelevanceExpression(intervals));
            sb.AppendLine(" from incipits i inner join musicalsources ms on ms.id = i.MusicalSourceId ");
            var isWhereBlockStarted = false;
            if (searchQuery.UseSpatialHashes && intervals.Any())
            {
                isWhereBlockStarted = true;
                sb.AppendLine($" WHERE i.Hash{intervalsForLsh.Length}d = {lshQueryDictionary[1]} ");
            }

            var parameters = new List<object>();
            if (!string.IsNullOrWhiteSpace(searchQuery.Rhythm))
            {
                parameters.Add(searchQuery.Rhythm + "%");
                if (searchQuery.IsRhythmRelative)
                    sb.AppendLine($" {(isWhereBlockStarted ? "AND" : "WHERE")} i.RhythmRelativeDigest LIKE @p0 ");
                else
                    sb.AppendLine($" {(isWhereBlockStarted ? "AND" : "WHERE")} i.RhythmDigest LIKE @p0 ");
            }
            if (intervals.Any()) sb.Append($" order by Relevance desc");
            else sb.Append($" order by i.Id asc");

            sb.AppendLine($" LIMIT {searchQuery.Take} OFFSET {searchQuery.Skip}");

            var sql = sb.ToString();
            Debug.WriteLine(sql);
            var incipits = context.RawSqlQuery(sql, parameters.ToArray());
            var results = incipits.Select(r =>
            {
                var score = plaineAndEasieService.ParseAndColorMatchingIntervals(r[1] as string, r[2] as string, r[3] as string, r[4] as string, intervals);
                return new SearchResultViewModel
                {
                    Id = r[0].ToString(),
                    IncipitSvg = score == null ? null : scoreRendererService.RenderScore(score),
                    CaptionOrHeading = r[5] as string,
                    ComposerName = r[6] as string,
                    RecordId = r[7] as string,
                    TextIncipit = r[8] as string,
                    Title = r[9] as string,
                    Voice = r[10] as string,
                    Relevance = r[11] is double ? (double)r[11] : (long)r[11],
                    ShowRelevance = intervals.Any(),
                    PlaineAndEasieCode = isAdminMode ? $"{r[1]};{r[2]};{r[3]};{r[4]}" : ""
                };
            }).ToArray();

            stopwatch.Stop();

            return new SearchResultsViewModel { Results = results, QueryTime = stopwatch.ElapsedMilliseconds };
        }
    }
}