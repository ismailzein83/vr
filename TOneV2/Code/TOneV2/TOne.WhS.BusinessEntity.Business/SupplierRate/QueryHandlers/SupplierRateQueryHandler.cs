using System.Collections.Generic;
using TOne.WhS.BusinessEntity.Data;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Business
{
    public class SupplierRateQueryHandler : BaseSupplierRateQueryHandler
    {
        public SupplierRateQuery Query { get; set; }

        public override IEnumerable<Entities.SupplierRate> GetFilteredSupplierRates()
        {
            ISupplierRateDataManager dataManager = BEDataManagerFactory.GetDataManager<ISupplierRateDataManager>();
            return Query.ShowPending
                ? dataManager.GetFilteredSupplierPendingRates(Query)
                : dataManager.GetFilteredSupplierRates(Query);
        }
    }
}
