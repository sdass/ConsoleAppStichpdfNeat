using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleAppStichpdfNeat.Util
{
    internal class PageBound
    {
        internal double top { get; set; }
        internal double runningDepth { get; set; }

        internal double depthNotYetUsed { get; set; }
        internal double bottom { get; set; }
        internal short entryCount { get; set; } // HeaderAndFirstHorse = 2 items. Each other entry (horse) counts 1.

        internal bool isEndMark { get; set; } // race ended with LAST horse but no new race [header+horse] begin after that horse.

        protected PageBound() {
            bottom = Config.Constants.pageHeight;
            depthNotYetUsed = Config.Constants.pageHeight;
        }
    

        public override string ToString()
        {
            
            return "{PageBound: " + " top=" + top + " runningDepth=" + runningDepth + " bottom=" + bottom + " depthNotYetUsed=" + depthNotYetUsed + " entryCount=" + entryCount + " isEndMark=" + isEndMark + "}"; 
        }

    }
}
