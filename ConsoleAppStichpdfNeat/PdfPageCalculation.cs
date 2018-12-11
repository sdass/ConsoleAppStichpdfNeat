﻿using System;
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
         travesingPagesAsInPrintedCopy(pages);
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
               log.Info(":::pg-begin::::: ");
               p.secondAndNextHorses.ForEach(horse => log.Info(horse));
               log.Info("::::pg-end:::: ");
            }
            else if ((p.isthereAheader) && (p.conjugate.firstHorse.raceNumber > p.secondAndNextHorses.First<Horse>().raceNumber))
            {
               log.Info(":::pg-begin:::::2ndhorselist has lower num ");
               List<Horse> horseList = p.secondAndNextHorses;

               bool headerFirstHorseNotProcessed = true;
               for (int i = 0; i < horseList.Count; i++)
               {
                  if (headerFirstHorseNotProcessed)
                  {
                     if (horseList[i].raceNumber == p.conjugate.firstHorse.raceNumber)
                     {
                        headerFirstHorseNotProcessed = false;
                        log.Info(p.conjugate.header);
                        log.Info(p.conjugate.firstHorse);
                     }
                     log.Info(horseList[i]);
                  }
                  else
                  {
                     log.Info(horseList[i]);
                  }

               }

               log.Info("::::pg-end:::: ");
            }
            else if ((p.isthereAheader) && (p.conjugate.firstHorse.raceNumber <= p.secondAndNextHorses.First<Horse>().raceNumber)) //else equivalent
            {
               log.Info(":::pg-begin::::: ");
               log.Info(p.conjugate.header);
               log.Info(p.conjugate.firstHorse);
               p.secondAndNextHorses.ForEach(horse => log.Info(horse));
               log.Info("::::pg-end:::: ");

            }

         });

         log.Info("-------******* End travesingPagesAsInPrintCopy() *****------");
      }

   }
}
