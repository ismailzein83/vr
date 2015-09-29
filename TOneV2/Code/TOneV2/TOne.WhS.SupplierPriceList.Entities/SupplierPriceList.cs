using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.SupplierPriceList.Entities
{
    public class SupplierPriceList
    {
        public string Code { get; set; }
        public decimal Rate { get; set; }
        public DateTime BED {get; set;}
    }
}
