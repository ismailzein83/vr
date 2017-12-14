using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Routing.Entities
{
    public class RPQualityConfigurationData
    {
        public Guid QualityConfigurationId { get; set; }
        public int? SupplierId { get; set; }
        public long? SaleZoneId { get; set; }
        public Decimal QualityData { get; set; }
    }
}
