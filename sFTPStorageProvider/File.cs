using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Contract;
using Renci.SshNet;

namespace sFTPStorageProvider
{
    public class File : IFile
    {
        string _name = "";
        string _path = "";
        SftpClient _client = null;

        public File(string path, SftpClient client, string name)
        {
            _client = client;
            _path = path;
            _name = name;
        }

        public IFile CopyTo(string path)
        {
            throw new NotImplementedException();
        }

        public string FullName
        {
            get { return _path; }
        }


        public byte[] GetBytes()
        {
            return _client.ReadAllBytes(_path);
        }


        public string Name
        {
            get { return _name; }
        }
    }
}
