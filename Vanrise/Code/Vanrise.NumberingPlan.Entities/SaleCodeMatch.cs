using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.NumberingPlan.Entities
{
    public class SaleCodeMatch
    {
        public string SaleCode { get; set; }

        public long SaleZoneId { get; set; }

        public int SellingNumberPlanId { get; set; }
    }
}
 