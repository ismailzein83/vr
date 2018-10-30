using System;
using System.Activities;
using System.Collections.Generic;
using Vanrise.Entities;
using Vanrise.Common;
using Vanrise.BusinessProcess;
using Retail.BusinessEntity.Entities;
using Retail.BusinessEntity.Business;
using Retail.BusinessEntity.MainExtensions.PackageTypes;
using System.Linq;

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
            AccountPackageRecurChargeManager accountPackageRecurChargeManager = new AccountPackageRecurChargeManager();

            Dictionary<DateTime, List<AccountPackageRecurCharge>> accountPackageRecurChargesByDate = new Dictionary<DateTime, List<AccountPackageRecurCharge>>();

            List<AccountPackageRecurChargePeriod> accountPackageRecurChargePeriods = new List<AccountPackageRecurChargePeriod>();
            List<AccountPackageRecurChargeDetails> accountPackageRecurChargeDetailsList = new List<AccountPackageRecurChargeDetails>();

            Dictionary<long, List<AccountPackageRecurCharge>> accountPackageRecurChargesByAccountPackage = null;

            HashSet<AccountDefinition> accountDefinitions = new HashSet<AccountDefinition>();

            foreach (AccountPackageRecurChargeData accountPackageRecurChargeData in accountPackageRecurChargeDataList)
            {
                AccountPackageRecurChargeDetails accountPackageRecurChargeDetails = BuildAccountPackageRecurChargeDetails(accountPackageRecurChargeData, packageManager, packageDefinitionManager);
                if (accountPackageRecurChargeDetails.RecurChargePackageSettings == null || accountPackageRecurChargeDetails.RecurChargePackageDefinitionSettings == null)
                    continue;
                AccountPackage accountPackage = accountPackageRecurChargeData.AccountPackage;

                accountDefinitions.Add(new Entities.AccountDefinition() { AccountBEDefinitionId = accountPackage.AccountBEDefinitionId, AccountId = accountPackage.AccountId });
                accountPackageRecurChargeDetailsList.Add(accountPackageRecurChargeDetails);
                object initializeData;

                DateTimeRange recurringChargePeriod = recurringChargeManager.InitializeRecurringCharge(accountPackageRecurChargeDetails.RecurChargePackageSettings.Evaluator, accountPackageRecurChargeData.BeginChargePeriod, accountPackageRecurChargeData.EndChargePeriod, accountPackageRecurChargeData.AccountPackage, out initializeData);
                accountPackageRecurChargeDetails.InitializeData = initializeData;

                if (recurringChargePeriod != null && recurringChargePeriod.From < accountPackageRecurChargeData.BeginChargePeriod)
                {
                    AccountPackageRecurChargePeriod accountPackageRecurChargePeriod = new Entities.AccountPackageRecurChargePeriod() { AccountPackageId = accountPackageRecurChargeData.AccountPackage.AccountPackageId, FromDate = recurringChargePeriod.From, ToDate = accountPackageRecurChargeData.BeginChargePeriod };
                    accountPackageRecurChargePeriods.Add(accountPackageRecurChargePeriod);
                }
            }

            if (accountPackageRecurChargePeriods.Count > 0)
                accountPackageRecurChargesByAccountPackage = accountPackageRecurChargeManager.GetAccountRecurringChargesByAccountPackage(accountPackageRecurChargePeriods);

            if (accountPackageRecurChargesByAccountPackage == null)
                accountPackageRecurChargesByAccountPackage = new Dictionary<long, List<AccountPackageRecurCharge>>();

            Dictionary<AccountDefinition, IOrderedEnumerable<AccountStatusHistory>> accountStatusHistoryListByAccountDefinition = new AccountStatusHistoryManager().GetAccountStatusHistoryListByAccountDefinition(accountDefinitions);

            foreach (AccountPackageRecurChargeDetails accountPackageRecurChargeDetails in accountPackageRecurChargeDetailsList)
            {
                AccountPackageRecurChargeData accountPackageRecurChargeData = accountPackageRecurChargeDetails.AccountPackageRecurChargeData;
                Guid accountBEDefinitionId = accountPackageRecurChargeData.AccountPackage.AccountBEDefinitionId;
                long accountId = accountPackageRecurChargeData.AccountPackage.AccountId;

                DateTime chargeDay = accountPackageRecurChargeData.BeginChargePeriod;
                while (chargeDay < accountPackageRecurChargeData.EndChargePeriod)
                {
                    List<RecurringChargeEvaluatorOutput> recurringChargeEvaluatorOutputs = recurringChargeManager.EvaluateRecurringCharge(accountPackageRecurChargeDetails.RecurChargePackageSettings.Evaluator, accountPackageRecurChargeDetails.RecurChargePackageDefinitionSettings.EvaluatorDefinitionSettings,
                    accountPackageRecurChargeData.BeginChargePeriod, accountPackageRecurChargeData.EndChargePeriod, accountBEDefinitionId, accountPackageRecurChargeData.AccountPackage, chargeDay, accountPackageRecurChargeDetails.InitializeData,
                    (accountPackageId, dateTimeRange) =>
                    {
                        return GetAccountPackageRecurCharges(accountPackageRecurChargesByAccountPackage, accountPackageId, dateTimeRange);
                    }, accountStatusHistoryListByAccountDefinition);

                    if (recurringChargeEvaluatorOutputs == null)
                        continue;

                    foreach (RecurringChargeEvaluatorOutput recurringChargeEvaluatorOutput in recurringChargeEvaluatorOutputs)
                    {
                        if (recurringChargeEvaluatorOutput.AmountPerDay > 0)
                        {
                            ChargeableEntitySettings chargeableEntitySettings = chargeableEntityManager.GetChargeableEntitySettings(recurringChargeEvaluatorOutput.ChargeableEntityId);
                            chargeableEntitySettings.ThrowIfNull("chargeableEntitySettings", recurringChargeEvaluatorOutput.ChargeableEntityId);


                            string classification = "Customer";

                            FinancialAccountRuntimeData financialAccountRuntimeData = financialAccountManager.GetAccountFinancialInfo(accountBEDefinitionId, accountPackageRecurChargeData.AccountPackage.AccountId, chargeDay, classification);
                            AccountPackageRecurCharge accountPackageRecurCharge = BuildAccountPackageRecurCharge(accountPackageRecurChargeData, accountBEDefinitionId, chargeDay, recurringChargeEvaluatorOutput, chargeableEntitySettings, financialAccountRuntimeData);

                            List<AccountPackageRecurCharge> dateAccountPackageRecurChargeList = accountPackageRecurChargesByDate.GetOrCreateItem(accountPackageRecurCharge.ChargeDay);
                            List<AccountPackageRecurCharge> accountPackageRecurChargeList = accountPackageRecurChargesByAccountPackage.GetOrCreateItem(accountPackageRecurCharge.AccountPackageID);

                            dateAccountPackageRecurChargeList.Add(accountPackageRecurCharge);
                            accountPackageRecurChargeList.Add(accountPackageRecurCharge);
                        }
                        chargeDay = chargeDay.AddDays(1);
                    }
                }
            }

            this.AccountPackageRecurChargesByDate.Set(context, accountPackageRecurChargesByDate);
            context.GetSharedInstanceData().WriteTrackingMessage(LogEntryType.Information, "Evaluate Reccuring Charge is done", null);
        }

        private AccountPackageRecurCharge BuildAccountPackageRecurCharge(AccountPackageRecurChargeData accountPackageRecurChargeData,
            Guid accountBEDefinitionId, DateTime chargeDay, RecurringChargeEvaluatorOutput recurringChargeEvaluatorOutput, ChargeableEntitySettings chargeableEntitySettings,
            FinancialAccountRuntimeData financialAccountRuntimeData)
        {
            AccountPackageRecurCharge accountPackageRecurCharge = new AccountPackageRecurCharge()
            {
                AccountPackageID = accountPackageRecurChargeData.AccountPackage.AccountPackageId,
                BalanceAccountID = financialAccountRuntimeData != null ? financialAccountRuntimeData.BalanceAccountId : null,
                BalanceAccountTypeID = financialAccountRuntimeData != null ? financialAccountRuntimeData.BalanceAccountTypeId : null,
                ChargeableEntityID = recurringChargeEvaluatorOutput.ChargeableEntityId,
                ChargeAmount = recurringChargeEvaluatorOutput.AmountPerDay,
                ChargeDay = chargeDay,
                CurrencyID = recurringChargeEvaluatorOutput.CurrencyId,
                TransactionTypeID = chargeableEntitySettings.TransactionTypeId,
                AccountID = accountPackageRecurChargeData.AccountPackage.AccountId,
                AccountBEDefinitionId = accountBEDefinitionId
            };
            return accountPackageRecurCharge;
        }

        private List<AccountPackageRecurCharge> GetAccountPackageRecurCharges(Dictionary<long, List<AccountPackageRecurCharge>> accountPackageRecurChargesByAccountPackage, int accountPackageId, DateTimeRange dateTimeRange)
        {
            List<AccountPackageRecurCharge> tempAccountPackageRecurCharge = accountPackageRecurChargesByAccountPackage.GetOrCreateItem(accountPackageId);
            if (tempAccountPackageRecurCharge == null)
                return null;

            IEnumerable<AccountPackageRecurCharge> matchingAccountPackageRecurCharges = tempAccountPackageRecurCharge.FindAllRecords(itm => itm.ChargeDay >= dateTimeRange.From && itm.ChargeDay < dateTimeRange.To);
            if (matchingAccountPackageRecurCharges == null)
                return null;

            return matchingAccountPackageRecurCharges.ToList();
        }

        private AccountPackageRecurChargeDetails BuildAccountPackageRecurChargeDetails(AccountPackageRecurChargeData accountPackageRecurChargeData, PackageManager packageManager, PackageDefinitionManager packageDefinitionManager)
        {
            Guid accountBEDefinitionId = accountPackageRecurChargeData.AccountPackage.AccountBEDefinitionId;
            Package package = packageManager.GetPackage(accountPackageRecurChargeData.AccountPackage.PackageId);
            package.ThrowIfNull("package", accountPackageRecurChargeData.AccountPackage.PackageId);
            package.Settings.ThrowIfNull("package.Settings", accountPackageRecurChargeData.AccountPackage.PackageId);

            PackageDefinition packageDefinition = packageDefinitionManager.GetPackageDefinitionById(package.Settings.PackageDefinitionId);
            packageDefinition.ThrowIfNull("packageDefinition", package.Settings.PackageDefinitionId);
            packageDefinition.Settings.ThrowIfNull("packageDefinition.Settings", package.Settings.PackageDefinitionId);

            RecurChargePackageDefinitionSettings recurChargePackageDefinitionSettings = packageDefinition.Settings.ExtendedSettings as RecurChargePackageDefinitionSettings;
            RecurChargePackageSettings recurChargePackageSettings = package.Settings.ExtendedSettings as RecurChargePackageSettings;

            return new EvaluateReccuringCharge.AccountPackageRecurChargeDetails()
            {
                AccountPackageRecurChargeData = accountPackageRecurChargeData,
                RecurChargePackageDefinitionSettings = recurChargePackageDefinitionSettings,
                RecurChargePackageSettings = recurChargePackageSettings
            };
        }

        private class AccountPackageRecurChargeDetails
        {
            public AccountPackageRecurChargeData AccountPackageRecurChargeData { get; set; }
            public RecurChargePackageDefinitionSettings RecurChargePackageDefinitionSettings { get; set; }
            public RecurChargePackageSettings RecurChargePackageSettings { get; set; }
            public Object InitializeData { get; set; }
        }
    }
}