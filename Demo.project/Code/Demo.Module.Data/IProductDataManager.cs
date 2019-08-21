using Demo.Module.Entities;
using System.Collections.Generic;

namespace Demo.Module.Data
{
    public interface IProductDataManager : IDataManager
    {
        List<Product> GetProducts();
        bool InsertProduct(Product product, out long insertedId);
        bool UpdateProduct(Product product);
        bool AreProductsUpdated(ref object updateHandle);
    }
}
