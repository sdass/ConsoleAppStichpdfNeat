using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConsoleAppStichpdfNeat.NestedElements;
using log4net;
using ConsoleAppStichpdfNeat.Util;

namespace ConsoleAppStichpdfNeat
{
   class PdfPageCalculation
   {
      private static readonly ILog log = LogManager.GetLogger(typeof(PdfPageCalculation).Name);
      public List<CardStruct.Race> calculate(List<CardStruct.Race> races) //***
      {

         // CardStruct.debugPrintACard(raceList);
         List<Race<Horse>> raceList = prepInput(races);
         log.Info(raceList);
         List<CardStruct.Race> raceout = processToFitOnPageAndReturn(raceList);
         
         System.Diagnostics.Debug.WriteLine("++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++\n");
         // CardStruct.debugPrintACard(raceout);

         return raceout;
      }

      private List<Race<Horse>> prepInput(List<CardStruct.Race> praces) //***
      {
         List<Race<Horse>> raceList = new List<Race<Horse>>();
         foreach (CardStruct.Race race in praces)
         {
            Race<Horse> cRace = new Race<Horse>();

            //secure header & 1st horse
            CardStruct.HorseOrHeader header = race.allHorsesWithRaceHeader[0];
            CardStruct.HorseOrHeader firstHorse = race.allHorsesWithRaceHeader[1];
            HeaderAndFirstHorse headerWithFirstHorse = new HeaderAndFirstHorse(new Header(header), new Horse(firstHorse));
            cRace.setRaceTop(headerWithFirstHorse);

            //next usual iteration below
            for (int i = 2; i < race.allHorsesWithRaceHeader.Count; i++)
            {
               cRace.addHorse(new Horse(race.allHorsesWithRaceHeader[i]));
            }

            raceList.Add(cRace);
         }
         return raceList;
      }

      private List<CardStruct.Race> processToFitOnPageAndReturn(List<Race<Horse>> races) //***
      {
         //TO_DO
         //debugPrintRace(raceList);
         List<Race<Horse>> outRaceList = doProcessList(races);
         List<CardStruct.Race> outRaces = new List<CardStruct.Race>();
         //int i = 0;
         foreach (Race<Horse> race in outRaceList)
         {
            CardStruct.Race arace = new CardStruct.Race();
            arace.allHorsesWithRaceHeader = new List<CardStruct.HorseOrHeader>();
            arace.raceNumber = "" + race.headerFirstHorse.firstHorse.raceNumber;

            //copy header  
            CardStruct.HorseOrHeader header = prepareHeaderStruct(race.headerFirstHorse.header);
            arace.allHorsesWithRaceHeader.Add(header);

            //copy first-horse
            CardStruct.HorseOrHeader horse = prepareHorseStruct(race.headerFirstHorse.firstHorse);
            arace.allHorsesWithRaceHeader.Add(horse);

            //copy 2nd and all horses
            foreach (Horse h in race.secondAndOtherHorseList)
            {
               horse = prepareHorseStruct(h);
               arace.allHorsesWithRaceHeader.Add(horse);
            }

            outRaces.Add(arace);
         }
         //mark last horse of last race.
         
         return outRaces;
      }

      private List<Race<Horse>> doProcessList(List<Race<Horse>> races)
      {

         List<Util.PageDetail> pages =  TestHarness.useOptimalSpace(races);
         log.Info("DEBUG PRINTING DEBUG PRINTING DEBUG PRINTING DEBUG PRINTING DEBUG PRINTING ");
         debugPrintPgDetails(pages);
         //travesingPagesAsInPrintedCopy(pages); //works
         List<CardStruct.HorseOrHeader> entries = packEntiresInPrintOder(pages);
         entries.ForEach(e => { string hh = e.IsHeader ? "Header" : "Horse"; log.Info("id=" + e.Id + " racenum=" + e.racenum + "<" + hh + ">"); } );
         return races; //call process class before
      }

      private CardStruct.HorseOrHeader prepareHeaderStruct(Header head)
      {
         CardStruct.HorseOrHeader header = new CardStruct.HorseOrHeader();
         header.Id = head.id;
         header.IsHeader = true;
         header.Height = head.height;
         header.NewHeight = head.newHeight;
         header.racenum = head.racenumber;
         header.IsHeader = head.isHeader;
         header.pgnum = -1; //1st horse's pg # = header's page number
         
         return header;
      }

      private CardStruct.HorseOrHeader prepareHorseStruct(Horse h)
      {
         CardStruct.HorseOrHeader horse = new CardStruct.HorseOrHeader();
         horse.Id = h.id;
         horse.Height = h.height;
         horse.NewHeight = h.newHeight;
         horse.racenum = h.raceNumber;
         horse.IsFirstHorseOfRace = h.isFirstHorseOfRace;
         horse.IsLastHorseOfRace = h.isLastHorseOfRace;
         horse.IsLastHorseOfRace = h.isLastHorseOfRace;
         horse.IsLastHorseOfTheCard = h.IsLastHorseOfTheCard;
         //firstHorse.pgnum = 
         horse.ResidualSpace = h.positionOnPage.leftspaceatEnd;
         /* other item based on logic */
         return horse;

      }

