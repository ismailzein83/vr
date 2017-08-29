using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Business;
using Vanrise.Common;
using Vanrise.Entities;

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
            DateTime chargingPeriodStart, DateTime chargingPeriodEnd, Guid accountBEDefinitionId, AccountPackage accountPackage, DateTime chargeDay, object initializeData,
            Func<int, DateTimeRange, List<AccountPackageRecurCharge>> getAccountPackageRecurCharges, Guid accountStatusId)
        {
            var context = new RecurringChargeEvaluatorContext(recurringChargeDefinition, chargingPeriodStart, chargingPeriodEnd, accountBEDefinitionId, accountPackage, chargeDay, initializeData, getAccountPackageRecurCharges, accountStatusId);
            return recurringCharge.Evaluate(context);
        }

        public DateTimeRange InitializeRecurringCharge(RecurringChargeEvaluatorSettings recurringCharge, DateTime chargingPeriodStart, DateTime chargingPeriodEnd, AccountPackage accountPackage, out object initializeData)
        {
            var context = new InitializeRecurringChargeContext(chargingPeriodStart, chargingPeriodEnd, accountPackage);
            recurringCharge.Initialize(context);
            initializeData = context.InitializeData;
            return context.RecurringChargePeriod;
        }

        #region Private Classes

        public class RecurringChargeEvaluatorContext : IRecurringChargeEvaluatorContext
        {
            RecurringChargeEvaluatorDefinitionSettings _recurringChargeDefinition;
            DateTime _chargingPeriodStart;
            DateTime _chargingPeriodEnd;
            Guid _accountBEDefinitionId;
            AccountPackage _accountPackage;
            DateTime _chargeDay;
            object _initializeData;
            Func<int, DateTimeRange, List<AccountPackageRecurCharge>> _getAccountPackageRecurCharges;
            Guid _accountStatusId;

            public RecurringChargeEvaluatorContext(RecurringChargeEvaluatorDefinitionSettings recurringChargeDefinition, DateTime chargingPeriodStart, DateTime chargingPeriodEnd,
                Guid accountBEDefinitionId, AccountPackage accountPackage, DateTime chargeDay, object initializeData, Func<int, DateTimeRange, List<AccountPackageRecurCharge>> getAccountPackageRecurCharges, Guid accountStatusId)
            {
                _recurringChargeDefinition = recurringChargeDefinition;
                _chargingPeriodStart = chargingPeriodStart;
                _chargingPeriodEnd = chargingPeriodEnd;
                _accountBEDefinitionId = accountBEDefinitionId;
                _accountPackage = accountPackage;
                _chargeDay = chargeDay;
                _initializeData = initializeData;
                _getAccountPackageRecurCharges = getAccountPackageRecurCharges;
                _accountStatusId = accountStatusId;
            }

            public RecurringChargeEvaluatorDefinitionSettings EvaluatorDefinitionSettings
            {
                get { return _recurringChargeDefinition; }
            }

            Account _account;
            public Account Account
            {
                get
                {
                    if (_account == null)
                    {
                        _account = new AccountBEManager().GetAccount(_accountBEDefinitionId, _accountPackage.AccountId);
                        _account.ThrowIfNull("_account", _accountPackage.AccountId);
                    }
                    return _account;
                }
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

            public DateTime ChargeDay
            {
                get { return _chargeDay; }
            }

            public Func<int, DateTimeRange, List<AccountPackageRecurCharge>> GetAccountPackageRecurCharges
            {
                get { return _getAccountPackageRecurCharges; }
            }

            public int AccountPackageId
            {
                get { return _accountPackage.AccountPackageId; }
            }

            public Object InitializeData
            {
                get { return _initializeData; }
            }

            public Guid AccountStatusId
            {
                get { return _accountStatusId; }
            }
        }

        public class InitializeRecurringChargeContext : IInitializeRecurringChargeContext
        {
            AccountPackage _accountPackage { get; set; }

            public DateTime ChargingPeriodStart { get; set; }

            public DateTime ChargingPeriodEnd { get; set; }

            public DateTimeRange RecurringChargePeriod { get; set; }

            public object InitializeData { get; set; }

            public DateTime PackageAssignmentStart { get { return _accountPackage.BED; } }

            public DateTime? PackageAssignmentEnd { get { return _accountPackage.EED; } }

            public InitializeRecurringChargeContext(DateTime chargingPeriodStart, DateTime chargingPeriodEnd, AccountPackage accountPackage)
            {
                ChargingPeriodStart = chargingPeriodStart;
                ChargingPeriodEnd = chargingPeriodEnd;
                _accountPackage = accountPackage;
            }
        }
        #endregion
    }
}
