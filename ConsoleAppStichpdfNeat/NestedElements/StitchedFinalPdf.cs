using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleAppStichpdfNeat.NestedElements
{
    public class StitchedFinalPdf
    {
        //front cover, last cover and other items 
        public List<Track> trackList { get; set; }

        public StitchedFinalPdf()
        {
            trackList = new List<Track>();
        }

    }


}
