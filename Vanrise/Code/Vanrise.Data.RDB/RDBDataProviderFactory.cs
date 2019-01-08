using System;
using System.Collections.Generic;
using System.Configuration;
using Vanrise.Common;

namespace Vanrise.Data.RDB
{
    public static class RDBDataProviderFactory
    {
        static bool s_appendRDBToConnStringName = ConfigurationManager.AppSettings["RDB_AppendRDBToConnStringName"] == "true";

        public static BaseRDBDataProvider CreateProvider(string moduleName, string appSettingWithConnectionStringName, string defaultConnectionStringName)
        {
            var connString = GetConnectionString(appSettingWithConnectionStringName, defaultConnectionStringName);
            return CreateProviderFromConnString(moduleName, connString);
        }

        public static BaseRDBDataProvider CreateProvider(string moduleName, string connectionStringName)
        {
            var connString = ConfigurationManager.ConnectionStrings[GetConnectionStringName(connectionStringName)].ConnectionString;
            return CreateProviderFromConnString(moduleName, connString);
        }

        public static BaseRDBDataProvider CreateProviderFromConnString(string moduleName, string connectionString)
        {
            return new DataProvider.Providers.MSSQLRDBDataProvider(connectionString);
        }

        private static string GetConnectionString(string appSettingWithConnectionStringName, string defaultConnectionStringName)
        {
            if (appSettingWithConnectionStringName == null && defaultConnectionStringName == null)
                throw new NullReferenceException("appSettingWithConnectionStringName & defaultConnectionStringName");
            string connStringName = null;
            if (appSettingWithConnectionStringName != null)
                connStringName = ConfigurationManager.AppSettings[appSettingWithConnectionStringName];
            if (connStringName == null)
                connStringName = defaultConnectionStringName;

            connStringName = GetConnectionStringName(connStringName);
            var connString = ConfigurationManager.ConnectionStrings[connStringName];
            connString.ThrowIfNull("connString", connStringName);
            
            return connString.ConnectionString;
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
