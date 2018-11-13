namespace Manufaktura.RismCatalogue.Shared.ViewModels
{
    public class SearchResultViewModel
    {
        public string IncipitSvg { get; set; }

        public string TextIncipit { get; set; }

        public string Voice { get; set; }

        public string ComposerName { get; set; }

        public string CaptionOrHeading { get; set; }
        public string Title { get; set; }

        public string Id { get; set; }

        public string RecordId { get; set; }

        public double Relevance { get; set; }

        public bool ShowRelevance { get; set; }
    }
}