using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.AccountBalance.Entities
{
    public class AlertRuleThresholdActionBatch
    {
        public List<AlertRuleThresholdAction> AlertRuleThresholdActions { get; set; }
        public List<AccountBalanceAlertRule> AccountBalanceAlertRules { get; set; }
    }
}
