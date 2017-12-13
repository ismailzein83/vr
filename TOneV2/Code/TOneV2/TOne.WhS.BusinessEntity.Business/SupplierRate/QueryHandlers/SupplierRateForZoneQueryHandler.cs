using System.Collections.Generic;
using TOne.WhS.BusinessEntity.Data;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Business
{
    public class SupplierRateForZoneQueryHandler : BaseSupplierRateQueryHandler
    {
        public SupplierRateForZoneQuery Query { get; set; }

        public override IEnumerable<Entities.SupplierRate> GetFilteredSupplierRates()
        {
            ISupplierRateDataManager dataManager = BEDataManagerFactory.GetDataManager<ISupplierRateDataManager>();
            return dataManager.GetSupplierRatesForZone(Query,EffectiveOn);
        }
    }
}
