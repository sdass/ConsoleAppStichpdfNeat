using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleAppStichpdfNeat.Config
{
    class Constants
    {
        public static int TENTHOUSAND = 10000;
        public static double pageWidth
        { //inch. content width of a pdf page
            get { return 7.38; }
        }
        public static double pageHeight
        { //inch content height of a pdf page
            get { return 9.7991; }
        }

        public static double pageHeightIn10thouMultiple
        { //inch content height of a pdf page
            get { return 9.7991 * TENTHOUSAND; }
        }


        public static double spacerHeight
        { //inch content height of a pdf page
            get { return pageHeight / (9 * 12); } //1/12(lowest horse height)
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
            get { return pageHeight * 0.38; } //inch on a page [more than 1/3 filled up
        }

    }
}
