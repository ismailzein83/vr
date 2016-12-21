using System;
using System.Collections.Generic;
using System.Data;
using NP.IVSwitch.Entities;
using Vanrise.Data.Postgres;

namespace NP.IVSwitch.Data.Postgres
{
    public class RouteTableDataManager : BasePostgresDataManager, ICustomerRouteDataManager
    {
        public TOne.WhS.RouteSync.IVSwitch.BuiltInIVSwitchSWSync IvSwitchSync { get; set; }
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

        public List<CustomerRoute> GetCustomerRoutes(string routeTableName, int top, string orderBy, string codePrefix)
        {
            string destinationQuery = string.Empty;
            if (!string.IsNullOrEmpty(codePrefix))
            {
                destinationQuery = string.Format("where destination like '{0}'", codePrefix);
            }
            string query = string.Format(@"
                            ;with TopDistinctCodes as (
                            select distinct destination from {0}
                            {3}
                            order by destination {2}
                            limit {1}
                            )
                            select rt190.destination,rt190.route_id,rt190.flag_1 from TopDistinctCodes
                            join {0} on {0}.destination = TopDistinctCodes.destination"
                               , routeTableName, top, orderBy, destinationQuery);
            return GetItemsText(query, CustomerRouteMapper, null);
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

        #region mapper

        private CustomerRoute CustomerRouteMapper(IDataReader reader)
        {
            return new CustomerRoute
            {
                Destination = reader["destination"] as string,
                RouteId = GetReaderValue<int>(reader, "route_id"),
                Percentage = GetReaderValue<decimal>(reader, "flag_1")
            };
        }
        #endregion
    }
}