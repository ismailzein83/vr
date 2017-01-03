using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.MainExtensions.AccountRecurringChargeRuleSets
{
    public class ApplyFirstAccountRecurringChargeRuleSetSettings : AccountRecurringChargeRuleSetSettings
    {
        public override Guid ConfigId
        {
            get { throw new NotImplementedException(); }
        }

        public List<AccountRecurringChargeRule> ChargeRules { get; set; }

        public override List<ApplicableRecurringCharge> GetApplicableCharges(IAccountRecurringChargeRuleSetContext context)
        {
            throw new NotImplementedException();
        }
    }
}
