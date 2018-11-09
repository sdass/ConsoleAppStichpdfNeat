using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleAppStichpdfNeat.NestedElements
{
    public class Header
    {
        public int depth { get; set; }


        public Header(int depth)
        {
            this.depth = depth;
        }

        public override string ToString()
        {
            return "{Header: depth=" + depth + "}";
        }
    }
}
