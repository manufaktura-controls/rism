using Manufaktura.LibraryStandards.Marc;

namespace Manufaktura.RismCatalogue.Model
{
    [MarcDatafield("100")]
    public class Person : MusicalSourceField
    {
        [MarcSubfield("a")]
        public string PersonalName { get; set; }

        [MarcSubfield("d")]
        public string Dates { get; set; }

        [MarcSubfield("0")]
        public string AuthorityId { get; set; }
    }
}