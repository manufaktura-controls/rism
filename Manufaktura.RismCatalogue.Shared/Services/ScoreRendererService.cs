using Manufaktura.Controls.Model;
using Manufaktura.Controls.Rendering.Implementations;
using System;
using System.Threading.Tasks;

namespace Manufaktura.RismCatalogue.Shared.Services
{
    public class ScoreRendererService
    {
        private readonly ISettingsService settingsService;

        public ScoreRendererService(ISettingsService settingsService)
        {
            this.settingsService = settingsService;
        }

        private static int canvasIdCount;

        public async Task<string> RenderScoreAsync(Score score)
        {
            var settings = await settingsService.GetRendererSettingsAsync();

            IScore2HtmlBuilder builder;
            if (settings.RenderSurface == HtmlScoreRendererSettings.HtmlRenderSurface.Canvas)
                builder = new Score2HtmlCanvasBuilder(score, string.Format("scoreCanvas{0}", canvasIdCount), settings);
            else if (settings.RenderSurface == HtmlScoreRendererSettings.HtmlRenderSurface.Svg)
                builder = new Score2HtmlSvgBuilder(score, string.Format("scoreCanvas{0}", canvasIdCount), settings);
            else throw new NotImplementedException("Unsupported rendering engine.");

            string html = builder.Build();

            canvasIdCount++;
            return html;
        }
    }
}