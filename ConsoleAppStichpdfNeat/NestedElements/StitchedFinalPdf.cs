using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleAppStichpdfNeat.NestedElements
{
    public class StitchedFinalPdf
    {
        //front cover, last cover and other items 
        public List<Track> trackList { get; set; }

        public StitchedFinalPdf()
        {
            trackList = new List<Track>();
        }

        public StitchedFinalPdf addTrack(Track t)
        {
            this.trackList.Add(t);
            return this;
        }

        public override string ToString()
        {
            string res = "A Pdf: ";
            int trackno = 1;
            foreach(Track vTrack in trackList) //readable
            {
                res = res + vTrack.ToString();
                
                //trackStr =
                //foreach(Race<Horse> vRace in vTrack.raceList)
                //{
                //    string raceStr = vRace.ToString() + "\n";
                //}

            }
            //trackList.ForEach(vtrack => { res = res + "444-"; vtrack.raceList.ForEach(vrace => { res = res + vrace.ToString() + "\n"; });  }); unweildy
            return String.Format("Entire tracklist: {0} ", res);
        }

    }


}
