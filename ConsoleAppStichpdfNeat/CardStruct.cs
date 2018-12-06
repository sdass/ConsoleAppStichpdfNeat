using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleAppStichpdfNeat
{
   class CardStruct
   {

      public struct Race
      {
         public string raceNumber;
         public List<HorseOrHeader> allHorsesWithRaceHeader;

      }
      public struct HorseOrHeader : IComparable<HorseOrHeader>
      {
         public int Id;
         public string FileName;
         public bool IsHeader;
         public bool IsFirstHorseOfRace;
         public bool IsLastHorseOfRace;
         public bool IsFirstHorseOnPage;
         public bool IsLastHorseOnPage;
         public double Height;
         public double NewHeight;
         public double SpaceBetween;
         public bool PageBreak;
         public bool ContinueOnNextPage;
         public double ResidualSpace;
         public int racenum;
         public int pgnum;

         public int CompareTo(HorseOrHeader other)
         {
            return this.Id.CompareTo(other.Id);
         }

         //TBD


         public override string ToString()
         {
            return "CardStruct.HorseOrHeader: Id=" + Id + " FileName=" + FileName + " IsHeader=" + IsHeader + " IsFirstHorseOfRace=" + IsFirstHorseOfRace
               + " IsLastHorseOfRace=" + IsLastHorseOfRace + " IsFirstHorseOnPage=" + IsFirstHorseOnPage + " IsLastHorseOnPage=" + IsLastHorseOnPage
               + " Height=" + Height + " NewHeight=" + NewHeight + " SpaceBetween=" + SpaceBetween + " PageBreak=" + PageBreak + " ContinueOnNextPage=" + ContinueOnNextPage
               + " ResidualSpace=" + ResidualSpace + " racenum=" + racenum + " pgnum=" + pgnum;
         }


      }


      public static void debugPrintACard(List<CardStruct.Race> track)
      {
         System.Diagnostics.Debug.WriteLine("========= Track race count=" + track.Count + " ===============");
         foreach (CardStruct.Race race in track)
         {
            System.Diagnostics.Debug.WriteLine("-----------------CardStruct.Race=" + race.raceNumber + "------------------");
            foreach (CardStruct.HorseOrHeader horse in race.allHorsesWithRaceHeader)
            {
               System.Diagnostics.Debug.WriteLine("\t...CardStruct.HorseOrHeader=" + horse + " ...");

            }
         }
      }

      /*
      public static void debugResult(List<HorseOrHeader> horses)
      {
         System.Diagnostics.Debug.WriteLine("========= Result  ===============");
         int currRace = horses.First().racenum;
         Console.WriteLine("--------- racenum=" + currRace + " ---------");
         horses.ForEach(hh => {
            if (hh.racenum > currRace)
            {
               currRace = hh.racenum;
               Console.WriteLine("--------- racenum=" + currRace + " ---------");
            }
            System.Diagnostics.Debug.WriteLine("CardStruct.HorseOrHeader=" + hh);

         });

      }
      */
   }

}
