using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.SQL;

namespace Retail.BusinessEntity.Data.SQL
{
    class ProductDataManager : BaseSQLDataManager, IProductDataManager
    {
        #region ctor/Local Variables
        public ProductDataManager()
            : base(GetConnectionStringName("Retail_BE_DBConnStringKey", "RetailDBConnString"))
        {

        }
         
        #endregion

        #region Public Methods
        public List<Product> GetProducts()
        {
            return GetItemsSP("Retail_BE.sp_Product_GetAll", ProductMapper);
        }

        public bool AreProductUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("Retail_BE.Product", ref updateHandle);
        }

        public bool Insert(Product productItem, out int insertedId)
        {
            object productID;
            string serializedSettings = productItem.Settings != null ? Vanrise.Common.Serializer.Serialize(productItem.Settings) : null;

            int affectedRecords = ExecuteNonQuerySP("[Retail_BE].[sp_Product_Insert]", out productID, productItem.Name, serializedSettings);

            insertedId = (affectedRecords > 0) ? (int)productID : -1;
            return (affectedRecords > 0);
        }

        public bool Update(Product productItem)
        {
            string serializedSettings = productItem.Settings != null ? Vanrise.Common.Serializer.Serialize(productItem.Settings) : null;
            int affectedRecords = ExecuteNonQuerySP("Retail_BE.sp_Product_Update", productItem.ProductId, productItem.Name, serializedSettings);
            return (affectedRecords > 0);
        }

        #endregion


        #region Mappers

        Product ProductMapper(IDataReader reader)
        {
            Product product = new Product
            {
                ProductId = (int)reader["ID"],
                Name = reader["Name"] as string,
                Settings = reader["Settings"] as string != null ? Vanrise.Common.Serializer.Deserialize<ProductSettings>(reader["Settings"] as string) : null,
            };
            return product;
        }

        #endregion
    }
}
