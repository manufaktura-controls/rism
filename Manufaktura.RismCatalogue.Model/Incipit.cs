using Manufaktura.LibraryStandards.Marc;

namespace Manufaktura.RismCatalogue.Model
{
    [MarcDatafield("031")]
    public class Incipit : MusicalSourceField
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

        public string RhythmDigest { get; set; }

        public string RhythmRelativeDigest { get; set; }

        public int Interval1 { get; set; }
        public int Interval2 { get; set; }
        public int Interval3 { get; set; }
        public int Interval4 { get; set; }
        public int Interval5 { get; set; }
        public int Interval6 { get; set; }
        public int Interval7 { get; set; }
        public int Interval8 { get; set; }
        public int Interval9 { get; set; }
        public int Interval10 { get; set; }
        public int Interval11 { get; set; }
        public int Interval12 { get; set; }

        public long? Hash1d { get; set; }
        public long? Hash2d { get; set; }
        public long? Hash3d { get; set; }
        public long? Hash4d { get; set; }
        public long? Hash5d { get; set; }
        public long? Hash6d { get; set; }
    }
}