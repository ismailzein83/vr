using System.Collections.Generic;
using CP.SupplierPricelist.Entities;
using Vanrise.Data.SQL;
using System.Data;
using Vanrise.Common;

namespace CP.SupplierPricelist.Data.SQL
{
    public class CustomerSupplierMappingDataManage : BaseSQLDataManager, ICustomerSupplierMappingDataManager
    {
        public CustomerSupplierMappingDataManage() :
            base(GetConnectionStringName("CP_DBConnStringKey", "CP_DBConnString"))
        {

        }

        public bool Insert(CustomerSupplierMapping customerSupplierMapping)
        {

            int recordsEffected = ExecuteNonQuerySP("[CP_SupPriceList].[sp_CustomerSupplierMapping_Insert]",
                customerSupplierMapping.UserId,
                customerSupplierMapping.CustomerId,
                customerSupplierMapping.Settings !=null ? Serializer.Serialize( customerSupplierMapping.Settings ) :null  
                );
            return (recordsEffected > 0);
        }
        public List<CustomerSupplierMapping> GetAllCustomerSupplierMappings()
        {
            return GetItemsSP("[CP_SupPriceList].[sp_CustomerSupplierMapping_GetAll]", CustomerSupplierMapper);
        }
        public bool AreCustomerSupplierMappingsUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("[CP_SupPriceList].[CustomerSupplierMapping]", ref updateHandle);
        }
        CustomerSupplierMapping CustomerSupplierMapper(IDataReader reader)
        {
            CustomerSupplierMapping customerSupplier = new CustomerSupplierMapping
            {
                UserId = (int)reader["UserID"],
                CustomerId = (int)reader["CustomerID"]
                
            };
            string settings = reader["MappingSettings"] as string;
            if (settings != null)
                customerSupplier.Settings = Serializer.Deserialize<CustomerSupplierMappingSettings>(settings);
            return customerSupplier;
        }
    }
}
