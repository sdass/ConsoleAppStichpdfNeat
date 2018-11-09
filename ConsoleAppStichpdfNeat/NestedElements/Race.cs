using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleAppStichpdfNeat.NestedElements
{
    public class Race<Horse>
    {
        public HeaderAndFirstHorse headerFirstHorse { get; set; }
        public List<Horse> secondAndOtherHorseList { get; set; }

        public override string ToString()
        {

            return String.Format("Race: {0} other={1}", headerFirstHorse.ToString(), secondAndOtherHorseList.ToString());
        }

        public Race()
        {
            secondAndOtherHorseList = new List<Horse>();
        }

        public Race<Horse> setRaceTop(HeaderAndFirstHorse hf)
        {
            headerFirstHorse = hf;
            return this;
        }
        public Race<Horse> addHorse(Horse h)
        {
            this.secondAndOtherHorseList.Add(h);
            return this;
        }

        //private String getRest()
        //{
        //    String res = (secondAndOtherHorseList) =>
        //}
    }
}
