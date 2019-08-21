using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data;
using Vanrise.Data.SQL;
using Demo.Module.Entities;
using System.Data;

namespace Demo.Module.Data.SQL
{
    public class ProductDataManager : BaseSQLDataManager, IProductDataManager
    {
        public ProductDataManager() : base(GetConnectionStringName("DemoProject_DBConnStringKey", "DemoProject_DBConnStringKey"))
        {
        }

        #region Public Methods

        public List<Product> GetProducts()
        {
            return GetItemsSP("[dbo].[sp_Product_GetAll]", ProductMapper);
        }

        public bool InsertProduct(Product product, out long insertedId)
        {
            string serializedSettings = product.Settings != null ? Vanrise.Common.Serializer.Serialize(product.Settings) : null;

            object id;
            int numberOfAffectedRows = ExecuteNonQuerySP("[dbo].[sp_Product_Insert]", out id, product.ManufactoryId, product.Name, serializedSettings);

            bool result = numberOfAffectedRows > 0;
            if (result)
                insertedId = Convert.ToInt64(id);
            else
                insertedId = -1;
            return result;
        }

        public bool UpdateProduct(Product product)
        {
            string serializedSettings = product.Settings != null ? Vanrise.Common.Serializer.Serialize(product.Settings) : null;

            int numberOfAffectedRows = ExecuteNonQuerySP("[dbo].[sp_Product_Update]", product.Id, product.ManufactoryId, product.Name, serializedSettings);

            return numberOfAffectedRows > 0;
        }

        public bool AreProductsUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("[dbo].[Product]", ref updateHandle);
        }

        #endregion

        #region Private Methods

        private Product ProductMapper(IDataReader dataReader)
        {
            return new Product()
            {
                Id = GetReaderValue<long>(dataReader, "Id"),
                ManufactoryId = GetReaderValue<int>(dataReader, "ManufactoryId"),
                Name = GetReaderValue<string>(dataReader, "Name"),
                Settings = Vanrise.Common.Serializer.Deserialize<ProductSettings>(dataReader["Settings"] as string)
            };
        }

        #endregion
    }
}
