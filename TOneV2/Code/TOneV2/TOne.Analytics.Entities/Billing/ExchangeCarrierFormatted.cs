using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.Analytics.Entities
{
    public class ExchangeCarrierFormatted
    {
        public string Customer { get; set; }
        public double CustomerProfit { get; set; }
        public string CustomerProfitFormatted { get; set; }
        public double SupplierProfit { get; set; }
        public string SupplierProfitFormatted { get; set; }
        public string Total { get; set; }

    }
}
