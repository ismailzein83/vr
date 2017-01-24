using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Data
{
    public interface IProductFamilyDataManager : IDataManager
    {
        List<ProductFamily> GetProductFamilies();

        bool AreProductFamilyUpdated(ref object updateHandle);

        bool Insert(ProductFamily productFamily, out int insertedId);

        bool Update(ProductFamily productFamily);
    }
}
