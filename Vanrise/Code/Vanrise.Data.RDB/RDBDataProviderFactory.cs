﻿using System;
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
