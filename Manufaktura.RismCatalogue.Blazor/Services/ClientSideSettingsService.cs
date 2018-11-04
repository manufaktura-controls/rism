using Manufaktura.Controls.Model.Fonts;
using Manufaktura.Controls.Rendering.Implementations;
using Manufaktura.Controls.SMuFL;
using Manufaktura.RismCatalogue.Shared.Services;
using System.Net.Http;
using System.Threading.Tasks;

namespace Manufaktura.RismCatalogue.Blazor.Services
{
    public class ClientSideSettingsService : ISettingsService
    {
        private readonly HttpClient httpClient;

        public ClientSideSettingsService(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        private HtmlScoreRendererSettings rendererSettings = null;

        public async Task<HtmlScoreRendererSettings> GetRendererSettingsAsync()
        {
            if (rendererSettings != null) return await Task.FromResult(rendererSettings);

            var fontMetadata = await httpClient.GetStringAsync("api/Settings/GetSettings");

            rendererSettings = new HtmlScoreRendererSettings();
            rendererSettings.RenderSurface = HtmlScoreRendererSettings.HtmlRenderSurface.Svg;
            rendererSettings.CustomElementPositionRatio = 1;

            var musicFontUris = new[] { "/fonts/bravura_metadata.json", "/fonts/Bravura.otf" };

            var profile = SMuFLMusicFont.CreateFromJsonString(fontMetadata);
            profile.FontSizes[MusicFontStyles.MusicFont] = 20;
            profile.FontSizes[MusicFontStyles.TimeSignatureFont] = 20;
            profile.FontSizes[MusicFontStyles.StaffFont] = 20;
            rendererSettings.SetMusicFont(profile, "Bravura", musicFontUris);

            rendererSettings.RenderingMode = Controls.Rendering.ScoreRenderingModes.AllPages;
            rendererSettings.IgnorePageMargins = true;

            return rendererSettings;
        }
    }
}