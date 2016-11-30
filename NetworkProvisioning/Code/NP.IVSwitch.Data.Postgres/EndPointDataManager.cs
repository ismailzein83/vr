using NP.IVSwitch.Entities;
 using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.Postgres;

namespace NP.IVSwitch.Data.Postgres
{
    public class EndPointDataManager : BasePostgresDataManager, IEndPointDataManager
    {
        public EndPointDataManager()
            : base(GetConnectionStringName("NetworkProvisioningDBConnStringKey", "NetworkProvisioningDBConnString"))
        {

        }

        private EndPoint EndPointMapper(IDataReader reader)
        {


            EndPoint endPoint = new EndPoint();

            endPoint.EndPointId = (int)reader["user_id"];
            endPoint.AccountId = (int)reader["account_id"];
            endPoint.Description = reader["description"] as string;
 
            endPoint.LogAlias = reader["log_alias"] as string;
            endPoint.CodecProfileId = (int)reader["codec_profile_id"];
            endPoint.TransRuleId = (int)reader["trans_rule_id"];
            endPoint.CurrentState = (State)GetReaderValue<Int16>(reader, "state_id");
            endPoint.ChannelsLimit = GetReaderValue<int>(reader, "channels_limit");
            endPoint.MaxCallDuration = (int)reader["max_call_dura"];
            endPoint.RtpMode = (RtpMode)(int)reader["rtp_mode"];
            endPoint.DomainId = (Int16)reader["domain_id"];
            endPoint.SipLogin = reader["sip_login"] as string;
            endPoint.SipPassword = reader["sip_password"] as string;
            endPoint.TechPrefix = reader["tech_prefix"] as string;

            System.Net.IPAddress Host = GetReaderValue<System.Net.IPAddress>(reader, "host");
            endPoint.Host = (Host == null) ? null : Host.ToString();



            if (endPoint.Host == null)
                endPoint.EndPointType = EndPointType.SIP;
            else
                endPoint.EndPointType = EndPointType.ACL;

            return endPoint;
        }

        private EndPointToUpdate EndPointToUpdateMapper(IDataReader reader)
        {
            EndPointToUpdate endPointToUpdate = new EndPointToUpdate();

            endPointToUpdate.TariffId = GetReaderValue<int?>(reader, "tariff_id");
            endPointToUpdate.RouteTableId = GetReaderValue<int?>(reader, "route_id");
            
            return endPointToUpdate;
        }

        public List<EndPoint> GetEndPoints()
        {

            String cmdText = @"
                                SELECT access_list.user_id,access_list.account_id,access_list.description,access_list.group_id,access_list.tariff_id,
                                access_list.log_alias,access_list.codec_profile_id,access_list.trans_rule_id,access_list.state_id,
                                access_list.channels_limit,access_list.route_table_id,access_list.max_call_dura,access_list.rtp_mode,access_list.domain_id,
                                access_list.host,access_list.tech_prefix,null as sip_login, null as sip_password
                                FROM access_list INNER JOIN users on access_list.user_id = users.user_id
                                UNION
                                SELECT users.user_id,users.account_id, users.description ,users.group_id,users.tariff_id,users.log_alias,users.codec_profile_id,
                                users.trans_rule_id,users.state_id,users.channels_limit,users.route_table_id,users.max_call_dura,users.rtp_mode,
                                users.domain_id,null as host,users.tech_prefix,users.sip_login,users.sip_password
                                FROM users LEFT JOIN access_list on users.user_id = access_list.user_id  where access_list.user_id is null";
            return GetItemsText(cmdText, EndPointMapper, (cmd) =>
            {
            });


        }

