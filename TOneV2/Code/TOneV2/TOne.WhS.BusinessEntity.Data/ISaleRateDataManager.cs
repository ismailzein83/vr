using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Sales.Entities.RatePlanning;

namespace TOne.WhS.BusinessEntity.Data
{
    public interface ISaleRateDataManager : IDataManager
    {
        List<SaleRate> GetEffectiveSaleRates(SalePriceListOwnerType ownerType, int ownerId, DateTime effectiveOn);

        List<SaleRate> GetEffectiveSaleRateByCustomers(IEnumerable<RoutingCustomerInfo> customerInfos, DateTime? effectiveOn, bool isEffectiveInFuture);

        bool AreSaleRatesUpdated(ref object updateHandle);

        bool CloseRates(IEnumerable<RateChange> rateChanges);

        bool InsertRates(IEnumerable<SaleRate> newRates);
    }
}
