using Npgsql;
using System.Collections.Generic;
using System.Data;
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
        public void Swap(string tableName)
        {
            string[] swapQuery =
            {
                string.Format("ALTER TABLE {0} RENAME TO {1}; ALTER TABLE {2} RENAME TO {0}; ", tableName, tableName + "_OLD", tableName + "_temp")
            };
            ExecuteNonQuery(swapQuery);
        }
        public void Bulk(List<IVSwitchTariff> tariffs, string tableName)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(GetConnectionString()))
            {
                if (conn.State == ConnectionState.Closed) conn.Open();
                using (var inStream = conn.BeginTextImport(string.Format("COPY {0} FROM STDIN", tableName)))
                {
                    tariffs.ForEach(tariff => inStream.WriteLine(tariff));
                }
            }
        }
        public void BuildTariffTable(string tariffTableName)
        {
            string[] tableSql =
            {
                string.Format("DROP TABLE IF EXISTS {0}_temp; ", tariffTableName),
                string.Format("DROP TABLE IF EXISTS {0}_OLD;", tariffTableName),
                string.Format(@"CREATE TABLE {0}_temp (
                              dest_code character varying(30) NOT NULL,
                              time_frame character varying(50) NOT NULL,
                              dest_name character varying(100) DEFAULT NULL::character varying,
                              init_period integer,
                              next_period integer,
                              init_charge numeric(18,9) DEFAULT NULL::numeric,
                              next_charge numeric(18,9) DEFAULT NULL::numeric
                            );", tariffTableName),
                string.Format(" ALTER TABLE {0}_temp OWNER TO {1};", tariffTableName, _ownerName),
                string.Format("ALTER TABLE {0}_temp ADD PRIMARY KEY (dest_code,time_frame)", tariffTableName)
            };
            ExecuteNonQuery(tableSql);
        }
    }
}
