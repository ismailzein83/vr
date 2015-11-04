using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Sales.Entities
{
    public class RatePlan
    {
        public int RatePlanId { get; set; }

        public RatePlanOwnerType OwnerType { get; set; }

        public int OwnerId { get; set; }

        public List<RatePlanItem> Details { get; set; }

        public RatePlanStatus Status { get; set; }
    }

    public enum RatePlanOwnerType
    {
        SellingProduct = 0,
        Customer = 1
    }

    public enum RatePlanStatus
    {
        Draft = 0,
        Completed = 1
    }
}
