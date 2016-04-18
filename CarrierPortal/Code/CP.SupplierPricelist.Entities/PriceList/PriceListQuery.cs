using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CP.SupplierPricelist.Entities
{
    public class PriceListQuery
    {
        public List<int> CarriersID { get; set; }
        public List<string> CarrierAccounts { get; set; }
        public int PriceListType { get; set; }
        public int PriceListResult { get; set; }
        public int PriceListStatus { get; set; }
        public DateTime FromEffectiveOnDate { get; set; }
        public DateTime ToEffectiveOnDate { get; set; }

        public PriceListQuery()
        {
            PriceListResult = -1;
            PriceListType = -1;
            PriceListStatus = -1;
        }
    }
}
