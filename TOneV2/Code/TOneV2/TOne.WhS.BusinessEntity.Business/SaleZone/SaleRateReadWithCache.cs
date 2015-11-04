using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Business
{
    public class SaleRateReadWithCache : ISaleRateReader
    {
        public SaleRateReadWithCache(DateTime? effectiveOn, bool isEffectiveInFuture)
        {

        }

        public SaleRatesByZone GetZoneRates(Entities.SalePriceListOwnerType ownerType, int ownerId)
        {
            throw new NotImplementedException();
        }
    }
}
