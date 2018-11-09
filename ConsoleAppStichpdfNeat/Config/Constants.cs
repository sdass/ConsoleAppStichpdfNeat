using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleAppStichpdfNeat.Config
{
    class Constants
    {
        public static double pageWidth
        { //inch. content width of a pdf page
            get { return 7.38; }
        }
        public static double pageHeight
        { //inch content height of a pdf page
            get { return 9.7991; }
        }

        public static double spacerHeight
        { //inch content height of a pdf page
            get { return pageHeight / (9 * 12); } //1/12(lowest horse depth)
        }
    }
}
