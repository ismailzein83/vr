using NP.IVSwitch.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
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

        public bool BlockEndPoints(IEnumerable<int> endPointIds, string tableName, int stateId)
        {
            string endPointsString = null;
            if (endPointIds != null && endPointIds.Any())
                endPointsString = string.Join(",", endPointIds);
            string blockQuery = string.Format("UPDATE {0} SET state_id = {1} WHERE user_id IN ( {2} )", tableName,
                stateId, endPointsString);

            return ExecuteNonQueryText(blockQuery, cmd => { }) > 0;
        }

        public bool UpdateEndPointState(List<EndPointStatus> endPointStatuses)
        {
            List<string> valueList = endPointStatuses.Select(
                                     endPointStatus =>
                                     string.Format("( {0}, {1})", endPointStatus.EndPointId, (int)endPointStatus.Status)).ToList();

            string endPointsString = null;
            if (valueList != null && valueList.Any())
                endPointsString = string.Join(",", valueList);
            string query = string.Format(@"update access_list as a set
                            state_id = c.state_id
                             from (values
                                     {0} 
                            ) as c(user_id, state_id) 
                        where c.user_id = a.user_id;
                        ", endPointsString);
            int recordsEffected = ExecuteNonQueryText(query, cmd =>
            {
            });
            return recordsEffected > 0;
        }
        public List<EndPointStatus> GetAccessListStatus(string whereCondition, List<int> endPointStatusIds)
        {
            string endPointStatusIdsString = null;
            if (endPointStatusIds != null && endPointStatusIds.Any())
                endPointStatusIdsString = string.Join(",", endPointStatusIds);

            string query = string.Format(" SELECT  user_id, state_id FROM access_list where state_id in({1}) {0};", whereCondition, endPointStatusIdsString);
            return GetItemsText(query, EndPointStatusMapper, cmd => { });
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
