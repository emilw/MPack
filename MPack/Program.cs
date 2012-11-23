using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Contract;

namespace MPack
{
    class Program
    {
        static void Main(string[] args)
        {

            if (args.Length == 0)
            {
                Console.WriteLine("You need to provide an argument");
                MenuItem_Help();
                return;
            }

            var command = args[0];
            var engine = new MPackEngine.Engine(@"E:\GlobalPackageRepository", "");

            Contract.LocalPackageRepositoryList list;

            switch (args[0].ToUpper())
            {
                case "UPDATE":
                    Console.WriteLine("Updating package repository....");
                    engine.UpdateLocalPackageList();
                    break;
                case "LIST":
                    MenuItem_List(args, engine);
                     
                    break;
                case "GET":
                    MenuItem_Get(args, engine);
                    break;
                case "INFO":
                    MenuItem_Info(args, engine);
                    break;
                case "HELP":
                    MenuItem_Help();
                    break;
                case "ABOUT":
                    MenuItem_About();
                    break;
                default:
                    Console.WriteLine("The argument does not exists!");
                    MenuItem_Help();
                    break;
            }

            Console.WriteLine("Done!");
        }

        static private bool checkArg(int index, string[] arg, ref string value)
        {
            if (arg.Length > index)
            {
                value = arg[index];
                return true;
            }
            else
                return false;
        }


        static public void MenuItem_List(string[] args, MPackEngine.Engine engine)
        {
            string filterCommand = "";

            List<Package> result;

            if (checkArg(1, args, ref filterCommand))
                result = engine.GetCurrentPackageRepositoryList().GetPackagesThatStartsWith(filterCommand);
            else
                result = engine.GetCurrentPackageRepositoryList().Packages;

            foreach (var item in result)
            {
                Console.WriteLine(String.Format("Application: {0} - Version: {1}", item.ApplicationName, item.ApplicationVersion));
            }
        }

        static public void MenuItem_Info(string[] args, MPackEngine.Engine engine)
        {
            string appName = "";
            string appVersion = "";
            if (checkArg(1, args, ref appName) && checkArg(2, args, ref appVersion))
            {
                try
                {
                    var result = engine.GetCurrentPackageRepositoryList().GetPackage(appName, appVersion);
                
                    Console.WriteLine("Getting app info:");
                    Console.WriteLine(String.Format("Application: {0} - Version: {1}", result.ApplicationName, result.ApplicationVersion));

                    Console.WriteLine("Dependecies:");
                    result.DependentPackages.ForEach(x => Console.WriteLine("Application: {0}, version: {1}", x.ApplicationName, x.ApplicationVersion));
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            else
            {
                Console.WriteLine("Check input, give app name as first argument and version as the second", appName, appVersion);
            }
        }

        static public void MenuItem_Get(string[] args, MPackEngine.Engine engine)
        {

            string appName = "";
            string appVersion = "";
            if (checkArg(1, args, ref appName) && checkArg(2, args, ref appVersion))
            {
                try
                {
                    var application = engine.GetCurrentPackageRepositoryList().GetPackage(appName, appVersion);

                    Console.WriteLine("Selected app to download: {0} - {1}", application.ApplicationName, application.ApplicationVersion);

                    var result = engine.GetCurrentPackageRepositoryList().GetFullDownloadListForApplication(appName, appVersion);

                    Console.WriteLine("Full download list:");
                    result.ForEach(x => Console.WriteLine("Application: {0}, version: {1}", x.ApplicationName, x.ApplicationVersion));

                    Console.WriteLine("Downloading...");
                    engine.GetApplication(result);

                    Console.WriteLine("All content was downloaded!");
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            else
            {
                Console.WriteLine("Check input, give app name as first argument and version as the second", appName, appVersion);
            }

        }

        static public void MenuItem_Help()
        {

            Console.WriteLine("Help section:");
            Console.WriteLine("----List [Filter]");
            Console.WriteLine("Lists all applications, Update needs to be performed the first time or when the list is going to be updated with new applications.");

            Console.WriteLine("----Update");
            Console.WriteLine("Downloads the latest list of applications.");


            Console.WriteLine("----Info [AppName] [AppVersion]");
            Console.WriteLine("Get's more detailed information about an application.");


            Console.WriteLine("----Get [AppName] [AppVersion]");
            Console.WriteLine("Get's the application and it's dependencies.");
        }

        static public void MenuItem_About()
        {
            Console.WriteLine("MPack info");
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                Console.WriteLine("Assembly name: {0}", assembly.FullName);
        }
    }
}