      public static void debugPrintPgDetails(List<PageDetail> pglist)
      {
         log.Info("-------******* Begin debugging *****------");
         pglist.ForEach(pg => log.Info(pg));
         log.Info("-------******* End debugging *****------");         
      }

      private static void travesingPagesAsInPrintedCopy(List<PageDetail> pglist)
      {
         log.Info("::::::::::travesingPagesAsInPrintedCopy():::::::::::");
         pglist.ForEach(pg =>
         {
            PageDetail p = pg;
            if (!p.isthereAheader)
            {
               log.Info(":::pg-begin-A::::: ");
               p.secondAndNextHorses.ForEach(horse => log.Info(horse));
               log.Info("::::pg-end:::: ");
            }
            else if ((p.isthereAheader) && (p.conjugate.firstHorse.raceNumber > p.secondAndNextHorses.First<Horse>().raceNumber))
            {
               log.Info(":::pg-begin-B:::::2ndhorselist has lower num ");
               List<Horse> secondAndNexthorseList = p.secondAndNextHorses;

               bool headerFirstHorseNotProcessed = true;
               for (int i = 0; i < secondAndNexthorseList.Count; i++)
               {
                  if (headerFirstHorseNotProcessed)
                  {
                     if (secondAndNexthorseList[i].raceNumber == p.conjugate.firstHorse.raceNumber)
                     {
                        headerFirstHorseNotProcessed = false;
                        log.Info(p.conjugate.header);
                        log.Info(p.conjugate.firstHorse);
                     }
                     log.Info(secondAndNexthorseList[i]);
                  }
                  else
                  {
                     log.Info(secondAndNexthorseList[i]);
                  }

               } //for ends
               /* if still headerFirstHorseNotProcessed is true then use case means: new|higher race # on header-first (at end
                * of page). Logic: horse.p.conjugate.firstHorse.raceNumber > all other race number on page. process as below.
               */
               if (headerFirstHorseNotProcessed)
               {
                  headerFirstHorseNotProcessed = false;
                  log.Info(p.conjugate.header);
                  log.Info(p.conjugate.firstHorse);
                  log.Info("::::new race at page-end:::: ");
               }

               log.Info("::::pg-end:::: ");
            }
            else if ((p.isthereAheader) && (p.conjugate.firstHorse.raceNumber <= p.secondAndNextHorses.First<Horse>().raceNumber)) //else equivalent
            {
               log.Info(":::pg-begin-C::::: ");
               log.Info(p.conjugate.header);
               log.Info(p.conjugate.firstHorse);
               p.secondAndNextHorses.ForEach(horse => log.Info(horse));
               log.Info("::::pg-end:::: ");

            }

         });

         log.Info("-------******* End travesingPagesAsInPrintCopy() *****------");
      }

