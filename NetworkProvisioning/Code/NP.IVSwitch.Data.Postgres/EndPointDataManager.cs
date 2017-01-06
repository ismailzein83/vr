using NP.IVSwitch.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;
using Vanrise.Data.Postgres;

namespace NP.IVSwitch.Data.Postgres
{
    public class EndPointDataManager : BasePostgresDataManager, IEndPointDataManager
    {
        public TOne.WhS.RouteSync.IVSwitch.BuiltInIVSwitchSWSync IvSwitchSync { get; set; }
        protected override string GetConnectionString()
        {
            return IvSwitchSync.MasterConnectionString;
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

        #region public functions
        private bool SipUpdate(EndPoint endPoint)
        {
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
               cmd.Parameters.AddWithValue("@state_id", (int)endPoint.CurrentState);
               cmd.Parameters.AddWithValue("@channels_limit", endPoint.ChannelsLimit);
               cmd.Parameters.AddWithValue("@max_call_dura", endPoint.MaxCallDuration);
               cmd.Parameters.AddWithValue("@rtp_mode", (int)endPoint.RtpMode);
               cmd.Parameters.AddWithValue("@domain_id", endPoint.DomainId);
               var prmSipLogin = new Npgsql.NpgsqlParameter("@sip_login", DbType.String)
               {
                   Value = CheckIfNull(endPoint.SipLogin)
               };
               cmd.Parameters.Add(prmSipLogin);
               var prmPassword = new Npgsql.NpgsqlParameter("@sip_password", DbType.String)
               {
                   Value = CheckIfNull(endPoint.SipPassword)
               };
               cmd.Parameters.Add(prmPassword);
               cmd.Parameters.AddWithValue("@tech_prefix", ".");
           }
          );
            return (recordsEffected > 0);
        }

        private bool AclUpdate(EndPoint endPoint)
        {
            String cmdText1 = @"UPDATE users
	                             SET  trans_rule_id=@trans_rule_id,state_id=@state_id,channels_limit=@channels_limit, 
                                     max_call_dura=@max_call_dura,rtp_mode=@rtp_mode,domain_id=@domain_id, tech_prefix= @tech_prefix
                                   WHERE  user_id = @user_id  ";

            int recordsEffected1 = ExecuteNonQueryText(cmdText1, cmd =>
            {
                cmd.Parameters.AddWithValue("@user_id", endPoint.EndPointId);
                cmd.Parameters.AddWithValue("@account_id", endPoint.AccountId);
                cmd.Parameters.AddWithValue("@trans_rule_id", endPoint.TransRuleId);
                cmd.Parameters.AddWithValue("@state_id", 1);
                cmd.Parameters.AddWithValue("@channels_limit", endPoint.ChannelsLimit);
                cmd.Parameters.AddWithValue("@max_call_dura", endPoint.MaxCallDuration);
                cmd.Parameters.AddWithValue("@rtp_mode", (int)endPoint.RtpMode);
                cmd.Parameters.AddWithValue("@domain_id", endPoint.DomainId);
                cmd.Parameters.AddWithValue("@tech_prefix", endPoint.TechPrefix ?? ".");
            }
           );

            if (recordsEffected1 <= 0) return false;

            String cmdText = @"UPDATE access_list
	                             SET  description=@description , 
                                   log_alias=@log_alias,codec_profile_id=@codec_profile_id,trans_rule_id=@trans_rule_id,state_id=@state_id,
                                   channels_limit=@channels_limit, max_call_dura=@max_call_dura,rtp_mode=@rtp_mode,domain_id=@domain_id,
                                   host=@host,tech_prefix=@tech_prefix 
                                   WHERE  user_id = @user_id AND NOT EXISTS(SELECT 1 FROM  access_list WHERE (user_id != @user_id and
                                    (domain_id=@domain_id and host=@host and tech_prefix=@tech_prefix )))";
            int recordsEffected = ExecuteNonQueryText(cmdText, cmd =>
            {
                cmd.Parameters.AddWithValue("@user_id", endPoint.EndPointId);
                cmd.Parameters.AddWithValue("@description", endPoint.Description);
                cmd.Parameters.AddWithValue("@log_alias", endPoint.LogAlias);
                cmd.Parameters.AddWithValue("@codec_profile_id", endPoint.CodecProfileId);
                cmd.Parameters.AddWithValue("@trans_rule_id", endPoint.TransRuleId);
                cmd.Parameters.AddWithValue("@state_id", (int)endPoint.CurrentState);
                cmd.Parameters.AddWithValue("@channels_limit", endPoint.ChannelsLimit);
                cmd.Parameters.AddWithValue("@max_call_dura", endPoint.MaxCallDuration);
                cmd.Parameters.AddWithValue("@rtp_mode", (int)endPoint.RtpMode);
                cmd.Parameters.AddWithValue("@domain_id", endPoint.DomainId);
                cmd.Parameters.AddWithValue("@host", System.Net.IPAddress.Parse(endPoint.Host));
                cmd.Parameters.AddWithValue("@tech_prefix", endPoint.TechPrefix ?? ".");
            }
                );
            return (recordsEffected > 0);
        }

