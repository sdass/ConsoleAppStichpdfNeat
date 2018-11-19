using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleAppStichpdfNeat.NestedElements
{
    public class Header
    {
        internal double height { get; set; }
        internal double newHeight { get; set; }
        internal int spCount { get; set; }

        internal string imgFileName { get; set; }
        internal string imgPath { get; set; }

        internal short racenumber { get; set; }
        internal string trackName { get; set; }

        internal double topPostion { get; set; }
        internal double bottomPosition { get; set; }



        public Header(int ht, string imgFileName, string imgPath, short raceNumber, string trackName)
        {
            this.height = ht;
            newHeight = ht;
            this.imgFileName = imgFileName;
            this.imgPath = imgPath;
            this.racenumber = raceNumber;
            this.trackName = trackName;

        }

        public override string ToString()
        {
            return "{Header: height=" + height + " newHeight=" + newHeight + " spCount=" + spCount + " imgFileName=" + imgFileName + " imgPath=" + imgPath + " racenumber=" + racenumber + "}\n";
        }

    }
}