        public bool SipUpdate(EndPoint endPoint)
        {

            int currentState, rtpMode;


            MapEnum(endPoint, out currentState, out rtpMode);

            String cmdText = @"UPDATE users
	                             SET  description=@description ,log_alias=@log_alias,codec_profile_id=@codec_profile_id,trans_rule_id=@trans_rule_id,state_id=@state_id,
                                   channels_limit=@channels_limit, max_call_dura=@max_call_dura,rtp_mode=@rtp_mode,domain_id=@domain_id,
                                   sip_login=@sip_login,sip_password=@sip_password ,  tech_prefix= @tech_prefix
                                   WHERE  user_id = @user_id AND NOT EXISTS(SELECT 1 FROM  users WHERE (user_id != @user_id and
                                                                            domain_id=@domain_id and sip_login=@sip_login and tech_prefix=@tech_prefix))";
            int recordsEffected = ExecuteNonQueryText(cmdText, (cmd) =>
           {
               cmd.Parameters.AddWithValue("@user_id", endPoint.EndPointId);
               cmd.Parameters.AddWithValue("@description", endPoint.Description);
               cmd.Parameters.AddWithValue("@log_alias", endPoint.LogAlias);
               cmd.Parameters.AddWithValue("@codec_profile_id", endPoint.CodecProfileId);
               cmd.Parameters.AddWithValue("@trans_rule_id", endPoint.TransRuleId);
               cmd.Parameters.AddWithValue("@state_id", currentState);
               cmd.Parameters.AddWithValue("@channels_limit", endPoint.ChannelsLimit);
               cmd.Parameters.AddWithValue("@max_call_dura", endPoint.MaxCallDuration);
               cmd.Parameters.AddWithValue("@rtp_mode", rtpMode);
               cmd.Parameters.AddWithValue("@domain_id", endPoint.DomainId);
               var prmSipLogin = new Npgsql.NpgsqlParameter("@sip_login", DbType.String);
               prmSipLogin.Value = CheckIfNull(endPoint.SipLogin);
               cmd.Parameters.Add(prmSipLogin);
               var prmPassword = new Npgsql.NpgsqlParameter("@sip_password", DbType.String);
               prmPassword.Value = CheckIfNull(endPoint.SipPassword);
               cmd.Parameters.Add(prmPassword);
               cmd.Parameters.AddWithValue("@tech_prefix", ".");


           }
          );
            return (recordsEffected > 0);
        }

        public bool AclUpdate(EndPoint endPoint)
        {

            int currentState, rtpMode;

            MapEnum(endPoint, out currentState, out rtpMode);

            String cmdText1 = @"UPDATE users
	                             SET  trans_rule_id=@trans_rule_id,state_id=@state_id,channels_limit=@channels_limit, 
                                     max_call_dura=@max_call_dura,rtp_mode=@rtp_mode,domain_id=@domain_id, tech_prefix= @tech_prefix
                                   WHERE  user_id = @user_id  ";

            int recordsEffected1 = ExecuteNonQueryText(cmdText1, (cmd) =>
            {
                cmd.Parameters.AddWithValue("@user_id", endPoint.EndPointId);
                cmd.Parameters.AddWithValue("@account_id", endPoint.AccountId);
                cmd.Parameters.AddWithValue("@trans_rule_id", endPoint.TransRuleId);
                cmd.Parameters.AddWithValue("@state_id", 1);
                cmd.Parameters.AddWithValue("@channels_limit", 1);
                cmd.Parameters.AddWithValue("@max_call_dura", endPoint.MaxCallDuration);
                cmd.Parameters.AddWithValue("@rtp_mode", 1);
                cmd.Parameters.AddWithValue("@domain_id", endPoint.DomainId);
                cmd.Parameters.AddWithValue("@tech_prefix", endPoint.TechPrefix);


            }
           );

            if (recordsEffected1 > 0)
            {

                String cmdText = @"UPDATE access_list
	                             SET  description=@description , 
                                   log_alias=@log_alias,codec_profile_id=@codec_profile_id,trans_rule_id=@trans_rule_id,state_id=@state_id,
                                   channels_limit=@channels_limit, max_call_dura=@max_call_dura,rtp_mode=@rtp_mode,domain_id=@domain_id,
                                   host=@host,tech_prefix=@tech_prefix 
                                   WHERE  user_id = @user_id AND NOT EXISTS(SELECT 1 FROM  access_list WHERE (user_id != @user_id and
                                                                            (domain_id=@domain_id and host=@host and tech_prefix=@tech_prefix )))";
                Object test = System.Net.IPAddress.Parse(endPoint.Host);

                int recordsEffected = ExecuteNonQueryText(cmdText, (cmd) =>
                {
                    cmd.Parameters.AddWithValue("@user_id", endPoint.EndPointId);
                    cmd.Parameters.AddWithValue("@description", endPoint.Description);
                      cmd.Parameters.AddWithValue("@log_alias", endPoint.LogAlias);
                    cmd.Parameters.AddWithValue("@codec_profile_id", endPoint.CodecProfileId);
                    cmd.Parameters.AddWithValue("@trans_rule_id", endPoint.TransRuleId);
                    cmd.Parameters.AddWithValue("@state_id", currentState);
                    cmd.Parameters.AddWithValue("@channels_limit", endPoint.ChannelsLimit);
                     cmd.Parameters.AddWithValue("@max_call_dura", endPoint.MaxCallDuration);
                    cmd.Parameters.AddWithValue("@rtp_mode", 1);
                    cmd.Parameters.AddWithValue("@domain_id", endPoint.DomainId);
                    cmd.Parameters.AddWithValue("@host", System.Net.IPAddress.Parse(endPoint.Host));
                    cmd.Parameters.AddWithValue("@tech_prefix", endPoint.TechPrefix);


                }
               );



                return (recordsEffected > 0);

            }

            return false;
        }

