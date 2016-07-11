using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.SupplierPriceList.Entities
{
    public class SupplierPriceListInput
    {
        public int InputFileId { get; set; }
        public SupplierPriceListSettings SupplierPriceListSettings { get; set; }
    }
}
