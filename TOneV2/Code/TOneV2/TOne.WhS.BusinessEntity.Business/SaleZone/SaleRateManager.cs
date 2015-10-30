using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Data;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Business
{
    public class SaleRateManager
    {
        public List<SalePriceListRate> GetRates(DateTime? effectiveOn, bool isEffectiveInFuture)
        {
            throw new NotImplementedException();
        }

        public List<SaleRate> GetSaleRatesByCustomerZoneIds(SalePriceListOwnerType ownerType, int customerId, List<long> customerZoneIds, DateTime? effectiveOn)
        {
            ISaleRateDataManager dataManager = BEDataManagerFactory.GetDataManager<ISaleRateDataManager>();
            return dataManager.GetSaleRatesByCustomerZoneIds(ownerType, customerId, customerZoneIds, effectiveOn);
        }
    }
}
