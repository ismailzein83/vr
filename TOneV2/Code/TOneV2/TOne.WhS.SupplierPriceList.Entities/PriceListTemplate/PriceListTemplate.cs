using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.SupplierPriceList.Entities
{
    public class PriceListTemplate
    {
        public int PriceListTemplateId { get; set; }
        public int UserId { get; set; }
        public string Name { get; set; }
        public Object ConfigDetails { get; set; }
    }
}
