using Demo.Module.Entities;
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

        bool Insert(Product product, out long insertedId);

        bool Update(Product product);

    }
}
