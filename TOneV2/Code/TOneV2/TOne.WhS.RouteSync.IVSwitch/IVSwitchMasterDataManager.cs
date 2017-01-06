using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
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

        private string GetSupplierQuery()
        {
            return @"select distinct route_id,account_id,group_id from routes where route_id is not null ";
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
        public List<RouteTable> GetRouteTables()
        {
            string query = GetSupplierQuery();
            return GetItemsText(query, RouteMapper, null);
        }

        #region Mapper
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
        #endregion

    }
}
