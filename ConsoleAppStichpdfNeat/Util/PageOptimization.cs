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
         } else
         {
            lastOf2ndHorseList.positionOnPage.where = EntryLocationOnPage.LastEntryOnPage;
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
            PageDetail curr = pages.Last<PageDetail>();

            if (curr.depthNotYetUsed > (hf.header.height + hf.firstHorse.height))
            {
               fitHeaderWithFirstHorse(curr, hf);
            }
            else //hf does not fit at the bottom. So do 3 tasks: (1) mark last horse on page. (2) add a page. (3) fit hf for new race
            {
               if (isSqueezable(curr.depthNotYetUsed, (hf.header.height + hf.firstHorse.height)))
               {
                  log.Info("isSqueezable TODO squeezeheaderFirsthorse");
                  squeezeHeaderFirsthorse(curr, hf, (hf.header.height + hf.firstHorse.height), curr.depthNotYetUsed); 
                  markLastHorseOnPage(curr);
               }
               else
               {
                  markLastHorseOnPage(curr); //(1) //override here
                  pages.Add(new PageDetail()); //(2)
                  curr = pages.Last<PageDetail>();
                  fitHeaderWithFirstHorse(curr, hf); //(3)
               }
            }
            //-------- fit 2nd and other horses ----------
            for (int h = 0; h < arace.secondAndOtherHorseList.Count; h++)
            {
               Horse ahorse = arace.secondAndOtherHorseList[h];
               curr = pages.Last<PageDetail>();

               if (curr.depthNotYetUsed > ahorse.height)
               {
                  fitAHorse(curr, ahorse);
               }
               else //horse height is larger than leftover space
               {
                  if (isSqueezable(curr.depthNotYetUsed, ahorse.height))
                  {
                     log.Info("isSqueezable TODO");
                     squeezeHorse(curr, ahorse, ahorse.height, curr.depthNotYetUsed); 
                     markLastHorseOnPage(curr);
                  }
                  else
                  {
                     //1. mark last horse on page. 2. Next, add a new page. 3. Fit h at new pg
                     markLastHorseOnPage(curr); //1
                     pages.Add(new PageDetail()); //2
                     curr = pages.Last<PageDetail>();
                     fitAHorse(curr, ahorse); //3
                  }
               }

            }

         }

         return pages;
      }

      private static bool isSqueezable(double depthNotYetUsed, double entryHeight)
      {
         log.Info("isSqueezable called() TO-DO");
         //return false; FOR DEBUGGING return here

         bool squeezable = (entryHeight <= (depthNotYetUsed + Constants.ShrinkMax)) ? true : false;
         return squeezable;
      }

      private static void squeezeHorse(PageDetail curr, Horse entry, double entryHeight, double depthNotYetUsed)
      {
         double shrinkFactor = Constants.PageHeight / ( Constants.PageHeight - depthNotYetUsed + entryHeight);
         // 1a. srink existing entries. header + firsthorse
         if (curr.conjugate != null)
         {
            curr.conjugate.header.newHeight = shrinkFactor * curr.conjugate.header.height;
            curr.conjugate.firstHorse.newHeight = shrinkFactor * curr.conjugate.firstHorse.height;
         }
         //1b. other horses
         curr.secondAndNextHorses.ForEach(h => h.newHeight = shrinkFactor * h.height);
         //2. shrink current and add in
         entry.newHeight = shrinkFactor * entry.height;
         curr.runningDepth = Constants.PageHeight;
         curr.depthNotYetUsed = 0;
         entry.positionOnPage.leftspaceatEnd = curr.depthNotYetUsed;
         curr.entryCount = curr.entryCount + 1;
         curr.secondAndNextHorses.Add(entry);
         //calling method will set it as last entry on page
      }
      private static void squeezeHeaderFirsthorse(PageDetail curr, HeaderAndFirstHorse hf, double entryHeight, double depthNotYetUsed)
      {
         double shrinkFactor = Constants.PageHeight / (Constants.PageHeight - depthNotYetUsed + entryHeight);
         //1. add and srink across         
         curr.conjugate = hf;
         curr.conjugate.header.newHeight = shrinkFactor * curr.conjugate.header.height;
         curr.conjugate.firstHorse.newHeight = shrinkFactor *  curr.conjugate.firstHorse.height;
         curr.secondAndNextHorses.ForEach(h => h.newHeight = shrinkFactor * h.height);
         curr.isthereAheader = true;         
         curr.runningDepth = Constants.PageHeight;
         curr.depthNotYetUsed = 0;
         hf.firstHorse.positionOnPage.leftspaceatEnd = curr.depthNotYetUsed;
         curr.entryCount = curr.entryCount + 2;
         //calling method responsible to set as last entry on page

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
