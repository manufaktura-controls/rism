using Manufaktura.LibraryStandards.Marc;

namespace Manufaktura.RismCatalogue.Model
{
    [MarcDatafield("031")]
    public class Incipit : Entity
    {
        [MarcSubfield("a")]
        public string NumberOfWork { get; set; }

        [MarcSubfield("b")]
        public string NumberOfMovement { get; set; }

        [MarcSubfield("c")]
        public string NumberOfExcerpt { get; set; }

        [MarcSubfield("d")]
        public string CaptionOrHeading { get; set; }

        [MarcSubfield("e")]
        public string Role { get; set; }

        [MarcSubfield("g")]
        public string Clef { get; set; }

        [MarcSubfield("m")]
        public string VoiceOrInstrument { get; set; }

        [MarcSubfield("n")]
        public string KeySignature { get; set; }

        [MarcSubfield("o")]
        public string TimeSignature { get; set; }

        [MarcSubfield("p")]
        public string MusicalNotation { get; set; }

        [MarcSubfield("q")]
        public string GeneralNote { get; set; }

        [MarcSubfield("r")]
        public string KeyOrMode { get; set; }

        [MarcSubfield("s")]
        public string CodedValidityNote { get; set; }

        [MarcSubfield("t")]
        public string TextIncipit { get; set; }

        [MarcSubfield("u")]
        public string UniformResourceIdentifier { get; set; }

        [MarcSubfield("y")]
        public string LinkText { get; set; }

        [MarcSubfield("z")]
        public string PublicNote { get; set; }

        [MarcSubfield("2")]
        public string SystemCode { get; set; }

        [MarcSubfield("6")]
        public string Linkage { get; set; }

        [MarcSubfield("8")]
        public string FieldLinkAndSeqNumber { get; set; }
    }
}