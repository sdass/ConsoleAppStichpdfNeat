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

        private static int horseSeq = 0;

        private static readonly Random random = new Random();

        internal static int HorseSequence
        {
            get
            {
                return ++horseSeq;
            }
        }
        private int generateFrom2ndHorseDepth()
        {
            int low = tenThoughInch/9;
            int high = tenThoughInch/5;
            int depth = random.Next(low, high);
            return depth;

        }
        private int[] generateHeaderAnd1stdHorseDepth()
        {
            // 2-10% of 
             int headerTemp = Convert.ToInt32( (tenThoughInch / 9)*(0.8)); //80% height of lowest height horse. 
            //random variation  1-8%
            int headerFinal = (headerTemp * random.Next(100, 108)) / 100;
            int firstHorse = generateFrom2ndHorseDepth();
            int[] conjugate = { headerFinal, firstHorse };
            Console.WriteLine("header=" + headerFinal + " firstHorse=" + firstHorse + " conjugatesum=" + conjugate[0] + conjugate[1]);
            return conjugate;

        }

        private int numberOfHorsePerRace()
        {
            //6-14
            //int totalHorseNumber = random.Next(6, 14);
            int totalHorseNumber = random.Next(6, 10); //for debugging small dataset lower the limit
            Console.WriteLine("numberOfHorses=" + totalHorseNumber + " for each race.");
            return totalHorseNumber;

        }

        private int numberOfRacesPerTrack()
        {
            //6-12 
            int totalRaceNumber = random.Next(6, 12);
            totalRaceNumber = 4; //hardcoded
            Console.WriteLine("number Of Races=" + totalRaceNumber + " per track." );
            return totalRaceNumber;

        }



        public StitchedFinalPdf generatePdfCard(int numTracks)
        {
            //tracks total = 6 through 12.
            //28,29,30,32 leaves
            StitchedFinalPdf pdfData = new StitchedFinalPdf();
            Console.WriteLine("---pdf card for  " + numTracks + " tracks -----");

            for (int t = 1; t <= numTracks; t++)
            {
                Console.Write("================track-" + t + "==================");

                int raceCount = numberOfRacesPerTrack();
                for (short r = 1; r <= raceCount; r++)
                {
                    Console.Write("     race-" + r + ">>>>>> ");
                    int horseCount = numberOfHorsePerRace();
                    //process header and 1st horse
                    int[] conjugate = generateHeaderAnd1stdHorseDepth();
                    HeaderAndFirstHorse headerAndFirstHorse_v = new HeaderAndFirstHorse(new Header(conjugate[0], "", "", r, ""), new Horse(GeneatingFunction.HorseSequence ,conjugate[1], "", "", r, ""));
                    //process 2nd horse onward
                    for (int h = 2; h <= horseCount; h++)
                    {
                        int depth = generateFrom2ndHorseDepth();
                        Console.Write("horse-" + h + " height=" + depth + " | ");
                    }
                    Console.WriteLine();
                    //spacer if needed. calculation                   

                }
            }
            return pdfData;

        }

        public StitchedFinalPdf generateAPdfStructure(int numTracks)
        {
            //tracks total = 5 through 12.
            //28,29,30,32 leaves
            // track count = 1 
            StitchedFinalPdf pdfData = new StitchedFinalPdf();
            Console.WriteLine("---pdf card for  " + numTracks + " tracks -----");

            for (int t = 1; t <= numTracks; t++)
            {
                Track track = new Track();
                Console.Write("================track-" + t + "==================");

                int raceCount = numberOfRacesPerTrack();
                for (short r = 1; r <= raceCount; r++)
                {
                    Console.Write("     race-" + r + ">>>>>> ");
                    Race<Horse> race = new Race<Horse>();
                    int horseCount = numberOfHorsePerRace();
                    //process header and 1st horse
                    int[] conjugate = generateHeaderAnd1stdHorseDepth();
                    HeaderAndFirstHorse hf = new HeaderAndFirstHorse(new Header(conjugate[0],"", "", r,""), new Horse(GeneatingFunction.HorseSequence,conjugate[1], "", "", r, ""));
                    race = race.setRaceTop(hf);

                    //process 2nd horse onward
                    for (int h = 2; h <= horseCount; h++)
                    {
                        int depth = generateFrom2ndHorseDepth();
                        //Horse horse2on = new Horse(height);
                        //horse2.spCount = 0; auto initialize to 0
                        race = race.addHorse(new Horse(GeneatingFunction.HorseSequence, depth, "", "", r, "")); //adding horses on race **********1
                        Console.Write("horse-" + h + " height=" + depth + " | ");

                    }//horse-for
                    Console.WriteLine();
                    track = track.addRace(race); //adding races to track *************************2
                                     
                }//race-for
                pdfData = pdfData.addTrack(track); //adding track to pdf ************************3
            }//track-for

            return pdfData;
        }


    }
}
