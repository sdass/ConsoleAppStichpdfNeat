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
      private int pgNumAtLastline; // holds page number on which "LAST TEXT LINE" of a horse fitted. !important explanation for very large horse
        private static readonly ILog log = LogManager.GetLogger(typeof(PageOptimization).Name);

      /* old implementation
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
      */

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
            hf.firstHorse.pgno = curr.pgNum;
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
         ahorse.pgno = curr.pgNum;
        }


      private void fitA_Large_horse(PageDetail curr, Horse ahorse, List<PageDetail> pages) //horse bigger than full pageHeight
      {
         if (curr.depthNotYetUsed == Config.Constants.PageHeight)
         {
            ahorse.positionOnPage.where = EntryLocationOnPage.FirstEntryOnPage;
         }
         //curr.secondAndNextHorses.Add(ahorse); // ???? here or at the ending page bring down // part of stats go in end page
         
         int endpageCount = ( ( (ahorse.height - curr.depthNotYetUsed) % Config.Constants.PageHeight) > 0) ? 1 : 0; //0 or 1
         int middlePageCount = Convert.ToInt16(Math.Floor((ahorse.height - curr.depthNotYetUsed) / Config.Constants.PageHeight)); // on or more
         int totalpageSpanned = 1 + middlePageCount + endpageCount; //begin page + middle pages + end page // for debugging
         Debug.Print("totalpageSpanned: " + totalpageSpanned);

         double contentGoesOnBeginPage = curr.depthNotYetUsed;
         //beginning page
         //no horse statistics
         curr.runningDepth = Config.Constants.PageHeight;
         curr.depthNotYetUsed = 0;
         curr.doesVeryLargeHorseBegin = true;         

         //full middle pages
         for(int i = 1; i<= middlePageCount; i++)
         {
            PageDetail midPg = new PageDetail(++pgNumAtLastline);
            pages.Add(midPg);
            midPg.runningDepth = Config.Constants.PageHeight;
            midPg.depthNotYetUsed = 0;
            midPg.doesVeryLargeHorseMiddle = true;
            if ( (endpageCount == 0) && (i == middlePageCount) )
            { //horse fit at the endline of this page : use case 1
               midPg.secondAndNextHorses.Add(ahorse);
               ahorse.positionOnPage.leftspaceatEnd = 0;
               midPg.doesVeryLargeHorseEnd = true;
               midPg.entryCount = midPg.entryCount + 1;
               ahorse.pgno = midPg.pgNum;
            }
         }//for

         //ending page (partly empty)
         if (endpageCount == 1) // has an ending page: use case horse fit on a last space but has space at the end
         {
            PageDetail lastPg = new PageDetail(++pgNumAtLastline);
            pages.Add(lastPg);
            lastPg.secondAndNextHorses.Add(ahorse);
            lastPg.runningDepth = lastPg.runningDepth + (ahorse.height - contentGoesOnBeginPage - (middlePageCount * Constants.PageHeight));
            lastPg.depthNotYetUsed = lastPg.bottom - lastPg.runningDepth;
            lastPg.doesVeryLargeHorseEnd = true;
            ahorse.positionOnPage.leftspaceatEnd = lastPg.depthNotYetUsed;
            lastPg.entryCount = lastPg.entryCount + 1;
            ahorse.pgno = lastPg.pgNum;
         }

      }


      private void fitA_header_with_Large_1sthorse(PageDetail curr, HeaderAndFirstHorse hf, List<PageDetail> pages)
      {
         if(curr.depthNotYetUsed == Constants.PageHeight)
         {
            hf.firstHorse.positionOnPage.where = EntryLocationOnPage.FirstEntryOnPage;
         }
         //decision 1: if space left does not fillup header + (1 x header height of horse content) give page break and start laying hf on next page
         if(curr.depthNotYetUsed < 2 * hf.header.height)
         {
            //don't layout. give page break. start on a brand new page
            markLastHorseOnPage(curr);
            //create new page below and start laying out
            int partiallyUsedBeginpage = 0; // partly filled initial page is none
            //brand new pages
            int middlePageCount = Convert.ToInt16(Math.Floor((hf.firstHorse.height + hf.header.height ) / Constants.PageHeight)); // 1 or more
            int endPageCount = (((hf.header.height + hf.firstHorse.height) % Config.Constants.PageHeight) > 0) ? 1 : 0; // 0 or 1
            int totalPageSpanned = partiallyUsedBeginpage + middlePageCount + endPageCount;
            Debug.Print("Header-first-horse begins on new page totalPageSpanned: " + totalPageSpanned);
            double contentGoesOnBeginPage = 0; // because started on a brand new page [notion: middle page filling up full-height]
            for (int i = 1; i <= middlePageCount; i++)
            {
               PageDetail midPg = new PageDetail(++pgNumAtLastline);
               pages.Add(midPg);
               midPg.runningDepth = Constants.PageHeight;
               midPg.depthNotYetUsed = 0;
               if (i == 1)
               {
                  midPg.doesVeryLargeHorseBegin = true;
               }
               else
               {
                  midPg.doesVeryLargeHorseMiddle = true;
               }
               if ((endPageCount == 0) && (i == middlePageCount))
               { //horse fit at the endline of this page: use case 1
                  midPg.addHeaderAndFirstHorse(hf);
                  hf.firstHorse.positionOnPage.leftspaceatEnd = 0;
                  midPg.doesVeryLargeHorseEnd = true;
                  midPg.entryCount = midPg.entryCount + 2;
                  hf.firstHorse.pgno = midPg.pgNum;

               }
            }//for
            // if block when horse fills up part of a ending page
            if (endPageCount == 1) // has an ending page. usecase: horse fit on a last page but has space at the end
            {
               PageDetail lastPg = new PageDetail(++pgNumAtLastline);
               pages.Add(lastPg);
               lastPg.addHeaderAndFirstHorse(hf);
               lastPg.runningDepth = lastPg.runningDepth + (hf.header.height + hf.firstHorse.height - contentGoesOnBeginPage - (middlePageCount * Constants.PageHeight));
               lastPg.depthNotYetUsed = lastPg.bottom - lastPg.runningDepth;
               lastPg.doesVeryLargeHorseEnd = true;
               hf.firstHorse.positionOnPage.leftspaceatEnd = lastPg.depthNotYetUsed;
               hf.firstHorse.pgno = lastPg.pgNum;

            }


         }
         else
         {
            //start laying on begin page because has enough space for header and good size of first horse content
            // TO-DO
            int endPageCount = ( ( (hf.header.height + hf.firstHorse.height - curr.depthNotYetUsed) % Config.Constants.PageHeight)  > 0) ? 1 : 0; // 0 or 1
            int middlePageCount = Convert.ToInt16 (Math.Floor( (hf.firstHorse.height + hf.header.height - curr.depthNotYetUsed ) /Constants.PageHeight) ); // 1 or more
            int partiallyUsedBeginpage = 1;
            int totalPageSpanned = partiallyUsedBeginpage + middlePageCount + endPageCount;
            Debug.Print("Header-first-horse begins on existing page totalPageSpanned: " + totalPageSpanned);
            double contentGoesOnBeginPage = curr.depthNotYetUsed;
            //on beginning page no header or horse statistics. Wait to give on to the last page
            curr.runningDepth = Constants.PageHeight;
            curr.depthNotYetUsed = 0;
            curr.doesVeryLargeHorseBegin = true;

            //full middle pages
            for(int i = 1; i<= middlePageCount; i++)
            {
               PageDetail midPg = new PageDetail(++pgNumAtLastline);
               pages.Add(midPg);
               midPg.runningDepth = Constants.PageHeight;
               midPg.depthNotYetUsed = 0;
               midPg.doesVeryLargeHorseMiddle = true;
               if( (endPageCount == 0) && (i == middlePageCount))
               { //horse fit at the endline of this page: use case 1
                  midPg.addHeaderAndFirstHorse(hf);
                  hf.firstHorse.positionOnPage.leftspaceatEnd = 0;
                  midPg.doesVeryLargeHorseEnd = true;
                  midPg.entryCount = midPg.entryCount + 2;
                  hf.firstHorse.pgno = midPg.pgNum;

               }

            }
            // if block when horse occupies part of a ending page
            if(endPageCount == 1) // has an ending page. usecase: horse fit on a last page but has space at the end
            {
               PageDetail lastPg = new PageDetail(++pgNumAtLastline);
               pages.Add(lastPg);
               lastPg.addHeaderAndFirstHorse(hf);
               lastPg.runningDepth = lastPg.runningDepth + (hf.header.height + hf.firstHorse.height - contentGoesOnBeginPage - (middlePageCount * Constants.PageHeight));
               lastPg.depthNotYetUsed = lastPg.bottom - lastPg.runningDepth;
               lastPg.doesVeryLargeHorseEnd = true;
               hf.firstHorse.positionOnPage.leftspaceatEnd = lastPg.depthNotYetUsed;
               hf.firstHorse.pgno = lastPg.pgNum;

            }

         }
      }

      private static void markLastHorseOnPage(PageDetail curr ) 
        {
         if(curr.getLastHorseOnPage() != null)
            curr.getLastHorseOnPage().positionOnPage.where = EntryLocationOnPage.LastEntryOnPage;

      }


      //00000000000000000000--- MOST REFINED begin---000000000000000000000000000
      public List<PageDetail> useOptimalSpace(List<Race<Horse>> listOfRace) //mimicked after tryPageCalculation() and enhanced
      {
         string raceStr = "";
         listOfRace.ForEach(r => raceStr += r);
         log.Info("PRE:" + raceStr);
         log.Info("@@@@@@@@@@@@@@@@@@@@@@Begin . . . useOptimalSpace ...PageDetail . . . . .1 @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@");
         List<PageDetail> pages = new List<PageDetail>();
         PageDetail firstPg = new PageDetail(++pgNumAtLastline);
         pages.Add(firstPg);

         for (int r = 0; r < listOfRace.Count; r++)
         {
            Race<Horse> arace = listOfRace[r];
            //---------- fit header & 1st horse ----------------            
            HeaderAndFirstHorse hf = arace.headerFirstHorse;
            PageDetail curr = pages.Last<PageDetail>();

            if( (hf.header.height + hf.firstHorse.height) > Constants.PageHeight)
            {
               Debug.Print("LARGE_ENTRY::header and firsthorse is bigger than pageHeight" + hf);
               fitA_header_with_Large_1sthorse(curr, hf, pages);
            }
            else if (curr.depthNotYetUsed >= (hf.header.height + hf.firstHorse.height))
            {
               fitHeaderWithFirstHorse(curr, hf);
            }
            else if ((curr.depthNotYetUsed + Constants.SHRINK_THRESHOLD_ONE_HORSE + 1) > (hf.header.height + hf.firstHorse.height))
            {
               //shrink and fit a head and firsthorse
               shrinkFitAHeader1stHorseAtTheBottom(curr, hf);

            }
            else //hf does not fit at the bottom. So do 3 tasks: (1) mark last horse on page. (2) add a page. (3) fit hf for new race
            {
               markLastHorseOnPage(curr); //(1) //override here
               pages.Add(new PageDetail(++pgNumAtLastline)); //(2)
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
                  Debug.Print("LARGE_ENTRY::horse [not 1st one] is bigger than pageHeight" + ahorse);
                  fitA_Large_horse(curr, ahorse, pages);
               }
               else if (curr.depthNotYetUsed >= ahorse.height)
               {
                  fitAHorse(curr, ahorse);
               }
               
               else if ((curr.depthNotYetUsed + Constants.SHRINK_THRESHOLD_ONE_HORSE + 1) > ahorse.height)
               {
                  //shrink and fit a horse
                  shrinkFitAHorseAtTheBottom(curr, ahorse);

               }
               
               else //horse height is larger than leftover space
               {
                  //1. mark last horse on page. 2. Next, add a new page. 3. Fit h at new pg
                  markLastHorseOnPage(curr); //1
                  pages.Add(new PageDetail(++pgNumAtLastline)); //2
                  curr = pages.Last<PageDetail>();
                  fitAHorse(curr, ahorse); //3

               }

            }

         }

         log.Info("see how it lays:");
         pages.ForEach(p => log.Info("debug<< " + p));
         log.Info("end debug:");

         if ((pages.Last().entryCount == 1) && pages.Last().secondAndNextHorses.Last().height < Constants.PageHeight / 3)
         {
            saveAPagebyshrinkLastHorseOfCard(pages);
         }
         //sparse must be after shrinkage
         sparseEntrieseOnSomePage(pages);

         //pages.ForEach(p => log.Info("debug<< " + p));
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

      private void shrinkFitAHorseAtTheBottom(PageDetail curr, Horse ahorse) // if needed only <= 10 dots
      {
         if (curr.depthNotYetUsed == Constants.PageHeight)
         {
            ahorse.positionOnPage.where = EntryLocationOnPage.FirstEntryOnPage;
         }
         ahorse.newHeight = ahorse.height - (ahorse.height - curr.depthNotYetUsed);
         curr.secondAndNextHorses.Add(ahorse);
         curr.runningDepth = Constants.PageHeight;
         curr.depthNotYetUsed = curr.bottom - curr.runningDepth;
         ahorse.positionOnPage.leftspaceatEnd = curr.depthNotYetUsed;
         curr.entryCount = curr.entryCount + 1;
         ahorse.pgno = curr.pgNum;
         ahorse.positionOnPage.where = EntryLocationOnPage.LastEntryOnPage;
      }

      private void shrinkFitAHeader1stHorseAtTheBottom(PageDetail curr, HeaderAndFirstHorse hf) // if needed only <= 10 dots
      {
         if (curr.depthNotYetUsed == Constants.PageHeight)
         {
            hf.firstHorse.positionOnPage.where = EntryLocationOnPage.FirstEntryOnPage;
         }
         double spaceToShrinkPerEntry = (hf.header.height + hf.firstHorse.height - curr.depthNotYetUsed) / 2;
         hf.header.newHeight = hf.header.newHeight - spaceToShrinkPerEntry;
         hf.firstHorse.newHeight = hf.firstHorse.newHeight - spaceToShrinkPerEntry;
         curr.addHeaderAndFirstHorse(hf);
         curr.runningDepth = Constants.PageHeight;
         curr.depthNotYetUsed = curr.bottom - curr.runningDepth;
         hf.firstHorse.positionOnPage.leftspaceatEnd = curr.depthNotYetUsed;
         curr.entryCount = curr.entryCount + 2;
         hf.firstHorse.pgno = curr.pgNum;
         hf.firstHorse.positionOnPage.where = EntryLocationOnPage.LastEntryOnPage;         
      }

      public static void sparseEntrieseOnSomePage(List<PageDetail> pages)
      {
         foreach (PageDetail p in pages)
         {
            if (p.doesVeryLargeHorseBegin || p.doesVeryLargeHorseMiddle)
               continue;
            double residualSpace = p.getLastHorseOnPage().positionOnPage.leftspaceatEnd;
            if (residualSpace <= Constants.MIN_SPACE_FOR_HEIGHT) // <= 10 dots //for lastentry on page set leftspaceatEnd=0 and do no calculation
            {
               p.getLastHorseOnPage().positionOnPage.leftspaceatEnd = 0;
            }
            /*
            else if (residualSpace > Constants.MIN_SPACE_FOR_HEIGHT && residualSpace <= Constants.MAX_SPACE_FOR_HEIGHT) // > 30 and <= 100 dots
            {
               //adjust to newHeights and lastentry on page set leftspaceatEnd=0
               //change newHeight
               if (p.secondAndNextHorses != null)
               {
                  p.secondAndNextHorses.ForEach(h => h.newHeight = h.height + (residualSpace / p.entryCount)); //do to all entries
               }
               if (p.seeHeaderAndFirstHorseList != null)
               {
                  //do to all entries
                  p.seeHeaderAndFirstHorseList.ForEach(hf => {
                     hf.header.newHeight = hf.header.height + (residualSpace / p.entryCount);

                     hf.firstHorse.newHeight = hf.firstHorse.height + (residualSpace / p.entryCount);
                  });
               }
               p.getLastHorseOnPage().positionOnPage.leftspaceatEnd = 0;
            }
            */
            else if (residualSpace > Constants.MIN_SPACE_FOR_HEIGHT && residualSpace <= Constants.EvenSpaceMax) // > 10 and <= 150  dots
            {
               // divide up the space evenly between etnties and for lastentry on page set leftspaceatEnd = 0
               double evenSpace = 0;
               if (p.entryCount > 1)
               {
                  evenSpace = getEvenSpaceAmount(residualSpace, p.entryCount - 1); //gap appears between adjacent entries
               }
               double netEvenSpace = evenSpace;
               if (p.seeHeaderAndFirstHorseList != null)
               {
                  p.seeHeaderAndFirstHorseList.ForEach(hf => {
                     hf.header.spCount = netEvenSpace;
                     if (hf.firstHorse.positionOnPage.where != EntryLocationOnPage.LastEntryOnPage)
                     {
                        hf.firstHorse.spCount = netEvenSpace;
                     }
                     //for last entry no space at end
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
                     //for last entry no space at end
                  });
               }
               p.getLastHorseOnPage().positionOnPage.leftspaceatEnd = 0; //override only last one's residual space

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
         entry.pgno = curr.pgNum;
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

         /* old implementation
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
      */

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
