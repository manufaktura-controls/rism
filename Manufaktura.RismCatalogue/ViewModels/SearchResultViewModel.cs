namespace Manufaktura.RismCatalogue.ViewModels
{
    public class SearchResultViewModel
    {
        public SearchResultViewModel(string incipitSvg) //TODO: Will be constructed from MusicalSource
        {
            IncipitSvg = incipitSvg;
        }

        public string IncipitSvg { get; set; }
    }
}