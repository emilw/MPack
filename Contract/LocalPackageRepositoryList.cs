using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Contract
{
    public class LocalPackageRepositoryList
    {
        public LocalPackageRepositoryList()
        {
            Packages = new List<Package>();
        }

        public Package GetPackage(string appName, string appVersion)
        {
            var list = GetPackages(appName, appVersion);

            if (list.Count > 1)
                throw new Exception(string.Format("There exists more then one application with name {0} and version {1}", appName, appVersion));
            else if(list.Count == 0)
                throw new Exception(string.Format("There exists no application with name {0} and version {1}", appName, appVersion));

            return list.FirstOrDefault();
        }

        public List<Package> GetPackages(string appName, string appVersion)
        {
            return getPackages(appName, appVersion, true);
        }

        private string getHighestVersionFromWildcard(string appName, string appVersion)
        {
            List<Package> result = null;

            if (string.Equals(appVersion.ToLower(), "latest", StringComparison.InvariantCultureIgnoreCase))
            {
                appVersion = ".*";
            }

            if (appVersion.Contains("*") || appVersion.Contains(".*"))
            {
                var filterVersion = appVersion;

                result = Packages.Where(x => string.Equals(x.ApplicationName, appName, StringComparison.InvariantCultureIgnoreCase)
                                && System.Text.RegularExpressions.Regex.IsMatch(x.ApplicationVersion, filterVersion, System.Text.RegularExpressions.RegexOptions.None)).OrderByDescending(y => y.ApplicationVersion, new VersionComparer()).ToList();

                return result.First().ApplicationVersion;
            }

            return appVersion;
        }

        private List<Package> getPackages(string appName, string appVersion, bool filterWithAnd)
        {

            appVersion = getHighestVersionFromWildcard(appName, appVersion);

            List<Package> package;
            if (filterWithAnd)
                package = Packages.Where(x => string.Equals(appName, x.ApplicationName, StringComparison.InvariantCultureIgnoreCase)
                                            && string.Equals(appVersion, x.ApplicationVersion, StringComparison.InvariantCultureIgnoreCase)).ToList();
            else
                package = Packages.Where(x => string.Equals(appName, x.ApplicationName, StringComparison.InvariantCultureIgnoreCase)
                                        || string.Equals(appVersion, x.ApplicationVersion, StringComparison.InvariantCultureIgnoreCase)).ToList();
            return package;
        }

        public List<Package> GetPackagesThatStartsWith(string filterCommand)
        {
            return Packages.Where(x => x.ApplicationName.StartsWith(filterCommand)
                                        || x.ApplicationVersion.StartsWith(filterCommand)).ToList();
        }

        public List<Package> GetFullDownloadListForApplication(string appName, string version)
        {
            return getAllDependenciesForApplication(appName, version).Distinct().ToList();
        }


        private List<Package> getAllDependenciesForApplication(string appName, string appVersion)
        {
            var dependencies = new List<Package>();
            var rootPackage = GetPackage(appName, appVersion);


            dependencies.Add(rootPackage);

            if (rootPackage.DependentPackages.Count > 0)
            {
                foreach (var package in rootPackage.DependentPackages)
                {
                    dependencies.AddRange(getAllDependenciesForApplication(package.ApplicationName, package.ApplicationVersion));
                }
            }

            return dependencies;
        }

        public DateTime Created { get; set; }
        public List<Package> Packages { get; set; }
    }

    public class Package
    {
        public string ForeignPath { get; set; }
        public string ApplicationName { get; set; }
        public string ApplicationVersion { get; set; }
        public List<Package> DependentPackages { get; set; }
    }
}
