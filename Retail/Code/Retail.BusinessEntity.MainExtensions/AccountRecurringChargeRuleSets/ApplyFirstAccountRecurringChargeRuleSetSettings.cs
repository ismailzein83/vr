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
        public override Guid ConfigId { get { return new Guid("1F5BF4F6-A2C5-408B-9E68-C6B1A32E6EF3"); } }

        public List<AccountRecurringChargeRule> ChargeRules { get; set; }

        public override List<ApplicableRecurringCharge> GetApplicableCharges(IAccountRecurringChargeRuleSetContext context)
        {
            throw new NotImplementedException();
        }
    }
}
