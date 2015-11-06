using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Data
{
    public interface ISaleRateDataManager : IDataManager
    {
        IEnumerable<SaleRate> GetSaleRatesByCustomerZoneIds(SalePriceListOwnerType ownerType, int ownerId, IEnumerable<long> customerZoneIds, DateTime? effectiveOn);

        List<SaleRate> GetEffectiveSaleRates(SalePriceListOwnerType ownerType,int ownerId, DateTime effectiveOn);
        bool AreSaleRatesUpdated(ref object updateHandle);
    }
}
