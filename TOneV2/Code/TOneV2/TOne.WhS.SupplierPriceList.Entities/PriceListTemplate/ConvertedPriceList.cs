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
    }
}
