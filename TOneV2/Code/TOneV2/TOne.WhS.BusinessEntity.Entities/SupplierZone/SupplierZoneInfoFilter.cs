using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class SupplierZoneInfoFilter
    {
        public IEnumerable<ISupplierZoneFilter> Filters { get; set; }
        public Vanrise.Entities.EntityFilterEffectiveMode EffectiveMode { get; set; }

        public IEnumerable<int> CountryIds { get; set; }

        public IEnumerable<long> AvailableZoneIds { get; set; }

        public IEnumerable<long> ExcludedZoneIds { get; set; }
    }
    public interface ISupplierZoneFilter
    {
        bool IsExcluded(ISupplierZoneFilterContext context);
    }
     public interface ISupplierZoneFilterContext
    {
        SupplierZone SupplierZone { get; }

        object CustomData { get; set; }
    }

     public class SupplierZoneFilterContext : ISupplierZoneFilterContext
     {
         public SupplierZone SupplierZone { get; set; }

         public object CustomData { get; set; }
     }

}
