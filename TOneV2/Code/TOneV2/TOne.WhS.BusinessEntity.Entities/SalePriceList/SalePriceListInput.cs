using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class SalePriceListInput
    {
        public int PriceListTypeId { get; set; }
        public int PricelistTemplateId { get; set; }
        public int PriceListId { get; set; }
    }
}
