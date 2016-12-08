using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.SupplierPriceList.Entities
{
     public class ConvertedPriceList
    {
         public List<PriceListRate> PriceListRates { get; set; }
         public List<PriceListCode> PriceListCodes { get; set; }
         public List<PriceListZoneService> PriceListServices { get; set; }
         public Dictionary<int, List<PriceListRate>> PriceListOtherRates { get; set; }
         public bool IncludeServices { get; set; }
    }
}
