using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConsoleAppStichpdfNeat.Util;
using ConsoleAppStichpdfNeat.Config;

namespace ConsoleAppStichpdfNeat
{
    class TestHarness
    {
        public static void tryPageCalculation()
        {
            Console.WriteLine("inside tryPageCalculation() . . . ");
            //PageBound pgBound = new PageBound();
           // Console.WriteLine("pgBound=" + pgBound);
            PageDetail pgDetail = new PageDetail();
            Console.WriteLine(pgDetail);
            //Console.WriteLine(Constants.lastPageMinimumFillout + " " + Constants.shrinkMax + " " + Constants.shrinkMin + " " + Constants.pageHeight);
 
        }

        public void optimizeSpace()
        {

        }

        private void peeking() //wroks
        {
            List<String> ls = new List<string>();
            ls.Add("a"); ls.Add("b"); ls.Add("c"); ls.Add("d"); ls.Add("d");

            for (int i = 0; i < ls.Count; i++)
            {
                int peek = i + 1 >= ls.Count ? ls.Count - 1 : i + 1;
                Console.WriteLine(ls[i] + " peek=" + ls[peek]);
            }
        }
    }
}
