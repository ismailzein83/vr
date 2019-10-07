using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.Billing.Entities
{

    public class AddContractResourceInput
    {
        public long ContractId { get; set; }

        public string Name { get; set; }

        public Guid ResourceTypeId { get; set; }

        public DateTime BET { get; set; }

        public DateTime? EET { get; set; }
    }

    public class AddContractResourceOutput
    {
        public long ContractResourceId { get; set; }
    }

    public class CloseContractResourceInput
    {
        public long ContractResourceId { get; set; }

        public DateTime CloseTime { get; set; }
    }

    public class CloseContractResourceOutput
    {

    }

    public class AddContractServiceActionInput
    {
        public long ContractServiceId { get; set; }

        public Guid ActionTypeId { get; set; }

        public Decimal? OverriddenCharge { get; set; }

        public DateTime? ChargeTime { get; set; }

        public bool PaidCash { get; set; }

        public Guid? OldServiceOptionId { get; set; }

        public Guid? NewServiceOptionId { get; set; }

        public decimal? OldServiceOptionActivationFee { get; set; }

        public decimal? NewServiceOptionActivationFee { get; set; }

        public decimal? OldSpeedInMbps { get; set; }

        public decimal? NewSpeedInMbps { get; set; }
    }

    public class AddContractServiceActionOutput
    {
        public long ServiceContractActionId { get; set; }
    }

    public class AddContractServiceInput
    {
        public long ContractId { get; set; }

        public Guid ServiceTypeId { get; set; }

        public Guid? ServiceTypeOptionId { get; set; }

        public long? BillingAccountId { get; set; }

        public DateTime BET { get; set; }

        public Guid? TechnologyId { get; set; }

        public Guid? SpecialNumberCategoryId { get; set; }

        public decimal? SpeedInMbps { get; set; }

        public int? SpeedType { get; set; }

        public int? PackageLimitInGB { get; set; }
    }

    public class ContractService
    {
        public long ContractServiceId { get; set; }

        public long ContractId { get; set; }

        public Guid ServiceTypeId { get; set; }

        public Guid? ServiceTypeOptionId { get; set; }

        public long? BillingAccountId { get; set; }

        public Guid StatusId { get; set; }

        public Guid? StatusReasonId { get; set; }

        public Guid? TechnologyId { get; set; }

        public Guid? SpecialNumberCategoryId { get; set; }

        public decimal? SpeedInMbps { get; set; }

        public int? SpeedType { get; set; }

        public int? PackageLimitInGB { get; set; }
    }
}
