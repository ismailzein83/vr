using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.NumberingPlan.Entities
{
    public class SaleZoneInfoFilter
    {
        public IEnumerable<ISaleZoneFilter> Filters { get; set; }

        public bool GetEffectiveOnly { get; set; }

        public IEnumerable<int> CountryIds { get; set; }

        public IEnumerable<long> AvailableZoneIds { get; set; }

        public IEnumerable<long> ExcludedZoneIds { get; set; }
    }

    public interface ISaleZoneFilter
    {
        bool IsExcluded(ISaleZoneFilterContext context);
    }

    public interface ISaleZoneFilterContext
    {
        SaleZone SaleZone { get; }

        object CustomData { get; set; }
    }

    public class SaleZoneFilterContext : ISaleZoneFilterContext
    {
        public SaleZone SaleZone { get; set; }

        public object CustomData { get; set; }
    }
}
