using System;
using System.Collections.Generic;
using System.Activities;
using Vanrise.BusinessProcess;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Common;
using Retail.BusinessEntity.Entities;
using Retail.BusinessEntity.Business;
using Retail.BusinessEntity.MainExtensions.PackageTypes;

namespace Retail.BusinessEntity.BP.Activities
{
    public sealed class EvaluateReccuringCharge : CodeActivity
    {
        [RequiredArgument]
        public InArgument<List<AccountPackageRecurChargeData>> AccountPackageRecurChargeDataList { get; set; }

        [RequiredArgument]
        public InArgument<DateTime> EffectiveDate { get; set; }

        [RequiredArgument]
        public OutArgument<Dictionary<AccountPackageRecurChargeKey, List<AccountPackageRecurCharge>>> AccountPackageRecurChargeDict { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            List<AccountPackageRecurChargeData> accountPackageRecurChargeDataList = context.GetValue(this.AccountPackageRecurChargeDataList);
            DateTime effectiveDate = context.GetValue(this.EffectiveDate);

            RecurringChargeManager recurringChargeManager = new RecurringChargeManager();
            PackageManager packageManager = new PackageManager();
            PackageDefinitionManager packageDefinitionManager = new PackageDefinitionManager();
            FinancialAccountManager financialAccountManager = new Business.FinancialAccountManager();
            ChargeableEntityManager chargeableEntityManager = new Business.ChargeableEntityManager();

            Dictionary<AccountPackageRecurChargeKey, List<AccountPackageRecurCharge>> accountPackageRecurChargeDict = new Dictionary<AccountPackageRecurChargeKey, List<AccountPackageRecurCharge>>();

            foreach (AccountPackageRecurChargeData accountPackageRecurChargeData in accountPackageRecurChargeDataList)
            {
                Guid accountBEDefinitionId = accountPackageRecurChargeData.AccountPackage.AccountBEDefinitionId;
                Package package = packageManager.GetPackage(accountPackageRecurChargeData.AccountPackage.PackageId);
                PackageDefinition packageDefinition = packageDefinitionManager.GetPackageDefinitionById(package.Settings.PackageDefinitionId);
                RecurChargePackageDefinitionSettings recurChargePackageDefinitionSettings = packageDefinition.Settings.ExtendedSettings as RecurChargePackageDefinitionSettings;
                RecurChargePackageSettings recurChargePackageSettings = package.Settings.ExtendedSettings as RecurChargePackageSettings;

                if (recurChargePackageSettings == null || recurChargePackageDefinitionSettings == null)
                    continue;

                List<RecurringChargeEvaluatorOutput> recurringChargeEvaluatorOutputs = recurringChargeManager.EvaluateRecurringCharge(recurChargePackageSettings.Evaluator, recurChargePackageDefinitionSettings.EvaluatorDefinitionSettings,
                    accountPackageRecurChargeData.BeginChargePeriod, accountPackageRecurChargeData.EndChargePeriod, accountBEDefinitionId, accountPackageRecurChargeData.AccountPackage);

                if (recurringChargeEvaluatorOutputs == null)
                    continue;

                FinancialAccountRuntimeData financialAccountRuntimeData = financialAccountManager.GetAccountFinancialInfo(accountBEDefinitionId, accountPackageRecurChargeData.AccountPackage.AccountId, effectiveDate);
                foreach (RecurringChargeEvaluatorOutput recurringChargeEvaluatorOutput in recurringChargeEvaluatorOutputs)
                {
                    ChargeableEntitySettings chargeableEntitySettings = chargeableEntityManager.GetChargeableEntitySettings(recurringChargeEvaluatorOutput.ChargeableEntityId);
                    chargeableEntitySettings.ThrowIfNull("chargeableEntitySettings", recurringChargeEvaluatorOutput.ChargeableEntityId);
                    if (!chargeableEntitySettings.TransactionTypeId.HasValue)
                        throw new NullReferenceException(string.Format("chargeableEntitySettings.TransactionTypeId ChargeableEntityId:{0}", recurringChargeEvaluatorOutput.ChargeableEntityId));

                    DateTime chargeDay = accountPackageRecurChargeData.BeginChargePeriod;
                    while (chargeDay <= accountPackageRecurChargeData.EndChargePeriod)
                    {
                        AccountPackageRecurCharge accountPackageRecurCharge = new AccountPackageRecurCharge()
                        {
                            AccountPackageID = accountPackageRecurChargeData.AccountPackage.AccountPackageId,
                            BalanceAccountID = financialAccountRuntimeData.BalanceAccountId,
                            BalanceAccountTypeID = financialAccountRuntimeData.BalanceAccountTypeId,
                            ChargeableEntityID = recurringChargeEvaluatorOutput.ChargeableEntityId,
                            ChargeAmount = chargeDay >= recurringChargeEvaluatorOutput.ChargingStart && chargeDay < recurringChargeEvaluatorOutput.ChargingEnd ? recurringChargeEvaluatorOutput.Amount : 0,
                            ChargeDay = chargeDay,
                            CurrencyID = recurringChargeEvaluatorOutput.CurrencyId,
                            ProcessInstanceID = context.GetSharedInstanceData().InstanceInfo.ProcessInstanceID,
                            TransactionTypeID = chargeableEntitySettings.TransactionTypeId.Value
                        };

                        List<AccountPackageRecurCharge> accountPackageRecurChargeList = accountPackageRecurChargeDict.GetOrCreateItem(BuildAccountPackageRecurChargeKey(accountPackageRecurCharge));
                        accountPackageRecurChargeList.Add(accountPackageRecurCharge);
                        chargeDay = chargeDay.AddDays(1);
                    }
                }
            }
            this.AccountPackageRecurChargeDict.Set(context, accountPackageRecurChargeDict);
        }

        private AccountPackageRecurChargeKey BuildAccountPackageRecurChargeKey(AccountPackageRecurCharge accountPackageRecurCharge)
        {
            return new AccountPackageRecurChargeKey()
            {
                BalanceAccountTypeID = accountPackageRecurCharge.BalanceAccountTypeID,
                ChargeDay = accountPackageRecurCharge.ChargeDay,
                TransactionTypeId = accountPackageRecurCharge.TransactionTypeID
            };
        }
    }
}