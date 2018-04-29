using Manufaktura.Controls.Model.Fonts;
using Manufaktura.Controls.Rendering.Implementations;
using Microsoft.AspNetCore.Hosting;
using System.IO;

namespace Manufaktura.RismCatalogue.Services
{
    public class SettingsService
    {
        private readonly IHostingEnvironment environment;

        public SettingsService(IHostingEnvironment environment)
        {
            this.environment = environment;
            RendererSettings = CreateScoreRendererSettings("Bravura", "/fonts/bravura_metadata.json", "/fonts/Bravura.otf");
        }

        public HtmlScoreRendererSettings RendererSettings { get; private set; }

        private HtmlScoreRendererSettings CreateScoreRendererSettings(string musicFontName, string fontMetadataPath, params string[] musicFontUris)
        {
            var fontMetadata = string.IsNullOrWhiteSpace(fontMetadataPath) ? null : File.ReadAllText(Path.Combine(environment.ContentRootPath, "wwwroot") + fontMetadataPath);
            var settings = new HtmlScoreRendererSettings();
            settings.RenderSurface = HtmlScoreRendererSettings.HtmlRenderSurface.Svg;
            if (musicFontName == "Polihymnia")
            {
                settings.Fonts.Add(MusicFontStyles.MusicFont, new HtmlFontInfo(musicFontName, 22, musicFontUris));
                settings.Fonts.Add(MusicFontStyles.StaffFont, new HtmlFontInfo(musicFontName, 24, musicFontUris));
                settings.Fonts.Add(MusicFontStyles.GraceNoteFont, new HtmlFontInfo(musicFontName, 12, musicFontUris));
                settings.Fonts.Add(MusicFontStyles.TimeSignatureFont, new HtmlFontInfo("Open Sans", 12, "/fonts/OpenSans-Regular.ttf"));
            }
            else
            {
                settings.LoadSMuFLFont(fontMetadata, musicFontName, 20, musicFontUris);
            }
            settings.Fonts.Add(MusicFontStyles.LyricsFont, new HtmlFontInfo("Open Sans", 12, "/fonts/OpenSans-Regular.ttf"));
            settings.Fonts.Add(MusicFontStyles.DirectionFont, new HtmlFontInfo("Open Sans", 14, "/fonts/OpenSans-Regular.ttf"));
            settings.RenderingMode = Controls.Rendering.ScoreRenderingModes.AllPages;
            settings.IgnorePageMargins = true;

            return settings;
        }
    }
}