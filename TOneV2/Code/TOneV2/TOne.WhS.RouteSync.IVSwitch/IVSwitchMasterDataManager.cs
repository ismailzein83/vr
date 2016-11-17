using System.Collections.Generic;
using System.Data;
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

        public Dictionary<string, CarrierDefinition> GetSupplierDefinition()
        {
            return GetDefinitions(GetSupplierQuery());
        }
        public Dictionary<string, CarrierDefinition> GetCustomerDefinition()
        {
            return GetDefinitions(GetCustomerQuery());
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
        private Dictionary<string, CarrierDefinition> GetDefinitions(string query)
        {
            Dictionary<string, CarrierDefinition> carrier = new Dictionary<string, CarrierDefinition>();
            using (Npgsql.NpgsqlConnection conn = new Npgsql.NpgsqlConnection(_connectionString))
            {
                using (Npgsql.NpgsqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandTimeout = 0;
                    cmd.CommandText = query;
                    if (conn.State == ConnectionState.Closed) conn.Open();
                    Npgsql.NpgsqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        CarrierDefinition carrierDefinition = new CarrierDefinition();
                        if (!dr.IsDBNull(0))
                        {
                            int id;
                            int.TryParse(dr[0].ToString(), out id);
                            carrierDefinition.RouteTableId = id;
                        }
                        if (!dr.IsDBNull(1)) carrierDefinition.AccountId = dr[1].ToString();
                        if (!dr.IsDBNull(2)) carrierDefinition.GroupId = dr[2].ToString();
                        string key = string.Format("{0}_{1}", carrierDefinition.AccountId, carrierDefinition.GroupId);
                        if (!carrier.ContainsKey(key)) carrier[key] = carrierDefinition;
                    }
                }
            }
            return carrier;
        }
    }
}
