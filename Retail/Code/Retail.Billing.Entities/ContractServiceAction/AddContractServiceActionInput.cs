using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.Billing.Entities
{
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
}
