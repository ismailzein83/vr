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
        static bool s_appendRDBToConnStringName = ConfigurationManager.AppSettings["RDB_AppendRDBToConnStringName"] == "true";
        public static BaseRDBDataProvider CreateProvider(string moduleName, string appSettingWithConnectionStringName, string defaultConnectionStringName)
        {
            return new DataProvider.Providers.MSSQLRDBDataProvider(GetConnectionString(appSettingWithConnectionStringName, defaultConnectionStringName));
        }

        public static BaseRDBDataProvider CreateProvider(string moduleName, string connectionStringName)
        {
            return new DataProvider.Providers.MSSQLRDBDataProvider(ConfigurationManager.ConnectionStrings[GetConnectionStringName(connectionStringName)].ConnectionString);
        }

        private static string GetConnectionString(string appSettingWithConnectionStringName, string defaultConnectionStringName)
        {
            if (String.IsNullOrEmpty(appSettingWithConnectionStringName))
                throw new ArgumentNullException("appSettingWithConnectionStringName");
            if (String.IsNullOrEmpty(defaultConnectionStringName))
                throw new ArgumentNullException("defaultConnectionStringName");

            string connStringName = ConfigurationManager.AppSettings[appSettingWithConnectionStringName] ?? defaultConnectionStringName;

            return ConfigurationManager.ConnectionStrings[GetConnectionStringName(connStringName)].ConnectionString;
        }

        private static string GetConnectionStringName(string origConnStringName)
        {
            if (s_appendRDBToConnStringName)
                return string.Concat(origConnStringName, "_RDB");
            else
                return origConnStringName;
        }
    }
}
