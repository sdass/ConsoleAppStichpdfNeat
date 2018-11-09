
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleAppStichpdfNeat.NestedElements
{
    public class Horse
    {
        double topPostion { get; set; }
        double depth { get; set; }
        //byte[] horseContent { get; set; }
        double bottomPosition { get; set; }
        int spCount { get; set; }

        public Horse(int depth)
        {
            this.depth = depth;
        }

        public override string ToString()
        {
            return "{Horse: depth= " + depth + " spCount=" + spCount + "} ";
        }
    }
}
