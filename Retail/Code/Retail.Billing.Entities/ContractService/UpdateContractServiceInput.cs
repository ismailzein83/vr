using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.Billing.Entities
{
    public class UpdateContractServiceInput
    {
        public long ContractServiceId { get; set; }

        public Guid? ServiceTypeOptionId { get; set; }

        //public long? BillingAccountId { get; set; }

        public Guid? StatusId { get; set; }

        public Guid? StatusReasonId { get; set; }

        public DateTime BET { get; set; }
    }

    public class UpdateContractServiceInternetPackageInput
    {
        public long ContractServiceId { get; set; }

        public Guid ServiceTypeOptionId { get; set; }

        public decimal? SpeedInMbps { get; set; }

        public int? SpeedType { get; set; }

        public int? PackageLimitInGB { get; set; }
    }

    public class UpdateContractServiceInternetPackageOutput
    {
    }


    public class UpdateContractServiceSpecialNumberInput
    {
        public long ContractServiceId { get; set; }

        public Guid ServiceTypeOptionId { get; set; }

        public Guid SpecialNumberCategoryId { get; set; }
    }

    public class UpdateContractServiceSpecialNumberOutput
    {
    }  

    public class UpdateContractServiceStatusInput
    {
        public long ContractServiceId { get; set; }

        public Guid StatusId { get; set; }

        public Guid? StatusReasonId { get; set; }
    }

    public class UpdateContractServiceStatusOutput
    {
    }
}
