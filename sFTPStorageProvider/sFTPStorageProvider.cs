using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Contract;
using System.Configuration;

namespace sFTPStorageProvider
{
    public class sFTPStorageProvider : ConfigurationSection, IStorageProvider
    {
        [ConfigurationProperty("RepositoryPath")]
        public string RepositoryPath
        {
            get
            {
                return (String)this["RepositoryPath"];
            }
        }

        [ConfigurationProperty("Host")]
        public string Host
        {
            get
            {
                return (String)this["Host"];
            }
        }

        [ConfigurationProperty("UserName")]
        public string UserName
        {
            get
            {
                return (String)this["UserName"];
            }
        }

        [ConfigurationProperty("Password")]
        public string Password
        {
            get
            {
                return (String)this["Password"];
            }
        }

        [ConfigurationProperty("Port")]
        public int Port
        {
            get
            {
                return (int)this["Port"];
            }
        }

        public IDirectory GetDirectoy(string path)
        {
            var client = new Renci.SshNet.SftpClient(this.Host, this.Port, this.UserName, this.Password);

            client.Connect();

            var name = path.Split(new string[]{@"\",@"/"}, StringSplitOptions.RemoveEmptyEntries).Last();

            return new Directory(path, client, name);
        }


        public IDirectory GetRootDirectory()
        {
            var client = new Renci.SshNet.SftpClient(this.Host, this.Port, this.UserName, this.Password);
            
            client.Connect();
            var dir = client.ListDirectory(this.RepositoryPath);

            var dir2 = client.ListDirectory("/PackageRepository/Core/0.0.1.193");

            //var sftp = new Sftp();
            return new Directory(this.RepositoryPath, client, "");
        }
    }
}
