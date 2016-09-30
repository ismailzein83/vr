using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.AccountBalance.Entities;
using Vanrise.Common.Business;

namespace Vanrise.AccountBalance.Business
{
    public class ConfigurationManager
    {
        public Guid GetAccountBEDefinitionId()
        {
            AccountBalanceConfig config = GetAccountBalanceConfig();
            if (config == null)
                throw new NullReferenceException("config");
            return config.AccountBusinessEntityDefinitionId;
        }

        private AccountBalanceConfig GetAccountBalanceConfig()
        {
            var settingManager = new SettingManager();
            return settingManager.GetSetting<AccountBalanceConfig>(AccountBalanceConfig.AccountBalanceConfigType);
        }

        public int GetBalanceAlertRuleDefinitionId()
        {
            AccountBalanceConfig config = GetAccountBalanceConfig();
            if (config == null)
                throw new NullReferenceException("config");
            return config.BalanceAlertRuleDefinitionId; 
        }

        public Guid GetUsageTransactionTypeId()
        {
            AccountBalanceConfig config = GetAccountBalanceConfig();
            if (config == null)
                throw new NullReferenceException("config");
            return config.UsageTransactionTypeId; 
        }
        public BalancePeriodSettings GetBalancePeriod()
        {
            AccountBalanceConfig config = GetAccountBalanceConfig();
            if (config == null)
                throw new NullReferenceException("config");
            return config.BalancePeriod; 
        }

    }
}
