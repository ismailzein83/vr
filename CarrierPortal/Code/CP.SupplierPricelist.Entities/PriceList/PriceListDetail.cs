using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CP.SupplierPricelist.Entities
{
    public class PriceListDetail
    {
        public PriceList Entity { get; set; }
        public string UserName { get; set; }
        public string PriceListStatusDescription { get; set; }
        public string PriceListResultDescription { get; set; }
        public string ScheduleName { get; set; }
        public string PriceListTypeValue { get; set; }
        public string CustomerName { get; set; }
        public string CarrierAccountName { get; set; }
    }
}
