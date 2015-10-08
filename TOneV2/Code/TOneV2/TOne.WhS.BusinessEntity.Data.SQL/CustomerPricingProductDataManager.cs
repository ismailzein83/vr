using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Data.SQL;

namespace TOne.WhS.BusinessEntity.Data.SQL
{
    public class CustomerPricingProductDataManager:BaseSQLDataManager,ICustomerPricingProductDataManager
    {
         private static Dictionary<string, string> _columnMapper = new Dictionary<string, string>();

        static CustomerPricingProductDataManager()
        {
            _columnMapper.Add("CustomerPricingProductId", "ID");
        }

        public CustomerPricingProductDataManager()
            : base(GetConnectionStringName("TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString"))
        {

        }

        public Vanrise.Entities.BigResult<Entities.CustomerPricingProductDetail> GetFilteredCustomerPricingProducts(Vanrise.Entities.DataRetrievalInput<Entities.CustomerPricingProductQuery> input)
        {
            Action<string> createTempTableAction = (tempTableName) =>
            {
                //string saleZonePackageIdsParam = null;
                //if (input.Query.SaleZonePackageIds != null)
                //    saleZonePackageIdsParam = string.Join(",", input.Query.SaleZonePackageIds);

                ExecuteNonQuerySP("TOneWhS_BE.sp_CustomerPricingProduct_CreateTempByFiltered", tempTableName, input.Query.CustomerId,input.Query.PricingProductId,input.Query.EffectiveDate);
            };

            return RetrieveData(input, createTempTableAction, CustomerPricingProductDetailMapper, _columnMapper);
        }


        public CustomerPricingProductDetail GetCustomerPricingProduct(int customerPricingProductId)
        {
            return GetItemSP("TOneWhS_BE.sp_CustomerPricingProduct_Get", CustomerPricingProductDetailMapper, customerPricingProductId);
        }

        public bool Insert(Entities.CustomerPricingProduct customerPricingProduct, out int insertedId)
        {
            object customerPricingProductId;

            int recordsEffected = ExecuteNonQuerySP("TOneWhS_BE.sp_CustomerPricingProduct_Insert", out customerPricingProductId, customerPricingProduct.CustomerId, customerPricingProduct.PricingProductId, customerPricingProduct.AllDestinations, customerPricingProduct.BED,
              customerPricingProduct.EED);
            insertedId = (int)customerPricingProductId;
            return (recordsEffected > 0);
        }

        public bool Delete(int customerPricingProductId)
        {
            int recordesEffected = ExecuteNonQuerySP("TOneWhS_BE.sp_CustomerPricingProduct_Delete", customerPricingProductId);
            return (recordesEffected > 0);
        }
        CustomerPricingProductDetail CustomerPricingProductDetailMapper(IDataReader reader)
        {
            CustomerPricingProductDetail customerPricingProductDetail = new CustomerPricingProductDetail
            {
                CustomerPricingProductId = (int)reader["ID"],
                CustomerId = (int)reader["CustomerID"],
                PricingProductId = (int)reader["PricingProductID"],
                BED = GetReaderValue<DateTime>(reader, "BED"),
                EED = GetReaderValue<DateTime?>(reader, "EED"),
                AllDestinations = GetReaderValue<bool>(reader, "AllDestinations"),
                CustomerName = reader["CustomerName"] as string,
                PricingProductName = reader["PricingProductName"] as string,
            };

            return customerPricingProductDetail;
        }
        CustomerPricingProduct CustomerPricingProductMapper(IDataReader reader)
        {
            CustomerPricingProduct customerPricingProduct = new CustomerPricingProduct
            {
                CustomerPricingProductId = (int)reader["ID"],
                CustomerId = (int)reader["CustomerID"],
                PricingProductId = (int)reader["PricingProductID"],
                BED = GetReaderValue<DateTime>(reader, "BED"),
                EED = GetReaderValue<DateTime?>(reader, "EED"),
                AllDestinations = GetReaderValue<bool>(reader, "AllDestinations"),
            };

            return customerPricingProduct;
        }


        public List<CustomerPricingProduct> GetCustomerPricingProductByCustomerID(int customerId)
        {
            return GetItemsSP("TOneWhS_BE.sp_CustomerPricingProduct_GetByCustomer", CustomerPricingProductMapper, customerId);
        }
    }
}
