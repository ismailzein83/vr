using NP.IVSwitch.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlTypes;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Npgsql;
using Vanrise.Data.Postgres;
using NP.IVSwitch.Entities.RouteTable;
using System.Transactions;

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
								SELECT                     
								case when users.type_id = @SIPType then users.user_id else access_list.user_id end,
								case when users.type_id = @SIPType then users.account_id else access_list.account_id end,
								case when users.type_id = @SIPType then users.description else access_list.description end,
								case when users.type_id = @SIPType then users.group_id else access_list.group_id end,
								case when users.type_id = @SIPType then users.tariff_id else access_list.tariff_id end,
								case when users.type_id = @SIPType then users.route_table_id else access_list.route_table_id end,
								case when users.type_id = @SIPType then users.log_alias else access_list.log_alias end,
								case when users.type_id = @SIPType then users.codec_profile_id else access_list.codec_profile_id end,
								case when users.type_id = @SIPType then users.cli_routing else access_list.cli_routing end,
								case when users.type_id = @SIPType then users.dst_routing else access_list.dst_routing end,
								case when users.type_id = @SIPType then users.trans_rule_id else access_list.trans_rule_id end,
								case when users.type_id = @SIPType then users.state_id else access_list.state_id end,
								case when users.type_id = @SIPType then users.channels_limit else access_list.channels_limit end,
								case when users.type_id = @SIPType then users.max_call_dura else access_list.max_call_dura end,
								case when users.type_id = @SIPType then users.rtp_mode else access_list.rtp_mode end,
								case when users.type_id = @SIPType then users.domain_id else access_list.domain_id end,
								access_list.host,
								case when access_list.tech_prefix is null then users.tech_prefix else access_list.tech_prefix end,
								users.sip_login,
								users.sip_password
								FROM users left join 
								access_list on access_list.user_id = users.user_id where users.type_id <> @RoutType";
			return GetItemsText(cmdText, EndPointMapper, (cmd) =>
			{
				cmd.Parameters.AddWithValue("@SIPType", (int)UserType.SIP);
				cmd.Parameters.AddWithValue("@RoutType", (int)UserType.VendroTermRoute);
			});
		}
		public int GetTableId(int userId)
		{
			string query = string.Format("select route_table_id FROM access_list where user_id={0}", userId);
			return (int)ExecuteScalarText(query, null);
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
			   int transRuleId = endPoint.RouteTableBasedRule ? -2 : 0;
			   cmd.Parameters.AddWithValue("@trans_rule_id", endPoint.TransRuleId.HasValue ? endPoint.TransRuleId.Value : transRuleId);
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
		public bool EndPointAclUpdate(IEnumerable<int> endPointIds, int value, RouteTableViewType routeTableViewType, UserType userType)
		{
			var transactionOptions = new TransactionOptions
			{
				IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted,
				Timeout = TransactionManager.DefaultTimeout
			};
			using (var transactionScope = new TransactionScope(TransactionScopeOption.Required, transactionOptions))
			{
				string sipQuery = "";
				string aclQuery = "";
				string userIds = "";
				userIds = "(" + string.Join(",", endPointIds) + ")";

				switch (routeTableViewType)
				{
					case RouteTableViewType.ANumber:
						if (userType == UserType.ACL)
							aclQuery = string.Format("UPDATE access_list SET  cli_routing=@value WHERE user_id in {0}", userIds);
						sipQuery = string.Format("UPDATE users SET  cli_routing=@value WHERE user_id in {0}", userIds);
						break;
					case RouteTableViewType.Whitelist:
						if (userType == UserType.ACL)
							aclQuery = string.Format("UPDATE access_list SET  dst_routing=@value WHERE user_id in {0}", userIds);
						sipQuery = string.Format("UPDATE users SET  dst_routing=@value WHERE user_id in {0}", userIds);
						break;
					case RouteTableViewType.BNumber:
						if (userType == UserType.ACL)
							aclQuery = string.Format("UPDATE access_list SET  route_table_id=@nullValue WHERE user_id in {0}", userIds);
						sipQuery = string.Format("UPDATE users SET  route_table_id=@nullValue WHERE user_id in {0}", userIds);
						break;
				}
				;


				ExecuteNonQueryText(sipQuery, cmd =>
				{
					cmd.Parameters.AddWithValue("@value", value);
					cmd.Parameters.AddWithValue("@nullValue", 0);


				});
				if (!aclQuery.Equals(""))
					ExecuteNonQueryText(aclQuery, cmd =>
				   {
					   cmd.Parameters.AddWithValue("@value", value);
					   cmd.Parameters.AddWithValue("@nullValue", 0);


				   });
				transactionScope.Complete();

			}
			return true;

		}
		private bool AclUpdate(EndPoint endPoint)
		{
			String cmdText1 = @"UPDATE users
	                             SET  trans_rule_id=@trans_rule_id,state_id=@state_id,channels_limit=@channels_limit, 
                                     max_call_dura=@max_call_dura,rtp_mode=@rtp_mode,domain_id=@domain_id, tech_prefix= @tech_prefix
                                   WHERE  user_id = @user_id  ";

			int transRuleId = endPoint.RouteTableBasedRule ? -2 : 0;
			int recordsEffected1 = ExecuteNonQueryText(cmdText1, cmd =>
			{
				cmd.Parameters.AddWithValue("@user_id", endPoint.EndPointId);
				cmd.Parameters.AddWithValue("@account_id", endPoint.AccountId);
				cmd.Parameters.AddWithValue("@trans_rule_id", endPoint.TransRuleId.HasValue ? endPoint.TransRuleId.Value : transRuleId);
				cmd.Parameters.AddWithValue("@state_id", 1);
				cmd.Parameters.AddWithValue("@channels_limit", endPoint.ChannelsLimit);
				cmd.Parameters.AddWithValue("@max_call_dura", endPoint.MaxCallDuration);
				cmd.Parameters.AddWithValue("@rtp_mode", (int)endPoint.RtpMode);
				cmd.Parameters.AddWithValue("@domain_id", endPoint.DomainId);
				cmd.Parameters.AddWithValue("@tech_prefix", endPoint.TechPrefix ?? ".");
			}
		   );

			if (recordsEffected1 <= 0) return false;

			string cmdText = string.Format(@"UPDATE access_list
	                             SET  description='{0}' , 
                                   log_alias='{1}',codec_profile_id={2},trans_rule_id={3},state_id={4},
                                   channels_limit={5}, max_call_dura={6},rtp_mode={7},domain_id={8},
                                   host='{9}',tech_prefix='{10}' 
                                   WHERE  user_id = {11} AND NOT EXISTS(SELECT 1 FROM  access_list WHERE (user_id != {11} and
                                    (domain_id={8} and host='{9}' and tech_prefix='{10}' )))"
				, endPoint.Description
				, endPoint.LogAlias
				, endPoint.CodecProfileId
				, endPoint.TransRuleId.HasValue ? endPoint.TransRuleId.Value : transRuleId
				, (int)endPoint.CurrentState
				, endPoint.ChannelsLimit
				, endPoint.MaxCallDuration
				, (int)endPoint.RtpMode
				, endPoint.DomainId
				, endPoint.Host
				, endPoint.TechPrefix ?? "."
				, endPoint.EndPointId);
			int recordsEffected = ExecuteNonQueryText(cmdText, null);
			return (recordsEffected > 0);
		}
		public bool SipInsert(EndPoint endPoint, int globalTariffTableId, List<EndPointInfo> endPointInfoList, out int insertedId, string carrierAccountName)
		{
			insertedId = -1;
			int groupId = GetGroupId(endPoint, endPointInfoList);
			AccessList accessList = GetOrCreateAccessList(endPoint.AccountId, globalTariffTableId, groupId, carrierAccountName);
			int? endPointId = ExecuteSipInsert(endPoint, groupId, accessList);

			if (!endPointId.HasValue)
				return false;

			insertedId = Convert.ToInt32(endPointId);
			return true;
		}
		public List<AccessList> GetAccessList()
		{
			return GetItemsText("select user_id,route_table_id,tariff_id from access_list", AccessListMapper, null);
		}
		public bool AclInsert(EndPoint endPoint, int globalTariffTableId, List<EndPointInfo> userEndPoints, List<EndPointInfo> aclEndPoints, out int insertedId, string carrierAccountName)
		{
			insertedId = -1;
			List<EndPointInfo> endpoints = userEndPoints.Concat(aclEndPoints).ToList();
			int groupId = GetGroupId(endPoint, endpoints);
			AccessList accessList = GetOrCreateAccessList(endPoint.AccountId, globalTariffTableId, groupId, carrierAccountName);

			int? endPointId = InserUser(endPoint, groupId, accessList);

			if (!endPointId.HasValue)
				return false;
			insertedId = endPointId.Value;
			return InsertAcl(endPointId.Value, endPoint, groupId, accessList);
		}
		public bool Insert(EndPoint endPoint, int globalTariffTableId, List<EndPointInfo> userEndPoints, List<EndPointInfo> aclEndPoints, out int insertedId, string carrierAccountName)
		{
			List<EndPointInfo> joinedEndPoint = userEndPoints.Concat(aclEndPoints).ToList();
			if (endPoint.EndPointType == UserType.ACL)
				return AclInsert(endPoint, globalTariffTableId, joinedEndPoint, aclEndPoints, out insertedId, carrierAccountName);
			return SipInsert(endPoint, globalTariffTableId, joinedEndPoint, out insertedId, carrierAccountName);
		}
		public bool Update(EndPoint endPoint)
		{
			if (endPoint.EndPointType == UserType.ACL)
				return AclUpdate(endPoint);
			return SipUpdate(endPoint);
		}
		public bool RouteTableEndPointUpdate(RouteTableInput routeTableInput, int routeTableId)
		{
			string query="";
			string endPointIds = "(";
			for (int index = 0; index < routeTableInput.EndPoints.Count; index++)
			{
				if (index < routeTableInput.EndPoints.Count - 1)
					endPointIds += routeTableInput.EndPoints[index].EndPointId + ",";
				else
					endPointIds += routeTableInput.EndPoints[index].EndPointId + ")";
			}
			switch (routeTableInput.RouteTableViewType)
			{
				case RouteTableViewType.ANumber:
					query = string.Format(@"UPDATE access_list SET  cli_routing=@route_table_id WHERE user_id in {0} ", endPointIds);
					break;
				case RouteTableViewType.Whitelist:
					query = string.Format(@"UPDATE access_list SET  dst_routing=@route_table_id WHERE user_id in {0} ", endPointIds);
					break;
				case RouteTableViewType.BNumber:
					query = string.Format(@"UPDATE access_list SET  cli_routing=0, dst_routing=0,route_table_id=@route_table_id WHERE user_id in {0} ", endPointIds);
					break;
			}

			int recordsEffected = ExecuteNonQueryText(query, cmd =>
			{
				cmd.Parameters.AddWithValue("@route_table_id", routeTableId);

			}

		   );
			return recordsEffected > 0;
		}
		public bool DeleteEndPoint(EndPoint endPoint)
		{

			var transactionOptions = new TransactionOptions
			{
				IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted,
				Timeout = TransactionManager.DefaultTimeout
			};

			using (var transactionScope = new TransactionScope(TransactionScopeOption.Required, transactionOptions))
			{
				String cmdDeleteACL = string.Format("delete from access_list where user_id={0}", endPoint.EndPointId);
				String cmdDeleteSIP = string.Format("delete from users where user_id={0}", endPoint.EndPointId);

				switch (endPoint.EndPointType)
				{
					case UserType.ACL:
						ExecuteNonQueryText(cmdDeleteACL, cmd => { });
						ExecuteNonQueryText(cmdDeleteSIP, cmd => { });
						break;
					case UserType.SIP:
						ExecuteNonQueryText(cmdDeleteSIP, cmd => { });
						break;
				}
				transactionScope.Complete();
			}
			return true;
		}

		public bool DeleteAccount(EndPoint endPoint)
		{
			var transactionOptions = new TransactionOptions
			{
				IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted,
				Timeout = TransactionManager.DefaultTimeout
			};

			using (var transactionScope = new TransactionScope(TransactionScopeOption.Required, transactionOptions))
			{
				CDRDataManager cDRDataManager = new CDRDataManager();
				cDRDataManager.IvSwitchSync = this.IvSwitchSync;
				cDRDataManager.DeleteHelperAccount(endPoint.AccountId);
				String cmdDeleteAccount = string.Format("delete from accounts where account_id={0}", endPoint.AccountId);
				ExecuteNonQueryText(cmdDeleteAccount, cmd => { });
				transactionScope.Complete();
			}
			return true;
		}
		public bool SetRouteTableAsDeleted(int endPointId)
		{
			Guid guid = Guid.NewGuid();
			String cmdUpdateRouteTableName = string.Format("update route_tables set route_table_name=CONCAT(route_table_name,'_deleted{0}') where route_table_id={1} ", guid,endPointId);
			int effectedRows = ExecuteNonQueryText(cmdUpdateRouteTableName, cmd => { });
			return effectedRows > 0;
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
				int transRuleId = endPoint.RouteTableBasedRule ? -2 : 0;
				cmd.Parameters.AddWithValue("@trans_rule_id", endPoint.TransRuleId.HasValue ? endPoint.TransRuleId.Value : transRuleId);
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
				int transRuleId = endPoint.RouteTableBasedRule ? -2 : 0;
				cmd.Parameters.AddWithValue("@trans_rule_id", endPoint.TransRuleId.HasValue ? endPoint.TransRuleId.Value : transRuleId);
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
			int transRuleId = endPoint.RouteTableBasedRule ? -2 : 0;
			string queries =
				string.Format(@"
                                INSERT INTO access_list(
	                            host, domain_id, tech_prefix, user_id, account_id, description
                                , trans_rule_id, state_id, channels_limit, log_alias
                                , tariff_id, route_table_id
                                , codec_profile_id, group_id, max_call_dura, rtp_mode)
	                            VALUES ('{0}', {1}, '{2}', {3}, {4}, '{5}',{6}, {7}, {8}, '{9}', {10}, {11}, {12}, {13}, {14}, {15});"
					, endPoint.Host, (int)endPoint.DomainId, endPoint.TechPrefix ?? ".", endPointId, endPoint.AccountId,
					endPoint.Description, endPoint.TransRuleId.HasValue ? endPoint.TransRuleId.Value : transRuleId
					, (int)endPoint.CurrentState, endPoint.ChannelsLimit, endPoint.LogAlias, accessList.TariffId,
					accessList.RouteTableId, endPoint.CodecProfileId, groupId, endPoint.MaxCallDuration
					, (int)endPoint.RtpMode);
			int recordAffected = ExecuteNonQueryText(queries, null);
			return recordAffected > 0;
		}
		private AccessList GetOrCreateAccessList(int accountId, int globalTariffTableId, int groupId, string carrierAccountName)
		{
			AccessList accessList = CheckAccessListExistense(accountId, groupId);
			return accessList ?? CreateRouteTable(carrierAccountName, globalTariffTableId);
		}
		private AccessList CheckAccessListExistense(int accountId, int groupId)
		{
			string query = @"select route_table_id,tariff_id,user_id from users
                             where account_id =  @account_id and group_id = @group_id";
			AccessList accessList = GetItemText(query, AccessListMapper, cmd =>
			 {
				 cmd.Parameters.AddWithValue("@account_id", accountId);
				 cmd.Parameters.AddWithValue("@group_id", groupId);
			 });
			return accessList;
		}
		private AccessList CreateRouteTable(String carrierAccountName, int globalTariffTableId)
		{
			String cmdText = @"INSERT INTO route_tables(route_table_name,description)
                               SELECT @route_table_name, @description
  	                           returning  route_table_id;";

			var routeId = ExecuteScalarText(cmdText, cmd =>
			{
				cmd.Parameters.AddWithValue("@route_table_name", carrierAccountName);
				cmd.Parameters.AddWithValue("@description", carrierAccountName);
			}
				);

			if (routeId == null) return null;

			var insertedRouteId = Convert.ToInt32(routeId);
			RouteTableRouteDataManager routeTableDataManager = new RouteTableRouteDataManager { IvSwitchSync = IvSwitchSync };
			routeTableDataManager.CreateRouteTable(insertedRouteId);

			return new AccessList
			{
				RouteTableId = insertedRouteId,
				TariffId = globalTariffTableId
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
			int hostOrdinal = reader.GetOrdinal("host");
			EndPoint endPoint = new EndPoint
			{

				EndPointId = (int)reader["user_id"],
				CliRouting = (Int16)reader["cli_routing"],
				DstRouting = (int)reader["dst_routing"],
				RouteTableId = GetReaderValue<int?>(reader, "route_table_id"),
				AccountId = (int)reader["account_id"],
				Description = reader["description"] as string,
				LogAlias = reader["log_alias"] as string,
				CodecProfileId = (int)reader["codec_profile_id"],
				TransRuleId = (int)reader["trans_rule_id"],
				CurrentState = (State)GetReaderValue<Int16>(reader, "state_id"),
				ChannelsLimit = GetReaderValue<int>(reader, "channels_limit"),
				MaxCallDuration = (int)reader["max_call_dura"],
				RtpMode = (RtpMode)(int)reader["rtp_mode"],
				DomainId = (Int16)reader["domain_id"],
				SipLogin = reader["sip_login"] as string,
				SipPassword = reader["sip_password"] as string,
				TechPrefix = reader["tech_prefix"] as string,
				RouteTableBasedRule = (int)reader["trans_rule_id"]==-2
			};
			;
			NpgsqlDataReader npgsqlreader = (NpgsqlDataReader)reader;
			string hostObj = npgsqlreader.GetProviderSpecificValue(hostOrdinal).ToString();
			endPoint.Host = hostObj;
			endPoint.EndPointType = string.IsNullOrEmpty(endPoint.Host) ? UserType.SIP : UserType.ACL;
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