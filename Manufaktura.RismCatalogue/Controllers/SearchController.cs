using Manufaktura.RismCatalogue.Model;
using Manufaktura.RismCatalogue.Services;
using Manufaktura.RismCatalogue.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace Manufaktura.RismCatalogue.Controllers
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
            var incipits = context.Incipits
                .OrderBy(i => i.Id)
                .Skip(skip)
                .Take(take)
                .ToArray();
            var viewModels = incipits.Select(i => new SearchResultViewModel
            {
                IncipitSvg = string.IsNullOrWhiteSpace(i.MusicalNotation) ? null : scoreRendererService.RenderScore(plaineAndEasieService.Parse(i)),
                CaptionOrHeading = i.CaptionOrHeading,
                TextIncipit = i.TextIncipit,
                Voice = i.VoiceOrInstrument
            }).ToArray();
            return viewModels;
        }
    }
}