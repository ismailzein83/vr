using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class SupplierPriceListDetail
    {
        public SupplierPriceList Entity { get; set; }
        public string Currency { get; set; }
        public string PricelistTypeDescription { get; set; }
        public string SupplierName { get; set; }
        public string UserName { get; set; }
    }
}
