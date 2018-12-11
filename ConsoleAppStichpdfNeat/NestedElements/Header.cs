using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleAppStichpdfNeat.NestedElements
{
    public class Header : GeneralAttributes
    {
      internal int id { get; set; }
        internal double height { get; set; }
        internal double newHeight { get; set; }
        internal int spCount { get; set; }

        internal string imgFileName { get; set; }
        internal string imgPath { get; set; }

        internal int racenumber { get; set; }
        internal string trackName { get; set; }

        internal double topPostion { get; set; }
        internal double bottomPosition { get; set; }



        public Header(double ht, string imgFileName, string imgPath, int raceNumber, string trackName, bool isHeader)
        {
            this.height = ht;
            newHeight = ht;
            this.imgFileName = imgFileName;
            this.imgPath = imgPath;
            this.racenumber = raceNumber;
            this.trackName = trackName;
         this.isHeader = isHeader;

        }

      internal Header(CardStruct.HorseOrHeader h) : this(h.Height, h.FileName, null, h.racenum, null, h.IsHeader)
      {

      }

      public override string ToString()
        {
            return "{Header: id=" + id + " height=" + height + " newHeight=" + newHeight + " spCount=" + spCount + " imgFileName=" + imgFileName + " racenumber=" + racenumber + "}\n";
        }

    }
}
