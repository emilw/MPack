using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Contract;
using System.Xml.Serialization;
using System.IO;

namespace MPackEngine
{
    public class LocalTempStorage
    {
        string _localPath;
        private System.IO.DirectoryInfo _tempLocalManifestDirectory;
        private const string _localRepositoryFileListName = "LocalRepositoryFileList.xml";
        LocalPackageRepositoryList _localPackageRepositoryList = null;

        public LocalTempStorage()
        {
            _tempLocalManifestDirectory = new System.IO.DirectoryInfo(System.IO.Path.GetTempPath());
        }

        public string SaveManifest(byte[] file)
        {
            var filePath = GetLocalTempFilePath();

            File.WriteAllBytes(filePath, file);

            return filePath;
        }

        public string GetLocalTempFilePath()
        {
            return _tempLocalManifestDirectory.FullName + @"\" + Guid.NewGuid().ToString() + ".xml";
        }

        public void CreatePackageRepositoryList(LocalPackageRepositoryList repoList)
        {
            var serializer = new XmlSerializer(typeof(LocalPackageRepositoryList));

            var stream = new FileStream(_localRepositoryFileListName, FileMode.Create);
            serializer.Serialize(stream, repoList);
            stream.Close();
        }

        public LocalPackageRepositoryList ReadPackageRepositoryList()
        {
            if (_localPackageRepositoryList == null)
            {
                var serializer = new XmlSerializer(typeof(LocalPackageRepositoryList));

                var stream = new FileStream(_localRepositoryFileListName, FileMode.Open);
                _localPackageRepositoryList = (LocalPackageRepositoryList)serializer.Deserialize(stream);
                stream.Close();
            }
            return _localPackageRepositoryList;
        }
        public void CleanTempManifests()
        {
            //Clean up code for temporary manifest files
        }
    }
}
