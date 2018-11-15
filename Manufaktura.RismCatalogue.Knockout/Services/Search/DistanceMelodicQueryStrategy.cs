using Manufaktura.RismCatalogue.Model;
using Manufaktura.RismCatalogue.Shared.Services;
using Manufaktura.RismCatalogue.Shared.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Manufaktura.RismCatalogue.Knockout.Services.Search
{
    public class DistanceMelodicQueryStrategy : MelodicQueryStrategy
    {
        public DistanceMelodicQueryStrategy(RismDbContext context, ScoreRendererService scoreRendererService, PlaineAndEasieService plaineAndEasieService)
            : base(context, scoreRendererService, plaineAndEasieService)
        {
        }

        public override MelodicQueryMode Mode => MelodicQueryMode.Distance;

        public override IOrderedQueryable<SearchResultViewModel> GetQuery(IQueryable<Incipit> basicQuery, SearchQuery searchQuery)
        {
            var intervals = searchQuery.Intervals.Take(12).ToArray();

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
                             Relevance = GetRelevance(i, intervals),    //I don't believe if it is going to actually convert to SQL but EF Core works like magic so give it a try :)
                             ShowRelevance = true
                         }).OrderByDescending(rm => rm.Relevance);

            return query;
        }

        private double GetRelevance(Incipit incipit, int[] intervals)
        {
            var sum = 0d;
            for (var i = 0; i < intervals.Length; i++)
            {
                sum += Math.Pow((double)GetInterval(incipit, i) - intervals[i], 2);
            }
            return sum;
        }

        private int GetInterval(Incipit incipit, int index)
        {
            if (index == 0) return incipit.Interval1;
            if (index == 1) return incipit.Interval2;
            if (index == 2) return incipit.Interval3;
            if (index == 3) return incipit.Interval4;
            if (index == 4) return incipit.Interval5;
            if (index == 5) return incipit.Interval6;
            if (index == 6) return incipit.Interval7;
            if (index == 7) return incipit.Interval8;
            if (index == 8) return incipit.Interval9;
            if (index == 9) return incipit.Interval10;
            if (index == 10) return incipit.Interval11;
            if (index == 11) return incipit.Interval12;
            return 0;
        }
    }
}