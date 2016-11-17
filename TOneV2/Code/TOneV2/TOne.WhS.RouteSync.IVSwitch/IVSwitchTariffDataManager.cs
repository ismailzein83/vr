using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.Postgres;

namespace TOne.WhS.RouteSync.IVSwitch
{
    public class IVSwitchTariffDataManager : BasePostgresDataManager
    {
        private string _connectionString;
        private string _ownerName;

        public IVSwitchTariffDataManager(string connectionString, string ownerName)
        {
            _connectionString = connectionString;
            _ownerName = ownerName;
        }
        protected override string GetConnectionString()
        {
            return _connectionString;
        }
        private void ExecuteNonQuery(string[] sqlStrings)
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(_connectionString))
            {
                if (connection.State == ConnectionState.Closed) connection.Open();
                using (NpgsqlCommand command = connection.CreateCommand())
                {
                    command.CommandType = CommandType.Text;
                    foreach (string sql in sqlStrings)
                    {
                        command.CommandText = sql;
                        command.ExecuteNonQuery();
                    }
                }
            }
        }
        public void Swap(string tableName)
        {
            string[] swapQuery =
            {
                string.Format("ALTER TABLE {0} RENAME TO {1}; ALTER TABLE {2} RENAME TO {0}; ", tableName, tableName + "_OLD", tableName + "_temp")
            };
            ExecuteNonQuery(swapQuery);
        }
        public void BuildTariffTable(string tariffTableName)
        {
            string[] tableSql = { 
              @"DROP TABLE IF EXISTS " + tariffTableName+"_temp" + "; ",
              @"DROP TABLE IF EXISTS " + tariffTableName+"_OLD" + "; ",               
              @"CREATE TABLE " + tariffTableName+"_temp" + @" (
              dest_code character varying(30) NOT NULL,
              time_frame character varying(50) NOT NULL,
              dest_name character varying(100) DEFAULT NULL::character varying,
              init_period integer,
              next_period integer,
              init_charge numeric(18,9) DEFAULT NULL::numeric,
              next_charge numeric(18,9) DEFAULT NULL::numeric
            );",
             " ALTER TABLE "+tariffTableName+"_temp OWNER TO "+_ownerName+";"  
                , "ALTER TABLE " + tariffTableName + "_temp" + "  ADD PRIMARY KEY (dest_code,time_frame)"
                };
            ExecuteNonQuery(tableSql);
        }
    }
}
