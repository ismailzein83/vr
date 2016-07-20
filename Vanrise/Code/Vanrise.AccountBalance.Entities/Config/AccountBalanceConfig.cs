using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.AccountBalance.Entities
{
    public class AccountBalanceConfig
    {
        public int BusinessEntityDefinitionId { get; set; }

        public string AccountSelector { get; set; }

        public Guid UsageTransactionTypeId { get; set; }

        public BalanceAlertConfig BalanceAlertConfig { get; set; }
    }
}
