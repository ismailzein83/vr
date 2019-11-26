using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.Billing.Entities
{
    public class ContractHistory
    {
        public long ID { get; set; }

        public long ContractId { get; set; }

        public long BillingAccountId { get; set; }

        public int RatePlanId { get; set; }

        public string MainResourceName { get; set; }

        public Guid StatusId { get; set; }

        public Guid? StatusReasonId { get; set; }

        public Guid? TechnologyId { get; set; }

        public long? NIMPathId { get; set; }

        public bool? HasInternet { get; set; }

        public Guid? SpecialNumberCategoryId { get; set; }

        public bool? HasTelephony { get; set; }

        public decimal? SpeedInMbps { get; set; }

        public int? SpeedType { get; set; }

        public int? PackageLimitInGB { get; set; }

        public int? NbOfLinks { get; set; }

        public DateTime BET { get; set; }

        public DateTime? EET { get; set; }

        public DateTime CreatedTime { get; set; }
    }
}
