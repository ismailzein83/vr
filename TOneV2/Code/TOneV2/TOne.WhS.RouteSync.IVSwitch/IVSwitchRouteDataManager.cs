using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data;
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
        public void CreatePrimaryKey(string routeTableName)
        {
            string[] query =
            {
                string.Format(
                    @"ALTER TABLE {0}_temp" + "  ADD PRIMARY KEY (destination,route_id,time_frame, preference)",
                    routeTableName)
            };
            ExecuteNonQuery(query);
        }
        public void BuildRouteTable(string routeTableName)
        {
            string[] tableSql = { 
                @"DROP TABLE IF EXISTS " + routeTableName+"_temp" + "; ",
                 @"DROP TABLE IF EXISTS " + routeTableName+"_OLD" + "; ",
                @"CREATE TABLE " + routeTableName+"_temp" + @" (        
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
                  );",
                 "ALTER TABLE "+routeTableName+"_temp OWNER TO "+_ownerName+";" 
                };
            ExecuteNonQuery(tableSql);
        }

    }
}
