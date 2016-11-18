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
                @"SELECT distinct u.type_id,u2.group_id,u2.account_id,u.table_name,al.route_table_id,al.tariff_id from user_types u
                        JOIN ( SELECT type_id,group_id,account_id from users) u2 on u2.type_id=u.type_id
                        join access_list al on al.account_id = u2.account_id and al.group_id = u2.group_id";
        }

        private string GetSupplierQuery()
        {
            return @"select distinct route_id,account_id,group_id from routes";
        }

        private Dictionary<string, CustomerDefinition> ToCustomerDictionary(List<CustomerDefinition> customersLst)
        {
            Dictionary<string, CustomerDefinition> result = new Dictionary<string, CustomerDefinition>();
            foreach (var customer in customersLst)
            {
                string key = GenerateKey(customer);
                if (!result.ContainsKey(key))
                    result[key] = customer;
            }
            return result;
        }
        public Dictionary<string, CustomerDefinition> GetCustomers()
        {
            string query = GetCustomerQuery();
            List<CustomerDefinition> customers = GetItemsText(query, CustomerDefinitionMapper, null);
            return ToCustomerDictionary(customers);
        }
        public Dictionary<string, CarrierDefinition> GetSuppliers()
        {
            string query = GetSupplierQuery();
            List<CarrierDefinition> supplierList = GetItemsText(query, SupplierDefinitionMapper, null);
            Dictionary<string, CarrierDefinition> supplierDictionary = new Dictionary<string, CarrierDefinition>();
            foreach (var customer in supplierList)
            {
                string key = GenerateKey(customer);
                if (!supplierDictionary.ContainsKey(key))
                    supplierDictionary[key] = customer;
            }
            return supplierDictionary;
        }

        private string GenerateKey(CarrierDefinition customerDefinition)
        {
            return string.Format("{0}_{1}", customerDefinition.AccountId, customerDefinition.GroupId);
        }

        #region CustomerMapper
        CarrierDefinition SupplierDefinitionMapper(IDataReader reader)
        {
            CarrierDefinition mapperCustomerDefinition = new CarrierDefinition
            {
                AccountId = reader["account_id"] != System.DBNull.Value ? reader["account_id"].ToString() : "",
                GroupId = reader["group_id"] != System.DBNull.Value ? reader["group_id"].ToString() : ""
            };
            if (reader["route_id"] != System.DBNull.Value)
            {
                int id;
                int.TryParse(reader["route_id"].ToString(), out id);
                mapperCustomerDefinition.RouteTableId = id;
            }
            return mapperCustomerDefinition;
        }
        CustomerDefinition CustomerDefinitionMapper(IDataReader reader)
        {
            CustomerDefinition mapperCustomerDefinition = new CustomerDefinition
            {
                AccountId = reader["account_id"] != System.DBNull.Value ? reader["account_id"].ToString() : "",
                GroupId = reader["group_id"] != System.DBNull.Value ? reader["group_id"].ToString() : ""
            };
            if (reader["route_table_id"] != System.DBNull.Value)
            {
                int id;
                int.TryParse(reader["route_table_id"].ToString(), out id);
                mapperCustomerDefinition.RouteTableId = id;
            }
            if (reader["tariff_id"] != System.DBNull.Value)
            {
                int id;
                int.TryParse(reader["tariff_id"].ToString(), out id);
                mapperCustomerDefinition.TariffTableId = id;
            }
            return mapperCustomerDefinition;
        }
        #endregion

    }
}
