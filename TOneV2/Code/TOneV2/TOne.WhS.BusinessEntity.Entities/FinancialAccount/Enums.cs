using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public enum WHSFinancialAccountEffectiveStatus {
         [Description("Expired")]
        Recent = 0,
         [Description("Current")]
        Current = 1,
         [Description("Future")]
        Future = 2 
    }

    public enum WHSFinancialAccountCarrierType
    {
        [Description("per Profile")]
        Profile = 0,

        [Description("per Account")]
        Account = 1
    }
}
