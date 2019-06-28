using NP.IVSwitch.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Transactions;
using Vanrise.Data.Postgres;

namespace TOne.WhS.RouteSync.IVSwitch
{
	public class IVSwitchMasterDataManager : BasePostgresDataManager
	{
		private string _connectionString;

		public IVSwitchMasterDataManager(string connectionString)
		{
			_connectionString = connectionString;
		}

		protected override string GetConnectionString()
		{
			return _connectionString;
		}

		private string GetCustomerQuery()
		{
			return
				@"SELECT distinct u.type_id,u2.group_id,u2.account_id,u.table_name,al.route_table_id,al.tariff_id,al.user_id from user_types u
                        JOIN ( SELECT type_id,group_id,account_id from users) u2 on u2.type_id=u.type_id
                        join access_list al on al.account_id = u2.account_id and al.group_id = u2.group_id
                        where route_table_id is not null and tariff_id is not null
                ";
		}
		private string GetCustomerQueryManual()
		{
			return
				@" SELECT distinct u.type_id,u2.group_id,u2.account_id,u.table_name,al.route_table_id,al.tariff_id from user_types u
                        JOIN ( SELECT type_id,group_id,account_id from users) u2 on u2.type_id=u.type_id
                        join access_list al on al.account_id = u2.account_id and al.group_id = u2.group_id
                ";
		}

		private string GetSupplierQuery(List<int> stateIds)
		{
			string stateCondition = string.Empty;
			if (stateIds != null && stateIds.Any())
			{
				string stateIdSring = string.Join(",", stateIds);
				stateCondition = string.Format("and state_id in ({0})", stateIdSring);
			}
			return string.Format(@"select distinct route_id,account_id,group_id from routes where route_id is not null {0}", stateCondition);
		}

		public DateTime GetSwitchDate()
		{
			string query = "select current_timestamp;";
			return (DateTime)ExecuteScalarText(query, null);
		}
		public List<AccessListTable> GetAccessListTables()
		{
			string query = GetCustomerQuery();
			return GetItemsText(query, AccessListMapper, null);
		}
		public List<AccessListTable> GetAccessListTablesManual()
		{
			string query = GetCustomerQueryManual();
			return GetItemsText(query, AccessListMapperManual, null);
		}
		public List<RouteTable> GetRouteTables(List<int> stateIds)
		{
			string query = GetSupplierQuery(stateIds);
			return GetItemsText(query, RouteMapper, null);
		}

