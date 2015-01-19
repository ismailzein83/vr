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
        public BaseDataManager(string connectionStringKey)
        {
            _connectionString = ConfigurationManager.ConnectionStrings[connectionStringKey].ConnectionString;
        }

        public BaseDataManager()
            : this("MainDBConnString")
        {
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
