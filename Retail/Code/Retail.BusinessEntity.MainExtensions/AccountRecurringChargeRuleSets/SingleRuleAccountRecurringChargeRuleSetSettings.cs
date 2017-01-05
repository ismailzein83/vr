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
        public override Guid ConfigId
        {
            get { throw new NotImplementedException(); }
        }

        public Guid RecurringChargeDefinitionId { get; set; }
        
        public AccountCondition Condition { get; set; }

        public AccountChargeEvaluator ChargeEvaluator { get; set; }

        public override List<ApplicableRecurringCharge> GetApplicableCharges(IAccountRecurringChargeRuleSetContext context)
        {
            throw new NotImplementedException();
        }
    }
}
