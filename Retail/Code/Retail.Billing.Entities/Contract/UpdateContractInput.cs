using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.Billing.Entities
{
    public class UpdateContractInput
    {
        public long ContractId { get; set; }

        public long? BillingAccountId { get; set; }

        public int? RatePlanId { get; set; }

        public string MainResourceName { get; set; }

        public Guid? StatusId { get; set; }

        public Guid? StatusReasonId { get; set; }

        public DateTime BET { get; set; }
    }

    public class UpdateContractInternetPackageInput
    {
        public long ContractId { get; set; }

        public decimal? SpeedInMbps { get; set; }

        public int? SpeedType { get; set; }

        public int? PackageLimitInGB { get; set; }
    }

    public class UpdateContractInternetPackageOutput
    {
    }

    public class UpdateContractResourceNameInput
    {
        public long ContractId { get; set; }

        public string ResourceName { get; set; }
    }

    public class UpdateContractResourceNameOutput
    {
    }


    public class UpdateContractStatusInput
    {
        public long ContractId { get; set; }

        public Guid StatusId { get; set; }

        public Guid? StatusReasonId { get; set; }
    }

    public class UpdateContractStatusOutput
    {
    }

}
