using System;
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
        protected string _connectionString;
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

        public BaseDataManager()
            : this("MainDBConnString")
        {
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
            var connectionString = ConfigurationManager.ConnectionStrings[connectionStringName];
            if (connectionString == null)
                throw new Exception(String.Format("Connection String not found. Connection String Name '{0}'", connectionStringName));
            return connectionString.ConnectionString;
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

        protected object ToDBNullIfDefault(DateTime value)
        {
            if (value == default(DateTime))
                return DBNull.Value;
            else
                return value;
        }
    }
}
