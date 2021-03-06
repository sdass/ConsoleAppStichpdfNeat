﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConsoleAppStichpdfNeat.Config;

namespace ConsoleAppStichpdfNeat.Util
{
    internal class PageBound
    {
        internal double top { get; set; }
        internal double runningDepth { get; set; }

        internal double depthNotYetUsed { get; set; }
        internal double bottom { get; set; }
        internal int entryCount { get; set; } // HeaderAndFirstHorse = 2 items. Each other entry (horse) counts 1.


        protected PageBound() {
         //bottom = Config.Constants.pageHeight * Constants.TENTHOUSAND;
         //depthNotYetUsed = Config.Constants.pageHeight * Constants.TENTHOUSAND;
         bottom = Config.Constants.PageHeight;
         depthNotYetUsed = Config.Constants.PageHeight;
        }
    

        public override string ToString()
        {
            
            return "{PageBound: " + " top=" + top + " runningDepth=" + runningDepth + " bottom=" + bottom + " depthNotYetUsed=" + depthNotYetUsed + " entryCount=" + entryCount  + "}"; 
        }

    }
}
