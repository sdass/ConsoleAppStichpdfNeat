using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConsoleAppStichpdfNeat.Config;
using ConsoleAppStichpdfNeat.NestedElements;
using ConsoleAppStichpdfNeat.Util;
using classLibraryTryTest;

namespace ConsoleAppStichpdfNeat
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Write to test...");
            Console.WriteLine("width=" + Constants.pageWidth + " height=" + Constants.pageHeight + "spacerHeight=" + Constants.spacerHeight + " in inches.");


            GeneatingFunction gen = new GeneatingFunction(); //create all data

            //testLibcall(); need to do pubicly with gacutil cmdline utility to avoid recompiling
            // gen.generatePdfCard(1);  //track 5-12 //data for debugging
            StitchedFinalPdf pdfData =  gen.generateAPdfStructure(1);// 1 track only
            Console.WriteLine("==============!!!!!!!!!!!================!!!!!!!!!!!!=============");
            Console.WriteLine("1StitchedFinalPdf=" + pdfData); // 1 track at a time
            Console.WriteLine("==============END================!!!!!!!!!!!!=============");
            //funcCreateRaceDelegatePrint(); //create a dummy race and see printout
            // stringTestForGeneralizing();
            TestHarness.tryPageCalculation();

            Console.ReadKey();


        }


        private static void funcCreateRaceDelegatePrint()
        {

            HeaderAndFirstHorse header_1sthorse = new HeaderAndFirstHorse(new Header(110, "", "", 1, ""), new Horse(180, "", "", 1, ""));
            Race<Horse> r = (new Race<Horse>()).setRaceTop(header_1sthorse).addHorse(new Horse(102, "", "", 1, "")).addHorse(new Horse(104, "", "", 1, "")).addHorse(new Horse(106, "", "", 1, "")).addHorse(new Horse(108, "", "", 1, ""));
            string raceDescription = getStringOf_Race(r);
            Console.WriteLine("Race description: " + raceDescription);
            Console.WriteLine("This is end.");
        }


        private static string getStringOf_Race(Race<Horse> aRace) // works
        {
            Func<List<Horse>, String> xListTostr = consolidate; //powerful delegate
            string raceHeaderAnd1stHorse = aRace.headerFirstHorse.ToString();
            
            string secondAndOtherHouse = xListTostr(aRace.secondAndOtherHorseList);
            return raceHeaderAnd1stHorse + secondAndOtherHouse;


        }
        private static void stringTestForGeneralizing()
        {
            List<String> ls = new List<string>(); ls.Add("A"); ls.Add("B"); ls.Add("C");
            Func<List<string>, string> xListTostr = consolidate; //delegate
            string outstr = xListTostr(ls);
            Console.WriteLine("outstr=" + outstr);
        }

        private static string consolidate<T>(List<T> lstr) //use it for general pupose
        {
            string restr = "Begin: ";

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
                    //TODO list

                }
            });


            return restr;
        }



        private static void testLibcall()
        {
            LibClass libclass = new LibClass();
            Console.WriteLine(libclass.showMix("abcd", 5));
            Console.WriteLine(libclass.showMulti("bcde ", 3));
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
