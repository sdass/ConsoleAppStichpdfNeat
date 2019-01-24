using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConsoleAppStichpdfNeat.NestedElements;
using log4net;
using ConsoleAppStichpdfNeat.Util;
using System.Diagnostics;
using ConsoleAppStichpdfNeat.Config;

namespace ConsoleAppStichpdfNeat
{
   class PdfPageCalculation
   {
      private static readonly ILog log = LogManager.GetLogger(typeof(PdfPageCalculation).Name);
      public List<CardStruct.Race> calculate(List<CardStruct.Race> races) //*** 1.
      {
         System.Diagnostics.Debug.WriteLine("++++++++++++11111+++++++");

         CardStruct.debugPrintACard(races);

         System.Diagnostics.Debug.WriteLine("++++++++++++start at +++++++" + DateTime.Now + "\n");
         Stopwatch sw = new Stopwatch(); sw.Start();

         List <Race<Horse>> raceList = prepInput(races);
         //log.Info( "Debug-5: " + raceList);
         List<CardStruct.Race> raceout = processToFitOnPageAndReturn(raceList);
         sw.Stop();
         Console.WriteLine(String.Format("elapsed time: {0} millisecond", sw.ElapsedMilliseconds ));

         
         System.Diagnostics.Debug.WriteLine("+++++++++++++++++end at+++++++++++++++++++" + DateTime.Now  + "\n");
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
         Horse lastHorseOfCard = raceList.Last().secondAndOtherHorseList.Last();
         lastHorseOfCard.IsLastHorseOfTheCard = true;                 
         return raceList;
      }

      private List<CardStruct.Race> processToFitOnPageAndReturn(List<Race<Horse>> races)
      {
         //debugPrintRace(raceList);
         List<CardStruct.Race> outRaceList = doProcess(races);
         return outRaceList;

      }


      private List<CardStruct.Race> doProcess(List<Race<Horse>> races)
      {
         List<Util.PageDetail> pages = new PageOptimization().useOptimalSpace(races);
         Debug.Print("DEBUG PRINTING DEBUG PRINTING DEBUG PRINTING DEBUG PRINTING DEBUG PRINTING ");
         //debugPrintPgDetails(pages);
         List<CardStruct.Race> horsesByrace = getEntriesGroupedInRace(pages);

         //debug horsesByrace.ForEach(r => r.allHorsesWithRaceHeader.ForEach(x => { if (x.SpaceBetween < 0) Console.WriteLine("space<>between=" + x.SpaceBetween); }));
         //TO-DO TO-DO
         return horsesByrace; //call process class before
      }

      internal static CardStruct.HorseOrHeader prepareHorseStruct(Horse h)
      {
         CardStruct.HorseOrHeader horse = new CardStruct.HorseOrHeader();
         horse.Id = h.id;
         horse.FileName = h.imgFileName;
         horse.IsHeader = h.isHeader; //false hardcode
         horse.IsFirstHorseOfRace = h.isFirstHorseOfRace;
         horse.IsLastHorseOfRace = h.isLastHorseOfRace;
         horse.IsFirstHorseOnPage = (h.positionOnPage.where == EntryLocationOnPage.FirstEntryOnPage) ? true : false;
         horse.IsLastHorseOnPage = (h.positionOnPage.where == EntryLocationOnPage.LastEntryOnPage) ? true : false;
         horse.Height = h.height;
         horse.NewHeight = h.newHeight;
         horse.SpaceBetween = h.spCount;
         horse.PageBreak = (h.positionOnPage.where == EntryLocationOnPage.LastEntryOnPage) ? true : false;         
         horse.ContinueOnNextPage = ((h.positionOnPage.where == EntryLocationOnPage.LastEntryOnPage) && !h.isLastHorseOfRace) ? true : false; // flag revised
         



         horse.IsLastHorseOfTheCard = h.IsLastHorseOfTheCard;
         //localize two fields below for simplification
         if (horse.IsLastHorseOfTheCard)
         {
            horse.IsLastHorseOnPage = true;
            horse.PageBreak = true;
            horse.ContinueOnNextPage = false;
         }
         horse.ResidualSpace = h.positionOnPage.leftspaceatEnd;
         horse.racenum = h.raceNumber;
         horse.pgnum = h.pgno;
         return horse;
      }


      internal static CardStruct.HorseOrHeader prepareHeaderStruct(Header h)
      {
         CardStruct.HorseOrHeader header = new CardStruct.HorseOrHeader();
         header.Id = h.id;
         header.FileName = h.imgFileName;
         header.IsHeader = h.isHeader; //true         
         header.Height = h.height;
         header.NewHeight = h.newHeight;
         header.SpaceBetween = h.spCount;
         header.racenum = h.racenumber;
         header.pgnum = -1;
         //other detail on header is unnecessary crowding

         return header;
      }

      internal List<CardStruct.Race> getEntriesGroupedInRace(List<PageDetail> pglist)
      {
         List<CardStruct.Race> zl = new List<CardStruct.Race>();
         List<CardStruct.HorseOrHeader> hlist = getEntrylist(pglist);
         List<CardStruct.HorseOrHeader> headers = hlist.FindAll(hh => hh.Id == 0);
         foreach (var head in headers)
         {
            CardStruct.Race r = new CardStruct.Race();
            r.raceNumber = "" + head.racenum;
            r.allHorsesWithRaceHeader = hlist.FindAll(hh => hh.racenum == head.racenum);
            r.allHorsesWithRaceHeader.Sort(); //must
            zl.Add(r);
         }
         return zl;
      }

      private List<CardStruct.HorseOrHeader> getEntrylist(List<PageDetail> pglist)
      {
         List<CardStruct.HorseOrHeader> ml = new List<CardStruct.HorseOrHeader>();
         foreach (PageDetail p in pglist)
         {
            if (p.seeHeaderAndFirstHorseList != null)
            {
               foreach (HeaderAndFirstHorse hf in p.seeHeaderAndFirstHorseList)
               {
                  ml.Add(prepareHeaderStruct(hf.header));
                  ml.Add(prepareHorseStruct(hf.firstHorse));
               }

            }
            foreach (Horse h in p.secondAndNextHorses)
            {
               ml.Add(prepareHorseStruct(h));

            }

         }

         // ml.Sort(); //not needed here
         // ml.ForEach((h => Debug.Print("id=" + h.Id + "race=" + h.racenum)));
         return ml;
      }

      internal void debugPrintPgDetails(List<PageDetail> pglist)
      {
         Debug.Print("-------******* Begin debugging *****------");
         pglist.ForEach(pg => Debug.Print(pg.ToString()));
         Debug.Print("-------******* End debugging *****------");
      }

      private static void tinydebug(List<CardStruct.Race> l)
      {
         l.ForEach(r => {
            Debug.Print("-----------raceNumber=" + r.raceNumber + "-----------");
            r.allHorsesWithRaceHeader.ForEach(h => Debug.Print("id=" + h.Id + "raceno=" + h.racenum));
         });
      }
     
   }
}
