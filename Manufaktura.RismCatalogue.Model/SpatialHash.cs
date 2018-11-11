namespace Manufaktura.RismCatalogue.Model
{
    public class SpatialHash
    {
        public long Id { get; set; }
        public long IncipitId { get; set; }
        public long Hash { get; set; }

        public int PlaneGroupNumber { get; set; }

        public Incipit Incipit { get; set; }
    }
}