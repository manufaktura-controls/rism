using Manufaktura.RismCatalogue.Model;
using Manufaktura.RismCatalogue.Shared.Services;
using Manufaktura.RismCatalogue.Shared.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
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

        [HttpGet("[action]")]
        public IEnumerable<SearchResultViewModel> Search(int skip, int take)
        {
            var incipits =
                (from i in context.Incipits orderby i.Id
                join ms in context.MusicalSources on i.MusicalSourceId equals ms.Id
                select new SearchResultViewModel
                {
                    Id = i.Id.ToString(),
                    IncipitSvg = string.IsNullOrWhiteSpace(i.MusicalNotation) ? null : scoreRendererService.RenderScore(plaineAndEasieService.Parse(i)),
                    CaptionOrHeading = i.CaptionOrHeading,
                    TextIncipit = i.TextIncipit,
                    Voice = i.VoiceOrInstrument,
                    Title = ms.Title,
                    ComposerName = ms.ComposerName
                })
                .Skip(skip)
                .Take(take)
                .ToArray();

            return incipits;
        }
    }
}