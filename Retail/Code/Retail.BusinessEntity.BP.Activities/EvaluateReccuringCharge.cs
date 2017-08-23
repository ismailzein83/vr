using System;
using System.Activities;
using System.Collections.Generic;
using Vanrise.Entities;
using Vanrise.Common;
using Vanrise.BusinessProcess;
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
        public OutArgument<Dictionary<DateTime, List<AccountPackageRecurCharge>>> AccountPackageRecurChargesByDate { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            context.GetSharedInstanceData().WriteTrackingMessage(LogEntryType.Information, "Evaluate Reccuring Charge has started", null);
            List<AccountPackageRecurChargeData> accountPackageRecurChargeDataList = context.GetValue(this.AccountPackageRecurChargeDataList);
            DateTime effectiveDate = context.GetValue(this.EffectiveDate);

            RecurringChargeManager recurringChargeManager = new RecurringChargeManager();
            PackageManager packageManager = new PackageManager();
            PackageDefinitionManager packageDefinitionManager = new PackageDefinitionManager();
            FinancialAccountManager financialAccountManager = new Business.FinancialAccountManager();
            ChargeableEntityManager chargeableEntityManager = new Business.ChargeableEntityManager();

            Dictionary<DateTime, List<AccountPackageRecurCharge>> accountPackageRecurChargesByDate = new Dictionary<DateTime, List<AccountPackageRecurCharge>>();

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

                int numberOfDays = Convert.ToInt32(accountPackageRecurChargeData.EndChargePeriod.Subtract(accountPackageRecurChargeData.BeginChargePeriod).TotalDays);

                if (numberOfDays > 0)
                {
                    foreach (RecurringChargeEvaluatorOutput recurringChargeEvaluatorOutput in recurringChargeEvaluatorOutputs)
                    {
                        ChargeableEntitySettings chargeableEntitySettings = chargeableEntityManager.GetChargeableEntitySettings(recurringChargeEvaluatorOutput.ChargeableEntityId);
                        chargeableEntitySettings.ThrowIfNull("chargeableEntitySettings", recurringChargeEvaluatorOutput.ChargeableEntityId);
                        if (!chargeableEntitySettings.TransactionTypeId.HasValue)
                            throw new NullReferenceException(string.Format("chargeableEntitySettings.TransactionTypeId ChargeableEntityId:{0}", recurringChargeEvaluatorOutput.ChargeableEntityId));

                        decimal amountPerDay = recurringChargeEvaluatorOutput.Amount / numberOfDays;


                        DateTime chargeDay = accountPackageRecurChargeData.BeginChargePeriod;
                        while (chargeDay < accountPackageRecurChargeData.EndChargePeriod)
                        {
                            FinancialAccountRuntimeData financialAccountRuntimeData = financialAccountManager.GetAccountFinancialInfo(accountBEDefinitionId, accountPackageRecurChargeData.AccountPackage.AccountId, chargeDay);
                            AccountPackageRecurCharge accountPackageRecurCharge = new AccountPackageRecurCharge()
                            {
                                AccountPackageID = accountPackageRecurChargeData.AccountPackage.AccountPackageId,
                                BalanceAccountID = financialAccountRuntimeData != null ? financialAccountRuntimeData.BalanceAccountId : null,
                                BalanceAccountTypeID = financialAccountRuntimeData != null ? financialAccountRuntimeData.BalanceAccountTypeId : null,
                                ChargeableEntityID = recurringChargeEvaluatorOutput.ChargeableEntityId,
                                ChargeAmount = chargeDay >= recurringChargeEvaluatorOutput.ChargingStart && chargeDay < recurringChargeEvaluatorOutput.ChargingEnd ? amountPerDay : 0,
                                ChargeDay = chargeDay,
                                CurrencyID = recurringChargeEvaluatorOutput.CurrencyId,
                                TransactionTypeID = chargeableEntitySettings.TransactionTypeId.Value,
                                AccountID = accountPackageRecurChargeData.AccountPackage.AccountId,
                                AccountBEDefinitionId = accountBEDefinitionId
                            };
                            List<AccountPackageRecurCharge> accountPackageRecurChargeList = accountPackageRecurChargesByDate.GetOrCreateItem(accountPackageRecurCharge.ChargeDay, () => { return new List<AccountPackageRecurCharge>(); });
                            accountPackageRecurChargeList.Add(accountPackageRecurCharge);
                            chargeDay = chargeDay.AddDays(1);
                        }
                    }
                }
            }
            this.AccountPackageRecurChargesByDate.Set(context, accountPackageRecurChargesByDate);
            context.GetSharedInstanceData().WriteTrackingMessage(LogEntryType.Information, "Evaluate Reccuring Charge is done", null);
        }
    }
}