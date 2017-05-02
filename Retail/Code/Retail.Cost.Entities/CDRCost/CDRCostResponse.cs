using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.Cost.Entities
{
    public class CDRCostResponse
    {
        public CDRCostRequest Request { get; set; }

        public decimal CostAmount { get; set; }

        public decimal CostRate { get; set; }

        public int CurrencyId { get; set; }
    }
}
