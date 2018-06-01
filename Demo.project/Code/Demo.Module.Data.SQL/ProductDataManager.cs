using Demo.Module.Entities.Product;
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
        public ProductDataManager() :
            base(GetConnectionStringName("DemoProject_DBConnStringKey", "DemoProject_DBConnStringKey"))
        {
        }

        public List<Product> GetProducts()
        {
            return GetItemsSP("[dbo].[sp_Product2_GetAll]", ProductMapper);
        }

        public bool Insert(Product product, out int insertedId)
        {
            object id;
            int nbOfRecordsAffected = ExecuteNonQuerySP("[dbo].[sp_Product2_Insert]", out id, product.Name);
            insertedId = Convert.ToInt32(id);
            return (nbOfRecordsAffected > 0);
        }

        public bool Update(Product product)
        {
            int nbOfRecordsAffected = ExecuteNonQuerySP("[dbo].[sp_Product2_Update]", product.ProductId, product.Name);
            return (nbOfRecordsAffected > 0);
        }
        
        public bool Delete(int productId)
        {
            int nbOfRecordsAffected = ExecuteNonQuerySP("[dbo].[sp_Product2_Delete]", productId);
            return (nbOfRecordsAffected > 0);
        }

        public bool AreProductsUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("[dbo].[Product]", ref updateHandle);
        }

        Product ProductMapper(IDataReader reader)
        {
            return new Product
            {
                ProductId = GetReaderValue<int>(reader, "ID"),
                Name = GetReaderValue<string>(reader, "Name")
            };
        }
    }
}
