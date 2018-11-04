using Manufaktura.Controls.Model.Fonts;
using Manufaktura.Controls.Rendering.Implementations;
using Manufaktura.Controls.SMuFL;
using System.IO;
using System.Threading.Tasks;

namespace Manufaktura.RismCatalogue.Shared.Services
{
    public class ServerSideSettingsService : ISettingsService
    {
        public ServerSideSettingsService()
        {
            rendererSettings = CreateScoreRendererSettings("Bravura", "/fonts/bravura_metadata.json", "/fonts/Bravura.otf");
        }

        private static HtmlScoreRendererSettings rendererSettings;

        public Task<HtmlScoreRendererSettings> GetRendererSettingsAsync()
        {
            return Task.FromResult(rendererSettings);
        }

        private HtmlScoreRendererSettings CreateScoreRendererSettings(string musicFontName, string fontMetadataPath, params string[] musicFontUris)
        {
            var fontMetadata = string.IsNullOrWhiteSpace(fontMetadataPath) ? null : File.ReadAllText(
                //Path.Combine(environment.ContentRootPath, "wwwroot") +
                $"wwwroot/{fontMetadataPath}");
            var settings = new HtmlScoreRendererSettings();
            settings.RenderSurface = HtmlScoreRendererSettings.HtmlRenderSurface.Svg;
            settings.CustomElementPositionRatio = 1;
            if (musicFontName == "Polihymnia")
            {
                settings.Fonts.Add(MusicFontStyles.MusicFont, new HtmlFontInfo(musicFontName, 22, musicFontUris));
                settings.Fonts.Add(MusicFontStyles.StaffFont, new HtmlFontInfo(musicFontName, 24, musicFontUris));
                settings.Fonts.Add(MusicFontStyles.GraceNoteFont, new HtmlFontInfo(musicFontName, 12, musicFontUris));
                settings.Fonts.Add(MusicFontStyles.TimeSignatureFont, new HtmlFontInfo("Open Sans", 12, "/fonts/OpenSans-Regular.ttf"));
                settings.Fonts.Add(MusicFontStyles.LyricsFont, new HtmlFontInfo("Open Sans", 12, "/fonts/OpenSans-Regular.ttf"));
                settings.Fonts.Add(MusicFontStyles.DirectionFont, new HtmlFontInfo("Open Sans", 14, "/fonts/OpenSans-Regular.ttf"));
            }
            else
            {
                var profile = SMuFLMusicFont.CreateFromJsonString(fontMetadata);
                profile.FontSizes[MusicFontStyles.MusicFont] = 20;
                profile.FontSizes[MusicFontStyles.TimeSignatureFont] = 20;
                profile.FontSizes[MusicFontStyles.StaffFont] = 20;
                settings.SetMusicFont(profile, musicFontName, musicFontUris);
            }

            settings.RenderingMode = Controls.Rendering.ScoreRenderingModes.AllPages;
            settings.IgnorePageMargins = true;

            return settings;
        }
    }
}