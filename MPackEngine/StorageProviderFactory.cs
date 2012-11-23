using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Contract;
using System.Configuration;

namespace MPackEngine
{
    static public class StorageProviderFactory
    {
        static public IStorageProvider GetSourceRepositoryProvider()
        {
            return ConfigurationManager.GetSection("SourceRepositoryProvider") as IStorageProvider;
        }

        static public IStorageProvider GetDestinationeRepositoryProvider()
        {
            return ConfigurationManager.GetSection("DestinationRepositoryProvider") as IStorageProvider;
        }
    }
}
