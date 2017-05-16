using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities.SalePricelistChanges
{
    public class SalePricelistRateChangeDetail
    {
        public string ZoneName { get; set; }
        public decimal Rate { get; set; }
        public IEnumerable<int> ServicesId { get; set; }
        public DateTime BED { get; set; }
        public DateTime? EED { get; set; }
    }
}
