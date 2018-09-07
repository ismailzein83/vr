using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Data.RDB
{
    public static class RDBDataProviderFactory
    {
        public static BaseRDBDataProvider CreateProvider(string moduleName, string appSettingWithConnectionStringName, string defaultConnectionStringName)
        {
            return new DataProvider.Providers.MSSQLRDBDataProvider(GetConnectionString(appSettingWithConnectionStringName, defaultConnectionStringName));
        }

        public static BaseRDBDataProvider CreateProvider(string moduleName, string connectionStringName)
        {
            return new DataProvider.Providers.MSSQLRDBDataProvider(ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString);
        }

        private static string GetConnectionString(string appSettingWithConnectionStringName, string defaultConnectionStringName)
        {
            if (String.IsNullOrEmpty(appSettingWithConnectionStringName))
                throw new ArgumentNullException("appSettingWithConnectionStringName");
            if (String.IsNullOrEmpty(defaultConnectionStringName))
                throw new ArgumentNullException("defaultConnectionStringName");

            string connStringName = ConfigurationManager.AppSettings[appSettingWithConnectionStringName] ?? defaultConnectionStringName;

            return ConfigurationManager.ConnectionStrings[connStringName].ConnectionString;
        }
    }
}
