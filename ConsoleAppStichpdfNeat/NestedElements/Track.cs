using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleAppStichpdfNeat.NestedElements
{
    public class Track
    {
        List<Race<Horse>> raceList { get; set; }

        public Track()
        {
            raceList = new List<Race<Horse>>();
        }

        public Track addTrack(Race<Horse> r)
        {
            this.raceList.Add(r);
            return this;
        }
    }
}
