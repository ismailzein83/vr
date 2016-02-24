using System.Collections.Generic;
using CP.SupplierPricelist.Entities;
using Vanrise.Data.SQL;
using System.Data;
using Vanrise.Common;

namespace CP.SupplierPricelist.Data.SQL
{
    public class CustomerDataManager : BaseSQLDataManager, ICustomerDataManager
    {
        public CustomerDataManager() :
            base(GetConnectionStringName("CP_DBConnStringKey", "CP_DBConnString"))
        {

        }

        public List<Customer> GetAllCustomers()
        {
            return GetItemsSP("[CP_SupPriceList].[sp_Customer_GetAll]", CustomerMapper);
        }
        public bool AreCustomersUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("[CP_SupPriceList].[Customer]", ref updateHandle);
        }
        Customer CustomerMapper(IDataReader reader)
        {
            Customer customer = new Customer
            {
                CustomerId = (int)reader["ID"],
                Name = reader["Name"] as string,

            };
            string settings = reader["Settings"] as string;
            if (settings != null)
                customer.Settings = Serializer.Deserialize<CustomerSettings>(settings);
            return customer;
        }
        public bool AddCustomer(Customer inputCustomer, out int customerId)
        {
            object id;
            int recordesEffected = ExecuteNonQuerySP("[CP_SupPriceList].[InsertCustomer]", out id, inputCustomer.Name, inputCustomer.Settings != null ? Serializer.Serialize(inputCustomer.Settings) : null);
            customerId = (int)id;
            return recordesEffected > 0;
        }
        public bool UpdateCustomer(Customer input)
        {
            int recordsEffected = ExecuteNonQuerySP("[CP_SupPriceList].[UpdateCustomer]",
                input.CustomerId, input.Name, input.Settings != null ? Serializer.Serialize(input.Settings) : null);
            return (recordsEffected > 0);
        }
    }
}
