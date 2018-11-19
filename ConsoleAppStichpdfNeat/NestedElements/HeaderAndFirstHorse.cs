using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleAppStichpdfNeat.NestedElements
{
    public class HeaderAndFirstHorse
    {
        public Header header { get; set; }
        public Horse firstHorse { get; set; }


        public HeaderAndFirstHorse(Header header, Horse firstHorse)
        {
            this.header = header;
            this.firstHorse = firstHorse;
        }
        public override string ToString()
        {
            return "{HeaderAndFirstHorse: " + header.ToString() + firstHorse.ToString() + "} ";
        }

    }
}
