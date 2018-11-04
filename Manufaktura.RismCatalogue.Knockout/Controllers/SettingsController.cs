using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System.IO;

namespace Manufaktura.RismCatalogue.Knockout.Controllers
{
    [Route("api/[controller]")]
    public class SettingsController
    {
        private readonly IHostingEnvironment hostingEnvironment;

        public SettingsController(IHostingEnvironment hostingEnvironment)
        {
            this.hostingEnvironment = hostingEnvironment;
        }

        [HttpGet("[action]")]
        public string GetSettings()
        {
            return File.ReadAllText(Path.Combine(hostingEnvironment.ContentRootPath, "wwwroot") + "/fonts/bravura_metadata.json");
        }
    }
}