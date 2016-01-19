﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Data.SQL
{
    public class MasterDatabaseDataManager : BaseSQLDataManager
    {
        public MasterDatabaseDataManager(string connectionString)
            : base(connectionString, false)
        {
            SqlConnectionStringBuilder connStringBuilder = new SqlConnectionStringBuilder(connectionString);
            connStringBuilder.InitialCatalog = "Master";
            _connectionString = connStringBuilder.ConnectionString;
        }

        string _connectionString;

        protected override string GetConnectionString()
        {
            return _connectionString;
        }

        /// <summary>
        /// Create new Database.
        /// </summary>
        /// <param name="databaseName">Database Name</param>
        /// <param name="dataFileDirectory">Data file Directory</param>
        /// <param name="logFileDirectory">Log file Directory</param>
        public void CreateDatabase(string databaseName, string dataFileDirectory, string logFileDirectory)
        {
            if (dataFileDirectory != null && logFileDirectory != null)
            {
                string dataFilePath = Path.Combine(dataFileDirectory, databaseName);
                string logFilePath = Path.Combine(logFileDirectory, databaseName);
                string createDBWithPath = String.Format(@"
                                                        CREATE DATABASE {0} ON PRIMARY
                                                        ( NAME = '{0}', FILENAME = N'{1}.mdf')
                                                         LOG ON 
                                                        ( NAME = '{0}_log',FILENAME = N'{2}_log.ldf' )",
                                                         databaseName, dataFilePath, logFilePath);
                ExecuteNonQueryText(createDBWithPath, null);
            }
            else
                ExecuteNonQueryText(String.Format(@"CREATE DATABASE {0}", databaseName), null);
        }

        /// <summary>
        /// Drop database by name
        /// </summary>
        /// <param name="databaseName">Database Name</param>
        public void DropDatabaseWithForceIfExists(string databaseName)
        {
            string queryDropDatabase = String.Format(@"USE master                                            
                                                        IF EXISTS(select * from sys.databases where name='{0}')
                                                        BEGIN
                                                            ALTER DATABASE [{0}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE
                                                            DROP DATABASE {0}
                                                        END", databaseName);
            ExecuteNonQueryText(queryDropDatabase, null);
        }
    }
}
