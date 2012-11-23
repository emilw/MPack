using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Contract
{
    public interface IFile
    {
        IFile CopyTo(string path);
        string FullName { get; }
        string Name { get; }
        byte[] GetBytes();
    }
}
