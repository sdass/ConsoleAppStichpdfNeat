using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConsoleAppStichpdfNeat.Util;
using ConsoleAppStichpdfNeat.Config;
using ConsoleAppStichpdfNeat.NestedElements;
using log4net;

namespace ConsoleAppStichpdfNeat
{
    class PageOptimization
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(PageOptimization).Name);
        public static void tryPageCalculation(Track aTrack)
        {
            log.Info("----->>>>>>>>>>>inside tryPageCalculation() . . . begin <<<<<<");
            log.Info("aTrack=" + aTrack); //correct
            log.Info("---->>>>>>>>>>inside tryPageCalculation() . . . end <<<<");
            List<PageBound> pages = new List<PageBound>();
            for (int r=0; r < aTrack.raceList.Count; r++)
            {
                Console.WriteLine("RACE-" + (r + 1) + " " + aTrack.raceList[r]);
                Race<Horse> arace = aTrack.raceList[r];
                HeaderAndFirstHorse hf = arace.headerFirstHorse;
                Console.WriteLine("Grannular race-" + (r+1) + "HeaderAndFirstHorse=" + hf);
                for(int h=0; h < arace.secondAndOtherHorseList.Count; h++)
                {
                    Horse ahorse = arace.secondAndOtherHorseList[h];
                    Console.WriteLine("HORSE-" + (h + 2) + " " + ahorse);
                }
            }
            PageDetail pgDetail = new PageDetail();
            Console.WriteLine(pgDetail);
            //Console.WriteLine(Constants.lastPageMinimumFillout + " " + Constants.shrinkMax + " " + Constants.shrinkMin + " " + Constants.pageHeight);
            List<PageDetail> allpages =  optimizeSpace(aTrack);
            debugPrintAllPages(allpages);


        }

        private static void debugPrintAllPages(List<PageDetail> pgs)
        {
            foreach(PageDetail apage in pgs)
            {
                // Console.WriteLine("pd=" + pd);
                log.Info("apage=" + apage);
            }
        }


        private static void fitHeaderWithFirstHorse(PageDetail curr, HeaderAndFirstHorse hf)
        {
          if(curr.depthNotYetUsed == Config.Constants.PageHeight)
           {
            hf.firstHorse.positionOnPage.where = EntryLocationOnPage.FirstEntryOnPage;
           }
            curr.isthereAheader = true;
            curr.conjugate = hf;
            curr.runningDepth = curr.runningDepth + hf.header.height + hf.firstHorse.height;
            curr.depthNotYetUsed = curr.bottom - curr.runningDepth;
            hf.firstHorse.positionOnPage.leftspaceatEnd = curr.depthNotYetUsed;
            curr.entryCount = curr.entryCount + 2;
      }

        private static void fitAHorse(PageDetail curr, Horse ahorse)
        {
         if (curr.depthNotYetUsed == Config.Constants.PageHeight)
         {
            ahorse.positionOnPage.where = EntryLocationOnPage.FirstEntryOnPage;
         }
         curr.secondAndNextHorses.Add(ahorse);
            curr.runningDepth = curr.runningDepth + ahorse.height;
            curr.depthNotYetUsed = curr.bottom - curr.runningDepth;
            ahorse.positionOnPage.leftspaceatEnd = curr.depthNotYetUsed;
            curr.entryCount = curr.entryCount + 1;
        }



      private static void markLastHorseOnPage(PageDetail curr ) 
        {
          //critical pre-condition
          Horse lastOf2ndHorseList = curr.secondAndNextHorses.Last();
          Horse mayhaveFirstHorse = (curr.conjugate != null)? curr.conjugate.firstHorse : null;

         if ((mayhaveFirstHorse != null) && (mayhaveFirstHorse.raceNumber > lastOf2ndHorseList.raceNumber))
         {
            //new horse begin at the end of page [use case]
            mayhaveFirstHorse.positionOnPage.where = EntryLocationOnPage.LastEntryOnPage;
           // mayhaveFirstHorse.positionOnPage.leftspaceatEnd = curr.depthNotYetUsed; //check this. It should be done earlier. Not here
         } else
         {
            lastOf2ndHorseList.positionOnPage.where = EntryLocationOnPage.LastEntryOnPage;
           // lastOf2ndHorseList.positionOnPage.leftspaceatEnd = curr.depthNotYetUsed; //check this. It should be done earlier. Not here
         }         

        }
        

      //00000000000000000000--- MOST REFINED begin---000000000000000000000000000
      public static List<PageDetail> useOptimalSpace(List<Race<Horse>> listOfRace) //mimicked after tryPageCalculation() and enhanced
      {
         string raceStr = "";
         listOfRace.ForEach(r => raceStr += r);
         log.Info("PRE:" + raceStr);
         log.Info("@@@@@@@@@@@@@@@@@@@@@@Begin . . . useOptimalSpace ...PageDetail . . . . .1 @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@");
         List<PageDetail> pages = new List<PageDetail>();
         PageDetail firstPg = new PageDetail();
         pages.Add(firstPg);

         for (int r = 0; r < listOfRace.Count; r++)
         {
            Race<Horse> arace = listOfRace[r];
            //---------- fit header & 1st horse ----------------            
            HeaderAndFirstHorse hf = arace.headerFirstHorse;
            if ((hf.header.height + hf.firstHorse.height) > Constants.PageHeight)
               throw new Exception("Horse or Header size too big " + hf);

            PageDetail curr = pages.Last<PageDetail>();
            if (curr.depthNotYetUsed > (hf.header.height + hf.firstHorse.height))
            {
               fitHeaderWithFirstHorse(curr, hf);
            }
            else //hf does not fit at the bottom. So do 3 tasks: (1) mark last horse on page. (2) add a page. (3) fit hf for new race
            {
               markLastHorseOnPage(curr); //(1) //override here
               pages.Add(new PageDetail()); //(2)
               curr = pages.Last<PageDetail>();
               fitHeaderWithFirstHorse(curr, hf); //(3)
            }
            //-------- fit 2nd and other horses ----------
            for (int h = 0; h < arace.secondAndOtherHorseList.Count; h++)
            {
               Horse ahorse = arace.secondAndOtherHorseList[h];
               curr = pages.Last<PageDetail>();
               if (ahorse.height > Constants.PageHeight)
                  throw new Exception("Horse size too big " + ahorse);
               if (curr.depthNotYetUsed > ahorse.height)
               {
                  fitAHorse(curr, ahorse);
               }
               else //horse height is larger than leftover space
               {
                  //1. mark last horse on page. 2. Next, add a new page
                  markLastHorseOnPage(curr); //1
                  pages.Add(new PageDetail()); //2
                  curr = pages.Last<PageDetail>();
                  fitAHorse(curr, ahorse);
               }

            }

         }

         return pages;
      }
      //00000000000000---end---0000000000000000000

      public static List<PageDetail> optimizeSpace(Track aTrack) //mimicked after tryPageCalculation() and enhanced
        {
            log.Info("PRE:" + aTrack);
            Console.WriteLine("@@@@@@@@@@@@@@@@@@@@@@Begin . . . PageDetail . . . . .1 @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@");
            List<PageDetail> pages = new List<PageDetail>();
            PageDetail firstPg = new PageDetail();
            pages.Add(firstPg);

            for (int r = 0; r < aTrack.raceList.Count; r++)
            {
                Race<Horse> arace = aTrack.raceList[r];
                //---------- fit header & 1st horse ----------------            
                HeaderAndFirstHorse hf = arace.headerFirstHorse;
                if ((hf.header.height + hf.firstHorse.height) > Constants.pageHeightIn10thouMultiple)
                    throw new Exception("Horse or Header size too big " + hf);

                PageDetail curr = pages.Last<PageDetail>();
                if(curr.depthNotYetUsed > (hf.header.height + hf.firstHorse.height))
                {
                    fitHeaderWithFirstHorse(curr, hf);
                }
                else //hf does not fit at the bottom. So do 3 tasks: (1) mark last horse. (2) add a page. (3) fit hf for new race
                {
                    markLastHorseOnPage(curr); //(1) //override here
                    pages.Add(new PageDetail()); //(2)
                    curr = pages.Last<PageDetail>();
                    fitHeaderWithFirstHorse(curr, hf); //(3)
                }
                //-------- fit 2nd and other horses ----------
                for (int h = 0; h < arace.secondAndOtherHorseList.Count; h++)
                {
                    Horse ahorse = arace.secondAndOtherHorseList[h];
                    curr = pages.Last<PageDetail>();
                    if (ahorse.height > Constants.pageHeightIn10thouMultiple)
                        throw new Exception("Horse size too big " + ahorse);
                    if (curr.depthNotYetUsed > ahorse.height)
                    {
                        fitAHorse(curr, ahorse);
                    }
                    else //horse height is larger than leftover space
                    {
                        pages.Add(new PageDetail());
                        curr = pages.Last<PageDetail>();
                        fitAHorse(curr, ahorse);
                    }

                }
               // This call no need markLastHorseOfRaceOnPage(curr, EntryLocationOnPage.LastEntryMOP); //last horse per race. conditionally overridden if LastEntryMOP

            }

            return pages;
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
