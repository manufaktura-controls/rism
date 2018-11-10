using System;

namespace Manufaktura.LibraryStandards.Marc
{
    /// <summary>
    /// For reference: https://www.loc.gov/marc/bibliographic/
    /// </summary>
    public class MarcSubfieldAttribute : Attribute

    {
        public MarcSubfieldAttribute(string code)
        {
            Code = code;
        }

        public string Code { get; set; }
    }
}