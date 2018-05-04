namespace Manufaktura.RismCatalogue.Model
{
    public class Entity
    {
        public long Id { get; set; }

        public string MusicalSourceId { get; set; }
        public MusicalSource MusicalSource { get; set; }
    }
}