using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.SupplierPriceList.Entities
{
    public class SupplierPriceListTemplate
    {
        public int SupplierPriceListTemplateId { get; set; }
        public int SupplierId { get; set; }
        public SupplierPriceListSettings ConfigDetails { get; set; }
        public SupplierPriceListSettings Draft { get; set; }
    }
}
