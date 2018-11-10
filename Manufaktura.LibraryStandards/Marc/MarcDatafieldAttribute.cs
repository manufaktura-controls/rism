using System;

namespace Manufaktura.LibraryStandards.Marc
{
    /// <summary>
    /// For reference: https://www.loc.gov/marc/bibliographic/
    /// </summary>
    public class MarcDatafieldAttribute : Attribute
    {
        public MarcDatafieldAttribute(string tag)
        {
            Tag = tag;
        }

        public string Tag { get; set; }
    }
}