using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.AccountBalance.Entities;

namespace Vanrise.AccountBalance.Business
{
    public class ConfigurationManager
    {
        public int GetAccountBEDefinitionId()
        {
            AccountBalanceConfig config = GetAccountBalanceConfig();
            if (config == null)
                throw new NullReferenceException("config");
            return config.AccountBusinessEntityDefinitionId;
        }

        private AccountBalanceConfig GetAccountBalanceConfig()
        {
            return new AccountBalanceConfig
            {
                AccountBusinessEntityDefinitionId = -2001
            };
        }

        public int GetBalanceAlertRuleDefinitionId()
        {
            throw new NotImplementedException();
        }

        public Guid GetUsageTransactionTypeId()
        {
            AccountBalanceConfig config = GetAccountBalanceConfig();
            if (config == null)
                throw new NullReferenceException("config");
            return config.UsageTransactionTypeId; 
        }
    }
}
