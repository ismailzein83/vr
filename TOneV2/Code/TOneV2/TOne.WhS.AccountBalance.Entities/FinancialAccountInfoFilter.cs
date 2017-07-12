using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace TOne.WhS.AccountBalance.Entities
{
    public enum CarrierType { Profile = 0, Account = 1 }
    public class FinancialAccountInfoFilter
    {
        public Guid AccountBalanceTypeId { get; set; }
        public VRAccountStatus? Status { get; set; }
        public DateTime? EffectiveDate { get; set; }
        public bool? IsEffectiveInFuture { get; set; }
        public CarrierType? CarrierType { get; set; }

    }
}
