using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.AccountBalance.Entities
{
    public class AccountBalanceConfig
    {
        public int AccountBusinessEntityDefinitionId { get; set; }

        public string AccountSelector { get; set; }

        public Guid UsageTransactionTypeId { get; set; }

        public int BalanceAlertRuleDefinitionId { get; set; }
    }
}
