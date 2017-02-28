using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class CustomerPriceListChange
    {
        public int CustomerId { get; set; }
        public List<SalePricelistCodeChange> CodeChanges { get; set; }
        public List<SalePricelistRateChange> RateChanges { get; set; }
    }
}
