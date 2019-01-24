using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleAppStichpdfNeat.Config
{
    class Constants
    {
      public static double pageHeightIndots = 772.0; // 80 dots/inch (approx  [702 9.75 inch * 72 dots/inch] 

      public static bool SKIP_ONGOING_SHRINK = true;

      public static double PageHeight
      {
         get { return pageHeightIndots; }
         set { pageHeightIndots = value; }
      }

      public static double ShrinkMax
      {
         get { return 0.9 * 80; } // tried with  2.2 * 80 //inch * dots per inch (approx) .75 - 1 inch
      }

      public static double EvenSpaceMax
      {
         get { return 150; } //100 dots
      }

      public static readonly double MIN_SPACE_FOR_HEIGHT = 10;
      public static readonly double MAX_SPACE_FOR_HEIGHT = 100;
      public static readonly double SHRINK_THRESHOLD_ONE_HORSE = 10;

      public static double ShrinkMin
      {
         get { return 0.125 * 80; } //inch * dots per inch
      }

      public static double LastPageMinimumFillout
      {
         get { return pageHeightIndots * 0.25; } //dots per page * 0.25 (1/4 of a page)
      }


      public static int TENTHOUSAND = 10000;
        public static double pageWidth
        { //inch. content width of a pdf page
            get { return 7.38; }
        }
        public static double InchpageHeight
        { //inch content height of a pdf page
            get { return 9.7991; }
        }

        public static double pageHeightIn10thouMultiple
        { //inch content height of a pdf page
            get { return 9.7991 * TENTHOUSAND; }
        }


        public static double spacerHeight
        { //inch content height of a pdf page
            get { return InchpageHeight / (9 * 12); } //1/12(lowest horse height)
        }

        public static double shrinkMax
        { 
            get { return 0.25; } //inch on a page
        }

        public static double shrinkMaxIn10thouMultiple
        {
            get { return 0.25 *TENTHOUSAND; } //10000 times inch on a page
        }

        public static double shrinkMin
        {
            get { return 0.125; } //inch on a page
        }

        public static double lastPageMinimumFillout
        {
            get { return InchpageHeight * 0.38; } //inch on a page [more than 1/3 filled up
        }

    }
}
