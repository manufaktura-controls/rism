using System;

namespace Manufaktura.LibraryStandards.Marc
{
    public class MarcSubfieldAttribute : Attribute

    {
        public MarcSubfieldAttribute(string code)
        {
            Code = code;
        }

        public string Code { get; set; }
    }
}