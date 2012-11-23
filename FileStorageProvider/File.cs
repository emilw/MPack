using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Contract;

namespace FileSystemStorageProvider
{
    public class File : IFile
    {
        FileInfo _file;

        public File(string path)
        {
            _file = new FileInfo(path);
        }

        public IFile CopyTo(string path)
        {
            var file = _file.CopyTo(path);
            
            return new File(file.FullName);
        }

        public byte[] GetBytes()
        {
            var fileStream = _file.OpenRead();
            var binaryReader = new BinaryReader(fileStream);
            return binaryReader.ReadBytes((int)_file.Length);
        }

        public string FullName
        {
            get { return _file.FullName; }
        }

        public string Name
        {
            get { return _file.Name; }
        }
    }
}
