using System.ComponentModel.DataAnnotations.Schema;

namespace Manufaktura.RismCatalogue.Model
{
    public class Entity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        public string MusicalSourceId { get; set; }
        public MusicalSource MusicalSource { get; set; }
    }
}