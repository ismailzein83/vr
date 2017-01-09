using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.MainExtensions.AccountRecurringChargeRuleSets
{
    public class SingleRuleAccountRecurringChargeRuleSetSettings : AccountRecurringChargeRuleSetSettings
    {
        public override Guid ConfigId { get { return new Guid("45C757B5-B2FE-44E5-943E-A9770A384AE9"); } }

        public Guid RecurringChargeDefinitionId { get; set; }

        public AccountCondition Condition { get; set; }

        public AccountChargeEvaluator ChargeEvaluator { get; set; }

        public override List<ApplicableRecurringCharge> GetApplicableCharges(IAccountRecurringChargeRuleSetContext context)
        {
            throw new NotImplementedException();
        }
    }
}
