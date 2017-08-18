using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public enum WHSFinancialAccountEffectiveStatus { Recent = 0, Current = 1, Future = 2 }

    public enum WHSFinancialAccountCarrierType
    {
        [Description("Prof")]
        Profile = 0,

        [Description("Acc")]
        Account = 1
    }
}