		public bool BlockEndPoints(IEnumerable<int> endPointIds, int stateId)
		{
			List<int> aclEndPointIds;
			List<int> sipEndPointIds;
			SeparateEndPointsTypes(endPointIds, out aclEndPointIds, out sipEndPointIds);

			string aclEndPointsString = null;
			if (aclEndPointIds != null && aclEndPointIds.Any())
				aclEndPointsString = string.Join(",", aclEndPointIds);

			string sipEndPointsString = null;
			if (sipEndPointIds != null && sipEndPointIds.Any())
				sipEndPointsString = string.Join(",", sipEndPointIds);

			var transactionOptions = new TransactionOptions
			{
				IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted,
				Timeout = TransactionManager.DefaultTimeout
			};
			using (var transactionScope = new TransactionScope(TransactionScopeOption.Required, transactionOptions))
			{
				if (aclEndPointsString != null && aclEndPointIds.Any())
				{
					string cmdBlockACL = string.Format("update access_list set state_id={0} where user_id IN ({1})", stateId, aclEndPointsString);
					string cmdBlockUser = string.Format("update users set state_id={0} where user_id IN ({1})", stateId, aclEndPointsString);
					ExecuteNonQueryText(cmdBlockACL, cmd => { });
					ExecuteNonQueryText(cmdBlockUser, cmd => { });
				}
				if (sipEndPointIds != null && sipEndPointIds.Any())
				{
					string cmdBlockSIP = string.Format("update users set state_id={0} where user_id IN ({1})", stateId, sipEndPointsString);
					ExecuteNonQueryText(cmdBlockSIP, cmd => { });
				}
				transactionScope.Complete();
			}
			return true;
		}
		public bool BlockRoutes(List<int> routeIds, int stateId)
		{
			string routesString = null;
			if (routeIds != null && routeIds.Any())
				routesString = string.Join(",", routeIds);
			string cmdUpdateRoutesStates = string.Format("update routes set state_id={0} where route_id in ({1})", stateId, routesString);
			int affectedRows = ExecuteNonQueryText(cmdUpdateRoutesStates, cmd => { });
			return affectedRows > 0;
		}
		public bool UpdateRoutesStates(List<RouteStatus> routeStatuses)
		{
			string routesString = null;

			List<string> routeValueList = new List<string>();

			if (routeStatuses != null)
			{
				var selectedRouteValue = routeStatuses.Select(routeStatus => string.Format("( {0}, {1})", routeStatus.RouteId, (int)routeStatus.Status));
				if (selectedRouteValue != null)
					routeValueList = selectedRouteValue.ToList();
			}

			if (routeValueList != null && routeValueList.Any())
				routesString = string.Join(",", routeValueList);

			string routeQuery = string.Format(@"update routes as a set
                            state_id = c.state_id
                             from (values
                                     {0} 
                            ) as c(route_id, state_id) 
                        where c.route_id = a.route_id;
                        ", routesString);
			if (routesString != null)
			{
				var affectedRows = ExecuteNonQueryText(routeQuery, cmd => { });
				return affectedRows > 0;
			}
			return true;
		}
		private void SeparateEndPointsTypes(IEnumerable<int> endPointIds, out List<int> aclEndPointIds, out List<int> sipEndPointIds)
		{
			IEndPointManager endPointManager = NPManagerFactory.GetManager<IEndPointManager>();
			aclEndPointIds = new List<int>();
			sipEndPointIds = new List<int>();
			if (endPointIds != null)
			{
				foreach (var endPointId in endPointIds)
				{
					NP.IVSwitch.Entities.EndPoint endPoint = endPointManager.GetEndPoint(endPointId);
					if (endPoint.EndPointType == NP.IVSwitch.Entities.UserType.ACL)
						aclEndPointIds.Add(endPointId);

					if (endPoint.EndPointType == NP.IVSwitch.Entities.UserType.SIP)
						sipEndPointIds.Add(endPointId);
				}
			}
		}

		public bool UpdateActivationStates(List<int> routeIds, List<int> endPointIds, int stateId)
		{
			List<int> aclEndPointIds;
			List<int> sipEndPointIds;
			SeparateEndPointsTypes(endPointIds, out aclEndPointIds, out sipEndPointIds);

			string aclEndPointsString = null;
			if (aclEndPointIds != null && aclEndPointIds.Any())
				aclEndPointsString = string.Join(",", aclEndPointIds);

			string sipEndPointsString = null;
			if (sipEndPointIds != null && sipEndPointIds.Any())
				sipEndPointsString = string.Join(",", sipEndPointIds);

			string routesString = null;
			if (routeIds != null && routeIds.Any())
				routesString = string.Join(",", routeIds);

			var transactionOptions = new TransactionOptions
			{
				IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted,
				Timeout = TransactionManager.DefaultTimeout
			};
			using (var transactionScope = new TransactionScope(TransactionScopeOption.Required, transactionOptions))
			{
				if (aclEndPointsString != null)
				{
					string cmdACLChangeState = string.Format("update access_list set state_id={0} where user_id IN ({1})", stateId, aclEndPointsString);
					string cmdUserChangeState = string.Format("update users set state_id={0} where user_id IN ({1})", stateId, aclEndPointsString);
					ExecuteNonQueryText(cmdACLChangeState, cmd => { });
					ExecuteNonQueryText(cmdUserChangeState, cmd => { });
				}
				if (sipEndPointsString != null)
				{
					string cmdSIPChangeState = string.Format("update users set state_id={0} where user_id IN ({1})", stateId, sipEndPointsString);
					ExecuteNonQueryText(cmdSIPChangeState, cmd => { });
				}
				if (routesString != null)
				{
					string cmdRouteChangeState = string.Format("update routes set state_id={0} where route_id IN ({1})", stateId, routesString);
					ExecuteNonQueryText(cmdRouteChangeState, cmd => { });
				}
				transactionScope.Complete();
			}

			return true;
		}
		public bool ReActivateCarrier(List<EndPointStatus> endPointStatuses, List<RouteStatus> routeStatuses)
		{
			string endPointsString = null;
			string routesString = null;
			List<string> endPointValueList = new List<string>();
			List<string> routeValueList = new List<string>();
			if (endPointStatuses != null)
			{
				var seletedEndPointValue = endPointStatuses.Select(endPointStatus => string.Format("( {0}, {1})", endPointStatus.EndPointId, (int)endPointStatus.Status));
				if (seletedEndPointValue != null)
					endPointValueList = seletedEndPointValue.ToList();
			}

			if (routeStatuses != null)
			{
				var selectedRouteValue = routeStatuses.Select(routeStatus => string.Format("( {0}, {1})", routeStatus.RouteId, (int)routeStatus.Status));
				if (selectedRouteValue != null)
					routeValueList = selectedRouteValue.ToList();
			}
			if (endPointValueList != null && endPointValueList.Any())
				endPointsString = string.Join(",", endPointValueList);

			if (routeValueList != null && routeValueList.Any())
				routesString = string.Join(",", routeValueList);

			var transactionOptions = new TransactionOptions
			{
				IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted,
				Timeout = TransactionManager.DefaultTimeout
			};
			using (var transactionScope = new TransactionScope(TransactionScopeOption.Required, transactionOptions))
			{
				string aclQuery = string.Format(@"update access_list as a set
                            state_id = c.state_id
                             from (values
                                     {0} 
                            ) as c(user_id, state_id) 
                        where c.user_id = a.user_id;
                        ", endPointsString);
				string sipQuery = string.Format(@"update users as a set
                            state_id = c.state_id
                             from (values
                                     {0} 
                            ) as c(user_id, state_id) 
                        where c.user_id = a.user_id;
                        ", endPointsString);

				string routeQuery = string.Format(@"update routes as a set
                            state_id = c.state_id
                             from (values
                                     {0} 
                            ) as c(route_id, state_id) 
                        where c.route_id = a.route_id;
                        ", routesString);
				if (endPointsString != null)
				{
					ExecuteNonQueryText(aclQuery, cmd => { });
					ExecuteNonQueryText(sipQuery, cmd => { });
				}
				if (routesString != null)
				{
					ExecuteNonQueryText(routeQuery, cmd => { });
				}
				transactionScope.Complete();
			}
			return true;
		}

		public bool UpdateEndPointState(List<EndPointStatus> endPointStatuses)
		{
			List<string> valueList = endPointStatuses.Select(
									 endPointStatus =>
									 string.Format("( {0}, {1})", endPointStatus.EndPointId, (int)endPointStatus.Status)).ToList();

			string endPointsString = null;
			if (valueList != null && valueList.Any())
				endPointsString = string.Join(",", valueList);

			var transactionOptions = new TransactionOptions
			{
				IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted,
				Timeout = TransactionManager.DefaultTimeout
			};
			using (var transactionScope = new TransactionScope(TransactionScopeOption.Required, transactionOptions))
			{
				string aclQuery = string.Format(@"update access_list as a set
                            state_id = c.state_id
                             from (values
                                     {0} 
                            ) as c(user_id, state_id) 
                        where c.user_id = a.user_id;
                        ", endPointsString);
				string sipQuery = string.Format(@"update users as a set
                            state_id = c.state_id
                             from (values
                                     {0} 
                            ) as c(user_id, state_id) 
                        where c.user_id = a.user_id;
                        ", endPointsString);
				if (endPointsString != null)
				{
					ExecuteNonQueryText(aclQuery, cmd => { });
					ExecuteNonQueryText(sipQuery, cmd => { });
				}
				transactionScope.Complete();
			}
			return true;
		}
		public List<EndPointStatus> GetAccessListStatus(string whereCondition, List<int> endPointStatusIds)
		{
			string endPointStatusIdsString = null;
			if (endPointStatusIds != null && endPointStatusIds.Any())
				endPointStatusIdsString = string.Join(",", endPointStatusIds);

			string query = string.Format(" SELECT  user_id, state_id FROM access_list where state_id in({1}) {0};", whereCondition, endPointStatusIdsString);
			return GetItemsText(query, EndPointStatusMapper, cmd => { });
		}
		public List<EndPointStatus> GetEndPointsStatus(string whereCondition, List<int> endPointStatusIds)
		{
			string endPointStatusIdsString = null;
			if (endPointStatusIds != null && endPointStatusIds.Any())
				endPointStatusIdsString = string.Join(",", endPointStatusIds);

			string query = string.Format(" SELECT  user_id, state_id FROM users where state_id in({1}) {0};", whereCondition, endPointStatusIdsString);
			return GetItemsText(query, EndPointStatusMapper, cmd => { });
		}

		public List<RouteStatus> GetRouteStatus(string whereCondition, List<int> routeStatusIds)
		{
			string routeStatusIdsString = null;
			if (routeStatusIds != null && routeStatusIds.Any())
				routeStatusIdsString = string.Join(",", routeStatusIds);

			string query = string.Format(" SELECT  route_id, state_id FROM routes where state_id in({1}) {0};", whereCondition, routeStatusIdsString);
			return GetItemsText(query, RouteStatusMapper, cmd => { });
		}
		#region Mapper
		private EndPointStatus EndPointStatusMapper(IDataReader reader)
		{
			return new EndPointStatus
			{
				EndPointId = (int)reader["user_id"],
				Status = (State)GetReaderValue<Int16>(reader, "state_id")
			};
		}
		private RouteStatus RouteStatusMapper(IDataReader reader)
		{
			return new RouteStatus
			{
				RouteId = (int)reader["route_id"],
				Status = (State)GetReaderValue<Int16>(reader, "state_id")
			};
		}
		RouteTable RouteMapper(IDataReader reader)
		{
			RouteTable supplierDefinition = new RouteTable
			{
				AccountId = reader["account_id"] != System.DBNull.Value ? reader["account_id"].ToString() : "",
				GroupId = reader["group_id"] != System.DBNull.Value ? reader["group_id"].ToString() : ""
			};
			if (reader["route_id"] != System.DBNull.Value)
			{
				int id;
				int.TryParse(reader["route_id"].ToString(), out id);
				supplierDefinition.RouteId = id;
			}
			return supplierDefinition;
		}
		AccessListTable AccessListMapper(IDataReader reader)
		{
			AccessListTable endPoint = new AccessListTable
			{
				AccountId = reader["account_id"] != System.DBNull.Value ? reader["account_id"].ToString() : "",
				GroupId = reader["group_id"] != System.DBNull.Value ? reader["group_id"].ToString() : ""
			};
			int id;
			if (reader["route_table_id"] != System.DBNull.Value)
			{
				int.TryParse(reader["route_table_id"].ToString(), out id);
				endPoint.RouteTableId = id;
			}
			if (reader["tariff_id"] != System.DBNull.Value)
			{
				int.TryParse(reader["tariff_id"].ToString(), out id);
				endPoint.TariffTableId = id;
			}
			if (reader["user_id"] != System.DBNull.Value)
			{
				int.TryParse(reader["user_id"].ToString(), out id);
				endPoint.UserId = id;
			}
			return endPoint;
		}
		AccessListTable AccessListMapperManual(IDataReader reader)
		{
			AccessListTable endPoint = new AccessListTable
			{
				AccountId = reader["account_id"] != System.DBNull.Value ? reader["account_id"].ToString() : "",
				GroupId = reader["group_id"] != System.DBNull.Value ? reader["group_id"].ToString() : ""
			};
			int id;
			if (reader["route_table_id"] != System.DBNull.Value)
			{
				int.TryParse(reader["route_table_id"].ToString(), out id);
				endPoint.RouteTableId = id;
			}
			if (reader["tariff_id"] != System.DBNull.Value)
			{
				int.TryParse(reader["tariff_id"].ToString(), out id);
				endPoint.TariffTableId = id;
			}
			return endPoint;
		}
		#endregion

	}
}
