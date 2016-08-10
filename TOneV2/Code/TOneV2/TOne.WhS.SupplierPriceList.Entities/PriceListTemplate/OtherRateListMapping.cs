using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.ExcelConversion.Entities;

namespace TOne.WhS.SupplierPriceList.Entities
{
    public class OtherRateListMapping
    {
        public int RateTypeId { get; set; }
        public ListMapping RateListMapping { get; set; }
    }
}
