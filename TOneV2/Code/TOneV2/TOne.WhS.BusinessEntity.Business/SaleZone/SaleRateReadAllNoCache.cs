using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Business
{
    public class SaleRateReadAllNoCache : ISaleRateReader
    {
        public SaleRateReadAllNoCache(DateTime? effectiveOn, bool isEffectiveInFuture)
        {

        }

        public SaleRatesByZone GetZoneRates(SalePriceListOwnerType ownerType, int ownerId)
        {
            throw new NotImplementedException();
        }
    }
}
