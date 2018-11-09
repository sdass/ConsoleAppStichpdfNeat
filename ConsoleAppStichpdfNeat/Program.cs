using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConsoleAppStichpdfNeat.Config;
using ConsoleAppStichpdfNeat.NestedElements;
using ConsoleAppStichpdfNeat.Util;

namespace ConsoleAppStichpdfNeat
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Write to test...");
            Console.WriteLine("width=" + Constants.pageWidth + " height=" + Constants.pageHeight + "spacerHeight=" + Constants.spacerHeight + " in inches.");


            GeneatingFunction gen = new GeneatingFunction();
            // gen.generatePdfCard(5); //GeneratgeneratePdfCard(6); //track 6-12
            funcDelegate();
            Console.ReadKey();


        }

        private static void funcDelegate()
        {
            //LinkedList<string> ls = new LinkedList<string>();
            List<String> ls = new List<string>();
            ls.Add("A");
            ls.Add("B");
            ls.Add("C");

            HeaderAndFirstHorse header_1sthorse = new HeaderAndFirstHorse(new Header(110), new Horse(180));
            Race<Horse> r = (new Race<Horse>()).setRaceTop(header_1sthorse).addHorse(new Horse(102)).addHorse(new Horse(104)).addHorse(new Horse(106)).addHorse(new Horse(108));
            //printStuff(r);
            Console.WriteLine(r.headerFirstHorse.ToString());
            List<Horse> r2 = r.secondAndOtherHorseList;
            string horse_list = printStuff(r2); //horse
            Console.WriteLine("horse_list=" + horse_list);


            List<Horse> lhorses = r.secondAndOtherHorseList;
            Console.WriteLine("printStuff=" + printStuff(ls)); //string
            Console.WriteLine("This is end.");
        }

        //main works

        private static string printStuff<T>(List<T> r)
        {
            Func<List<T>, String> xListTostr = consolidate;
            return xListTostr(r);


        }
        private static string consolidate<T>(List<T> lstr)
        {
            //return "dfsfsf";
            string restr = "Begin: ";
            // lstr.ForEach(((string)x) => { restr = restr + x + ", " });
            lstr.ForEach(b =>
            {
                if (b.GetType() == typeof(string)) {
                    Console.WriteLine("string type");
                    restr += b.ToString() + ": ";
                } else if (b.GetType() == typeof(Horse))
                {
                    Console.WriteLine("Horse type");
                    restr += b.ToString() + ", ";
                }else if (b.GetType() == typeof(Race<Horse>))
                {
                    Console.WriteLine("Race type");

                }
            });


            return restr;
        }


        /*  stub works
            private static string printStuff(List<string> r)
            {
                Func<List<string>, string> xListTostr = consolidate;
                return xListTostr(r);


            }
            private static string consolidate(List<string> lstr)
            {
                //return "dfsfsf";
                string restr = "Begin: ";
                int t = 5;
                string result = restr + t;
                lstr.ForEach( m => { restr = restr + m; } );
                Console.WriteLine("whole=" + restr);
                return restr;
            }


            private static void printStuff(Race r)
            {
                Console.WriteLine(r.headerFirstHorse);
                Console.WriteLine(r.secondAndOtherHorseList.ToString());

            }
            */
    }
}