      //mimicking below from traversingPagesInPrintedCopy()
      private static List<CardStruct.HorseOrHeader> packEntiresInPrintOder(List<PageDetail> pglist)
      {
         List<CardStruct.HorseOrHeader> allentries = new List<CardStruct.HorseOrHeader>();
         log.Info("::::::::::packEntiresInPrintOder():::::::::::");
         pglist.ForEach(pg =>
         {
            PageDetail p = pg;
            if (!p.isthereAheader)
            {
               log.Info(":::pg-begin-A::::: ");
               p.secondAndNextHorses.ForEach(horse => { log.Info(horse); allentries.Add(getHorseHeaderFromHorse(horse)); });
               log.Info("::::pg-end:::: ");
            }
            else if ((p.isthereAheader) && (p.conjugate.firstHorse.raceNumber > p.secondAndNextHorses.First<Horse>().raceNumber))
            {
               log.Info(":::pg-begin-B:::::2ndhorselist has lower num ");
               List<Horse> secondAndNexthorseList = p.secondAndNextHorses;

               bool headerFirstHorseNotProcessed = true;
               for (int i = 0; i < secondAndNexthorseList.Count; i++)
               {
                  if (headerFirstHorseNotProcessed)
                  {
                     if (secondAndNexthorseList[i].raceNumber == p.conjugate.firstHorse.raceNumber)
                     {
                        headerFirstHorseNotProcessed = false;
                        log.Info(p.conjugate.header); allentries.Add(getHorseHeaderFromHeader(p.conjugate.header));
                        log.Info(p.conjugate.firstHorse); allentries.Add(getHorseHeaderFromHorse(p.conjugate.firstHorse));
                     }
                     log.Info(secondAndNexthorseList[i]); allentries.Add(getHorseHeaderFromHorse(secondAndNexthorseList[i]));
                  }
                  else
                  {
                     log.Info(secondAndNexthorseList[i]); allentries.Add(getHorseHeaderFromHorse(secondAndNexthorseList[i]));
                  }

               } //for ends
               /* if still headerFirstHorseNotProcessed is true then use case means: new|higher race # on header-first (at end
                * of page). Logic: horse.p.conjugate.firstHorse.raceNumber > all other race number on page. process as below.
               */
               if (headerFirstHorseNotProcessed)
               {
                  headerFirstHorseNotProcessed = false;
                  log.Info(p.conjugate.header); allentries.Add(getHorseHeaderFromHeader(p.conjugate.header));
                  log.Info(p.conjugate.firstHorse); allentries.Add(getHorseHeaderFromHorse(p.conjugate.firstHorse));
                  log.Info("::::new race at page-end:::: ");
               }

               log.Info("::::pg-end:::: ");
            }
            else if ((p.isthereAheader) && (p.conjugate.firstHorse.raceNumber <= p.secondAndNextHorses.First<Horse>().raceNumber)) //else equivalent
            {
               log.Info(":::pg-begin-C::::: ");
               log.Info(p.conjugate.header); allentries.Add(getHorseHeaderFromHeader(p.conjugate.header));
               log.Info(p.conjugate.firstHorse); allentries.Add(getHorseHeaderFromHorse(p.conjugate.firstHorse));
               p.secondAndNextHorses.ForEach(horse => { log.Info(horse); allentries.Add(getHorseHeaderFromHorse(horse)); });
               log.Info("::::pg-end:::: ");

            }

         });

         log.Info("-------******* End packEntiresInPrintOder() *****------");
         return allentries;
      }


     private static CardStruct.HorseOrHeader getHorseHeaderFromHorse(Horse h)
      {
         CardStruct.HorseOrHeader horse = new CardStruct.HorseOrHeader();
         horse.Id = h.id;
         horse.FileName = h.imgFileName;
         horse.IsHeader = h.isHeader; //false hardcode
         horse.IsFirstHorseOfRace = h.isFirstHorseOfRace;
         horse.IsLastHorseOfRace = h.isLastHorseOfRace;
         horse.IsFirstHorseOnPage = (h.positionOnPage.where == EntryLocationOnPage.FirstEntryOnPage) ? true : false;
         horse.IsLastHorseOnPage = (h.positionOnPage.where == EntryLocationOnPage.LastEntryOnPage) ? true : false;
         horse.IsLastHorseOfTheCard = h.IsLastHorseOfTheCard; 
         horse.Height = h.height;
         horse.NewHeight = h.newHeight;
         horse.SpaceBetween = h.spCount;
         horse.PageBreak = (h.positionOnPage.where == EntryLocationOnPage.LastEntryOnPage) ? true : false;

         horse.ContinueOnNextPage = ((h.positionOnPage.where == EntryLocationOnPage.LastEntryOnPage) && h.isLastHorseOfRace ) ? false : true;
         horse.ResidualSpace = h.positionOnPage.leftspaceatEnd;
         horse.racenum = h.raceNumber;
         horse.pgnum = h.pgno;
        return horse;
      }

      private static CardStruct.HorseOrHeader getHorseHeaderFromHeader(Header h)
      {
         CardStruct.HorseOrHeader header = new CardStruct.HorseOrHeader();

         header.Id = h.id;
         header.FileName = h.imgFileName;
         header.IsHeader = h.isHeader; //true hardcode
         //header.IsFirstHorseOfRace =???
         //header.IsLastHorseOfRace = ???

         /* revisit
         header.IsFirstHorseOnPage = (h.positionOnPage.where == EntryLocationOnPage.FirstEntryOnPage) ? true : false;
         header.IsLastHorseOnPage = ((h.positionOnPage.where == EntryLocationOnPage.LastEntryOnPage) || (h.positionOnPage.where == EntryLocationOnPage.LastEntryEOP)) ? true : false;
         header.IsLastHorseOfTheCard = (h.positionOnPage.where == EntryLocationOnPage.THELAST) ? true : false;
         */
         header.Height = h.height;
         header.NewHeight = h.newHeight;
         header.SpaceBetween = h.spCount;

         /* revisit
         header.PageBreak = (h.positionOnPage.where == EntryLocationOnPage.LastEntryOnPage) ? true : false;
         header.ContinueOnNextPage = (h.positionOnPage.where == EntryLocationOnPage.LastEntryEOP) ? false : true;
         header.ResidualSpace = h.positionOnPage.leftspaceatEnd;

         */
         header.racenum = h.racenumber;
         header.pgnum = h.pgno;

         return header;
      }
   }
}