        public bool SipInsert(EndPoint endPoint, List<EndPointInfo> endPointInfoList, out int insertedId, string carrierAccountName)
        {
            insertedId = -1;
            int groupId = GetGroupId(endPoint, endPointInfoList);
            AccessList accessList = PrepareDataForInsert(endPoint.AccountId, groupId, carrierAccountName);
            int? endPointId = ExecuteSipInsert(endPoint, groupId, accessList);
            if (!endPointId.HasValue) return false;
            insertedId = Convert.ToInt32(endPointId);
            return true;
        }
        public List<AccessList> GetAccessList()
        {
            return GetItemsText("select user_id,route_table_id,tariff_id from access_list", AccessListMapper, null);
        }
        public bool AclInsert(EndPoint endPoint, List<EndPointInfo> userEndPoints, List<EndPointInfo> aclEndPoints, out int insertedId, string carrierAccountName)
        {
            insertedId = -1;
            List<EndPointInfo> endpoints = userEndPoints.Concat(aclEndPoints).ToList();
            int groupId = GetGroupId(endPoint, endpoints);
            AccessList accessList = PrepareDataForInsert(endPoint.AccountId, groupId, carrierAccountName);

            int? endPointId = InserUser(endPoint, groupId, accessList);
            if (!endPointId.HasValue) return false;
            insertedId = endPointId.Value;
            return InsertAcl(endPointId.Value, endPoint, groupId, accessList);
        }
        public bool Insert(EndPoint endPoint, List<EndPointInfo> userEndPoints, List<EndPointInfo> aclEndPoints, out int insertedId, string carrierAccountName)
        {
            List<EndPointInfo> joinedEndPoint = userEndPoints.Concat(aclEndPoints).ToList();
            if (endPoint.EndPointType == UserType.ACL)
                return AclInsert(endPoint, joinedEndPoint, aclEndPoints, out insertedId, carrierAccountName);
            return SipInsert(endPoint, joinedEndPoint, out insertedId, carrierAccountName);
        }

        public bool Update(EndPoint endPoint)
        {
            if (endPoint.EndPointType == UserType.ACL)
                return AclUpdate(endPoint);
            return SipUpdate(endPoint);
        }

        #endregion

        #region private functions
        private int? ExecuteSipInsert(EndPoint endPoint, int groupId, AccessList accessList)
        {
            String cmdText = @"INSERT INTO users(account_id,description,group_id, 
                                   log_alias,codec_profile_id,trans_rule_id,state_id, channels_limit, max_call_dura,rtp_mode,domain_id,
                                    sip_login,sip_password, tech_prefix,type_id, tariff_id,route_table_id)
	                             SELECT  @account_id, @description, @group_id,   @log_alias, @codec_profile_id, @trans_rule_id,@state_id,
                                 @channels_limit,   @max_call_dura, @rtp_mode, @domain_id,@sip_login, @sip_password , @tech_prefix,@type_id,@tariff_id,@route_table_id
                                   WHERE   NOT EXISTS(SELECT 1 FROM  users WHERE (domain_id=@domain_id and sip_login=@sip_login and tech_prefix=@tech_prefix))
 	                             returning  user_id;";

