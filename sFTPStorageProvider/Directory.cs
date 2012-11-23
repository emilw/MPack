using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Contract;
using System.IO;
using Renci.SshNet;

namespace sFTPStorageProvider
{
    public class Directory : IDirectory
    {
        //System.IO.DirectoryInfo _dirInfo;
        string _path = "";
        SftpClient _client = null;
        string _name = "";
        
        public Directory(string path, SftpClient client, string name)
        {
            _path = path + "/";
            _client = client;
            _name = name;
        }

        //Needs to check if it exists
        public bool Exists
        {
            get { return true; }
        }


        public List<Contract.IFile> GetFiles()
        {
            return GetFiles(null);
        }

        public List<IFile> GetFiles(string filter)
        {
            var files = new List<IFile>();

            var fileList = _client.ListDirectory(_path).Where(x => !x.IsDirectory).ToList();

            if (filter != null)
                fileList = fileList.Where(x => x.Name.Equals(filter, StringComparison.InvariantCultureIgnoreCase)).ToList();
            
            fileList.ForEach(x => files.Add(new File(x.FullName, _client, x.Name)));

            return files;
        }

        public List<IDirectory> GetDirectories()
        {
            return GetDirectories(null);
        }

        public List<IDirectory> GetDirectories(string filter)
        {

            var list = new List<IDirectory>();
            
            var dirList = _client.ListDirectory(_path).Where(x => x.IsDirectory).ToList();
            dirList = dirList.Where(x => x.Name != "." && x.Name != "..").ToList();

            if (filter != null)
                dirList = dirList.Where(x => x.Name.Contains(filter)).ToList();
            
            dirList.ForEach(x => list.Add(new Directory(x.FullName, _client, x.Name)));

            return list;
        }


        public string Name
        {
            get { return _name; }
        }

        public IDirectory CreateSubdirectory(string name)
        {
            var newPath = _path + '/' + name;
            _client.CreateDirectory(newPath);

            return new Directory(newPath, _client, name);

        }


        public IFile CreateFile(IFile file)
        {
            var filePath = _path + '/' + file.Name;
            var stream = _client.Create(_path);
            byte[] fileBytes = file.GetBytes();
            stream.Write(fileBytes, 0, fileBytes.Length);

            return new File(filePath, _client, file.Name);
        }

        public override string  ToString()
        {
            return _path;
        }
    }
}
