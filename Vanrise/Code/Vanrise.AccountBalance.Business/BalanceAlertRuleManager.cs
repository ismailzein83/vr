using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.AccountBalance.Entities;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;

namespace Vanrise.AccountBalance.Business
{
    public class BalanceAlertRuleManager : GenericRuleManager<BalanceAlertRule>
    {
        public BalanceAlertRule GetMatchRule(dynamic account)
        {
            var configurationManager = new ConfigurationManager();
            var accountBEDefinitionId = configurationManager.GetAccountBEDefinitionId();
            GenericRuleTarget ruleTarget = BuildRuleTarget(account);
            return base.GetMatchRule(configurationManager.GetBalanceAlertRuleDefinitionId(), ruleTarget);
        }

        private GenericRuleTarget BuildRuleTarget(dynamic account)
        {
            GenericRuleTarget ruleTarget = new GenericRuleTarget
            {
                EffectiveOn = DateTime.Now,
                Objects = { { "Account", account } }
            };
            return ruleTarget;
        }
    }
}
