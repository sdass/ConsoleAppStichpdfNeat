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

        
        public override string ToString()
        {

            return String.Format("Race: {0} ", getStringOf_Race(this));
        }


        private string getStringOf_Race(Race<Horse> aRace)
        {
            Func<List<Horse>, String> xListTostr = consolidate; //powerful delegate
            string raceHeaderAnd1stHorse = aRace.headerFirstHorse.ToString();

            string secondAndOtherHouse = xListTostr(aRace.secondAndOtherHorseList);
            return raceHeaderAnd1stHorse + secondAndOtherHouse;
        }

        private string consolidate<T>(List<T> lstr) { 
            string restr = "Begin: ";

            lstr.ForEach(b =>
            {
                if (b.GetType() == typeof(Horse))
                {
                   // Console.WriteLine("Horse type"); //debug
                    restr += b.ToString() + ", ";
                }
                else
                {
                    restr += restr + "error";
                }
  
            });


            return restr;
        }
    } //class ends
}
