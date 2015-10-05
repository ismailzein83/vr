using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.SQL;

namespace TOne.WhS.BusinessEntity.Data.SQL
{
    public class PricingProductDataManager : BaseSQLDataManager, IPricingProductDataManager
    {
        public Vanrise.Entities.BigResult<Entities.PricingProduct> GetFilteredPricingProducts(Vanrise.Entities.DataRetrievalInput<Entities.PricingProductQuery> input)
        {
            throw new NotImplementedException();
        }

        public Entities.PricingProduct GetPricingProduct(int pricingProductId)
        {
            throw new NotImplementedException();
        }

        public bool Insert(Entities.PricingProduct pricingProduct, out int insertedId)
        {
            throw new NotImplementedException();
        }

        public bool Update(Entities.PricingProduct pricingProduct)
        {
            throw new NotImplementedException();
        }

        public bool Delete(int pricingProductId)
        {
            throw new NotImplementedException();
        }
    }
}
