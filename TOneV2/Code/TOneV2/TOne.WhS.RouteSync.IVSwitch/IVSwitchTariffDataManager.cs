using Npgsql;
using System.Collections.Generic;
using System.Data;
using Vanrise.Data.Postgres;
using System.Linq;
using System;
using System.Text;

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

        public Dictionary<string, string> GetRoutesTableNames()
        {
            Dictionary<string, string> supplierDictionary = new Dictionary<string, string>();
            string query = @"select tablename  
                            from pg_tables  
                            where schemaname = 'public'
                            and tablename not like '%_temp'
                            and tablename not like '%_old'
                            and tablename like 'trf%'";
            using (Npgsql.NpgsqlConnection conn = new Npgsql.NpgsqlConnection(_connectionString))
            {
                using (Npgsql.NpgsqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = query;
                    if (conn.State == ConnectionState.Closed) conn.Open();
                    Npgsql.NpgsqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        if (reader["tablename"] != DBNull.Value)
                        {
                            string tableName = reader["tablename"].ToString();
                            if (!supplierDictionary.ContainsKey(tableName)) supplierDictionary[tableName] = tableName;
                        }
                    }
                }
            }
            return supplierDictionary;
        }
        public void Bulk(List<IVSwitchTariff> tariffs, string tableName)
        {
            var tariffSb = new StringBuilder();
            var tariffCounts = tariffs.Count;
            for (int i = 0; i < tariffCounts; i++)
            {
                var tariff = tariffs[i];
                AppendValueWithTabToBuilder(tariffSb, tariff.DestinationCode);
                AppendValueWithTabToBuilder(tariffSb, tariff.TimeFrame);
                AppendValueWithTabToBuilder(tariffSb, tariff.DestinationName);
                AppendValueWithTabToBuilder(tariffSb, tariff.InitPeiod);
                AppendValueWithTabToBuilder(tariffSb, tariff.NextPeriod);
                AppendValueWithTabToBuilder(tariffSb, tariff.InitCharge);
                tariffSb.Append(tariff.NextCharge);
                if (i < tariffCounts - 1) tariffSb.AppendLine();
            }
            using (NpgsqlConnection conn = new NpgsqlConnection(GetConnectionString()))
            {
                if (conn.State == ConnectionState.Closed) conn.Open();
                using (var inStream = conn.BeginTextImport(string.Format("COPY {0} FROM STDIN", tableName)))
                {
                    inStream.WriteLine(tariffSb);
                }
            }
        }
        private void AppendValueWithTabToBuilder(StringBuilder tariffSb, object value)
        {
            tariffSb.Append(value);
            tariffSb.Append("\t");
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
