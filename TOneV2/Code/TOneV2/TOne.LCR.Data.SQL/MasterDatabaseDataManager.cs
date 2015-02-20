using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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

        public void CreateDatabase(string databaseName)
        {
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
