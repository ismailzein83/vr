using NP.IVSwitch.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.Postgres;

namespace NP.IVSwitch.Data.Postgres
{
    public class RouteDataManager : BasePostgresDataManager, IRouteDataManager
    {public RouteDataManager()
            : base(GetConnectionStringName("NetworkProvisioningDBConnStringKey", "NetworkProvisioningDBConnString"))
        {

        }

        private Route RouteMapper(IDataReader reader)
        {

            Route route = new Route();

            route.RouteId = (int)reader["route_id"];
            route.AccountId = (int)reader["account_id"];
            route.Description = reader["description"] as string;
            route.GroupId = (int)reader["group_id"];
            route.TariffId = (int)reader["tariff_id"];
            route.LogAlias = reader["log_alias"] as string;
            route.CodecProfileId = (int)reader["codec_profile_id"];
            route.TransRuleId = (int)reader["trans_rule_id"];
            route.CurrentState =  (State)(Int16)reader["state_id"];
            route.ChannelsLimit = (int)reader["channels_limit"];
            route.WakeUpTime =  (DateTime)reader["wakeup_time"];
            route.Host = reader["host"] as string;
            route.Port = reader["port"] as string;
       //     route.TransportModeId = (TransportMode)(int)reader["transport_mode_id"];
            route.ConnectionTimeOut = (int)reader["timeout"];
 

            return route;
        }


        public List<Route> GetRoutes()
        {
            String cmdText = @"SELECT route_id,account_id,description,group_id,tariff_id,log_alias,codec_profile_id,trans_rule_id,state_id,channels_limit,
                                      wakeup_time,host,port ,timeout          
                                       FROM routes;";
            return GetItemsText(cmdText, RouteMapper, (cmd) =>
            {
            });
        }

        public bool Update(Route route)
        {

            int currentState  ;

            MapEnum(route, out currentState );
 
            String cmdText = @"UPDATE routes
	                             SET  description=@description,group_id=@group_id,tariff_id=@tariff_id,
                                   log_alias=@log_alias,codec_profile_id=@codec_profile_id,trans_rule_id=@trans_rule_id,state_id=@state_id,
                                   channels_limit=@channels_limit, wakeup_time=@wakeup_time,host=@host,port=@port,
                                     timeout=@timeout 
                                   WHERE  route_id = @route_id;";

            int recordsEffected = ExecuteNonQueryText(cmdText, (cmd) =>
            {
                cmd.Parameters.AddWithValue("@route_id", route.RouteId);                 
                cmd.Parameters.AddWithValue("@description", route.Description);
                cmd.Parameters.AddWithValue("@group_id", route.GroupId);
                cmd.Parameters.AddWithValue("@tariff_id", route.TariffId);
                cmd.Parameters.AddWithValue("@log_alias", route.LogAlias);
                cmd.Parameters.AddWithValue("@codec_profile_id", route.CodecProfileId);
                cmd.Parameters.AddWithValue("@trans_rule_id", route.TransRuleId);
                cmd.Parameters.AddWithValue("@state_id", currentState);
                cmd.Parameters.AddWithValue("@channels_limit", route.ChannelsLimit);
                cmd.Parameters.AddWithValue("@wakeup_time", route.WakeUpTime);
                cmd.Parameters.AddWithValue("@host", route.Host);
                cmd.Parameters.AddWithValue("@port", route.Port);
           //     cmd.Parameters.AddWithValue("@transport_mode_id", transportPortId);
                cmd.Parameters.AddWithValue("@timeout", route.ConnectionTimeOut);
              

            }
           );
            return (recordsEffected > 0);
        }

        public bool Insert(Route route, out int insertedId)
        {
            object routeId;
            int currentState;


            MapEnum(route, out currentState );


            String cmdText = @"INSERT INTO routes(account_id,description,group_id,tariff_id,
                                   log_alias,codec_profile_id,trans_rule_id,state_id, channels_limit, wakeup_time, host,port,
                                    timeout  )
	                             SELECT  @account_id, @description, @group_id, @tariff_id, @log_alias, @codec_profile_id, @trans_rule_id,@state_id,
                                 @channels_limit,  @wakeup_time, @host, @port,  @timeout 
 	                             returning  route_id;";

            routeId = ExecuteScalarText(cmdText, (cmd) =>
            {
                 cmd.Parameters.AddWithValue("@account_id", route.AccountId);
                cmd.Parameters.AddWithValue("@description", route.Description);
                cmd.Parameters.AddWithValue("@group_id", route.GroupId);
                cmd.Parameters.AddWithValue("@tariff_id", route.TariffId);
                cmd.Parameters.AddWithValue("@log_alias", route.LogAlias);
                cmd.Parameters.AddWithValue("@codec_profile_id", route.CodecProfileId);
                cmd.Parameters.AddWithValue("@trans_rule_id", route.TransRuleId);
                cmd.Parameters.AddWithValue("@state_id", currentState);
                cmd.Parameters.AddWithValue("@channels_limit", route.ChannelsLimit);
                cmd.Parameters.AddWithValue("@wakeup_time", route.WakeUpTime);
                cmd.Parameters.AddWithValue("@host", route.Host);
                cmd.Parameters.AddWithValue("@port", CheckIfNull(route.Port, "5060"));
               // cmd.Parameters.AddWithValue("@transport_mode_id", transportPortId);
                cmd.Parameters.AddWithValue("@timeout", route.ConnectionTimeOut);
 
            }
            );

            insertedId = -1;
            if (routeId != null)
            {
                insertedId = Convert.ToInt32(routeId);
                return true;
            }
            else
                return false;

        }

        private void MapEnum(Route route, out int currentState )
        {
            var currentStateValue = Enum.Parse(typeof(State), route.CurrentState.ToString());
            currentState = (int)currentStateValue;
  
     

        }

        private Object CheckIfNull(String parameter, Object DefaultValue)
        {

            return (String.IsNullOrEmpty(parameter)) ? DefaultValue : parameter;

        }

    }
}