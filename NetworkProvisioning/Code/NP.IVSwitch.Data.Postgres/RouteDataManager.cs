using NP.IVSwitch.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.RouteSync.IVSwitch;
using Vanrise.Data.Postgres;

namespace NP.IVSwitch.Data.Postgres
{
    public class RouteDataManager : BasePostgresDataManager, IRouteDataManager
    {
        #region public Methods
        public TOne.WhS.RouteSync.IVSwitch.BuiltInIVSwitchSWSync IvSwitchSync { get; set; }
        protected override string GetConnectionString()
        {
            return IvSwitchSync.MasterConnectionString;
        }
        private Route RouteMapper(IDataReader reader)
        {
            Route route = new Route
            {
                RouteId = (int)reader["route_id"],
                UserId = reader["user_id"] != DBNull.Value ? (int)reader["user_id"] : 0,
                AccountId = (int)reader["account_id"],
                Description = reader["description"] as string,
                TariffId = (int)reader["tariff_id"],
                LogAlias = reader["log_alias"] as string,
                CodecProfileId = (int)reader["codec_profile_id"],
                TransRuleId = (int)reader["trans_rule_id"],
                CurrentState = (State)(Int16)reader["state_id"],
                ChannelsLimit = (int)reader["channels_limit"],
                WakeUpTime = (DateTime)reader["wakeup_time"],
                Host = reader["host"] as string,
                Port = reader["port"] as string,
                TransportModeId = (TransportMode)(int)reader["transport_mode_id"],
                ConnectionTimeOut = (int)reader["timeout"]
            };
            return route;
        }
        public DateTime GetSwitchDateTime()
        {
            string query = "select current_timestamp;";
            return (DateTime)ExecuteScalarText(query, null);
        }
        public int GetGlobalTariffTableId()
        {
            String cmdText = @" select tariff_id
                                from tariffs
                                where tariff_name = 'Global'";
            int? tariffId = (int?)ExecuteScalarText(cmdText, null);
            if (!tariffId.HasValue)
            {
                string insertCmd = @"insert into tariffs (tariff_name, description) 
                                                select  'Global' ,'Global' 
                                                where not exists(select 1 from tariffs where tariff_name = 'Global')
                                                returning tariff_id";
                tariffId = (int?)ExecuteScalarText(insertCmd, null);
                TariffDataManager tariffDataManager = new TariffDataManager(IvSwitchSync.TariffConnectionString, IvSwitchSync.OwnerName);
                tariffDataManager.CreateGlobalTable(tariffId.Value);
            }
            return tariffId.Value;
        }
        public List<Route> GetRoutes()
        {
            String cmdText = @"SELECT route_id,user_id,account_id,description,group_id,tariff_id,log_alias,codec_profile_id,trans_rule_id,state_id,channels_limit,
                                      wakeup_time,host,port ,transport_mode_id,timeout          
                                       FROM routes;";
            return GetItemsText(cmdText, RouteMapper, (cmd) =>
            {
            });
        }
        public bool Update(Route route)
        {
            int currentState, transportPortId;
            MapEnum(route, out currentState, out transportPortId);

            String cmdText = @"UPDATE routes
	                             SET  description=@description,log_alias=@log_alias,codec_profile_id=@codec_profile_id,trans_rule_id=@trans_rule_id,state_id=@state_id,
                                   channels_limit=@channels_limit, wakeup_time=@wakeup_time,host=@host,port=@port,transport_mode_id=@transport_mode_id,
                                     timeout=@timeout 
                                   WHERE  route_id = @route_id;";

            int recordsEffected = ExecuteNonQueryText(cmdText, (cmd) =>
            {
                cmd.Parameters.AddWithValue("@route_id", route.RouteId);
                cmd.Parameters.AddWithValue("@description", route.Description);
                cmd.Parameters.AddWithValue("@log_alias", route.LogAlias);
                cmd.Parameters.AddWithValue("@codec_profile_id", route.CodecProfileId);
                cmd.Parameters.AddWithValue("@trans_rule_id", route.TransRuleId);
                cmd.Parameters.AddWithValue("@state_id", currentState);
                cmd.Parameters.AddWithValue("@channels_limit", route.ChannelsLimit);
                cmd.Parameters.AddWithValue("@wakeup_time", route.WakeUpTime);
                cmd.Parameters.AddWithValue("@host", route.Host);
                cmd.Parameters.AddWithValue("@port", CheckIfNull(route.Port, "5060"));
                cmd.Parameters.AddWithValue("@transport_mode_id", transportPortId);
                cmd.Parameters.AddWithValue("@timeout", route.ConnectionTimeOut);
            }
           );
            return recordsEffected > 0;
        }

