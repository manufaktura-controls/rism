using Manufaktura.Controls.Model.Fonts;
using Manufaktura.Controls.Rendering.Implementations;
using Manufaktura.Controls.SMuFL;
using Manufaktura.RismCatalogue.Shared.Services;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;

namespace Manufaktura.RismCatalogue.Blazor.Services
{
    public class ClientSideSettingsService : ISettingsService
    {
        private readonly HttpClient httpClient;

        private string fontMetadata;

        private HtmlScoreRendererSettings rendererSettings = null;

        public ClientSideSettingsService(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public bool IsInitialized { get; private set; }
        public HtmlScoreRendererSettings GetRendererSettings()
        {
            if (rendererSettings != null) return rendererSettings;

            rendererSettings = new HtmlScoreRendererSettings();
            rendererSettings.RenderSurface = HtmlScoreRendererSettings.HtmlRenderSurface.Svg;
            rendererSettings.CustomElementPositionRatio = 1;

            var musicFontUris = new[] { "/fonts/Polihymnia.ttf" };

            rendererSettings.Fonts.Add(MusicFontStyles.MusicFont, new HtmlFontInfo("Polihymnia", 22, musicFontUris));
            rendererSettings.Fonts.Add(MusicFontStyles.StaffFont, new HtmlFontInfo("Polihymnia", 24, musicFontUris));
            rendererSettings.Fonts.Add(MusicFontStyles.GraceNoteFont, new HtmlFontInfo("Polihymnia", 12, musicFontUris));
            rendererSettings.Fonts.Add(MusicFontStyles.TimeSignatureFont, new HtmlFontInfo("Open Sans", 12, "/fonts/OpenSans-Regular.ttf"));
            rendererSettings.Fonts.Add(MusicFontStyles.LyricsFont, new HtmlFontInfo("Open Sans", 12, "/fonts/OpenSans-Regular.ttf"));
            rendererSettings.Fonts.Add(MusicFontStyles.DirectionFont, new HtmlFontInfo("Open Sans", 14, "/fonts/OpenSans-Regular.ttf"));

            /*var profile = SMuFLMusicFont.CreateFromJsonString(fontMetadata, false); //TODO: Proxy disabled becasue Mono.wasm does not support Reflection.Emit yet
            profile.FontSizes[MusicFontStyles.MusicFont] = 20;
            profile.FontSizes[MusicFontStyles.TimeSignatureFont] = 20;
            profile.FontSizes[MusicFontStyles.StaffFont] = 20;
            rendererSettings.SetMusicFont(profile, "Bravura", musicFontUris);*/

            rendererSettings.RenderingMode = Controls.Rendering.ScoreRenderingModes.Panorama;
            rendererSettings.IgnorePageMargins = true;

            return rendererSettings;
        }

        public async Task InitializeAsync()
        {
            fontMetadata = await httpClient.GetStringAsync("api/Settings/GetSettings");
            IsInitialized = true;
        }
    }
}