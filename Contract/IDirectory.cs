using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Contract
{
    public interface IDirectory
    {
        bool Exists { get;}
        List<IFile> GetFiles();
        List<IFile> GetFiles(string filter);
        List<IDirectory> GetDirectories();
        List<IDirectory> GetDirectories(string filter);
        string Name { get; }
        IDirectory CreateSubdirectory(string Name);
        IFile CreateFile(IFile file);


    }
}
