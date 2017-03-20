using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.AccountBalance.Entities
{
    public class FinancialAccountInfo
    {
        public int FinancialAccountId { get; set; }

        public FinancialAccountCarrierType CarrierType { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public FinancialAccountEffectiveStatus EffectiveStatus { get; set; }
    }

    public enum FinancialAccountEffectiveStatus { Recent = 0, Current = 1, Future = 2 }

    public enum FinancialAccountCarrierType
    {
        [Description("Prof")]
        Profile = 0,

        [Description("Acc")]
        Account = 1
    }
}
