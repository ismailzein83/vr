using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.Analytics.Entities
{
    public class CustomerRoutingFormatted
    {

        public string Customer { get; set; }

        public string Destination { get; set; }

        public string SaleDuration { get; set; }

        public string SaleRate { get; set; }

        public string Supplier { get; set; }

        public string CostDuration { get; set; }

        public string CostRate { get; set; }

        public string Profit { get; set; }
        public string ProfitPerc { get; set; }
    }
}
