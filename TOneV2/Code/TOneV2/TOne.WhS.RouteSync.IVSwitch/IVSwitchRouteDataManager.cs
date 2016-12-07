using System;
using Npgsql;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Vanrise.Data.Postgres;

namespace TOne.WhS.RouteSync.IVSwitch
{
    public class IVSwitchRouteDataManager : BasePostgresDataManager
    {
        private string _connectionString;
        private string _ownerName;


        public IVSwitchRouteDataManager(string connectionString, string ownerName)
        {
            _connectionString = connectionString;
            _ownerName = ownerName;
        }

        protected override string GetConnectionString()
        {
            return _connectionString;
        }

        public Dictionary<string, string> GetAccessListTableNames()
        {
            Dictionary<string, string> customerDictionary = new Dictionary<string, string>();
            string query = @"select tablename 
                            from pg_tables  
                            where schemaname = 'public'
                            and tablename not like '%_temp'
                            and tablename not like '%_old'
                            and tablename like 'rt%'";
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
                            if (!customerDictionary.ContainsKey(tableName)) customerDictionary[tableName] = tableName;
                        }
                    }
                }
            }
            return customerDictionary;
        }
        public void Bulk(List<IVSwitchRoute> routeLst, string tableName)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(GetConnectionString()))
            {
                if (conn.State == ConnectionState.Closed) conn.Open();
                using (var inStream = conn.BeginTextImport(string.Format("COPY {0} FROM STDIN", tableName)))
                {
                    foreach (var route in routeLst)
                        inStream.WriteLine(route);
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
        public void CreatePrimaryKey(string routeTableName)
        {
            string[] query =
            {
                string.Format(
                    @"ALTER TABLE {0} ADD PRIMARY KEY (destination,route_id,time_frame, preference)",
                    routeTableName)
            };
            ExecuteNonQuery(query);
        }
        public void BuildRouteTable(string routeTableName)
        {
            string[] tableSql = { 
               string.Format("DROP TABLE IF EXISTS {0}_temp;",routeTableName),
                string.Format("DROP TABLE IF EXISTS {0}_OLD; ",routeTableName),
               string.Format(@"CREATE TABLE {0}_temp (        
                  destination character varying(20) NOT NULL,
                  route_id integer NOT NULL,
                  time_frame character varying(50) NOT NULL,
                  preference smallint,
                  huntstop smallint,
                  huntstop_rc character varying(50) DEFAULT NULL::character varying,
                  min_profit numeric(5,5) DEFAULT NULL::numeric,
                  state_id smallint,
                  wakeup_time timestamp without time zone,
                  description character varying(50),
                  routing_mode integer DEFAULT 1,
                  total_bkts integer DEFAULT 1,
                  bkt_serial integer DEFAULT 1,
                  bkt_capacity integer DEFAULT 1,
                  bkt_tokens integer DEFAULT 1,
                  p_score integer DEFAULT 0,
                  flag_1 numeric DEFAULT 0,
                  flag_2 numeric DEFAULT 0,
                  flag_3 integer DEFAULT 0,
                  flag_4 integer DEFAULT 0,
                  flag_5 numeric DEFAULT 0,
                  tech_prefix character varying(20)
                  );",routeTableName),
                 string.Format("ALTER TABLE {0}_temp OWNER TO {1}",routeTableName,_ownerName) 
                };
            ExecuteNonQuery(tableSql);
        }

    }
}
