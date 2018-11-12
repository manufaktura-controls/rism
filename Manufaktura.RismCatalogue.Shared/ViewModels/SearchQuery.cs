namespace Manufaktura.RismCatalogue.Shared.ViewModels
{
    public class SearchQuery
    {
        public int[] Intervals { get; set; } = new int[0];

        public int Skip { get; set; }

        public int Take { get; set; } = 30;
    }
}