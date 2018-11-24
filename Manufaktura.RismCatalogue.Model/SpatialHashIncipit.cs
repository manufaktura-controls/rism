namespace Manufaktura.RismCatalogue.Model
{
    public class SpatialHashIncipit
    {
        public SpatialHash SpatialHash { get; set; }
        public long SpatialHashId { get; set; }

        public long IncipitId { get; set; }

        public Incipit Incipit { get; set; }
    }
}