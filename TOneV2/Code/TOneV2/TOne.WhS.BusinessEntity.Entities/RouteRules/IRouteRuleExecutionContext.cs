using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public interface IRouteRuleExecutionContext
    {
        SupplierCodeMatchBySupplier SupplierCodeMatches { get; }

        SupplierZoneRatesByZone SupplierZoneRates { get; }

        RouteRule RouteRule { get; }

        int? NumberOfOptions { get; }

        bool TryAddOption(RouteOptionRuleTarget optionTarget);

        ReadOnlyCollection<RouteOptionRuleTarget> GetOptions();
    }

    public class SupplierCodeMatch
    {
        public int SupplierId { get; set; }

        public long SupplierZoneId { get; set; }

        public string SupplierCode { get; set; }
    }

    public class SupplierCodeMatchBySupplier : Dictionary<int, List<BusinessEntity.Entities.SupplierCodeMatch>>
    {
    }

    public class SupplierZoneRate
    {
        public int SupplierId { get; set; }

        public long SupplierZoneId { get; set; }

        public Decimal Rate { get; set; }
    }

    public class SupplierZoneRatesByZone : Dictionary<long, SupplierZoneRate>
    {

    }
}
