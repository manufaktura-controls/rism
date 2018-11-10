namespace Manufaktura.RismCatalogue.Model
{
    /// <summary>
    /// Corresponds to MARC record but also contains some properties extracted from subfields for easier search and display. 
    /// </summary>
    public class MusicalSource
    {
        public string Id { get; set; }

        public string ComposerName { get; set; }
        public string ComposerDates { get; set; }
        public string Title { get; set; }

        public string FormSubheading { get; set; }

        public string MediumOfPerformance { get; set; }

        public string PartOrSectionNumber { get; set; }
    }
}