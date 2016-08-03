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
            GenericRuleTarget ruleTarget = BuildRuleTarget(account);
            return base.GetMatchRule(configurationManager.GetBalanceAlertRuleDefinitionId(), ruleTarget);
        }
        public BalanceAlertRule GetMatchRule(long accountId)
        {
            var configurationManager = new ConfigurationManager();
            var accountManager = new AccountManager();
            var account = accountManager.GetAccount(accountId);
            GenericRuleTarget ruleTarget = BuildRuleTarget(account);
            return base.GetMatchRule(configurationManager.GetBalanceAlertRuleDefinitionId(), ruleTarget);
        }
        private GenericRuleTarget BuildRuleTarget(dynamic account)
        {
            GenericRuleTarget ruleTarget = new GenericRuleTarget
            {
                EffectiveOn = DateTime.Now,
                Objects = new Dictionary<string,dynamic> { { "Account", account } }
            };
            return ruleTarget;
        }
    }
}