        public bool SipInsert(EndPoint endPoint, List<int> endPointIds, out int insertedId)
        {
          
            object endPointId;
            int currentState, rtpMode;
             MapEnum(endPoint, out currentState, out rtpMode);

            // get group_id   
            int groupId = GetGroupId(endPoint,  endPointIds);
            

            String cmdText = @"INSERT INTO users(account_id,description,group_id, 
                                   log_alias,codec_profile_id,trans_rule_id,state_id, channels_limit, max_call_dura,rtp_mode,domain_id,
                                    sip_login,sip_password, tech_prefix,type_id)
	                             SELECT  @account_id, @description, @group_id,   @log_alias, @codec_profile_id, @trans_rule_id,@state_id,
                                 @channels_limit,   @max_call_dura, @rtp_mode, @domain_id,@sip_login, @sip_password , @tech_prefix,@type_id 
                                   WHERE   NOT EXISTS(SELECT 1 FROM  users WHERE (domain_id=@domain_id and sip_login=@sip_login and tech_prefix=@tech_prefix))
 	                             returning  user_id;";

            endPointId = ExecuteScalarText(cmdText, (cmd) =>
            {
                cmd.Parameters.AddWithValue("@account_id", endPoint.AccountId);
                cmd.Parameters.AddWithValue("@description", endPoint.Description);
                cmd.Parameters.AddWithValue("@group_id", groupId);
                 cmd.Parameters.AddWithValue("@log_alias", endPoint.LogAlias);
                cmd.Parameters.AddWithValue("@codec_profile_id", endPoint.CodecProfileId);
                cmd.Parameters.AddWithValue("@trans_rule_id", endPoint.TransRuleId);
                cmd.Parameters.AddWithValue("@state_id", currentState);
                cmd.Parameters.AddWithValue("@channels_limit", endPoint.ChannelsLimit);
                 cmd.Parameters.AddWithValue("@max_call_dura", endPoint.MaxCallDuration);
                cmd.Parameters.AddWithValue("@rtp_mode", rtpMode);
                cmd.Parameters.AddWithValue("@domain_id", endPoint.DomainId);
                var prmSipLogin = new Npgsql.NpgsqlParameter("@sip_login", DbType.String);
                prmSipLogin.Value = CheckIfNull(endPoint.SipLogin);
                cmd.Parameters.Add(prmSipLogin);
                var prmPassword = new Npgsql.NpgsqlParameter("@sip_password", DbType.String);
                prmPassword.Value = CheckIfNull(endPoint.SipPassword);
                cmd.Parameters.Add(prmPassword);
                cmd.Parameters.AddWithValue("@tech_prefix", ".");
                cmd.Parameters.AddWithValue("@type_id", 2);
            }
            );

            insertedId = -1;
            if (endPointId != null)
            {
                insertedId = Convert.ToInt32(endPointId);
                return true;
            }
            else
                return false;

        }

