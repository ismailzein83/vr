using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.Postgres;

namespace NP.IVSwitch.Data.Postgres
{
    public class RouteTableDataManager:  BasePostgresDataManager 
    {
        public RouteTableDataManager()
            : base(GetConnectionStringName("NetworkProvisioningDBConnStringRouteTablesKey", "NetworkProvisioningDBConnStringRouteTables"))
        {

        }

        public int CreateRouteTable(int routeId)
        {

            if (routeId != -1)
            {
                // Create routeId table
                String[] cmdText  = {
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
                                        string.Format("ALTER TABLE  rt{0}  OWNER TO zeinab;",routeId.ToString()) 

                                    };

                ExecuteNonQuery(cmdText);
                return routeId;
            }

 

            return -1;
        }

    }
}