using Demo.Module.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.SQL;

namespace Demo.Module.Data.SQL
{
    public class ProductDataManager : BaseSQLDataManager, IProductDataManager
    {

        
        #region Constructors
        public ProductDataManager() :
            base(GetConnectionStringName("DemoProject_DBConnStringKey", "DemoProject_DBConnStringKey"))
        {
        }
        #endregion

        #region Public Methods
        public bool AreProductsUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("[dbo].[Product]", ref updateHandle);
        }
        public List<Entities.Product> GetProducts()
        {
            return GetItemsSP("[dbo].[sp_Product2_GetAll]", ProductMapper);
        }
        public bool Insert(Product product, out long insertedId)
        {
            object id;
            int nbOfRecordsAffected = ExecuteNonQuerySP("[dbo].[sp_Product2_Insert]", out id, product.Name);
            bool result = (nbOfRecordsAffected > 0);
            if (result)
                insertedId = (long)id;
            else
                insertedId = 0;
            return result;
        }
        public bool Update(Product product)
        {

            int nbOfRecordsAffected = ExecuteNonQuerySP("[dbo].[sp_Product2_Update]", product.ProductId, product.Name);
            return (nbOfRecordsAffected > 0);
        }

        #endregion

        #region Mappers
        Product ProductMapper(IDataReader reader)
        {
            return new Product
            {
                ProductId = GetReaderValue<long>(reader, "ID"),
                Name = GetReaderValue<string>(reader, "Name")
            };
        }
        #endregion
    }
}
