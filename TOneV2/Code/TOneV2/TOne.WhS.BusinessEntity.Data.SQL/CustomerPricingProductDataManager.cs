using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Data.SQL;

namespace TOne.WhS.BusinessEntity.Data.SQL
{
    public class CustomerPricingProductDataManager : BaseSQLDataManager, ICustomerPricingProductDataManager
    {

        public CustomerPricingProductDataManager()
            : base(GetConnectionStringName("TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString"))
        {

        }



        public CustomerPricingProductDetail GetCustomerPricingProduct(int customerPricingProductId)
        {
            return GetItemSP("TOneWhS_BE.sp_CustomerPricingProduct_Get", CustomerPricingProductDetailMapper, customerPricingProductId);
        }

        public bool Insert(List<CustomerPricingProductDetail> customerPricingProduct, out List<CustomerPricingProductDetail> insertedObjects)
        {

            DataTable table = BuildUpdatedCarriersTable(customerPricingProduct);

            insertedObjects = GetItemsSPCmd("TOneWhS_BE.sp_CustomerPricingProduct_Insert", CustomerPricingProductDetailMapper, (cmd) =>
            {
                var tableParameter = new SqlParameter("@UpdatedCustomerPricingProducts", SqlDbType.Structured);
                tableParameter.Value = table;
                cmd.Parameters.Add(tableParameter);

            });

            //int recordsEffected = ExecuteNonQuerySPCmd("TOneWhS_BE.sp_CustomerPricingProduct_Insert", (cmd) =>
            //{
            //    var tableParameter = new SqlParameter("@UpdatedCustomerPricingProducts", SqlDbType.Structured);
            //    tableParameter.Value = table;
            //    cmd.Parameters.Add(tableParameter);
            //});

            return true;
        }
        private DataTable BuildUpdatedCarriersTable(List<CustomerPricingProductDetail> customerPricingProducts)
        {
            DataTable table = new DataTable();

            table.Columns.Add("ID", typeof(int));
            table.Columns.Add("CustomerId", typeof(int));
            table.Columns.Add("PricingProductId", typeof(int));
            table.Columns.Add("AllDestinations", typeof(bool));
            table.Columns.Add("BED", typeof(DateTime));
            DataColumn column;
            column = new DataColumn("EED", typeof(DateTime));
            column.AllowDBNull = true;
            table.Columns.Add(column);

            table.BeginLoadData();
            foreach (var customerPricingProduct in customerPricingProducts)
            {

                DataRow row = table.NewRow();
                row["ID"] = customerPricingProduct.CustomerPricingProductId == 0 ? 0 : customerPricingProduct.CustomerPricingProductId;
                row["CustomerId"] = customerPricingProduct.CustomerId;
                row["PricingProductId"] = customerPricingProduct.PricingProductId;
                row["AllDestinations"] = customerPricingProduct.AllDestinations;
                row["BED"] = customerPricingProduct.BED;
                if (customerPricingProduct.EED.HasValue)
                    row["EED"] = customerPricingProduct.EED.Value;

                table.Rows.Add(row);
            }

            table.EndLoadData();

            return table;
        }





        public bool Delete(int customerPricingProductId)
        {
            int recordesEffected = ExecuteNonQuerySP("TOneWhS_BE.sp_CustomerPricingProduct_Delete", customerPricingProductId,DateTime.Now);
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

        public bool AreCustomerPricingProductsUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("TOneWhS_BE.CustomerPricingProduct", ref updateHandle);
        }

        public List<CustomerPricingProductDetail> GetCustomerPricingProducts()
        {
            return GetItemsSP("TOneWhS_BE.sp_CustomerPricingProduct_GetAll", CustomerPricingProductDetailMapper);
        }

    }
}
