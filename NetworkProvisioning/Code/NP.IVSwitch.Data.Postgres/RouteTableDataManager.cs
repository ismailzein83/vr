using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.Postgres;

namespace NP.IVSwitch.Data.Postgres
{
    public class RouteTableDataManager : BasePostgresDataManager
    {
        private string ConnectionString { get; set; }
        private string Owner { get; set; }

        public RouteTableDataManager(string connectionString, string owner)
        {
            ConnectionString = connectionString;
            Owner = owner;
        }
        protected override string GetConnectionString()
        {
            return ConnectionString;
        }
        public bool InsertHelperRoute(int routeId, string description)
        {
            string query = @"INSERT INTO ui_helper_routes(
	                            route_id, description)
	                            SELECT @route_id, @description WHERE NOT EXISTS 
                                ( SELECT 1 FROM ui_helper_routes WHERE (route_id = @route_id ))";
            int recordsEffected = ExecuteNonQueryText(query, cmd =>
            {
                cmd.Parameters.AddWithValue("@route_id", routeId);
                cmd.Parameters.AddWithValue("@description", description);
            });
            return recordsEffected > 0;
        }
        public int CreateRouteTable(int routeId)
        {
            if (routeId == -1) return -1;

            String[] cmdText = {
                string.Format(@"CREATE TABLE rt{0} (
                                                      destination character varying(20) NOT NULL,
                                                      route_id integer NOT NULL,
                                                      time_frame character varying(50) NOT NULL,
                                                      preference smallint NOT NULL,
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
                                                      flag_1 numeric DEFAULT 1,
                                                      flag_2 numeric DEFAULT 1,
                                                      flag_3 integer DEFAULT 1,
                                                      flag_4 integer DEFAULT 1,
                                                      flag_5 numeric DEFAULT 1,
                                                      tech_prefix character varying(20),
                                                      CONSTRAINT rt{0}_pkey PRIMARY KEY (destination, route_id, time_frame, preference)
                                                    )
                                                    WITH (
                                                      OIDS=FALSE
                                                    );",routeId.ToString()),
                string.Format("ALTER TABLE  rt{0}  OWNER TO {1};",routeId.ToString(),Owner) 

            };
            ExecuteNonQuery(cmdText);
            return routeId;
        }

    }
}