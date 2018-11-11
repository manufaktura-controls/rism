namespace Manufaktura.RismCatalogue.Model
{
    public abstract class MusicalSourceField : Entity
    {
        public string MusicalSourceId { get; set; }
        public MusicalSource MusicalSource { get; set; }
    }
}