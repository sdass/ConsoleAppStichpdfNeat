﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.Design;

namespace ConsoleAppStichpdfNeat.Util
{
    public enum EntryLocationOnPage
    {
        // no differentiation between top and middle entries. LastEntry is marked and treated differently.
        //top
        FirstEntry = 0,

       //middle
        MiddleEntry,

       //last horse of race at the End of page
        LastEntryEOP,

        //last horse of race at the Middle of page
        LastEntryMOP


    };


}
