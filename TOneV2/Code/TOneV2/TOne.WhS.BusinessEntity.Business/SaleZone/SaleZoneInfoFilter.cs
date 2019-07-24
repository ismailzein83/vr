using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Sales.Entities;

namespace TOne.WhS.BusinessEntity.Business
{
    public class SaleZoneInfoFilter
    {
        public IEnumerable<ISaleZoneFilter> Filters { get; set; }

        public SaleZoneFilterSettings SaleZoneFilterSettings { get; set; }

        public Vanrise.Entities.EntityFilterEffectiveMode EffectiveMode { get; set; }

        public IEnumerable<int> CountryIds { get; set; }

        public IEnumerable<long> AvailableZoneIds { get; set; }

        public IEnumerable<long> ExcludedZoneIds { get; set; }
        public DateTime? EffectiveDate { get; set; }
        public bool ExcludePendingClosedZones { get; set; }
    }

    public interface ISaleZoneFilter
    {
        bool IsExcluded(ISaleZoneFilterContext context);
    }

    public interface ISaleZoneFilterContext
    {
        int SellingNumberPlanId { get; }
        SaleZone SaleZone { get; }
        object CustomData { get; set; }
        List<CostCalculationMethod> CostCalculationMethods { get; set; }
        int? RoutingDatabaseId { get; set; }
        Guid? PolicyConfigId { get; set; }
        int? NumberOfOptions { get; set; }
        int? CurrencyId { get; set; }
    }

    public class SaleZoneFilterContext : ISaleZoneFilterContext
    {
        public int SellingNumberPlanId { get; set; }

        public SaleZone SaleZone { get; set; }

        public object CustomData { get; set; }

        public List<CostCalculationMethod> CostCalculationMethods { get; set; }

        public int? RoutingDatabaseId { get; set; }

        public Guid? PolicyConfigId { get; set; }

        public int? NumberOfOptions { get; set; }

        public int? CurrencyId { get; set; }
    }
}