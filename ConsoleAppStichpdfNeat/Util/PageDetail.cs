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

      internal bool doesVeryLargeHorseBegin; //true if very large horse begin on a page (page will have 0 residual space)
      internal bool doesVeryLargeHorseEnd; //true if very large horse ends on a page (page will have residual space)
      internal bool doesVeryLargeHorseMiddle; //true if very large horse occupies whole page in between begin-page and end-page
      internal int pgNum;

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

      public PageDetail(int pgNum) : base()
      {
         secondAndNextHorses = new List<Horse>();
         this.pgNum = pgNum;
      }
      public override string ToString()
        {

            return base.ToString() + "{PageDetail: pgNum=" + pgNum + " isthereAheader =" + isthereAheader + "[ headerAndFirstHorseList=" + stringifyHeaderAndFirstHorseList() + "] secondAndNextHorses=" + stringify2ndAndOtherHorses() + " }";

        }

      public Horse getLastHorseOnPage()
      {
         //critical pre-condition
         Horse lastOfHeaderAndFirstHorseList = (headerAndFirstHorseList != null) ? headerAndFirstHorseList.Last().firstHorse : null;
         Horse lastOf2ndHorseList = (secondAndNextHorses.Count > 0) ? secondAndNextHorses.Last() : null;
         
         Horse lastHorseOnPg = null;
         
         //deal large horse 1st for begin and middle pages
         if(doesVeryLargeHorseBegin || doesVeryLargeHorseMiddle)
         {
            return lastHorseOnPg; // 2 use cases: very LARGE horse occupying 1st page residual but spill to next page OR full height middle pages (with the horse entry in the ending page)
            //we are not marking lastHorse on the page because horse still continuing. Therefore no page break.
         }

         //Now deal regular or "large horse at the ending page" (total 4 use cases)
         if ((lastOfHeaderAndFirstHorseList != null) && (lastOf2ndHorseList != null))
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
         else if (lastOfHeaderAndFirstHorseList != null) //implicit lastOf2ndHorseList is null
         {
            lastHorseOnPg = lastOfHeaderAndFirstHorseList;
         }
         else if (lastOf2ndHorseList != null) //implicit lastOfHeaderAndFirstHorseList is null
         {
            lastHorseOnPg = lastOf2ndHorseList;
         }
         else //both are null
         {
            return lastHorseOnPg; // null
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
