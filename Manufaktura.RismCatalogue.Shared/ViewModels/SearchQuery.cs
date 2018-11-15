namespace Manufaktura.RismCatalogue.Shared.ViewModels
{
    public class SearchQuery
    {
        public int[] Intervals { get; set; } = new int[0];

        public string Rhythm { get; set; }

        public bool IsRhythmRelative { get; set; }

        public int Skip { get; set; }

        public int Take { get; set; } = 30;

        public MelodicQueryMode Mode { get; set; }
    }
}