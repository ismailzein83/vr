using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.Data.SQL;

namespace TOne.LCR.Data.SQL
{
    internal class MasterDatabaseDataManager : BaseTOneDataManager
    {
        public MasterDatabaseDataManager(string connectionString)
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

        public void CreateDatabase(string databaseName, string dataFileDirectory, string logFileDirectory)
        {
            if (dataFileDirectory != null && logFileDirectory != null)
            {
                string dataFilePath = Path.Combine(dataFileDirectory, databaseName);
                string logFilePath = Path.Combine(logFileDirectory, databaseName);
                string createDBWithPath = String.Format(@"
                                                        CREATE DATABASE {0} ON PRIMARY
                                                        ( NAME = '{0}', FILENAME = N'{1}.mdf', SIZE = 5120000KB , FILEGROWTH = 10%)
                                                         LOG ON 
                                                        ( NAME = '{0}_log',FILENAME = N'{2}_log.ldf', SIZE = 204800KB , FILEGROWTH = 10% )
                                                        

",
                                                         databaseName, dataFilePath, logFilePath);
                ExecuteNonQueryText(createDBWithPath, null);
                ExecuteNonQueryText(string.Format("ALTER DATABASE {0} SET RECOVERY SIMPLE", databaseName), null);
            }
            else
                ExecuteNonQueryText(String.Format(@"CREATE DATABASE {0}", databaseName), null);
        }

        internal void DropDatabaseWithForceIfExists(string databaseName)
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
