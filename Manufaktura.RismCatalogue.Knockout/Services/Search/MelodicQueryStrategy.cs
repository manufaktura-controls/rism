using Manufaktura.RismCatalogue.Model;
using Manufaktura.RismCatalogue.Shared.Services;
using Manufaktura.RismCatalogue.Shared.ViewModels;
using System.Linq;

namespace Manufaktura.RismCatalogue.Knockout.Services.Search
{
    public abstract class MelodicQueryStrategy
    {
        protected readonly RismDbContext context;
        protected readonly ScoreRendererService scoreRendererService;
        protected readonly PlaineAndEasieService plaineAndEasieService;

        protected MelodicQueryStrategy(RismDbContext context, ScoreRendererService scoreRendererService, PlaineAndEasieService plaineAndEasieService)
        {
            this.context = context;
            this.scoreRendererService = scoreRendererService;
            this.plaineAndEasieService = plaineAndEasieService;
        }

        public abstract MelodicQueryMode Mode { get; }

        public abstract IOrderedQueryable<SearchResultViewModel> GetQuery(IQueryable<Incipit> basicQuery, SearchQuery searchQuery);
    }
}