namespace Manufaktura.RismCatalogue.Model
{
    public class SpatialHash : Entity
    {
        public long Hash1 { get; set; }
        public long Hash2 { get; set; }
        public long Hash3 { get; set; }

        public int NumberOfDimensions { get; set; }

    }
}