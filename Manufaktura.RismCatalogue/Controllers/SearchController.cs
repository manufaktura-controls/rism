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
        private readonly SettingsService settingsService;

        public SearchController([FromServices] RismDbContext context, SettingsService settingsService)
        {
            this.context = context;
            this.settingsService = settingsService;
        }

        [HttpGet("[action]")]
        public IEnumerable<SearchResultViewModel> Search(int skip, int take)
        {
            return context.Incipits.OrderBy(i => i.Id).Skip(skip).Take(take).Select(i => new SearchResultViewModel(i, settingsService.RendererSettings)).ToArray();
        }
    }
}