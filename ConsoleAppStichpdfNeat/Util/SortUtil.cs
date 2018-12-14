using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConsoleAppStichpdfNeat.NestedElements;
using ConsoleAppStichpdfNeat.Util;
using System.Diagnostics;

namespace ConsoleAppStichpdfNeat.Util
{
   internal class SortUtil
   {
      internal static List<CardStruct.Race> getEntriesGroupedInRace(List<PageDetail> pglist)
      {
         List < CardStruct.Race > zl = new List<CardStruct.Race>();
         List<CardStruct.HorseOrHeader> hlist = getEntrylist(pglist);
         List<CardStruct.HorseOrHeader> headers =  hlist.FindAll(hh => hh.Id == 0);
         foreach(var head in headers)
         {
            CardStruct.Race r = new CardStruct.Race();
            r.raceNumber = "" + head.racenum;
            r.allHorsesWithRaceHeader = hlist.FindAll(hh => hh.racenum == head.racenum);
            r.allHorsesWithRaceHeader.Sort();
            zl.Add(r);
         }
         //CardStruct.debugPrintACard(zl);
         tinydebug(zl);
         return zl;
      }

      private static void tinydebug(List<CardStruct.Race> l)
      {
         l.ForEach(r => {
            Debug.Print("-----------raceNumber=" + r.raceNumber + "--------");
            r.allHorsesWithRaceHeader.ForEach(h => Debug.Print("id=" + h.Id + "raceno=" + h.racenum));
         });
      }
      private static List<CardStruct.HorseOrHeader> getEntrylist(List<PageDetail> pglist)
      {
         List <CardStruct.HorseOrHeader> ml = new List<CardStruct.HorseOrHeader>();
         foreach (PageDetail p in pglist)
         {
            if(p.conjugate != null)
            {
               ml.Add(PdfPageCalculation.getHorseHeaderFromHeader(p.conjugate.header));
               ml.Add(PdfPageCalculation.getHorseHeaderFromHorse(p.conjugate.firstHorse));
            }
            foreach (Horse h in p.secondAndNextHorses)
            {
               ml.Add(PdfPageCalculation.getHorseHeaderFromHorse(h));

            }
         }

         /* WORKS except memory
         List<CardStruct.HorseOrHeader> nl = ml.OrderBy(h => h.Id).OrderBy(o => o.racenum).ToList(); //LINQ works!!! but memory intensive
         nl.ForEach((h => Debug.Print("id=" + h.Id + "race=" + h.racenum)));

         */
         ml.Sort(); //prfect done through model's custom compareTo()
   
        ml.ForEach((h => Debug.Print("id=" + h.Id + "race=" + h.racenum)));
         return ml;
      }
   }
}
