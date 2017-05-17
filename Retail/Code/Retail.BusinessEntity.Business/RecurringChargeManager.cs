using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Business;
using Vanrise.Common;

namespace Retail.BusinessEntity.Business
{
    public class RecurringChargeManager
    {
        public IEnumerable<AccountRecurringChargeEvaluatorConfig> GetAccountRecurringChargeEvaluatorExtensionConfigs()
        {
            var templateConfigManager = new ExtensionConfigurationManager();
            return templateConfigManager.GetExtensionConfigurations<AccountRecurringChargeEvaluatorConfig>(AccountRecurringChargeEvaluatorConfig.EXTENSION_TYPE);
        }

        public IEnumerable<AccountRecurringChargeRuleSetSettingsConfig> GetAccountRecurringChargeRuleSetSettingsExtensionConfigs()  
        {
            var templateConfigManager = new ExtensionConfigurationManager();
            return templateConfigManager.GetExtensionConfigurations<AccountRecurringChargeRuleSetSettingsConfig>(AccountRecurringChargeRuleSetSettingsConfig.EXTENSION_TYPE); 
        }

        public List<RecurringChargeEvaluatorOutput> EvaluateRecurringCharge(RecurringChargeEvaluatorSettings recurringCharge, RecurringChargeEvaluatorDefinitionSettings recurringChargeDefinition,
            DateTime chargingPeriodStart, DateTime chargingPeriodEnd, Guid accountBEDefinitionId, AccountPackage accountPackage)
        {
            var context = new RecurringChargeEvaluatorContext(recurringChargeDefinition, chargingPeriodStart, chargingPeriodEnd, accountBEDefinitionId, accountPackage);
            return recurringCharge.Evaluate(context);
        }

        #region Private Classes 

        public class RecurringChargeEvaluatorContext : IRecurringChargeEvaluatorContext
        {
            RecurringChargeEvaluatorDefinitionSettings _recurringChargeDefinition;
            DateTime _chargingPeriodStart;
            DateTime _chargingPeriodEnd;
            Guid _accountBEDefinitionId;
            AccountPackage _accountPackage;

            public RecurringChargeEvaluatorContext(RecurringChargeEvaluatorDefinitionSettings recurringChargeDefinition, DateTime chargingPeriodStart, DateTime chargingPeriodEnd, Guid accountBEDefinitionId, AccountPackage accountPackage)
            {
                _recurringChargeDefinition = recurringChargeDefinition;
                _chargingPeriodStart = chargingPeriodStart;
                _chargingPeriodEnd = chargingPeriodEnd;
                _accountBEDefinitionId = accountBEDefinitionId;
                _accountPackage = accountPackage;
            }

            public RecurringChargeEvaluatorDefinitionSettings EvaluatorDefinitionSettings
            {
                get { return _recurringChargeDefinition; }
            }

            Account _account;
            public Account Account
            {
                get {
                    if(_account == null)
                    {
                        _account = new AccountBEManager().GetAccount(_accountBEDefinitionId, _accountPackage.AccountId);
                        _account.ThrowIfNull("_account", _accountPackage.AccountId);
                    }
                    return _account; }
            }

            public DateTime ChargingPeriodStart
            {
                get { return _chargingPeriodStart; }
            }

            public DateTime ChargingPeriodEnd
            {
                get { return _chargingPeriodEnd; }
            }

            public DateTime PackageAssignmentStart
            {
                get { return _accountPackage.BED; }
            }

            public DateTime? PackageAssignmentEnd
            {
                get { return _accountPackage.EED; }
            }
        }


        #endregion
    }
}
 