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
    
        #region ctor/Local Variables
        public CustomerSellingProductDataManager()
            : base(GetConnectionStringName("TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString"))
        {

        }
        #endregion

        #region Public Methods
        public CustomerSellingProduct GetCustomerSellingProduct(int customerSellingProductId)
        {
            return GetItemSP("TOneWhS_BE.sp_CustomerSellingProduct_Get", CustomerSellingProductDetailMapper, customerSellingProductId);
        }
        public bool Insert(List<CustomerSellingProduct> customerSellingProduct, out List<CustomerSellingProduct> insertedObjects)
        {
            DataTable table = BuildUpdatedCarriersTable(customerSellingProduct);

            insertedObjects = GetItemsSPCmd("TOneWhS_BE.sp_CustomerSellingProduct_Insert", CustomerSellingProductDetailMapper, (cmd) =>
            {
                var tableParameter = new SqlParameter("@UpdatedCustomerSellingProducts", SqlDbType.Structured);
                tableParameter.Value = table;
                cmd.Parameters.Add(tableParameter);

            });
            return true;
        }
        public bool AreCustomerSellingProductsUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("TOneWhS_BE.CustomerSellingProduct", ref updateHandle);
        }
        public bool Update(CustomerSellingProduct customerSellingProduct)
        {
            int recordsEffected = ExecuteNonQuerySP("TOneWhS_BE.sp_CustomerSellingProduct_Update", customerSellingProduct.CustomerSellingProductId, customerSellingProduct.SellingProductId, customerSellingProduct.BED);
            return (recordsEffected > 0);
        }
        public List<CustomerSellingProduct> GetCustomerSellingProducts()
        {
            return GetItemsSP("TOneWhS_BE.sp_CustomerSellingProduct_GetAll", CustomerSellingProductDetailMapper);
        }
        #endregion

        #region Private Methods
        private DataTable BuildUpdatedCarriersTable(List<CustomerSellingProduct> customerSellingProducts)
        {
            DataTable table = new DataTable();

            table.Columns.Add("ID", typeof(int));
            table.Columns.Add("CustomerId", typeof(int));
            table.Columns.Add("SellingProductId", typeof(int));
            table.Columns.Add("BED", typeof(DateTime));

            table.BeginLoadData();
            foreach (var customerSellingProduct in customerSellingProducts)
            {

                DataRow row = table.NewRow();
                row["ID"] = customerSellingProduct.CustomerSellingProductId == 0 ? 0 : customerSellingProduct.CustomerSellingProductId;
                row["CustomerId"] = customerSellingProduct.CustomerId;
                row["SellingProductId"] = customerSellingProduct.SellingProductId;
                row["BED"] = customerSellingProduct.BED;
                table.Rows.Add(row);
            }

            table.EndLoadData();

            return table;
        }

        #endregion

        #region  Mappers
        CustomerSellingProduct CustomerSellingProductDetailMapper(IDataReader reader)
        {
            CustomerSellingProduct customerSellingProductDetail = new CustomerSellingProduct
            {
                CustomerSellingProductId = (int)reader["ID"],
                CustomerId = (int)reader["CustomerID"],
                SellingProductId = (int)reader["SellingProductID"],
                BED = GetReaderValue<DateTime>(reader, "BED"),
            };

            return customerSellingProductDetail;
        }

        #endregion

    }
}
