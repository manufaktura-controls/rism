using System;

namespace Manufaktura.LibraryStandards.Marc
{
    public class MarcDatafieldAttribute : Attribute
    {
        public MarcDatafieldAttribute(string tag)
        {
            Tag = tag;
        }

        public string Tag { get; set; }
    }
}