        #endregion

        #region private Methods
        private int? InsertRoutes(Route route, int groupId, int tariffId)
        {
            String cmdText = @"INSERT INTO routes(account_id,user_id,description,group_id,tariff_id,
                                   log_alias,codec_profile_id,trans_rule_id,state_id, channels_limit, wakeup_time, host,port,transport_mode_id,
                                    timeout  )
	                             SELECT  @account_id,@user_id, @description, @group_id, @tariff_id, @log_alias, @codec_profile_id, @trans_rule_id,@state_id,
                                 @channels_limit,  @wakeup_time, @host, @port, @transport_mode_id, @timeout 
 	                             returning  route_id;";

            return (int?)ExecuteScalarText(cmdText, (cmd) =>
            {
                cmd.Parameters.AddWithValue("@account_id", route.AccountId);
                cmd.Parameters.AddWithValue("@user_id", route.UserId);
                cmd.Parameters.AddWithValue("@description", route.Description);
                cmd.Parameters.AddWithValue("@group_id", groupId);
                cmd.Parameters.AddWithValue("@tariff_id", tariffId);
                cmd.Parameters.AddWithValue("@log_alias", route.LogAlias);
                cmd.Parameters.AddWithValue("@codec_profile_id", route.CodecProfileId);
                cmd.Parameters.AddWithValue("@trans_rule_id", route.TransRuleId);
                cmd.Parameters.AddWithValue("@state_id", (int)route.CurrentState);
                cmd.Parameters.AddWithValue("@channels_limit", route.ChannelsLimit);
                cmd.Parameters.AddWithValue("@wakeup_time", route.WakeUpTime);
                cmd.Parameters.AddWithValue("@host", route.Host);
                cmd.Parameters.AddWithValue("@port", CheckIfNull(route.Port, "5060"));
                cmd.Parameters.AddWithValue("@transport_mode_id", (int)route.TransportModeId);
                cmd.Parameters.AddWithValue("@timeout", route.ConnectionTimeOut);
            }
                );
        }
        public int? Insert(Route route)
        {
            int groupId = GetGroupId(route);
            int tariffId = GetGlobalTariffTableId();
            int? userIdInserted = InserVendorUSer(route, groupId);

            if (!userIdInserted.HasValue)
                return null;

            route.UserId = userIdInserted.Value;
            int? routeId = InsertRoutes(route, groupId, tariffId);
            if (routeId.HasValue)
            {
                RouteTableRouteDataManager routeTableDataManager = new RouteTableRouteDataManager {
                    IvSwitchSync = IvSwitchSync
                };
                routeTableDataManager.InsertHelperRoute(routeId.Value, route.Description);
            }
            return routeId;
        }
        private int? InserVendorUSer(Route route, int groupId)
        {
            String cmdText1 = @"INSERT INTO users(account_id,group_id, trans_rule_id,state_id , 
                                                 channels_limit,channels_active,type_id ,enable_trace,log_alias,codec_profile_id)
	                             SELECT  @account_id,  @group_id, @trans_rule_id,@state_id,
                                 @channels_limit, @channels_active,@type_id,@enable_trace,@log_alias,@codec_profile_id
 	                             returning  user_id;";

