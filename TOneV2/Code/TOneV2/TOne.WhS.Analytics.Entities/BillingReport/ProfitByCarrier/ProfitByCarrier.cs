using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Analytics.Entities.BillingReport
{
    public class ProfitByCarrier
    {
        public string Customer { get; set; }
        public double CustomerProfit { get; set; }
        public string FormattedCustomerProfit { get; set; }
        public double SupplierProfit { get; set; }
        public string FormattedSupplierProfit { get; set; }
        public string Total { get; set; }
    }
}
