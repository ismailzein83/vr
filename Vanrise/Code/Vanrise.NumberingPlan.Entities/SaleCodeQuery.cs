using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.NumberingPlan.Entities
{
    public class SaleCodeQuery
    {
        public int? SellingNumberPlanId { get; set; }

        public List<long> ZonesIds { get; set; }

        public string Code { get; set; }

        public DateTime? EffectiveOn { get; set; }
    }
}
