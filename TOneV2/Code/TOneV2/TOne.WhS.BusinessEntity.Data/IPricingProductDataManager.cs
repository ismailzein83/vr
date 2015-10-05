using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Data
{
    public interface IPricingProductDataManager:IDataManager
    {
        Vanrise.Entities.BigResult<PricingProduct> GetFilteredPricingProducts(Vanrise.Entities.DataRetrievalInput<PricingProductQuery> input);

        PricingProduct GetPricingProduct(int pricingProductId);
        bool Insert(PricingProduct pricingProduct, out int insertedId);

        bool Update(PricingProduct pricingProduct);

        bool Delete(int pricingProductId);
    }
}
