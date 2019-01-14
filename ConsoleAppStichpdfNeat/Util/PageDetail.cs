using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConsoleAppStichpdfNeat.NestedElements;

namespace ConsoleAppStichpdfNeat.Util
{
    internal class PageDetail : PageBound
    {
        internal bool isthereAheader { get; set; } //  on this page. can be none, 1st, middle, or 2nd to bottom entry.
      private List<HeaderAndFirstHorse> headerAndFirstHorseList; //if isthereAheader == false then it is null
        internal List<Horse> secondAndNextHorses { get; set; } //always exist

      internal bool doesVeryLargeHorseBegin;
      internal bool doesVeryLargeHorseEnd;

      private int pgNumAtLastline;
      internal void addHeaderAndFirstHorse(HeaderAndFirstHorse hf)
      {

         if (headerAndFirstHorseList == null)
         {
            headerAndFirstHorseList = new List<HeaderAndFirstHorse>();
            headerAndFirstHorseList.Add(hf);
         }
         else
         {
            //has already one header
            headerAndFirstHorseList.Add(hf);
         }
      }
      public List<HeaderAndFirstHorse> seeHeaderAndFirstHorseList
      {
         get
         {
            return headerAndFirstHorseList;
         }
      }


      public PageDetail() : base()
        {
            secondAndNextHorses = new List<Horse>();
        }
        public override string ToString()
        {

            return base.ToString() + "{PageDetail: " + "isthereAheader=" + isthereAheader + "[ headerAndFirstHorseList=" + stringifyHeaderAndFirstHorseList() + "] secondAndNextHorses=" + stringify2ndAndOtherHorses() + " }";

        }

      public Horse getLastHorseOnPage()
      {
         //critical pre-condition
         Horse lastOfHeaderAndFirstHorseList = (headerAndFirstHorseList != null) ? headerAndFirstHorseList.Last().firstHorse : null;
         Horse lastOf2ndHorseList = secondAndNextHorses.Last();
         Horse lastHorseOnPg = null;
         if (lastOfHeaderAndFirstHorseList != null)
         {
            //2 possibilites when header on page
            if (lastOf2ndHorseList.raceNumber >= lastOfHeaderAndFirstHorseList.raceNumber)
            {
               lastHorseOnPg = lastOf2ndHorseList;
            }
            else
            {
               lastHorseOnPg = lastOfHeaderAndFirstHorseList;
            }
         }
         else
         {
            //no header on page
            lastHorseOnPg = lastOf2ndHorseList;
         }
         return lastHorseOnPg;
      }

      private string stringifyHeaderAndFirstHorseList()
      {
         int count = (headerAndFirstHorseList == null) ? 0 : headerAndFirstHorseList.Count;
         string init = "count=" + count + ": ";

         if (headerAndFirstHorseList != null)
         {
            headerAndFirstHorseList.ForEach(hf => { init = init + hf.ToString(); });
         }

         return init;
      }

      //public addHeaderAndFirstHorse
      private string stringify2ndAndOtherHorses()
        {
            string init = "";
            secondAndNextHorses.ForEach(h => { init = init + h.ToString(); });
            return init;
        }



    }

}