        public bool AclInsert(EndPoint  endPoint, List<int> endPointIds, out int insertedId)
        {
          

            object endPointId;
            int currentState, rtpMode;

            MapEnum(endPoint, out currentState, out rtpMode);

            // get group_id from previous endpoint
            int groupId = GetGroupId(endPoint,  endPointIds);

            // insert into users and get user id
            String cmdText1 = @"INSERT INTO users(account_id,group_id, trans_rule_id,state_id , 
                                                 channels_limit ,max_call_dura,rtp_mode,domain_id,
                                                  tech_prefix,type_id)
	                             SELECT  @account_id,  @group_id, @trans_rule_id,@state_id,
                                 @channels_limit,   @max_call_dura, @rtp_mode, @domain_id, @tech_prefix ,@type_id
 	                             returning  user_id;";

            endPointId = ExecuteScalarText(cmdText1, (cmd) =>
            {
                cmd.Parameters.AddWithValue("@account_id", endPoint.AccountId);
                cmd.Parameters.AddWithValue("@group_id", groupId);
                cmd.Parameters.AddWithValue("@trans_rule_id", endPoint.TransRuleId);
                cmd.Parameters.AddWithValue("@state_id", 1);
                cmd.Parameters.AddWithValue("@channels_limit", 1);
                cmd.Parameters.AddWithValue("@max_call_dura", endPoint.MaxCallDuration);
                cmd.Parameters.AddWithValue("@rtp_mode", 1);
                cmd.Parameters.AddWithValue("@domain_id", endPoint.DomainId);
                cmd.Parameters.AddWithValue("@tech_prefix", endPoint.TechPrefix);
                cmd.Parameters.AddWithValue("@type_id", 2);


            }
            );


            insertedId = -1;
            if (endPointId != null)
            {

                // insert into access list table using user_id
                String cmdText = @"INSERT INTO access_list(user_id,account_id,description,group_id, 
                                   log_alias,codec_profile_id,trans_rule_id,state_id, channels_limit,  max_call_dura,rtp_mode,domain_id,
                                    host,tech_prefix)
	                             SELECT  @user_id,@account_id, @description, @group_id,   @log_alias, @codec_profile_id, @trans_rule_id,@state_id,
                                 @channels_limit,   @max_call_dura, @rtp_mode, @domain_id,@host, @tech_prefix
                                 WHERE NOT EXISTS(SELECT 1 FROM  access_list WHERE (domain_id=@domain_id and host=@host and tech_prefix=@tech_prefix))";

                int recordsEffected = ExecuteNonQueryText(cmdText, (cmd) =>
               {
                   cmd.Parameters.AddWithValue("@user_id", endPointId);
                   cmd.Parameters.AddWithValue("@account_id", endPoint.AccountId);
                   cmd.Parameters.AddWithValue("@description", endPoint.Description);
                   cmd.Parameters.AddWithValue("@group_id", groupId);
                    cmd.Parameters.AddWithValue("@log_alias", endPoint.LogAlias);
                   cmd.Parameters.AddWithValue("@codec_profile_id", endPoint.CodecProfileId);
                   cmd.Parameters.AddWithValue("@trans_rule_id", endPoint.TransRuleId);
                   cmd.Parameters.AddWithValue("@state_id", currentState);
                   cmd.Parameters.AddWithValue("@channels_limit", endPoint.ChannelsLimit);
                    cmd.Parameters.AddWithValue("@max_call_dura", endPoint.MaxCallDuration);
                   cmd.Parameters.AddWithValue("@rtp_mode", 1);
                   cmd.Parameters.AddWithValue("@domain_id", endPoint.DomainId);
                   cmd.Parameters.AddWithValue("@host", System.Net.IPAddress.Parse(endPoint.Host));
                   cmd.Parameters.AddWithValue("@tech_prefix", endPoint.TechPrefix);

               }
               );

                insertedId = Convert.ToInt32(endPointId);


