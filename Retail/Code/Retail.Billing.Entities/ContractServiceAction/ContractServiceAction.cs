using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.Billing.Entities
{
    public class ContractServiceAction
    {
        public long ID { get; set; }
        public long Contract { get; set; }
        public long ContractService { get; set; }
        public Guid ServiceType { get; set; }
        public Guid? ServiceTypeOption { get; set; }
        public Guid ActionType { get; set; }
        public decimal Charge { get; set; }
        public decimal? OverriddenCharge { get; set; }
        public DateTime ChargeTime { get; set; }
        public bool PaidCash { get; set; }
        public long? ContractServiceHistory { get; set; }
        public Guid? OldServiceOption { get; set; }
        public Guid? NewServiceOption { get; set; }
        public Decimal? OldServiceOptionActivationFee { get; set; }
        public Decimal? NewServiceOptionActivationFee { get; set; }
        public Decimal? OldSpeedInMbps { get; set; }
        public Decimal? NewSpeedInMbps { get; set; }
        public DateTime CreatedTime { get; set; }
        public DateTime LastModifiedTime { get; set; }
        public int LastModifiedBy { get; set; }
        public int CreatedBy { get; set; }
    }
        
}
