using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Contract
{
    public interface IStorageProvider
    {
        IDirectory GetDirectoy(string path);
        IDirectory GetRootDirectory();
    }
}
