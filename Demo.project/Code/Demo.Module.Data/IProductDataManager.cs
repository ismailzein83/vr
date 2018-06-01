using Demo.Module.Entities.Product;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Module.Data
{
    public interface IProductDataManager : IDataManager
    {
        bool AreProductsUpdated(ref object updateHandle);

        List<Product> GetProducts();

        bool Insert(Product product, out int insertedId);

        bool Update(Product product);

        bool Delete(int Id);
    }
}
