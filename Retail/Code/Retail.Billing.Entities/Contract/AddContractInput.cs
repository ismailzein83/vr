using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.Billing.Entities
{
    public class AddContractInput
    {
        public Guid ContractTypeId { get; set; }

        public long CustomerId { get; set; }

        public long BillingAccountId { get; set; }

        public int RatePlanId { get; set; }

        public string MainResourceName { get; set; }

        public DateTime BET { get; set; }

        public Guid? TechnologyId { get; set; }

        public Guid? SpecialNumberCategoryId { get; set; }

        public bool? HasTelephony { get; set; }

        public bool? HasInternet { get; set; }

        public decimal? SpeedInMbps { get; set; }

        public int? SpeedType { get; set; }

        public int? PackageLimitInGB { get; set; }

        public int? NbOfLinks { get; set; }
    }
}
