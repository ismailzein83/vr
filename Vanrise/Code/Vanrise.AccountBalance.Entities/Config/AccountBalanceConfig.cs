using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.AccountBalance.Entities
{
    public class AccountBalanceConfig:SettingData
    {
        public static string AccountBalanceConfigType = "VR_AccountBalance_Configuration";
        public Guid AccountBusinessEntityDefinitionId { get; set; }

        public string AccountSelector { get; set; }

        public Guid UsageTransactionTypeId { get; set; }

        public int BalanceAlertRuleDefinitionId { get; set; }

        public Guid AlertMailMessageTypeId { get; set; }
        public BalancePeriodSettings BalancePeriod{ get; set; }
    }
}
