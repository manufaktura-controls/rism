namespace Manufaktura.RismCatalogue.Model
{
    public class SpatialHash : Entity
    {
        public long IncipitId { get; set; }
        public long Hash { get; set; }

        public int PlaneGroupNumber { get; set; }

        public int NumberOfDimensions { get; set; }

        public Incipit Incipit { get; set; }
    }
}