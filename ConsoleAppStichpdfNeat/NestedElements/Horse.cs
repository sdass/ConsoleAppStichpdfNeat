
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConsoleAppStichpdfNeat.Util;

namespace ConsoleAppStichpdfNeat.NestedElements
{
    public class Horse
    {
        internal int id { get; set; }
        internal string imgFileName { get; set; }
        internal string imgPath { get; set; }

        internal double topPostion { get; set; }
        internal double height { get; set; }
        internal double bottomPosition { get; set; }

        internal int spCount { get; set; }
        internal double newHeight { get; set; }

        internal short raceNumber { get; set; }
        internal string trackName { get; set; }

        //following will be filled out during calculation
        internal PageBased positionOnPage;
        

        public Horse(int id, int ht, string imgFileName, string imgPath, short raceNumber, string trackName)
        {
            this.id = id;

            height = ht;
            newHeight = ht;

            this.imgFileName = imgFileName;
            this.imgPath = imgPath;
            this.raceNumber = raceNumber;
            this.trackName = trackName;
            positionOnPage = new PageBased();
        }

        public override string ToString()
        {
            return "{Horse: id=" + id + " height= " + height +  " newHeight=" + newHeight + " spCount=" + spCount +  " imgFileName=" + imgFileName + " imgPath=" + imgPath + " raceNumber=" + raceNumber +  " positionOnPage=" + positionOnPage + "}\n";
        }
    }

    internal class PageBased
    {
        // domain {0,1,2} 0=position regular.  1=position end of page but middle of race;=> needed "Continued". 2=position end of page AND end of race; => not needed "Continued"
        //internal int where { get; set; } //0,1,2 
        internal EntryLocationOnPage where { get; set; }
        internal double leftspaceatEnd { get; set; }

        internal PageBased()
        {
            where = EntryLocationOnPage.MiddleEntry; // most situation and safe
            leftspaceatEnd = -1; // indicate not set yet

        }
        public override string ToString()
        {
            return "{PageBased: where=" + where + " leftspaceatEnd=" + leftspaceatEnd + " }";
        }
    }
}
