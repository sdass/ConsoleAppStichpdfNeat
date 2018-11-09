using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConsoleAppStichpdfNeat.NestedElements;

namespace ConsoleAppStichpdfNeat.Util
{
    class GeneatingFunction
    {
        //Height = 9.7991 inch = 97991 tenThouInch
        private int tenThoughInch = 97991;

        private static readonly Random random = new Random();

        private int generateFrom2ndHorseDepth()
        {
            int low = tenThoughInch/9;
            int high = tenThoughInch/5;
            int depth = random.Next(low, high);
            return depth;

        }
        private int generateHeaderAnd1stdHorseDepth()
        {
            // 2-10% of 
             int headerTemp = Convert.ToInt32( (tenThoughInch / 9)*(0.8)); //80% depth of lowest depth horse. 
            //random variation  1-8%
            int headerFinal = (headerTemp * random.Next(100, 108)) / 100;
            int firstHorse = generateFrom2ndHorseDepth();
            int conjugate = headerFinal + firstHorse;
            Console.WriteLine("header=" + headerFinal + " firstHorse=" + firstHorse + " conjugate=" + conjugate);
            return conjugate;

        }

        private int numberOfHorsePerRace()
        {
            //6-14
            int totalHorseNumber = random.Next(6, 14);
            Console.WriteLine("numberOfHorses=" + totalHorseNumber + " for each race.");
            return totalHorseNumber;

        }

        private int numberOfRacesPerTrack()
        {
            //6-12 
            int totalRaceNumber = random.Next(6, 12);
            Console.WriteLine("number Of Races=" + totalRaceNumber + " per track." );
            return totalRaceNumber;

        }



        public void generatePdfCard(int numTracks)
        {
            //tracks total = 6 through 12.
            //28,29,30,32 leaves
            StitchedFinalPdf pdfData = new StitchedFinalPdf();
            Console.WriteLine("---pdf card for  " + numTracks + " tracks -----");

            for (int t = 1; t <= numTracks; t++)
            {
                Console.Write("================track-" + t + "==================");

                int raceCount = numberOfRacesPerTrack();
                for (int r = 1; r <= raceCount; r++)
                {
                    Console.Write("     race-" + r + ">>>>>> ");
                    int horseCount = numberOfHorsePerRace();
                    //process header and 1st horse
                    int conjugate = generateHeaderAnd1stdHorseDepth();
                    //process 2nd horse onward
                    for (int h = 2; h <= horseCount; h++)
                    {
                        int depth = generateFrom2ndHorseDepth();
                        Console.Write("horse-" + h + " depth=" + depth + " | ");
                    }
                    Console.WriteLine();
                    //spacer if needed. calculation                   

                }
            }

        }


    }
}
