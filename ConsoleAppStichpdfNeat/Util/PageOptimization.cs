using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConsoleAppStichpdfNeat.Util;
using ConsoleAppStichpdfNeat.Config;
using ConsoleAppStichpdfNeat.NestedElements;
using log4net;
using System.Diagnostics;

namespace ConsoleAppStichpdfNeat
{
    class PageOptimization
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(PageOptimization).Name);
        public  void tryPageCalculation(Track aTrack)
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
            curr.addHeaderAndFirstHorse(hf);
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
         curr.getLastHorseOnPage().positionOnPage.where = EntryLocationOnPage.LastEntryOnPage;

         /*
         //Debug.Print("TO-DO" + EntryLocationOnPage.LastEntryOnPage);
         //critical pre-condition
         Horse lastOfHeaderAndFirstHorseList = (curr.seeHeaderAndFirstHorseList != null) ? curr.seeHeaderAndFirstHorseList.Last().firstHorse : null;
         Horse lastOf2ndHorseList = curr.secondAndNextHorses.Last();
         if (lastOfHeaderAndFirstHorseList != null)
         {
            //2 possibilites when header on page
            if (lastOf2ndHorseList.raceNumber >= lastOfHeaderAndFirstHorseList.raceNumber)
            {
               lastOf2ndHorseList.positionOnPage.where = EntryLocationOnPage.LastEntryOnPage;
            }
            else
            {
               lastOfHeaderAndFirstHorseList.positionOnPage.where = EntryLocationOnPage.LastEntryOnPage;
            }

         }
         else
         {
            //no header on page
            lastOf2ndHorseList.positionOnPage.where = EntryLocationOnPage.LastEntryOnPage;
         }

         */

      }


      //00000000000000000000--- MOST REFINED begin---000000000000000000000000000
      public List<PageDetail> useOptimalSpace(List<Race<Horse>> listOfRace) //mimicked after tryPageCalculation() and enhanced
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

            if( (hf.header.height + hf.firstHorse.height) > Constants.PageHeight)
            {
               Debug.Print("header and firsthorse is bigger then pageHeight" + hf);
            }
            else if (curr.depthNotYetUsed > (hf.header.height + hf.firstHorse.height))
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

               if(ahorse.height > Constants.PageHeight)
               {
                  Debug.Print("horse [not 1st one] is bigger then pageHeight" + ahorse);
               }
               else if (curr.depthNotYetUsed > ahorse.height)
               {
                  fitAHorse(curr, ahorse);
               }
               else //horse height is larger than leftover space
               {
                  //1. mark last horse on page. 2. Next, add a new page. 3. Fit h at new pg
                  markLastHorseOnPage(curr); //1
                  pages.Add(new PageDetail()); //2
                  curr = pages.Last<PageDetail>();
                  fitAHorse(curr, ahorse); //3

               }

            }

         }
         if ((pages.Last().entryCount == 1) && pages.Last().secondAndNextHorses.Last().height < Constants.PageHeight / 3)
         {
            saveAPagebyshrinkLastHorseOfCard(pages);
         }
         //sparse must be after shrinkage
         sparseEntrieseOnSomePage(pages);

         return pages;
      }

      private static void saveAPagebyshrinkLastHorseOfCard(List<PageDetail> pages)
      {
         Horse lastHorseOftheCard = pages.Last().secondAndNextHorses.Last();
         PageDetail pgBeforeLast = pages[pages.Count - 2];
         if (isSqueezable(pgBeforeLast.depthNotYetUsed, lastHorseOftheCard.height))
         {
            Debug.WriteLine("saving last page by shrinking in saveAPagebyshrinkLastHorseOfCard");
            pgBeforeLast.secondAndNextHorses.Last().positionOnPage.where = EntryLocationOnPage.MiddleEntryOnPage; 
            squeezeHorseThenAdd(pgBeforeLast, lastHorseOftheCard, lastHorseOftheCard.height, pgBeforeLast.depthNotYetUsed);
            markLastHorseOnPage(pgBeforeLast); //override firstEntryOnPage to lastEntryonPage                     
            pages.RemoveAt(pages.Count - 1);

         }

      }

      public static void sparseEntrieseOnSomePage(List<PageDetail> pages)
      {
         foreach (PageDetail p in pages)
         {
            if (p.doesVeryLargeHorseBegin)
               continue;
            double residualSpace = p.getLastHorseOnPage().positionOnPage.leftspaceatEnd;
            if (residualSpace <= Constants.EvenSpaceMax)
            {
               double evenSpace = getEvenSpaceAmount(residualSpace, p.entryCount -1); //gap appears between adjacent entries
               int netEvenSpace = Convert.ToInt32( Math.Floor(evenSpace));
               if (p.seeHeaderAndFirstHorseList != null)
               {
                  p.seeHeaderAndFirstHorseList.ForEach(hf => {
                     hf.header.spCount = netEvenSpace;
                     if (hf.firstHorse.positionOnPage.where != EntryLocationOnPage.LastEntryOnPage)
                     {
                        hf.firstHorse.spCount = netEvenSpace;
                     }else
                     {
                        //no need this else block but calculating explicitly if caller needs
                        //last entry on the page
                        hf.firstHorse.spCount = Convert.ToInt32(Math.Floor(residualSpace - ((p.entryCount -1) * netEvenSpace )));
                     }
                  });
               }
               if (p.secondAndNextHorses != null)
               {
                  p.secondAndNextHorses.ForEach(h =>
                  {
                     if (h.positionOnPage.where != EntryLocationOnPage.LastEntryOnPage)
                     {
                        h.spCount = netEvenSpace;
                     }
                     else
                     {
                        //no need this else block but calculating explicitly if caller needs
                        //last entry on the page
                        h.spCount = Convert.ToInt32(Math.Floor(residualSpace - ((p.entryCount - 1) * netEvenSpace)));
                     }
                  });
               }
               //line below may not be needed discuss
               // p.getLastHorseOnPage().positionOnPage.leftspaceatEnd = 0; //override only last one's residual space

            }


         }

      }

      public static double getEvenSpaceAmount(double residual, int divisor)
      {
         return residual/ divisor;
      }


      /* INCOMPLETE Unused but useful code  to add in while loop for squeezing from the beginning of lastrace 
      private static void saveAPagebyshrinkLastHorseOfCard(List<PageDetail> pages)
      {
         Debug.WriteLine("7777 Inside saveAPagebyshrinkLastHorseOfCard()");
         //step 1. find index of Page where last race begin
         //what is this lamda |\ doing? Answer: First, finding the PageDetail (page) in which header of the last race exists. Then with this info finding the index of the pageDetail in 'pages' list
         int lastRaceNo = pages.Last().secondAndNextHorses.Last().raceNumber;
         int index = pages.FindIndex(p => {
            HeaderAndFirstHorse headerAndFirstHorseOfLastRace = null;
            if (p.seeHeaderAndFirstHorseList != null) //some page has no header
            {
               headerAndFirstHorseOfLastRace = p.seeHeaderAndFirstHorseList.Find(hf => hf.header.racenumber == lastRaceNo);
            }
            return headerAndFirstHorseOfLastRace != null;
         });

         //step 2. iterate and shrink from that spot
         shrinkForLastHorse(index, pages);


      }

      private static void shrinkForLastHorse(int index, List<PageDetail> pages)
      {
         Debug.WriteLine("8888 Inside shrinkForLastHorse()");
         int lastPgIndex = pages.Count - 1;
         for (int i = index; i <= lastPgIndex; i++)
         {

            PageDetail curr = pages[i];
            Horse lastHorseOnThisPg = curr.secondAndNextHorses.Last();
            PageDetail pnext = null;
            if( (i + 1) <= lastPgIndex)
            {
               pnext = pages[i + 1];
            }
            Horse firstHorseOnNextPg = pnext.secondAndNextHorses.First();
            if(isSqueezable(curr.depthNotYetUsed, firstHorseOnNextPg.height))
            {
               //act 1: rm existing lasthorseOnPg marking
               lastHorseOnThisPg.positionOnPage.where = EntryLocationOnPage.MiddleEntryOnPage; //important
               //act 2: fit horse on this page and mark as last
               squeezeHorseThenAdd(curr, firstHorseOnNextPg, firstHorseOnNextPg.height, curr.depthNotYetUsed);
               markLastHorseOnPage(curr);
               // act 3: correct/adjust next page
               remove1stHorseOfNxtPgAdjustStatistics(pnext);

            }
            Debug.WriteLine(pages[i]);

         }

      }

      private static void remove1stHorseOfNxtPgAdjustStatistics(PageDetail nxtPg)
      {
         double removedHeight = nxtPg.secondAndNextHorses.First().height;
         nxtPg.secondAndNextHorses.RemoveAt(0);
         nxtPg.entryCount = nxtPg.entryCount - 1; //removed a horse
         nxtPg.runningDepth = nxtPg.runningDepth - removedHeight;
         nxtPg.depthNotYetUsed = nxtPg.bottom - nxtPg.runningDepth;
         //make now 1st horse as 1st horse
         nxtPg.secondAndNextHorses.First().positionOnPage.where = EntryLocationOnPage.FirstEntryOnPage;
         //adjust last horse statistics
         nxtPg.secondAndNextHorses.Last().positionOnPage.leftspaceatEnd = nxtPg.depthNotYetUsed;
      }


      */

      private static bool isSqueezable(double depthNotYetUsed, double entryHeight)
      {
         log.Info("isSqueezable called() TO-DO");
         //return false; FOR DEBUGGING return here

         bool squeezable = (entryHeight <= (depthNotYetUsed + Constants.ShrinkMax)) ? true : false;
         return squeezable;
      }

      private static void squeezeHorseThenAdd(PageDetail curr, Horse entry, double entryHeight, double depthNotYetUsed)
      {

         double shrinkFactor = Constants.PageHeight / ( Constants.PageHeight - depthNotYetUsed + entryHeight);
         // 1a. srink existing entries. list of header+firsthorse
         if (curr.seeHeaderAndFirstHorseList != null)
         {
            curr.seeHeaderAndFirstHorseList.ForEach(hf => {
               hf.header.newHeight = shrinkFactor * hf.header.height;
               hf.firstHorse.newHeight = shrinkFactor * hf.firstHorse.height;
            });
         }
         //1b. other horses
         curr.secondAndNextHorses.ForEach(h => h.newHeight = shrinkFactor * h.height);
         //2. shrink current|last horse, add page statistic. Then add the horse to page
         entry.newHeight = shrinkFactor * entry.height;
         curr.runningDepth = Constants.PageHeight;
         curr.depthNotYetUsed = 0;
         entry.positionOnPage.leftspaceatEnd = curr.depthNotYetUsed;
         curr.entryCount = curr.entryCount + 1;
         curr.secondAndNextHorses.Add(entry);        
         
      }
      private static void squeezeHeaderFirsthorseThenAdd(PageDetail curr, HeaderAndFirstHorse hf, double entryHeight, double depthNotYetUsed)
      {
         double shrinkFactor = Constants.PageHeight / (Constants.PageHeight - depthNotYetUsed + entryHeight);


         //1. srink existing secondAndNextHorses list
         curr.secondAndNextHorses.ForEach(h => h.newHeight = shrinkFactor * h.height);
         //2. srink existing shrink header items
         if(curr.seeHeaderAndFirstHorseList != null)
         {
            curr.seeHeaderAndFirstHorseList.ForEach( entry => {
                                                                entry.firstHorse.newHeight = shrinkFactor * entry.firstHorse.height;
                                                                entry.header.newHeight = shrinkFactor * entry.header.height;
                                                      });
         }
         //3. shrink HeaderAndFirstHorse before putting
         hf.firstHorse.newHeight = shrinkFactor * hf.firstHorse.height;
         hf.header.newHeight = shrinkFactor * hf.firstHorse.height;         
         //addd the hf to page
         curr.addHeaderAndFirstHorse(hf);
         //set the page statistic
         curr.runningDepth = Constants.PageHeight;
         curr.depthNotYetUsed = 0;
         hf.firstHorse.positionOnPage.leftspaceatEnd = curr.depthNotYetUsed;
         curr.isthereAheader = true;
         curr.entryCount = curr.entryCount + 2;
         //calling method responsible to set as last entry on page


      }
      //00000000000000---end---0000000000000000000

      public List<PageDetail> optimizeSpace(Track aTrack) //mimicked after tryPageCalculation() and enhanced
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
