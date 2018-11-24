namespace Manufaktura.RismCatalogue.Model
{
    public class SpatialHashIncipit : Entity
    {
        public SpatialHash SpatialHash { get; set; }
        public string SpatialHashId { get; set; }

        public long IncipitId { get; set; }

        public Incipit Incipit { get; set; }
    }
}