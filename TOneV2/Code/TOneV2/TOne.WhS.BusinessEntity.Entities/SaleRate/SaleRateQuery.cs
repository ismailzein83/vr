using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class SaleRateQuery
    {
        public DateTime? EffectiveOn { get; set; }

        public List<int> ZonesIds { get; set; }

        public SalePriceListOwnerType OwnerType { get; set; }

        public int OwnerId { get; set; }

        public int? SellingNumberPlanId { get; set; }

        public int? CurrencyId { get; set; }
    }
}
