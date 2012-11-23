using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Contract;
using Contract.HitList;
using System.Xml.Serialization;
using System.Configuration;

namespace MPackEngine
{
    public class Engine
    {
        private string _foreignPackageStore;
        private string _localPackageStore;
        //private string _localPackageRepositoryList;
        private LocalTempStorage _localTempStorage;
        string _manifestFileName;

        public Engine(string foreignPackageStore, string localPackageStore)
        {
            _foreignPackageStore = foreignPackageStore;
            _localPackageStore = localPackageStore;
            _localTempStorage = new LocalTempStorage();
            _manifestFileName = ConfigurationManager.AppSettings.Get("ManifestFileName");
        }

        public LocalPackageRepositoryList GetCurrentPackageRepositoryList()
        {
            return _localTempStorage.ReadPackageRepositoryList();
        }

        public void UpdateLocalPackageList()
        {
            var localTempList = gatherPackageManifests();

            var localPackageRepositoryList = createLocalPackageRepositoryList(localTempList);
            _localTempStorage.CreatePackageRepositoryList(localPackageRepositoryList);
            //savePackageRepositoryList(localPackageRepositoryList);           
        }

        private void readPackageRepositoryList()
        {

        }

        private LocalPackageRepositoryList createLocalPackageRepositoryList(List<LocalTempList> list)
        {
            var localPackageRepositoryList = new LocalPackageRepositoryList();
            
            foreach (var manifest in list)
            {
                var manifestReader = new XPathManifestReader(manifest.LocalPath);

                var name = manifestReader.GetValue(ConfigurationManager.AppSettings.Get("XPathToGetAppNameInManifest"));
                var version = manifestReader.GetValue(ConfigurationManager.AppSettings.Get("XPathToGetAppVersionInManifest"));
                var dependentPackageList = manifestReader.GetDependenApplicationExpression("/Package/Dependecies/Package",
                                                                                            ConfigurationManager.AppSettings.Get("XPathToDependencyAppName"),
                                                                                            ConfigurationManager.AppSettings.Get("XPathToDependencyAppVersion"));

                //Create the local package
                var package = new Package()
                    {
                        ApplicationName = name,
                        ApplicationVersion = version,
                        ForeignPath = manifest.PackageStorePath,
                        DependentPackages = new List<Package>()
                    };

                //Create the local dependent packages
                foreach (var dependentPackage in dependentPackageList)
                {
                    package.DependentPackages.Add(new Package()
                    {
                        ApplicationName = dependentPackage.Item1,
                        ApplicationVersion = dependentPackage.Item2
                    });
                }

                localPackageRepositoryList.Packages.Add(package);
            }

            _localTempStorage.CleanTempManifests();

            return localPackageRepositoryList;
        }

        private List<LocalTempList> gatherPackageManifests()
        {
            var provider = StorageProviderFactory.GetSourceRepositoryProvider();
            var rootDir = provider.GetRootDirectory();
            //var rootDir = new System.IO.DirectoryInfo(_foreignPackageStore);
            if(!rootDir.Exists)
                throw new Exception(string.Format("The foreign source with path {0} can not be accessed or found", _foreignPackageStore));

            var tempLocalList = findManifest(rootDir);

            return tempLocalList;
        }

        private List<LocalTempList> findManifest(IDirectory rootDir)
        {
            var tempLocalList = new List<LocalTempList>();
            //"Manifest.xml"

            var file = rootDir.GetFiles(_manifestFileName).FirstOrDefault();

            if (file != null)
            {
                //Create the temp manifest file
                var localFileName = _localTempStorage.SaveManifest(file.GetBytes());
                tempLocalList.Add(new LocalTempList() { LocalPath = localFileName, PackageStorePath = file.FullName });
                Console.WriteLine("Found app at path: {0}", file.FullName);
            }
            else
            {
                foreach (var dir in rootDir.GetDirectories())
                {
                    tempLocalList.AddRange(findManifest(dir));
                }
            }

            return tempLocalList;
        }

        public void GetApplication(List<Package> packages)
        {
            var sourceProvider = StorageProviderFactory.GetSourceRepositoryProvider();
            var destinationProvider = StorageProviderFactory.GetDestinationeRepositoryProvider();

            foreach (var package in packages)
            {
                var path = package.ForeignPath.Replace(_manifestFileName, "");
                Console.WriteLine("Getting application {0} from path: {1}", package.ApplicationName, path);

                var sourceAppDir = sourceProvider.GetDirectoy(path);
                var destRootDir = destinationProvider.GetRootDirectory();
                //var destAppDir = destRootDir.CreateSubdirectory(package.ApplicationName +"_"+ package.ApplicationVersion);

                createDirectoriesAndFiles(sourceAppDir, destRootDir, package.ApplicationName + "_" + package.ApplicationVersion);
                Console.WriteLine("Done!");
            }
        }

        private void createDirectoriesAndFiles(IDirectory sourceDirectory, IDirectory destinationDirectory, string name)
        {
            IDirectory destDir;

            if (name == null)
                destDir = destinationDirectory.CreateSubdirectory(sourceDirectory.Name);
            else
                destDir = destinationDirectory.CreateSubdirectory(name);

            foreach (var dir in sourceDirectory.GetDirectories())
            {
                createDirectoriesAndFiles(dir, destDir, null);
            }

            foreach (var file in sourceDirectory.GetFiles())
            {
                destDir.CreateFile(file);
            }
        }
    }
}
