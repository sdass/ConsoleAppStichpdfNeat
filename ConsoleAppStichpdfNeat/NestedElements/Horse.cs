
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleAppStichpdfNeat.NestedElements
{
    public class Horse
    {
        internal string imgFileName { get; set; }
        internal string imgPath { get; set; }

        internal double topPostion { get; set; }
        internal double height { get; set; }
        internal double bottomPosition { get; set; }

        internal int spCount { get; set; }
        internal double newHeight { get; set; }

        internal short raceNumber { get; set; }
        internal string trackName { get; set; }
        

        public Horse(int ht, string imgFileName, string imgPath, short raceNumber, string trackName)
        {
            height = ht;
            newHeight = ht;

            this.imgFileName = imgFileName;
            this.imgPath = imgPath;
            this.raceNumber = raceNumber;
            this.trackName = trackName;
        }

        public override string ToString()
        {
            return "{Horse: height= " + height +  " newHeight=" + newHeight + " spCount=" + spCount +  " imgFileName=" + imgFileName + " imgPath=" + imgPath + " raceNumber=" + raceNumber + "}\n";
        }
    }
}
