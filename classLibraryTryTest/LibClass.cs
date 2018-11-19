using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace classLibraryTryTest
{
    public class LibClass
    {
        public string showMulti(string rs, int  c)
        {
            string res = "";
            for(int i=0; i< c; i++)
            {
                res = res + rs;
            }
            return res;
        }

        public string showMix(string rs, int c)
        {
            string res = "";
            res = c + rs;
            return res;
        }

    }

}
