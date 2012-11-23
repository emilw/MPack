using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Contract;
using System.Configuration;

namespace FileSystemStorageProvider
{
    public class FileSystemStorageProvider : ConfigurationSection, IStorageProvider
    {
        [ConfigurationProperty("RepositoryPath")]
        public string RepositoryPath
        {
            get
            {
                return (String)this["RepositoryPath"];
            }
        }

        public IDirectory GetDirectoy(string path)
        {
            return new Directory(path);
        }


        public IDirectory GetRootDirectory()
        {
            return new Directory(RepositoryPath);
        }
    }
}
