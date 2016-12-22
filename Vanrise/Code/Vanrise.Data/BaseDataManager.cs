﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Configuration;
using System.Data.Common;

namespace Vanrise.Data
{
    public abstract class BaseDataManager
    {
        static int s_decimalPrecision;
        private string _connectionString;

        static BaseDataManager()
        {
            if (!int.TryParse(ConfigurationManager.AppSettings["BCP_DecimalPrecision"], out s_decimalPrecision))
                s_decimalPrecision = 6;
        }
        public BaseDataManager(string connectionStringName): this(connectionStringName, true)
        {

        }

        public BaseDataManager(string connectionString, bool getFromConfigSection)
        {
            if (connectionString == null)
                throw new ArgumentNullException("connectionString");

            if (getFromConfigSection)
                _connectionString = GetConnectionString(connectionString);
            else
                _connectionString = connectionString;
        }

        public static object GetDecimalForBCP(Decimal value)
        {
            return Math.Round(value, s_decimalPrecision);
        }

        public static object GetDateTimeForBCP(DateTime? value)
        {
            if (!value.HasValue)
                return null;

            return GetDateTimeForBCP(value.Value);
        }

        public static object GetDateTimeForBCP(DateTime value)
        {
            return value.ToString("yyyy-MM-dd HH:mm:ss.fff");
        }

        public BaseDataManager()
            : this("MainDBConnString")
        {
        }

        protected virtual string GetConnectionString()
        {
            return _connectionString;
        }

        protected static string GetConnectionStringName(string appSettingWithConnectionStringName, string defaultConnectionStringName)
        {
            if(String.IsNullOrEmpty(appSettingWithConnectionStringName))
                throw new ArgumentNullException("appSettingWithConnectionStringName");
            if (String.IsNullOrEmpty(defaultConnectionStringName))
                throw new ArgumentNullException("defaultConnectionStringName");

            return ConfigurationManager.AppSettings[appSettingWithConnectionStringName] ?? defaultConnectionStringName;           
        }

        protected static string GetConnectionString(string connectionStringName)
        {
            if (String.IsNullOrEmpty(connectionStringName))
                throw new ArgumentNullException("connectionStringName");
            //string connectionString;
            //if(Vanrise.Security.Entities.ContextFactory.GetContext().DoesCurrentTenantHaveConnectionString(connectionStringName, out connectionString))
            //    return connectionString;
            var connectionStringEntry = ConfigurationManager.ConnectionStrings[connectionStringName];
            if (connectionStringEntry == null)
                throw new Exception(String.Format("Connection String not found. Connection String Name '{0}'", connectionStringName));
            return connectionStringEntry.ConnectionString;
        }

        #region Get Reader Field

        protected T GetReaderValue<T>(IDataReader reader, string fieldName)
        {
            return reader[fieldName] != DBNull.Value ? (T)reader[fieldName] : default(T);
        }

        #endregion

        protected object ToDBNullIfDefault(int value)
        {
            if (value == default(int))
                return DBNull.Value;
            else
                return value;
        }

        protected object ToDBNullIfDefault(long value)
        {
            if (value == default(long))
                return DBNull.Value;
            else
                return value;
        }

        protected object ToDBNullIfDefault(DateTime value)
        {
            if (value == default(DateTime))
                return DBNull.Value;
            else
                return value;
        }
    }
}
