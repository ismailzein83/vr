using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class SaleEntityZoneRate
    {
        public SaleRate Rate { get; set; }

        public SalePriceListOwnerType Source { get; set; }

        public SalePriceList PriceList { get; set; }
    }
}
