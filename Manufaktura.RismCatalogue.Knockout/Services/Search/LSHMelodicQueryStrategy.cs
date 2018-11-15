using Manufaktura.RismCatalogue.Model;
using Manufaktura.RismCatalogue.Shared.Algorithms;
using Manufaktura.RismCatalogue.Shared.Services;
using Manufaktura.RismCatalogue.Shared.ViewModels;
using System.Collections.Generic;
using System.Linq;

namespace Manufaktura.RismCatalogue.Knockout.Services.Search
{
    public class LSHMelodicQueryStrategy : MelodicQueryStrategy
    {
        public LSHMelodicQueryStrategy(RismDbContext context, ScoreRendererService scoreRendererService, PlaineAndEasieService plaineAndEasieService)
            : base(context, scoreRendererService, plaineAndEasieService)
        {
        }

        public override MelodicQueryMode Mode => MelodicQueryMode.LSH;

        public override IOrderedQueryable<SearchResultViewModel> GetQuery(IQueryable<Incipit> basicQuery, SearchQuery searchQuery)
        {
            var intervals = searchQuery.Intervals.Take(Constants.MaxNumberOfDimensions).Select(i => (double)i).ToArray();
            var numberOfDimensions = intervals.Length;

            var queryDictionary = new Dictionary<int, int>();
            for (var i = 1; i < 11; i++)
            {
                var planes = context.Planes.Where(p => p.GroupNumber == i && p.NumberOfDimensions == numberOfDimensions).ToArray();
                var lshAlgorithm = new LSHAlgorithm(planes.Select(p => new TranslatedVector(new double[] {
                    p.Coordinate1, p.Coordinate2, p.Coordinate3, p.Coordinate4,
                    p.Coordinate5, p.Coordinate6, p.Coordinate7, p.Coordinate8,
                    p.Coordinate9, p.Coordinate10, p.Coordinate11, p.Coordinate12}.Take(numberOfDimensions).ToArray(),
                    new double[] {
                    p.Translation1, p.Translation2, p.Translation3, p.Translation4,
                    p.Translation5, p.Translation6, p.Translation7, p.Translation8,
                    p.Translation9, p.Translation10, p.Translation11, p.Translation12}.Take(numberOfDimensions).ToArray()
                    )).ToArray());
                queryDictionary.Add(i, lshAlgorithm.ComputeHash(new Vector(intervals)));
            }

            var query = (from i in basicQuery
                     join ms in context.MusicalSources on i.MusicalSourceId equals ms.Id
                     join sh in (from s in context.SpatialHashes
                                 where s.NumberOfDimensions == numberOfDimensions && (
                                                        (s.PlaneGroupNumber == 1 && s.Hash == queryDictionary[1]) ||
                                                        (s.PlaneGroupNumber == 2 && s.Hash == queryDictionary[2]) ||
                                                        (s.PlaneGroupNumber == 3 && s.Hash == queryDictionary[3]) ||
                                                        (s.PlaneGroupNumber == 4 && s.Hash == queryDictionary[4]) ||
                                                        (s.PlaneGroupNumber == 5 && s.Hash == queryDictionary[5]) ||
                                                        (s.PlaneGroupNumber == 6 && s.Hash == queryDictionary[6]) ||
                                                        (s.PlaneGroupNumber == 7 && s.Hash == queryDictionary[7]) ||
                                                        (s.PlaneGroupNumber == 8 && s.Hash == queryDictionary[8]) ||
                                                        (s.PlaneGroupNumber == 9 && s.Hash == queryDictionary[9]) ||
                                                        (s.PlaneGroupNumber == 10 && s.Hash == queryDictionary[10])// ||
                                                                                                                   //(s.PlaneGroupNumber == 11 && s.Hash == queryDictionary[11]) ||
                                                                                                                   //(s.PlaneGroupNumber == 12 && s.Hash == queryDictionary[12]) ||
                                                                                                                   //(s.PlaneGroupNumber == 13 && s.Hash == queryDictionary[13]) ||
                                                                                                                   //(s.PlaneGroupNumber == 14 && s.Hash == queryDictionary[14]) ||
                                                                                                                   //(s.PlaneGroupNumber == 15 && s.Hash == queryDictionary[15])
                )
                                 select new { s.IncipitId }) on i.Id equals sh.IncipitId into shh

                     //TODO: Create this query dynamically
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
                         Relevance = (double)shh.Count() / 10,
                         ShowRelevance = true
                     }).OrderByDescending(rm => rm.Relevance);

            return query;
        }
    }
}