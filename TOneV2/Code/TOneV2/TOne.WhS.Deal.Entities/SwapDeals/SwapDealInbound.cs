using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Deal.Entities
{
    public class SwapDealInbound
    {
        public string Name { get; set; }

        public List<long> SaleZoneIds { get; set; }

        public int Volume { get; set; }

        public Decimal Rate { get; set; }
    }
}
