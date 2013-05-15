using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Contract
{
    public class VersionComparer : IComparer<string>
    {

        public int Compare(string x, string y)
        {
            var versionX = new Version(x);
            var versionY = new Version(y);

            if (versionX < versionY) return -1;
            else if(versionX > versionY) return 1;
            else return 0;
        }
    }
}
