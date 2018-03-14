using System.Linq;
using System.Collections.Generic;
using TOne.WhS.BusinessEntity.Data;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Business
{
    public class SupplierRateHistoryQueryHandler : BaseSupplierRateQueryHandler
    {
        public SupplierRateHistoryQuery Query { get; set; }

        public override IEnumerable<SupplierRate> GetFilteredSupplierRates()
        {
            ISupplierRateDataManager dataManager = BEDataManagerFactory.GetDataManager<ISupplierRateDataManager>();
            SupplierZoneManager supplierZoneManager = new SupplierZoneManager();
            var allZones = supplierZoneManager.GetCachedSupplierZones();
            var filteredZones =
                allZones.Values.Where(z => z.Name.ToLower().Equals(Query.SupplierZoneName.ToLower())
                                           && z.SupplierId == Query.SupplierId);
            if (!filteredZones.Any())
                return null;

            List<long> allZonesIds = filteredZones.Select(z => z.SupplierZoneId).ToList();
            return dataManager.GetZoneRateHistory(allZonesIds);
        }
    }
}