            return (int?)ExecuteScalarText(cmdText1, cmd =>
            {
                cmd.Parameters.AddWithValue("@account_id", route.AccountId);
                cmd.Parameters.AddWithValue("@group_id", groupId);
                cmd.Parameters.AddWithValue("@trans_rule_id", route.TransRuleId);
                cmd.Parameters.AddWithValue("@channels_limit", route.ChannelsLimit);
                cmd.Parameters.AddWithValue("@channels_active", 0);
                cmd.Parameters.AddWithValue("@type_id", (int)UserType.VendroTermRoute);
                cmd.Parameters.AddWithValue("@enable_trace", 1);
                cmd.Parameters.AddWithValue("@log_alias", route.LogAlias);
                cmd.Parameters.AddWithValue("@codec_profile_id", route.CodecProfileId);
                cmd.Parameters.AddWithValue("@state_id", (int)route.CurrentState);

            }
                );
        }
        public bool UpdateVendorUSer(Route route)
        {
            String cmdText1 = @"UPDATE users
	                             SET  trans_rule_id=@trans_rule_id,channels_limit=@channels_limit,log_alias=@log_alias
                                      ,codec_profile_id= @codec_profile_id,state_id=@state_id
                                      WHERE  user_id = @user_id  ";

            int recordsEffected = ExecuteNonQueryText(cmdText1, cmd =>
            {
                cmd.Parameters.AddWithValue("@user_id", route.UserId);
                cmd.Parameters.AddWithValue("@trans_rule_id", route.TransRuleId);
                cmd.Parameters.AddWithValue("@channels_limit", route.ChannelsLimit);
                cmd.Parameters.AddWithValue("@log_alias", route.LogAlias);
                cmd.Parameters.AddWithValue("@codec_profile_id", route.CodecProfileId);
                cmd.Parameters.AddWithValue("@state_id", (int)route.CurrentState);
            }
           );
            return recordsEffected > 0;
        }
        private void MapEnum(Route route, out int currentState, out int transportPortId)
        {
            var currentStateValue = Enum.Parse(typeof(State), route.CurrentState.ToString());
            currentState = (int)currentStateValue;

            var transportPortIdValue = Enum.Parse(typeof(TransportMode), route.TransportModeId.ToString());
            transportPortId = (int)transportPortIdValue;
        }
        private Object CheckIfNull(String parameter, Object defaultValue)
        {
            return (String.IsNullOrEmpty(parameter)) ? defaultValue : parameter;
        }
        private int GetGroupId(Route route)
        {
            int groupId, nextGroupId;

            String cmdText = @"Select max(group_id) as group_id
                                from routes
                                where account_id = @account_id";
            int groupIdIncremented = GetItemText(cmdText, (reader) => { return GetReaderValue<int>(reader, "group_id"); }, (cmd) =>
            {
                cmd.Parameters.AddWithValue("@account_id", route.AccountId);
            });

            if (groupIdIncremented != 0)
            {
                groupId = groupIdIncremented - route.AccountId;

                String cmdText2 = @"Select next_group_id  from(
                                        Select  group_id as  group_id,  lead(group_id) OVER (ORDER BY group_id ) as next_group_id   
                                        from user_groups)  x
                                         where  group_id = @group_id;";

                nextGroupId = GetItemText(cmdText2, (reader) => { return GetReaderValue<int>(reader, "next_group_id"); }, (cmd) =>
              {
                  cmd.Parameters.AddWithValue("@group_id", groupId);
              });

                if (nextGroupId == 0)
                {
                    //insert new record
                    String cmdText3 = @"INSERT INTO user_groups(description)
	                                        VALUES('Dummy Group')
                                            returning group_id;";

                    object nextGroupIdObject = ExecuteScalarText(cmdText3, (cmd) => { });

                    nextGroupId = Convert.ToInt32(nextGroupIdObject);
                }

            }

            else
            {
                String cmdText2 = @"Select  group_id
                                       from user_groups 
                                       order by group_id
                                       limit 1 ";

                nextGroupId = GetItemText(cmdText2, (reader) => { return GetReaderValue<int>(reader, "group_id"); }, (cmd) => { });
            }


            return nextGroupId + route.AccountId;

        }

        #endregion
    }
}