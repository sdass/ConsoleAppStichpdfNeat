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
        internal bool isthereAheader { get; set; } // on this page. can be (1) 1st, (2) middle, or (3) 2nd to bottom entry.
        private HeaderAndFirstHorse headerAndFirstHorse; //if isthereAheader = false then it is empty
        internal List<Horse> secondAndNextHorses { get; set; } //always exist

        public HeaderAndFirstHorse conjugate
        {
            get
            {
                return headerAndFirstHorse;
            }
            set
            {
                headerAndFirstHorse = value;
                //do pageBound staff
                //value.firstHorse.height

            }

        }

        public PageDetail() : base()
        {
            secondAndNextHorses = new List<Horse>();
            Console.WriteLine("333");
        }
        public override string ToString()
        {

            return base.ToString() + "{PageDetail: " + "isthereAheader=" + isthereAheader + " headerAndFirstHorse=" + headerAndFirstHorse +  " secondAndNextHorses=" + stringify2ndAndOtherHorses() + " }";

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
