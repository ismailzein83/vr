using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CP.SupplierPricelist.Entities
{
    public class PriceListQuery
    {
        public List<int> CustomersIDs { get; set; }
        public List<string> CarrierAccounts { get; set; }
        public List<int> PriceListTypes { get; set; }
        public List<int> PriceListResults { get; set; }
        public List<int> PriceListStatuses { get; set; }
        public DateTime FromEffectiveOnDate { get; set; }
        public DateTime? ToEffectiveOnDate { get; set; }

       
    }
}
