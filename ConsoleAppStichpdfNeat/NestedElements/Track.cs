using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleAppStichpdfNeat.NestedElements
{
    public class Track
    {
        public List<Race<Horse>> raceList { get; set; }

        public Track()
        {
            raceList = new List<Race<Horse>>();
        }

        public Track addRace(Race<Horse> r)
        {
            this.raceList.Add(r);
            return this;
        }

        public override string ToString()
        {
            string beginStr = "A Track>>>>>>>>>>>: ";
            string raceListStr = " raceList: \n";
            raceListStr = raceListStr + raceDetail();
            //raceList.ForEach(race => raceListStr = raceListStr + race.ToString() + "\n");
            return String.Format("Begin raceList: {0} {1} {2} ", beginStr, raceListStr, "\n" );

        }

        private string raceDetail()
        {
            string raceInitStr = "";
            raceList.ForEach( (Race<Horse> r) =>{ raceInitStr += r; });
            return raceInitStr;
        }
    }
}
