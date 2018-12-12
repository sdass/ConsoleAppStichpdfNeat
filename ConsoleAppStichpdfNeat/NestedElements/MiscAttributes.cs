using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleAppStichpdfNeat.NestedElements
{
   public abstract class MiscAttributes
   {
      internal bool isHeader { get; set; }
      internal bool isFirstHorseOfRace { get; set; }
      internal bool isLastHorseOfRace { get; set; }
      internal bool IsLastHorseOfTheCard { get; set; }
      internal int pgno { get; set; }
   }
}
