using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Sales.Business;

namespace TOne.WhS.Sales.Entities
{
    public class ContextZoneItemInput
    {
        public SalePriceListOwnerType OwnerType { get; set; }

        public int OwnerId { get; set; }

        public IEnumerable<SaleZone> SaleZones { get; set; }

        public Changes Draft { get; set; }

        public Dictionary<long, ZoneChanges> ZoneDraftsByZoneId { get; set; }

        public int SellingProductId { get; set; }

        public IEnumerable<int> ChangedCountryIds { get; set; }

        public DateTime EffectiveOn { get; set; }

        #region Routing Properties

        public int RoutingDatabaseId { get; set; }

        public Guid PolicyConfigId { get; set; }

        public int NumberOfOptions { get; set; }

        public List<CostCalculationMethod> CostCalculationMethods { get; set; }

        public int CurrencyId { get; set; }

        #endregion

        #region Managers

        public ZoneRateManager RateManager { get; set; }

        public ZoneRPManager RoutingProductManager { get; set; }

        public SaleRateManager SaleRateManager { get; set; }

        #endregion
    }
}
