using Manufaktura.LibraryStandards.Marc;

namespace Manufaktura.RismCatalogue.Model
{
    [MarcDatafield("240")]
    public class UniformTitle : MusicalSourceField
    {
        [MarcSubfield("a")]
        public string Title { get; set; }

        [MarcSubfield("k")]
        public string FormSubheading { get; set; }

        [MarcSubfield("m")]
        public string MediumOfPerformance { get; set; }

        [MarcSubfield("n")]
        public string PartOrSectionNumber { get; set; }
    }
}