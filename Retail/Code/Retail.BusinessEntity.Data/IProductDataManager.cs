using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Data
{
    public interface IProductDataManager : IDataManager
    { 
        List<Product> GetProducts();

        bool AreProductUpdated(ref object updateHandle);

        bool Insert(Product product, out int insertedId);

        bool Update(Product product);
    }
}