                return (recordsEffected > 0);
            }

            return false;
        }

        public bool Insert(EndPoint endPoint, List<int> endPointIds, out int insertedId)
        {
            if (endPoint.EndPointType == EndPointType.ACL)
                return AclInsert(endPoint, endPointIds, out insertedId);
            else
                return SipInsert(endPoint, endPointIds, out insertedId);

        }

        public bool Update(EndPoint endPoint)
        {
            if (endPoint.EndPointType == EndPointType.ACL)
                return AclUpdate(endPoint);
            else
                return SipUpdate(endPoint);

        }

        public void CheckTariffAndRouteTables(EndPoint endPoint, String carrierAccountName)
        {
            //check if route and tariff exist for this accountid
            // get existing route_id and tariff_id
            String cmdText = @"SELECT COALESCE (users.tariff_id,access_list.tariff_id) as tariff_id, COALESCE (users.route_table_id,access_list.route_table_id) as route_id
                                from users 
                                LEFT  JOIN access_list
                                on users.user_id = access_list.user_id
                                where users.account_id = @account_id
                                LIMIT 1 ";

            EndPointToUpdate tariffRouteIds = GetItemText(cmdText, EndPointToUpdateMapper, (cmd) =>
            {
                cmd.Parameters.AddWithValue("@account_id", endPoint.AccountId);
            });

            if (tariffRouteIds.RouteTableId != null || tariffRouteIds.TariffId != null)
            {
                // update new endpoint with existing route_id and table_id
                AddTariffAndRouteToEndpoint(tariffRouteIds, endPoint);
            }

            else
            {
                // create tariff and route id then update endpoint
                CreateTariffAndRouteTables(carrierAccountName, endPoint);

            }
        }

        public void CreateTariffAndRouteTables(String carrierAccountName, EndPoint  endPoint)
        {

            object tariffId;
            int insertedTariffId =-1;

            // Add tariff record to tariffs table
            String cmdText = @"INSERT INTO tariffs(tariff_name,description)
                               SELECT @tariff_name, @description
                               WHERE  NOT EXISTS(SELECT 1 FROM  tariffs WHERE (tariff_name=@tariff_name))  
  	                           returning  tariff_id;";

            tariffId = ExecuteScalarText(cmdText, (cmd) =>
            {
                cmd.Parameters.AddWithValue("@tariff_name", carrierAccountName);
                cmd.Parameters.AddWithValue("@description", carrierAccountName);
            }
            );

            if (tariffId != null)
            {
                     insertedTariffId = Convert.ToInt32(tariffId);
                    // create tariff table
                    TariffDataManager tariffDataManager = new TariffDataManager();
                    tariffDataManager.CreateTariffTable(insertedTariffId);

            }

               
                object routeId;
                int insertedRouteId=-1;

                // Add route record to route_tables
                String cmdText2 = @"INSERT INTO route_tables(route_table_name,description)
                               SELECT @route_table_name, @description
                               WHERE  NOT EXISTS(SELECT 1 FROM  route_tables WHERE (route_table_name=@route_table_name))
  	                           returning  route_table_id;";

                routeId = ExecuteScalarText(cmdText2, (cmd) =>
                {
                    cmd.Parameters.AddWithValue("@route_table_name", carrierAccountName);
                    cmd.Parameters.AddWithValue("@description", carrierAccountName);
                }
                );

                if (routeId != null)
                {
                    insertedRouteId = Convert.ToInt32(routeId);
                    // create route table
                    RouteTableDataManager routeTableDataManager = new RouteTableDataManager();
                    routeTableDataManager.CreateRouteTable(insertedRouteId);

                }

                // update endpoint (tariff_id and route_id columns)
                if (endPoint.EndPointType == EndPointType.ACL)
                {
                    //update access_list
                    String cmdText3 = "UPDATE access_list  SET  tariff_id=@tariff_id, route_table_id=@route_table_id WHERE  user_id = @user_id";
                    int recordsEffected = ExecuteNonQueryText(cmdText3, (cmd) =>
                    {
                        cmd.Parameters.AddWithValue("@tariff_id", insertedTariffId);
                        cmd.Parameters.AddWithValue("@route_table_id", insertedRouteId);
                        cmd.Parameters.AddWithValue("@user_id", endPoint.EndPointId);

                    });
                }
                else
                {
                    //update users
                    String cmdText3 = "UPDATE users  SET  tariff_id=@tariff_id, route_table_id=@route_table_id WHERE  user_id = @user_id";
                    int recordsEffected = ExecuteNonQueryText(cmdText3, (cmd) =>
                    {
                        cmd.Parameters.AddWithValue("@tariff_id", insertedTariffId);
                        cmd.Parameters.AddWithValue("@route_table_id", insertedRouteId);
                        cmd.Parameters.AddWithValue("@user_id", endPoint.EndPointId);

                    });
                }            
        }

        public bool AddTariffAndRouteToEndpoint(EndPointToUpdate tariffRouteIds, EndPoint endPoint)
        {
                        
            // update new endpoint (tariff_id and route_id columns)
            if (endPoint.EndPointType == EndPointType.ACL)
            {
                //update access_list
                String cmdText2 = "UPDATE access_list  SET  tariff_id=@tariff_id, route_table_id=@route_table_id WHERE  user_id = @user_id";
                int recordsEffected = ExecuteNonQueryText(cmdText2, (cmd) =>
                {
                    cmd.Parameters.AddWithValue("@tariff_id", tariffRouteIds.TariffId);
                    cmd.Parameters.AddWithValue("@route_table_id", tariffRouteIds.RouteTableId);
                    cmd.Parameters.AddWithValue("@user_id", endPoint.EndPointId);

                });
            }
            else
            {
                //update users
                String cmdText3 = "UPDATE users  SET  tariff_id=@tariff_id, route_table_id=@route_table_id WHERE  user_id = @user_id";
                int recordsEffected = ExecuteNonQueryText(cmdText3, (cmd) =>
                {
                    cmd.Parameters.AddWithValue("@tariff_id", tariffRouteIds.TariffId);
                    cmd.Parameters.AddWithValue("@route_table_id", tariffRouteIds.RouteTableId);
                    cmd.Parameters.AddWithValue("@user_id", endPoint.EndPointId);

                });
            }    


            return true;

         }
        private void MapEnum(EndPoint endPoint, out int currentState, out int rtpMode)
        {
            var currentStateValue = Enum.Parse(typeof(State), endPoint.CurrentState.ToString());
            currentState = (int)currentStateValue;
            var rtpModeValue = Enum.Parse(typeof(RtpMode), endPoint.RtpMode.ToString());
            rtpMode = (int)rtpModeValue;


        }

        private Object CheckIfNull(String parameter)
        {

            return (String.IsNullOrEmpty(parameter)) ? (Object)DBNull.Value : parameter;

        }

        private int GetGroupId(EndPoint endPoint,  List<int> endPointIds)
        {
            if (endPointIds.Count == 0)
            {
                int groupId = 0, nextGroupId = 0;

                String cmdText = @"Select max(group_id) as group_id
                                from users
                                where account_id = @account_id";
                int groupIdIncremented = GetItemText(cmdText, (reader) => { return GetReaderValue<int>(reader, "group_id"); }, (cmd) =>
                {
                    cmd.Parameters.AddWithValue("@account_id", endPoint.AccountId);
                });

                if (groupIdIncremented != 0)
                {
                    groupId = groupIdIncremented - endPoint.AccountId;

                    String cmdText2 = @"Select next_group_id
                                from(
                                Select  group_id as  group_id,  lead(group_id) OVER (ORDER BY group_id ) as next_group_id   
                                from user_groups)  x
                                 where  group_id = @group_id;";

                    nextGroupId = GetItemText(cmdText2, (reader) => { return GetReaderValue<int>(reader, "next_group_id"); }, (cmd) =>
                  {
                      cmd.Parameters.AddWithValue("@group_id", groupId);
                  });

                }

                else
                {
                    String cmdText2 = @"Select  group_id
                                   from user_groups 
                                   order by group_id
                                   limit 1 ";

                    nextGroupId = GetItemText(cmdText2, (reader) => { return GetReaderValue<int>(reader, "group_id"); }, (cmd) => { });
                }


                return nextGroupId + endPoint.AccountId;

            }

            else
            {
 
                String cmdText = @"Select group_id  
                                   from users
                                   where user_id = @user_id";
                int groupId  = GetItemText(cmdText, (reader) => { return GetReaderValue<int>(reader, "group_id"); }, (cmd) =>
                {
                    cmd.Parameters.AddWithValue("@user_id", endPointIds[0]);
                });

                return groupId;
            }
        }
         



    }
}