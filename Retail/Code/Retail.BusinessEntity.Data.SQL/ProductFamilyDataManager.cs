using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.SQL;
using Vanrise.Common;
using System.Data;

namespace Retail.BusinessEntity.Data.SQL
{
    public class ProductFamilyDataManager : BaseSQLDataManager, IProductFamilyDataManager
    {
        #region ctor/Local Variables
        public ProductFamilyDataManager()
            : base(GetConnectionStringName("Retail_BE_DBConnStringKey", "RetailDBConnString"))
        {

        }

        #endregion

        #region Public Methods

        public List<ProductFamily> GetProductFamilies()
        {
            return GetItemsSP("[Retail_BE].[sp_ProductFamily_GetAll]", ProductFamilyMapper);
        }

        public bool AreProductFamilyUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("[Retail_BE].[ProductFamily]", ref updateHandle);
        }

        public bool Insert(ProductFamily productFamily, out int insertedId)
        {
            object productFamilyID;
            string serializedSettings = productFamily.Settings != null ? Vanrise.Common.Serializer.Serialize(productFamily.Settings) : null;

            int affectedRecords = ExecuteNonQuerySP("[Retail_BE].[sp_ProductFamily_Insert]", out productFamilyID, productFamily.Name, serializedSettings);

            insertedId = (affectedRecords > 0) ? (int)productFamilyID : -1;
            return (affectedRecords > 0);
        }

        public bool Update(ProductFamily productFamily)
        {
            string serializedSettings = productFamily.Settings != null ? Vanrise.Common.Serializer.Serialize(productFamily.Settings) : null;
            int affectedRecords = ExecuteNonQuerySP("[Retail_BE].[sp_ProductFamily_Update]", productFamily.ProductFamilyId, productFamily.Name, serializedSettings);
            return (affectedRecords > 0);
        }

        #endregion

        #region Mappers

        ProductFamily ProductFamilyMapper(IDataReader reader)
        {
            ProductFamily productFamily = new ProductFamily
            {
                ProductFamilyId = (int)reader["ID"],
                Name = reader["Name"] as string,
                Settings = reader["Settings"] as string != null ? Vanrise.Common.Serializer.Deserialize<ProductFamilySettings>(reader["Settings"] as string) : null,
            };
            return productFamily;
        }

        #endregion
    }
}
