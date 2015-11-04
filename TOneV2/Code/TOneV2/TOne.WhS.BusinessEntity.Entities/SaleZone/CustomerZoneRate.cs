using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public enum CustomerZoneRateSource { Customer, Product }

    public class CustomerZoneRate
    {
        public SaleRate Rate { get; set; }

        public CustomerZoneRateSource Source { get; set; }

        public SalePriceList PriceList { get; set; }
    }
}
