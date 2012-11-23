using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Contract;
using System.IO;

namespace FileSystemStorageProvider
{
    public class Directory : IDirectory
    {
        System.IO.DirectoryInfo _dirInfo;

        public Directory(string path)
        {
            _dirInfo = new System.IO.DirectoryInfo(path);
        }

        public bool Exists
        {
            get { return _dirInfo.Exists; }
        }


        public List<Contract.IFile> GetFiles()
        {
            return GetFiles(null);
        }

        public List<IFile> GetFiles(string filter)
        {
            var files = new List<IFile>();

            List<FileInfo> fileList;

            if (filter == null)
                fileList = _dirInfo.GetFiles().ToList();
            else
                fileList = _dirInfo.GetFiles(filter).ToList();

            fileList.ForEach(x => files.Add(new File(x.FullName)));

            return files;
        }

        public List<IDirectory> GetDirectories()
        {
            return GetDirectories(null);
        }

        public List<IDirectory> GetDirectories(string filter)
        {

            var list = new List<IDirectory>();
            
            List<DirectoryInfo> dirList;

            if (filter == null)
                dirList = _dirInfo.GetDirectories().ToList();
            else
                dirList = _dirInfo.GetDirectories(filter).ToList();

            dirList.ForEach(x => list.Add(new Directory(x.FullName)));

            return list;
        }


        public string Name
        {
            get { return _dirInfo.Name; }
        }

        public IDirectory CreateSubdirectory(string name)
        {
            var newDir = new DirectoryInfo(_dirInfo.FullName+@"\"+name);
            newDir.Create();
            return new Directory(newDir.FullName);
        }


        public IFile CreateFile(IFile file)
        {
            var filePath = _dirInfo.FullName + @"\" + file.Name;
            System.IO.File.WriteAllBytes(filePath, file.GetBytes());
            return new File(filePath);
        }
    }
}
