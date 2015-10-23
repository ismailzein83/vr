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
    public class CustomerSellingProductDataManager : BaseSQLDataManager, ICustomerSellingProductDataManager
    {

        public CustomerSellingProductDataManager()
            : base(GetConnectionStringName("TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString"))
        {

        }



        public CustomerSellingProductDetail GetCustomerSellingProduct(int customerSellingProductId)
        {
            return GetItemSP("TOneWhS_BE.sp_CustomerSellingProduct_Get", CustomerSellingProductDetailMapper, customerSellingProductId);
        }

        public bool Insert(List<CustomerSellingProductDetail> customerSellingProduct, out List<CustomerSellingProductDetail> insertedObjects)
        {

            DataTable table = BuildUpdatedCarriersTable(customerSellingProduct);

            insertedObjects = GetItemsSPCmd("TOneWhS_BE.sp_CustomerSellingProduct_Insert", CustomerSellingProductDetailMapper, (cmd) =>
            {
                var tableParameter = new SqlParameter("@UpdatedCustomerSellingProducts", SqlDbType.Structured);
                tableParameter.Value = table;
                cmd.Parameters.Add(tableParameter);

            });

            //int recordsEffected = ExecuteNonQuerySPCmd("TOneWhS_BE.sp_CustomerSellingProduct_Insert", (cmd) =>
            //{
            //    var tableParameter = new SqlParameter("@UpdatedCustomerSellingProducts", SqlDbType.Structured);
            //    tableParameter.Value = table;
            //    cmd.Parameters.Add(tableParameter);
            //});

            return true;
        }
        private DataTable BuildUpdatedCarriersTable(List<CustomerSellingProductDetail> customerSellingProducts)
        {
            DataTable table = new DataTable();

            table.Columns.Add("ID", typeof(int));
            table.Columns.Add("CustomerId", typeof(int));
            table.Columns.Add("SellingProductId", typeof(int));
            table.Columns.Add("AllDestinations", typeof(bool));
            table.Columns.Add("BED", typeof(DateTime));
            DataColumn column;
            column = new DataColumn("EED", typeof(DateTime));
            column.AllowDBNull = true;
            table.Columns.Add(column);

            table.BeginLoadData();
            foreach (var customerSellingProduct in customerSellingProducts)
            {

                DataRow row = table.NewRow();
                row["ID"] = customerSellingProduct.CustomerSellingProductId == 0 ? 0 : customerSellingProduct.CustomerSellingProductId;
                row["CustomerId"] = customerSellingProduct.CustomerId;
                row["SellingProductId"] = customerSellingProduct.SellingProductId;
                row["AllDestinations"] = customerSellingProduct.AllDestinations;
                row["BED"] = customerSellingProduct.BED;
                if (customerSellingProduct.EED.HasValue)
                    row["EED"] = customerSellingProduct.EED.Value;

                table.Rows.Add(row);
            }

            table.EndLoadData();

            return table;
        }





        public bool Delete(int customerSellingProductId)
        {
            int recordesEffected = ExecuteNonQuerySP("TOneWhS_BE.sp_CustomerSellingProduct_Delete", customerSellingProductId,DateTime.Now);
            return (recordesEffected > 0);
        }
        CustomerSellingProductDetail CustomerSellingProductDetailMapper(IDataReader reader)
        {
            CustomerSellingProductDetail customerSellingProductDetail = new CustomerSellingProductDetail
            {
                CustomerSellingProductId = (int)reader["ID"],
                CustomerId = (int)reader["CustomerID"],
                SellingProductId = (int)reader["SellingProductID"],
                BED = GetReaderValue<DateTime>(reader, "BED"),
                EED = GetReaderValue<DateTime?>(reader, "EED"),
                AllDestinations = GetReaderValue<bool>(reader, "AllDestinations"),
                CustomerName = reader["CustomerName"] as string,
                SellingProductName = reader["SellingProductName"] as string,
            };

            return customerSellingProductDetail;
        }

        public bool AreCustomerSellingProductsUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("TOneWhS_BE.CustomerSellingProduct", ref updateHandle);
        }

        public List<CustomerSellingProductDetail> GetCustomerSellingProducts()
        {
            return GetItemsSP("TOneWhS_BE.sp_CustomerSellingProduct_GetAll", CustomerSellingProductDetailMapper);
        }

    }
}
