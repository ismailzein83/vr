using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Routing.Entities
{
    public class CustomerRouteQualityConfigurationData
    {
        public Guid QualityConfigurationId { get; set; }
        public long? SupplierZoneId { get; set; }
        public Decimal QualityData { get; set; }
    }
}
