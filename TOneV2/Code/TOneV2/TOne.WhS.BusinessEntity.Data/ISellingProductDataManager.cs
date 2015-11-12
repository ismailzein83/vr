using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Data
{
    public interface ISellingProductDataManager:IDataManager
    {
        List<SellingProduct> GetSellingProducts();
        bool Insert(SellingProduct sellingProduct, out int insertedId);

        bool Update(SellingProduct sellingProduct);

        bool Delete(int sellingProductId);
        bool AreSellingProductsUpdated(ref object updateHandle);
    }
}