            return (int?)ExecuteScalarText(cmdText, cmd =>
            {
                cmd.Parameters.AddWithValue("@account_id", endPoint.AccountId);
                cmd.Parameters.AddWithValue("@description", endPoint.Description);
                cmd.Parameters.AddWithValue("@group_id", groupId);
                cmd.Parameters.AddWithValue("@log_alias", endPoint.LogAlias);
                cmd.Parameters.AddWithValue("@codec_profile_id", endPoint.CodecProfileId);
                cmd.Parameters.AddWithValue("@trans_rule_id", endPoint.TransRuleId);
                cmd.Parameters.AddWithValue("@state_id", (int)endPoint.CurrentState);
                cmd.Parameters.AddWithValue("@channels_limit", endPoint.ChannelsLimit);
                cmd.Parameters.AddWithValue("@max_call_dura", endPoint.MaxCallDuration);
                cmd.Parameters.AddWithValue("@rtp_mode", (int)endPoint.RtpMode);
                cmd.Parameters.AddWithValue("@domain_id", endPoint.DomainId);
                var prmSipLogin = new NpgsqlParameter("@sip_login", DbType.String)
                {
                    Value = CheckIfNull(endPoint.SipLogin)
                };
                cmd.Parameters.Add(prmSipLogin);
                var prmPassword = new NpgsqlParameter("@sip_password", DbType.String)
                {
                    Value = CheckIfNull(endPoint.SipPassword)
                };
                cmd.Parameters.Add(prmPassword);
                cmd.Parameters.AddWithValue("@tech_prefix", ".");
                cmd.Parameters.AddWithValue("@type_id", (int)endPoint.EndPointType);
                cmd.Parameters.AddWithValue("@route_table_id", accessList.RouteTableId);
                cmd.Parameters.AddWithValue("@tariff_id", accessList.TariffId);
            }
                );
        }
        private int? InserUser(EndPoint endPoint, int groupId, AccessList accessList)
        {
            String cmdText1 = @"INSERT INTO users(account_id,group_id, trans_rule_id,state_id , 
                                                 channels_limit ,max_call_dura,rtp_mode,domain_id,
                                                  tech_prefix,type_id, tariff_id,route_table_id)
	                             SELECT  @account_id,  @group_id, @trans_rule_id,@state_id,
                                 @channels_limit,   @max_call_dura, @rtp_mode, @domain_id, @tech_prefix ,@type_id,@tariff_id,@route_table_id
 	                             returning  user_id;";

            return (int?)ExecuteScalarText(cmdText1, cmd =>
            {
                cmd.Parameters.AddWithValue("@account_id", endPoint.AccountId);
                cmd.Parameters.AddWithValue("@group_id", groupId);
                cmd.Parameters.AddWithValue("@trans_rule_id", endPoint.TransRuleId);
                cmd.Parameters.AddWithValue("@state_id", (int)endPoint.CurrentState);
                cmd.Parameters.AddWithValue("@channels_limit", endPoint.ChannelsLimit);
                cmd.Parameters.AddWithValue("@max_call_dura", endPoint.MaxCallDuration);
                cmd.Parameters.AddWithValue("@rtp_mode", (int)endPoint.RtpMode);
                cmd.Parameters.AddWithValue("@domain_id", endPoint.DomainId);
                cmd.Parameters.AddWithValue("@tech_prefix", endPoint.TechPrefix ?? ".");
                cmd.Parameters.AddWithValue("@type_id", (int)endPoint.EndPointType);
                cmd.Parameters.AddWithValue("@route_table_id", accessList.RouteTableId);
                cmd.Parameters.AddWithValue("@tariff_id", accessList.TariffId);
            }
                );
        }
        private bool InsertAcl(int endPointId, EndPoint endPoint, int groupId, AccessList accessList)
        {
            string queries =
                string.Format(@"
                                INSERT INTO access_list(
	                            host, domain_id, tech_prefix, user_id, account_id, description
                                , trans_rule_id, state_id, channels_limit, log_alias
                                , tariff_id, route_table_id
                                , codec_profile_id, group_id, max_call_dura, rtp_mode)
	                            VALUES ('{0}', {1}, '{2}', {3}, {4}, '{5}',{6}, {7}, {8}, '{9}', {10}, {11}, {12}, {13}, {14}, {15});"
                    , endPoint.Host, (int)endPoint.DomainId, endPoint.TechPrefix ?? ".", endPointId, endPoint.AccountId,
                    endPoint.Description, endPoint.TransRuleId
                    , (int)endPoint.CurrentState, endPoint.ChannelsLimit, endPoint.LogAlias, accessList.TariffId,
                    accessList.RouteTableId, endPoint.CodecProfileId, groupId, endPoint.MaxCallDuration
                    , (int)endPoint.RtpMode);
            int recordAffected = ExecuteNonQueryText(queries, null);
            return recordAffected > 0;
        }
        private AccessList PrepareDataForInsert(int accountId, int groupId, string carrierAccountName)
        {
            AccessList accessList = CheckAccessListExistense(accountId, groupId);
            return accessList ?? CreateTariffAndRouteTables(carrierAccountName);
        }
        private AccessList CheckAccessListExistense(int accountId, int groupId)
        {
            string query = @"select route_table_id,tariff_id,user_id from users
                             where account_id =  @account_id and group_id = @group_id";
            List<AccessList> accessLists = GetItemsText(query, AccessListMapper, cmd =>
             {
                 cmd.Parameters.AddWithValue("@account_id", accountId);
                 cmd.Parameters.AddWithValue("@group_id", groupId);
             });
            return accessLists.Count > 0 ? accessLists.First() : null;
        }
        private AccessList CreateTariffAndRouteTables(String carrierAccountName)
        {
            String cmdText = @"INSERT INTO tariffs(tariff_name,description)
                               SELECT @tariff_name, @description 
  	                           returning  tariff_id;";

            var tariffId = ExecuteScalarText(cmdText, cmd =>
            {
                cmd.Parameters.AddWithValue("@tariff_name", carrierAccountName);
                cmd.Parameters.AddWithValue("@description", carrierAccountName);
            }
                );

            if (tariffId == null) return null;

            var insertedTariffId = Convert.ToInt32(tariffId);
            TariffDataManager tariffDataManager = new TariffDataManager(IvSwitchSync.TariffConnectionString, IvSwitchSync.OwnerName);
            tariffDataManager.CreateTariffTable(insertedTariffId);


            String cmdText2 = @"INSERT INTO route_tables(route_table_name,description)
                               SELECT @route_table_name, @description
  	                           returning  route_table_id;";

            var routeId = ExecuteScalarText(cmdText2, cmd =>
            {
                cmd.Parameters.AddWithValue("@route_table_name", carrierAccountName);
                cmd.Parameters.AddWithValue("@description", carrierAccountName);
            }
                );

            if (routeId == null) return null;

            var insertedRouteId = Convert.ToInt32(routeId);
            RouteTableDataManager routeTableDataManager = new RouteTableDataManager(IvSwitchSync.RouteConnectionString, IvSwitchSync.OwnerName);
            routeTableDataManager.CreateRouteTable(insertedRouteId);

            return new AccessList
            {
                RouteTableId = insertedRouteId,
                TariffId = insertedTariffId
            };
        }
        private Object CheckIfNull(String parameter)
        {
            return (String.IsNullOrEmpty(parameter)) ? (Object)DBNull.Value : parameter;
        }
        private int GetGroupId(EndPoint endPoint, List<EndPointInfo> endPointInfoList)
        {
            if (endPointInfoList.Count == 0)
            {
                int groupId, nextGroupId;
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
                    if (nextGroupId == 0)
                    {
                        String cmdText3 = @"INSERT INTO user_groups(description)
	                                        VALUES('Dummy Group')
                                            returning group_id;";
                        nextGroupId = (int)ExecuteScalarText(cmdText3, cmd => { });
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
                return nextGroupId + endPoint.AccountId;
            }
            else
            {
                String cmdText = @"Select group_id  
                                   from users
                                   where user_id = @user_id";
                int groupId = GetItemText(cmdText, (reader) => { return GetReaderValue<int>(reader, "group_id"); }, (cmd) =>
                {
                    cmd.Parameters.AddWithValue("@user_id", endPointInfoList[0].EndPointId);
                });
                return groupId;
            }
        }

        #endregion

        #region mappers
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

            endPoint.EndPointType = endPoint.Host == null ? UserType.SIP : UserType.ACL;

            return endPoint;
        }
        private EndPointToUpdate EndPointToUpdateMapper(IDataReader reader)
        {
            EndPointToUpdate endPointToUpdate = new EndPointToUpdate();

            endPointToUpdate.TariffId = GetReaderValue<int?>(reader, "tariff_id");
            endPointToUpdate.RouteTableId = GetReaderValue<int?>(reader, "route_id");

            return endPointToUpdate;
        }
        private AccessList AccessListMapper(IDataReader reader)
        {
            AccessList endPoint = new AccessList();
            int id;
            if (reader["route_table_id"] != DBNull.Value)
            {
                int.TryParse(reader["route_table_id"].ToString(), out id);
                endPoint.RouteTableId = id;
            }
            if (reader["tariff_id"] != DBNull.Value)
            {
                int.TryParse(reader["tariff_id"].ToString(), out id);
                endPoint.TariffId = id;
            }
            if (reader["user_id"] != DBNull.Value)
            {
                int.TryParse(reader["user_id"].ToString(), out id);
                endPoint.UserId = id;
            }
            return endPoint;
        }

        #endregion
    }
}