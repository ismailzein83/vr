using NP.IVSwitch.Entities.RouteTable;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.Postgres;

namespace NP.IVSwitch.Data.Postgres
{
    public class RouteTableDataManager : BasePostgresDataManager, IRouteTableDataManager
    {
        public TOne.WhS.RouteSync.IVSwitch.BuiltInIVSwitchSWSync IvSwitchSync { get; set; }

        protected override string GetConnectionString()
        {
            return IvSwitchSync.MasterConnectionString;
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
                string.Format("ALTER TABLE  rt{0}  OWNER TO {1};",routeId.ToString(),IvSwitchSync.OwnerName) 

            };
            ExecuteNonQuery(cmdText);
            return routeId;
        }

        public bool Insert(RouteTableInput routeTableInput, out int insertedId)
        {

            String cmdText = @"INSERT INTO route_tables(route_table_name,description,p_score)
	                             SELECT  @route_table_name,@description, @p_score 
 	                             returning  route_table_id;";

            int? id = (int)ExecuteScalarText(cmdText, (cmd) =>
            {
                cmd.Parameters.AddWithValue("@route_table_name", routeTableInput.RouteTable.Name);
                cmd.Parameters.AddWithValue("@description", routeTableInput.RouteTable.Description);
                cmd.Parameters.AddWithValue("@p_score", routeTableInput.RouteTable.PScore);
            });

            bool result = id.HasValue;
            if (result)
                insertedId = (int)id;
            else
                insertedId = 0;
            return result;

        }

        public bool Update(RouteTableInput routeTableInput)
        {
            String cmdText1 = @"UPDATE route_tables
	                             SET  route_table_name=@route_table_name,description=@description,p_score=@p_score
                                      WHERE  route_table_id = @route_table_id  ";

            int recordsEffected = ExecuteNonQueryText(cmdText1, cmd =>
            {
                cmd.Parameters.AddWithValue("@route_table_name", routeTableInput.RouteTable.Name);
                cmd.Parameters.AddWithValue("@description", routeTableInput.RouteTable.Description);
                cmd.Parameters.AddWithValue("@p_score", routeTableInput.RouteTable.PScore);
                cmd.Parameters.AddWithValue("@route_table_id", routeTableInput.RouteTable.RouteTableId);

            }
           );
            return recordsEffected > 0;
        }

        public List<RouteTable> GetRouteTables()
        {
            String cmdText = @"SELECT route_table_id,route_table_name,description,p_score FROM route_tables;";
            return GetItemsText(cmdText, RouteTableMapper, (cmd) =>
            {
            });
        }

        private RouteTable RouteTableMapper(IDataReader reader)
        {
            RouteTable routeTable = new RouteTable();

            var routeTableId = reader["route_table_id"];
            var name = reader["route_table_name"] ;
            var description = reader["description"] ;
            var pScore = reader["p_score"];

            if (routeTableId != DBNull.Value)
            {
                routeTable.RouteTableId = (int)routeTableId;
            }
            if (name != DBNull.Value)
            {
                routeTable.Name = (string)name;
            }
            if (description != DBNull.Value)
            {
                routeTable.Description = (string)description;
            }
            if (pScore != DBNull.Value)
            {
                routeTable.PScore = (int?)pScore;
            }
         
            return routeTable;
        }


    }
}