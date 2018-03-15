using Retail.BusinessEntity.Business;
using Retail.BusinessEntity.Entities;
using Retail.BusinessEntity.MainExtensions.PackageTypes;
using Retail.MultiNet.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Analytic.Business;
using Vanrise.Analytic.Entities;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;
using Vanrise.GenericData.Pricing;
using Vanrise.GenericData.Transformation;
using Vanrise.Invoice.Business;
using Vanrise.Invoice.Business.Context;
using Vanrise.Invoice.Entities;
using Vanrise.NumberingPlan.Business;

namespace Retail.MultiNet.Business
{
    public enum TrafficDirection { InComming = 1, OutGoing = 2 }

    public class MultiNetSubscriberInvoiceGenerator : InvoiceGenerator
    {
        #region Fields

        private Guid _acountBEDefinitionId;
        private List<Guid> _salesTaxChargeableEntities;
        private List<Guid> _wHTaxChargeableEntities;
        private Guid _inComingChargeableEntity;
        private Guid _outGoingChargeableEntity;
        private Guid _otcChargeableEntity;
        private Guid _lineRentChargeableEntity;

        private Guid _salesTaxRuleDefinitionId;
        private Guid _wHTaxRuleDefinitionId;
        private Guid _latePaymentRuleDefinitionId;
        private Guid _mainDataRecordStorageId;

        private Guid _branchTypeId;
        private Guid _companyTypeId;

        private TaxRuleManager _taxRuleManager = new TaxRuleManager();
        private MappingRuleManager _mappingRuleManager = new MappingRuleManager();
        private GenericLKUPManager _genericLKUPManager = new GenericLKUPManager();
        private ServiceTypeManager _serviceTypeManager = new ServiceTypeManager();
        private AccountBEManager _accountBEManager = new AccountBEManager();
        private PackageManager _packageManager = new PackageManager();
        private AccountPackageManager _accountPackageManager = new AccountPackageManager();
        private PackageDefinitionManager _packageDefinitionManager = new PackageDefinitionManager();
        private AnalyticManager _analyticManager = new AnalyticManager();
        private FinancialAccountManager _financialAccountManager = new FinancialAccountManager();
        private GeneralSettingsManager _generalSettingsManager = new GeneralSettingsManager();
        private DataRecordStorageManager _dataRecordStorageManager = new DataRecordStorageManager();
        private CurrencyExchangeRateManager _currencyExchangeRateManager = new CurrencyExchangeRateManager();

        #endregion

        #region Constructors

        public MultiNetSubscriberInvoiceGenerator(Guid acountBEDefinitionId, List<Guid> salesTaxChargeableEntities, List<Guid> wHTaxChargeableEntities, Guid inComingChargeableEntity, Guid outGoingChargeableEntity, Guid salesTaxRuleDefinitionId, Guid wHTaxRuleDefinitionId, Guid latePaymentRuleDefinitionId, Guid mainDataRecordStorageId, Guid branchTypeId, Guid companyTypeId, Guid otcChargeableEntity, Guid lineRentChargeableEntity)
        {
            this._acountBEDefinitionId = acountBEDefinitionId;
            this._salesTaxChargeableEntities = salesTaxChargeableEntities;
            this._wHTaxChargeableEntities = wHTaxChargeableEntities;
            this._inComingChargeableEntity = inComingChargeableEntity;
            this._outGoingChargeableEntity = outGoingChargeableEntity;
            this._salesTaxRuleDefinitionId = salesTaxRuleDefinitionId;
            this._wHTaxRuleDefinitionId = wHTaxRuleDefinitionId;
            this._latePaymentRuleDefinitionId = latePaymentRuleDefinitionId;
            this._mainDataRecordStorageId = mainDataRecordStorageId;
            this._branchTypeId = branchTypeId;
            this._companyTypeId = companyTypeId;
            this._otcChargeableEntity = otcChargeableEntity;
            this._lineRentChargeableEntity = lineRentChargeableEntity;
        }

        #endregion

        public override void GenerateInvoice(IInvoiceGenerationContext context)
        {
            FinancialAccountData financialAccountData = _financialAccountManager.GetFinancialAccountData(_acountBEDefinitionId, context.PartnerId);

            if (context.FromDate < financialAccountData.FinancialAccount.BED || context.ToDate > financialAccountData.FinancialAccount.EED)
            {
                context.ErrorMessage = "From date and To date should be within the effective date of financial account.";
                context.GenerateInvoiceResult = GenerateInvoiceResult.Failed;
                return;

            }

            int currencyId = _accountBEManager.GetCurrencyId(this._acountBEDefinitionId, financialAccountData.Account.AccountId);

            var multiNetInvoiceGeneratorContext = new MultiNetInvoiceGeneratorContext()
            {
                IssueDate = context.IssueDate,
                FinancialAccount = financialAccountData.Account
            };

            var branchSummaries = new List<BranchSummary>();
            PartnerManager partnerManager = new PartnerManager();
            var excludeCDRsPart = partnerManager.GetInvoicePartnerSettingPart<IncludeCDRsInvoiceSettingPart>(context.InvoiceTypeId,context.PartnerId);
            bool includeCDRs = false;
            if (excludeCDRsPart != null)
            {
                includeCDRs = excludeCDRsPart.IncludeCDRs;
            }

            bool excludeSaleTaxes = false;
            bool excludeWHTaxes = false;

            var companyAccount = _accountBEManager.GetSelfOrParentAccountOfType(this._acountBEDefinitionId, financialAccountData.Account.AccountId, _companyTypeId);
            companyAccount.ThrowIfNull("companyAccount");
            companyAccount.Settings.ThrowIfNull("companyAccount.Settings");
            companyAccount.Settings.Parts.ThrowIfNull(" companyAccount.Settings.Parts");
            foreach (var accountPart in companyAccount.Settings.Parts)
            {
                var multiNetCompanyExtendedInfo = accountPart.Value.Settings as MultiNetCompanyExtendedInfo;
                if (multiNetCompanyExtendedInfo != null)
                {
                    excludeSaleTaxes = multiNetCompanyExtendedInfo.ExcludeSaleTaxes;
                    excludeWHTaxes = multiNetCompanyExtendedInfo.ExcludeWHTaxes;
                }
            }
            if (financialAccountData.Account.TypeId == this._branchTypeId)
            {

                var branchSummary = BuildBranchSummary(multiNetInvoiceGeneratorContext, financialAccountData.Account, currencyId, context.FromDate, context.ToDate, _branchTypeId, excludeSaleTaxes,excludeWHTaxes, includeCDRs);
                if (branchSummary != null)
                    branchSummaries.Add(branchSummary);
            }
            else if (financialAccountData.Account.TypeId == this._companyTypeId)
            {
                List<Account> childAccounts = _accountBEManager.GetChildAccounts(this._acountBEDefinitionId, financialAccountData.Account.AccountId, false);
                foreach (Account branchAccount in childAccounts)
                {
                    var branchSummary = BuildBranchSummary(multiNetInvoiceGeneratorContext, branchAccount, currencyId, context.FromDate, context.ToDate, branchAccount.TypeId, excludeSaleTaxes,excludeWHTaxes, includeCDRs);
                    if (branchSummary != null)
                        branchSummaries.Add(branchSummary);
                }
                if (branchSummaries.Count > 0)
                    BuildGeneratedBranchSummaryItemSet(multiNetInvoiceGeneratorContext, branchSummaries);
            }

            if (branchSummaries.Count > 0)
            {
                InvoiceDetails retailSubscriberInvoiceDetails = BuildGeneratedInvoiceDetails(branchSummaries, context.FromDate, context.ToDate, context.IssueDate, currencyId, financialAccountData.Account, multiNetInvoiceGeneratorContext.FinancialAccount.TypeId);
                if (retailSubscriberInvoiceDetails != null)
                    retailSubscriberInvoiceDetails.IncludeCDRs = includeCDRs;
                context.Invoice = new GeneratedInvoice
                {
                    InvoiceDetails = retailSubscriberInvoiceDetails,
                    InvoiceItemSets = multiNetInvoiceGeneratorContext.GeneratedInvoiceItemSets,
                };

                SetInvoiceBillingTransactions(context, retailSubscriberInvoiceDetails);
            }
            else
            {
                context.GenerateInvoiceResult = GenerateInvoiceResult.NoData;
                return;
            }
        }

        private void SetInvoiceBillingTransactions(IInvoiceGenerationContext context, InvoiceDetails invoiceDetails)
        {
            Vanrise.Invoice.Entities.InvoiceType invoiceType = new Vanrise.Invoice.Business.InvoiceTypeManager().GetInvoiceType(context.InvoiceTypeId);
            invoiceType.ThrowIfNull("invoiceType", context.InvoiceTypeId);
            invoiceType.Settings.ThrowIfNull("invoiceType.Settings", context.InvoiceTypeId);
            MultiNetSubscriberInvoiceSettings invoiceSettings = invoiceType.Settings.ExtendedSettings.CastWithValidate<MultiNetSubscriberInvoiceSettings>("invoiceType.Settings.ExtendedSettings");

            Guid accountTypeId = GetBillingTransactionAccountTypeId(context.InvoiceTypeId, context.PartnerId, context.IssueDate);

            var billingTransaction = new GeneratedInvoiceBillingTransaction()
            {
                AccountTypeId = accountTypeId,
                AccountId = context.PartnerId,
                TransactionTypeId = invoiceSettings.InvoiceTransactionTypeId,
                Amount = invoiceDetails.TotalCurrentCharges,
                CurrencyId = invoiceDetails.CurrencyId
            };

            billingTransaction.Settings = new GeneratedInvoiceBillingTransactionSettings();
            billingTransaction.Settings.UsageOverrides = new List<GeneratedInvoiceBillingTransactionUsageOverride>();

            foreach (Guid usageTransactionTypeId in invoiceSettings.UsageTransactionTypeIds)
            {
                billingTransaction.Settings.UsageOverrides.Add(new GeneratedInvoiceBillingTransactionUsageOverride()
                {
                    TransactionTypeId = usageTransactionTypeId
                });
            }
            AddRecurringChargeTransactionType(billingTransaction, _otcChargeableEntity);
            AddRecurringChargeTransactionType(billingTransaction, _lineRentChargeableEntity);

            context.BillingTransactions = new List<GeneratedInvoiceBillingTransaction>() { billingTransaction };
        }

        private void AddRecurringChargeTransactionType(GeneratedInvoiceBillingTransaction billingTransaction, Guid chargeableEntityId)
        {
            ChargeableEntityManager chargeableEntityManager = new ChargeableEntityManager();
            ChargeableEntitySettings chargeableEntitySettings = chargeableEntityManager.GetChargeableEntitySettings(chargeableEntityId);

            chargeableEntitySettings.ThrowIfNull("chargeableEntitySettings", chargeableEntityId);
            if (chargeableEntitySettings.TransactionTypeId.HasValue)
            {
                billingTransaction.Settings.UsageOverrides.Add(new GeneratedInvoiceBillingTransactionUsageOverride()
                {
                    TransactionTypeId = chargeableEntitySettings.TransactionTypeId.Value
                });
            }
        }

        private Guid GetBillingTransactionAccountTypeId(Guid invoiceTypeId, string accountId, DateTime effectiveOn)
        {
            var relationManager = new Vanrise.InvToAccBalanceRelation.Business.InvToAccBalanceRelationDefinitionManager();
            List<Vanrise.InvToAccBalanceRelation.Entities.BalanceAccountInfo> invoiceBalanceAccounts = relationManager.GetInvoiceBalanceAccounts(invoiceTypeId, accountId, effectiveOn);

            invoiceBalanceAccounts.ThrowIfNull("invoiceBalanceAccounts", accountId);
            if (invoiceBalanceAccounts.Count == 0)
                throw new Vanrise.Entities.DataIntegrityValidationException("invoiceBalanceAccounts.Count == 0");

            return invoiceBalanceAccounts.FirstOrDefault().AccountTypeId;
        }

        #region Build Branch Summary

        private BranchSummary BuildBranchSummary(MultiNetInvoiceGeneratorContext multiNetInvoiceGeneratorContext, Account account, int currencyId, DateTime fromDate, DateTime toDate, Guid branchTypeId, bool excludeSaleTaxes, bool excludeWHTaxes, bool includeCDRs)
        {

            BuildTrafficData(multiNetInvoiceGeneratorContext, account, currencyId, fromDate, toDate, branchTypeId, includeCDRs);
            AddRecurringChargeToBranchSummary(multiNetInvoiceGeneratorContext, account, currencyId, fromDate, toDate);
            if (multiNetInvoiceGeneratorContext.SummaryItemsByBranch != null && multiNetInvoiceGeneratorContext.SummaryItemsByBranch.Count > 0)
            {
                BuildGeneratedBranchItemSummaryItemSet(multiNetInvoiceGeneratorContext, account.AccountId);
                return BuildBranchSummary(multiNetInvoiceGeneratorContext, account, currencyId, excludeSaleTaxes, excludeWHTaxes);
            }
            return null;
        }
        private BranchSummary BuildBranchSummary(MultiNetInvoiceGeneratorContext multiNetInvoiceGeneratorContext, Account account, int currencyId, bool excludeSaleTaxes, bool excludeWHTaxes)
        {
            BranchSummary branchSummary = null;

            var summaryItems = multiNetInvoiceGeneratorContext.SummaryItemsByBranch.GetRecord(account.AccountId);
            if (summaryItems != null && summaryItems.Count > 0)
            {
                branchSummary = new BranchSummary();
                decimal saleAmount = 0;
                decimal whAmount = 0;
                foreach (var summaryItem in summaryItems)
                {
                    if (summaryItem.NetAmount != 0)
                    {
                        branchSummary.Quantity += summaryItem.Quantity;
                        branchSummary.CurrentCharges += summaryItem.NetAmount;
                       
                        if (this._salesTaxChargeableEntities.Contains(summaryItem.ChargeableEntityId))
                            saleAmount += summaryItem.NetAmount;
                        if (this._wHTaxChargeableEntities.Contains(summaryItem.ChargeableEntityId))
                            whAmount += summaryItem.NetAmount;
                       


                        if (summaryItem.ChargeableEntityId == this._otcChargeableEntity)
                            branchSummary.OTC += summaryItem.NetAmount;
                        if (summaryItem.ChargeableEntityId == this._lineRentChargeableEntity)
                            branchSummary.LineRent += summaryItem.NetAmount;
                        if (summaryItem.ChargeableEntityId == this._inComingChargeableEntity)
                        {
                            branchSummary.InComing += summaryItem.NetAmount;
                            branchSummary.IncomingDurationInSec += summaryItem.DurationInSec;
                        }
                        if (summaryItem.ChargeableEntityId == this._outGoingChargeableEntity)
                        {
                            branchSummary.OutGoing += summaryItem.NetAmount;
                            branchSummary.OutgoingDurationInSec += summaryItem.DurationInSec;
                        }
                    }
                   
                }
                if (!excludeSaleTaxes)
                {
                    decimal saleTaxPercentage = 0;
                    branchSummary.SalesTaxAmount = GetSaleTaxAmount(account, saleAmount, currencyId, multiNetInvoiceGeneratorContext.IssueDate, out saleTaxPercentage);
                    branchSummary.SalesTax = saleTaxPercentage;
                }

                if (!excludeWHTaxes)
                {
                    decimal whAmountSaleTaxPercentage = 0;
                    whAmount += GetSaleTaxAmount(account, whAmount, currencyId, multiNetInvoiceGeneratorContext.IssueDate, out whAmountSaleTaxPercentage);

                    decimal whTaxPercentage = 0;
                    branchSummary.WHTaxAmount = GetWHTaxAmount(account, whAmount, currencyId, multiNetInvoiceGeneratorContext.IssueDate, out whTaxPercentage);
                    branchSummary.WHTax = whTaxPercentage;

                }

                branchSummary.TotalCurrentCharges = branchSummary.CurrentCharges + branchSummary.WHTaxAmount + branchSummary.SalesTaxAmount;

                branchSummary.CurrencyId = currencyId;
                branchSummary.AccountId = account.AccountId;
            }
            return branchSummary;

        }
        private void BuildGeneratedBranchSummaryItemSet(MultiNetInvoiceGeneratorContext multiNetInvoiceGeneratorContext, List<BranchSummary> branchesSummaries)
        {

            if (multiNetInvoiceGeneratorContext.GeneratedInvoiceItemSets == null)
                multiNetInvoiceGeneratorContext.GeneratedInvoiceItemSets = new List<GeneratedInvoiceItemSet>();
            GeneratedInvoiceItemSet generatedSummaryItemSet = new GeneratedInvoiceItemSet();
            generatedSummaryItemSet.SetName = "BranchSummary";
            generatedSummaryItemSet.Items = new List<GeneratedInvoiceItem>();
            foreach (var item in branchesSummaries)
            {
                generatedSummaryItemSet.Items.Add(new GeneratedInvoiceItem
                {
                    Details = item,
                    Name = ""
                });
            }
            multiNetInvoiceGeneratorContext.GeneratedInvoiceItemSets.Add(generatedSummaryItemSet);
        }

        private void BuildTrafficData(MultiNetInvoiceGeneratorContext multiNetInvoiceGeneratorContext, Account account, int currencyId, DateTime fromDate, DateTime toDate, Guid branchTypeId, bool includeCDRs)
        {
            LoadTrafficData(multiNetInvoiceGeneratorContext, account.AccountId, fromDate, toDate, currencyId, branchTypeId);
            var billingSummaries = multiNetInvoiceGeneratorContext.BillingSummariesByBranch.GetOrCreateItem(account.AccountId);
            if (billingSummaries != null && billingSummaries.Count > 0)
            {
                Dictionary<string, UsageSummary> usagesSummariesBySubItemIdentifier = new Dictionary<string, UsageSummary>();
                BranchSummaryItem callAggregationSummary = new BranchSummaryItem
                {
                    AccountId = account.AccountId,

                };
                BranchSummaryItem outgoingCallsSummary = new BranchSummaryItem
                {
                    AccountId = account.AccountId
                };
                bool hasCallAggregations = false;
                bool hasOutgoingCall = false;
                foreach (var billingSummary in billingSummaries)
                {
                    if(billingSummary.Amount != 0)
                    {
                        string usageDescription = null;
                        string subItemIdentifier = null;
                        switch (billingSummary.TrafficDirection)
                        {
                            case TrafficDirection.InComming:
                                hasCallAggregations = true;
                                ModifySummaryItem(callAggregationSummary, billingSummary.CountCDRs, billingSummary.Amount, billingSummary.TotalDuration, this._inComingChargeableEntity);
                                subItemIdentifier = GetSubItemIdentifier(account.AccountId, null);
                                usageDescription = _genericLKUPManager.GetGenericLKUPItemName(this._inComingChargeableEntity);
                                AddUsageSummary(usagesSummariesBySubItemIdentifier, account.AccountId, subItemIdentifier, billingSummary.CountCDRs, billingSummary.Amount, billingSummary.TotalDuration, usageDescription);
                                break;
                            case TrafficDirection.OutGoing:
                                hasOutgoingCall = true;
                                ModifySummaryItem(outgoingCallsSummary, billingSummary.CountCDRs, billingSummary.Amount, billingSummary.TotalDuration, this._outGoingChargeableEntity);
                                subItemIdentifier = GetSubItemIdentifier(account.AccountId, billingSummary.ServiceTypeId);
                                usageDescription = _serviceTypeManager.GetServiceTypeName(billingSummary.ServiceTypeId);
                                AddUsageSummary(usagesSummariesBySubItemIdentifier, account.AccountId, subItemIdentifier, billingSummary.CountCDRs, billingSummary.Amount, billingSummary.TotalDuration, usageDescription);
                                break;
                        }
                    }
                }
                int normalPrecisionValue = _generalSettingsManager.GetNormalPrecisionValue();
                foreach (var usageSummary in usagesSummariesBySubItemIdentifier.Values)
                {
                    usageSummary.TotalDuration = usageSummary.TotalDuration;
                    usageSummary.NetAmount = Decimal.Round(usageSummary.NetAmount, normalPrecisionValue);
                }
                if (multiNetInvoiceGeneratorContext.SummaryItemsByBranch == null)
                    multiNetInvoiceGeneratorContext.SummaryItemsByBranch = new Dictionary<long, List<BranchSummaryItem>>();
                var summaryItems = multiNetInvoiceGeneratorContext.SummaryItemsByBranch.GetOrCreateItem(account.AccountId);

                if (hasCallAggregations)
                {
                    summaryItems.Add(callAggregationSummary);
                    callAggregationSummary.NetAmount = Decimal.Round(callAggregationSummary.NetAmount, normalPrecisionValue);
                }
                if (hasOutgoingCall)
                {
                    summaryItems.Add(outgoingCallsSummary);
                    outgoingCallsSummary.NetAmount = decimal.Round(outgoingCallsSummary.NetAmount, normalPrecisionValue);
                }
                BuildGeneratedUsageSummaryItemSet(multiNetInvoiceGeneratorContext, usagesSummariesBySubItemIdentifier, account.AccountId);
                if (includeCDRs)
                 LoadAndBuildUsageCDRs(multiNetInvoiceGeneratorContext, account.AccountId, currencyId, fromDate, toDate, branchTypeId);
            }
        }

        #region  Build Traffic Data

        private void BuildGeneratedBranchItemSummaryItemSet(MultiNetInvoiceGeneratorContext multiNetInvoiceGeneratorContext, long accountId)
        {
            if (multiNetInvoiceGeneratorContext.GeneratedInvoiceItemSets == null)
                multiNetInvoiceGeneratorContext.GeneratedInvoiceItemSets = new List<GeneratedInvoiceItemSet>();

            var summaryItems = multiNetInvoiceGeneratorContext.SummaryItemsByBranch.GetRecord(accountId);
            if (summaryItems != null && summaryItems.Count > 0)
            {
                GeneratedInvoiceItemSet generatedSummaryItemSet = new GeneratedInvoiceItemSet();
                generatedSummaryItemSet.SetName = string.Format("BranchChargeableItem_{0}", accountId);
                generatedSummaryItemSet.Items = new List<GeneratedInvoiceItem>();

                foreach (var summaryItem in summaryItems)
                {
                    if (summaryItem.NetAmount != 0)
                    {
                        generatedSummaryItemSet.Items.Add(new GeneratedInvoiceItem
                        {
                            Details = summaryItem,
                            Name = ""
                        });
                    }
                 
                }
                multiNetInvoiceGeneratorContext.GeneratedInvoiceItemSets.Add(generatedSummaryItemSet);
            }
        }
        private void BuildGeneratedUsageSummaryItemSet(MultiNetInvoiceGeneratorContext multiNetInvoiceGeneratorContext, Dictionary<string, UsageSummary> usagesSummariesBySubItemIdentifier, long accountId)
        {
            GeneratedInvoiceItemSet generatedSummaryItemSet = new GeneratedInvoiceItemSet();
            generatedSummaryItemSet.SetName = string.Format("BranchUsageSummary_{0}", accountId);
            generatedSummaryItemSet.Items = new List<GeneratedInvoiceItem>();
            foreach (var item in usagesSummariesBySubItemIdentifier)
            {
                generatedSummaryItemSet.Items.Add(new GeneratedInvoiceItem
                {
                    Details = item.Value,
                    Name = ""
                });
            }
            if (multiNetInvoiceGeneratorContext.GeneratedInvoiceItemSets == null)
                multiNetInvoiceGeneratorContext.GeneratedInvoiceItemSets = new List<GeneratedInvoiceItemSet>();
            multiNetInvoiceGeneratorContext.GeneratedInvoiceItemSets.Add(generatedSummaryItemSet);
        }
        private void LoadAndBuildUsageCDRs(MultiNetInvoiceGeneratorContext multiNetInvoiceGeneratorContext, long accountId, int currencyId, DateTime fromDate, DateTime toDate, Guid branchTypeId)
        {

            LoadAndStructureCDRData(multiNetInvoiceGeneratorContext, accountId, fromDate, toDate, branchTypeId);
            if (multiNetInvoiceGeneratorContext.BillingCDRByBranch != null)
            {
                var billingCDRs = multiNetInvoiceGeneratorContext.BillingCDRByBranch.GetRecord(accountId);
                if (billingCDRs != null && billingCDRs.Count > 0)
                {
                    var cdrsBySubItemIdentifier = new Dictionary<string, List<MultiNetCDR>>();
                    int normalPrecisionValue = _generalSettingsManager.GetNormalPrecisionValue();
                    foreach (var billingCDR in billingCDRs)
                    {
                        if(billingCDR.SaleAmount != 0)
                        {
                            MultiNetCDR multiNetCDR = new MultiNetCDR
                            {
                                AttemptDateTime = billingCDR.AttemptDateTime,
                                CalledNumber = billingCDR.CalledNumber,
                                CallingNumber = billingCDR.CallingNumber,
                                DurationInSeconds = billingCDR.DurationInSeconds,
                                DurationDescription = FormatDuration((Double)billingCDR.DurationInSeconds),
                                ZoneId = billingCDR.ZoneId,
                                OperatorName = billingCDR.OperatorName,
                                SaleCurrencyId = billingCDR.SaleCurrencyId
                            };
                            multiNetCDR.SaleAmount = multiNetCDR.SaleCurrencyId != currencyId ? _currencyExchangeRateManager.ConvertValueToCurrency(billingCDR.SaleAmount, multiNetCDR.SaleCurrencyId, currencyId, multiNetCDR.AttemptDateTime) : billingCDR.SaleAmount;
                            multiNetCDR.SaleAmount = Decimal.Round(multiNetCDR.SaleAmount, normalPrecisionValue);
                            string identifierName = null;
                            switch (billingCDR.TrafficDirection)
                            {
                                case TrafficDirection.InComming:
                                    identifierName = GetSubItemIdentifier(accountId, null);
                                    break;
                                case TrafficDirection.OutGoing:
                                    identifierName = GetSubItemIdentifier(accountId, billingCDR.ServiceTypeId);
                                    break;
                            }
                            multiNetCDR.SubItemIdentifier = identifierName;
                            List<MultiNetCDR> cdrs = cdrsBySubItemIdentifier.GetOrCreateItem(identifierName);
                            cdrs.Add(multiNetCDR);
                        }
                    }
                    BuildGeneratedUsageCDRsItemSet(multiNetInvoiceGeneratorContext, cdrsBySubItemIdentifier);
                }
            }
        }

        private string FormatDuration(double durationInSeconds)
        {
            TimeSpan timeSpan = TimeSpan.FromSeconds(durationInSeconds);
            return String.Format("{0}:{1:mm}:{1:ss}", (int)timeSpan.TotalHours, timeSpan);
        }

        #region  Load And Build Usage CDRs
        private void LoadAndStructureCDRData(MultiNetInvoiceGeneratorContext multiNetInvoiceGeneratorContext, long accountId, DateTime fromDate, DateTime toDate, Guid branchTypeId)
        {
            if (multiNetInvoiceGeneratorContext.BillingCDRByBranch == null)
            {
                var columns = new List<string> { "FinancialAccountId", "AttemptDateTime", "SaleDurationInSeconds", "Calling", "Called", "SaleAmount", "TrafficDirection", "ServiceType", "SubscriberAccountId", "Zone", "InterconnectOperator", "SaleCurrencyId" };
                var cdrData = _dataRecordStorageManager.GetFilteredDataRecords(new DataRetrievalInput<DataRecordQuery>
                {
                    Query = new DataRecordQuery()
                    {
                        DataRecordStorageIds = new List<Guid> { this._mainDataRecordStorageId },
                        Columns = columns,
                        ColumnTitles = columns,
                        FilterGroup = new RecordFilterGroup
                        {
                            Filters = new List<RecordFilter>
                       {
                           new NumberRecordFilter{
                       FieldName = "FinancialAccountId",
                       CompareOperator = NumberRecordFilterOperator.Equals,
                       Value = multiNetInvoiceGeneratorContext.FinancialAccount.AccountId,
                           }
                       }
                        },
                        LimitResult = int.MaxValue,
                        FromTime = fromDate,
                        ToTime = toDate,
                        SortColumns = new List<SortColumn> { new SortColumn { FieldName = "AttemptDateTime" } },
                    },
                    SortByColumnName = "FieldValues.AttemptDateTime.Value"
                }) as Vanrise.Entities.BigResult<DataRecordDetail>;
                if (cdrData != null && cdrData.Data != null)
                {
                    if (multiNetInvoiceGeneratorContext.BillingCDRByBranch == null)
                        multiNetInvoiceGeneratorContext.BillingCDRByBranch = new Dictionary<long, List<BillingCDR>>();
                    foreach (var dataRecordDetail in cdrData.Data)
                    {

                        DataRecordFieldValue saleAmount;
                        dataRecordDetail.FieldValues.TryGetValue("SaleAmount", out saleAmount);
                        decimal saleAmountValue = Convert.ToDecimal(saleAmount.Value ?? 0.0);
                        if(saleAmountValue != 0)
                        {
                            DataRecordFieldValue subscriberAccount;
                            dataRecordDetail.FieldValues.TryGetValue("SubscriberAccountId", out subscriberAccount);
                            var subscriberAccountId = Convert.ToInt64(subscriberAccount.Value);
                            var account = _accountBEManager.GetSelfOrParentAccountOfType(this._acountBEDefinitionId, subscriberAccountId, branchTypeId);
                            var billingCDRs = multiNetInvoiceGeneratorContext.BillingCDRByBranch.GetOrCreateItem(account.AccountId);

                            DataRecordFieldValue attemptDateTime;
                            dataRecordDetail.FieldValues.TryGetValue("AttemptDateTime", out attemptDateTime);

                            DataRecordFieldValue durationInSeconds;
                            dataRecordDetail.FieldValues.TryGetValue("SaleDurationInSeconds", out durationInSeconds);

                            DataRecordFieldValue calling;
                            dataRecordDetail.FieldValues.TryGetValue("Calling", out calling);

                            DataRecordFieldValue called;
                            dataRecordDetail.FieldValues.TryGetValue("Called", out called);



                            DataRecordFieldValue trafficDirection;
                            dataRecordDetail.FieldValues.TryGetValue("TrafficDirection", out trafficDirection);

                            DataRecordFieldValue serviceType;
                            dataRecordDetail.FieldValues.TryGetValue("ServiceType", out serviceType);

                            DataRecordFieldValue zone;
                            dataRecordDetail.FieldValues.TryGetValue("Zone", out zone);

                            DataRecordFieldValue interconnectOperator;
                            dataRecordDetail.FieldValues.TryGetValue("InterconnectOperator", out interconnectOperator);

                            DataRecordFieldValue saleCurrencyId;
                            dataRecordDetail.FieldValues.TryGetValue("SaleCurrencyId", out saleCurrencyId);

                            BillingCDR billingCDR = new BillingCDR
                            {
                                AttemptDateTime = Convert.ToDateTime(attemptDateTime.Value),
                                CallingNumber = Convert.ToString(calling.Value),
                                CalledNumber = Convert.ToString(called.Value),
                                SaleAmount = saleAmountValue,
                                DurationInSeconds = Convert.ToDecimal(durationInSeconds.Value ?? 0.0),
                                TrafficDirection = (TrafficDirection)trafficDirection.Value,
                                ZoneId = zone != null && zone.Value != null ? Convert.ToInt64(zone.Value) : default(long?),
                                OperatorName = interconnectOperator != null ? interconnectOperator.Description : null,
                                SaleCurrencyId = Convert.ToInt32(saleCurrencyId.Value),
                            };
                            Guid serviceTypeId;
                            if (Guid.TryParse(serviceType.Value.ToString(), out serviceTypeId))
                            {
                                billingCDR.ServiceTypeId = serviceTypeId;
                            }
                            billingCDRs.Add(billingCDR);
                        }
                       
                    }
                }
            }
        }
        private void BuildGeneratedUsageCDRsItemSet(MultiNetInvoiceGeneratorContext multiNetInvoiceGeneratorContext, Dictionary<string, List<MultiNetCDR>> cdrsBySubItemIdentifier)
        {
            if (multiNetInvoiceGeneratorContext.GeneratedInvoiceItemSets == null)
                multiNetInvoiceGeneratorContext.GeneratedInvoiceItemSets = new List<GeneratedInvoiceItemSet>();
            foreach (var subItemIdentifierEntity in cdrsBySubItemIdentifier)
            {
                GeneratedInvoiceItemSet cdrsItemSet = new GeneratedInvoiceItemSet();
                cdrsItemSet.Items = new List<GeneratedInvoiceItem>();
                cdrsItemSet.SetName = subItemIdentifierEntity.Key;
                foreach (var item in subItemIdentifierEntity.Value)
                {
                    if(item.SaleAmount != 0)
                    {
                        cdrsItemSet.Items.Add(new GeneratedInvoiceItem
                        {
                            Details = item,
                            Name = ""
                        });
                    }
                  
                }
                multiNetInvoiceGeneratorContext.GeneratedInvoiceItemSets.Add(cdrsItemSet);
            }
        }

        #endregion

        private void ModifySummaryItem(BranchSummaryItem summary, int countCDRs, decimal netAmount, Decimal totalDuration, Guid chargeableEntityId)
        {
            summary.Quantity += countCDRs;
            summary.DurationInSec += totalDuration;
            summary.NetAmount += netAmount;
            summary.ChargeableEntityId = chargeableEntityId;
            if (summary.UsageDescription == null)
                summary.UsageDescription = _genericLKUPManager.GetGenericLKUPItemName(chargeableEntityId);
        }
        private void AddUsageSummary(Dictionary<string, UsageSummary> usagesSummariesBySubItemIdentifier, long accountId, string subItemIdentifier, int countCDRs, decimal netAmount, Decimal totalDuration, string usageDescription)
        {
            UsageSummary usageSummary = usagesSummariesBySubItemIdentifier.GetOrCreateItem(subItemIdentifier, () =>
            {
                return new UsageSummary
                {
                    UsageDescription = usageDescription,
                    SubItemIdentifier = subItemIdentifier,
                    AccountId = accountId
                };
            });
            usageSummary.Quantity += countCDRs;
            usageSummary.NetAmount += netAmount;
            usageSummary.TotalDuration += totalDuration;
            usageSummary.TotalDurationDescription = FormatDuration((Double)usageSummary.TotalDuration);
        }




        #endregion

        private void AddRecurringChargeToBranchSummary(MultiNetInvoiceGeneratorContext multiNetInvoiceGeneratorContext, Account account, int currencyId, DateTime fromDate, DateTime toDate)
        {
            if (multiNetInvoiceGeneratorContext.SummaryItemsByBranch == null)
                multiNetInvoiceGeneratorContext.SummaryItemsByBranch = new Dictionary<long, List<BranchSummaryItem>>();

            var billingSummaryItems = multiNetInvoiceGeneratorContext.SummaryItemsByBranch.GetOrCreateItem(account.AccountId);

            AccountPackageRecurChargeManager accountPackageRecurChargeManager = new AccountPackageRecurChargeManager();
            List<AccountPackageRecurCharge> accountPackageRecurChargeList = accountPackageRecurChargeManager.GetAccountRecurringCharges(_acountBEDefinitionId, account.AccountId, fromDate.Date, toDate.Date);

            if (accountPackageRecurChargeList != null)
            {
                CurrencyExchangeRateManager currencyExchangeRateManager = new CurrencyExchangeRateManager();

                Dictionary<Guid, AmountData> amountDataByChargeableEntity = new Dictionary<Guid, AmountData>();
                foreach (AccountPackageRecurCharge accountPackageRecurCharge in accountPackageRecurChargeList)
                {
                    if(accountPackageRecurCharge.ChargeAmount != 0)
                    {
                        AmountData amountData = amountDataByChargeableEntity.GetOrCreateItem(accountPackageRecurCharge.ChargeableEntityID, () => { return new AmountData() { Amount = 0 }; });
                        decimal convertedAmount = currencyExchangeRateManager.ConvertValueToCurrency(accountPackageRecurCharge.ChargeAmount, accountPackageRecurCharge.CurrencyID, currencyId, accountPackageRecurCharge.ChargeDay);
                        amountData.Amount += convertedAmount;
                    }
                }
                foreach (var amountDataItem in amountDataByChargeableEntity)
                {
                    if(amountDataItem.Value.Amount != 0)
                    {
                        BranchSummaryItem branchSummary = new BranchSummaryItem()
                        {
                            AccountId = account.AccountId,
                            ChargeableEntityId = amountDataItem.Key,
                            UsageDescription = _genericLKUPManager.GetGenericLKUPItemName(amountDataItem.Key),
                            Quantity = 1,
                            NetAmount = amountDataItem.Value.Amount
                        };
                        billingSummaryItems.Add(branchSummary);
                    }
                    
                }
            }

            //var accountPackages = _accountPackageManager.GetAccountPackagesByAccountId(account.AccountId);
            //List<int> accountPackagesIds = new List<int>();
            //if (accountPackages != null && accountPackages.Count > 0)
            //{
            //    if (multiNetInvoiceGeneratorContext.SummaryItemsByBranch == null)
            //        multiNetInvoiceGeneratorContext.SummaryItemsByBranch = new Dictionary<long, List<BranchSummaryItem>>();

            //    var billingSummaryItems = multiNetInvoiceGeneratorContext.SummaryItemsByBranch.GetOrCreateItem(account.AccountId);
            //    RecurringChargeManager recurringChargeManager = new RecurringChargeManager();
            //    PackageDefinitionManager packageDefinitionManager = new PackageDefinitionManager();
            //    foreach (var accountPackage in accountPackages)
            //    {
            //        if (Vanrise.Common.Utilities.AreTimePeriodsOverlapped(accountPackage.BED, accountPackage.EED, fromDate, toDate.AddDays(1).Date))
            //        {
            //            var package = _packageManager.GetPackage(accountPackage.PackageId);
            //            var packageDefinition = packageDefinitionManager.GetPackageDefinitionById(package.Settings.PackageDefinitionId);
            //            var recurChargePackageDefinitionSettings = packageDefinition.Settings.ExtendedSettings as RecurChargePackageDefinitionSettings;
            //            var recurChargePackageSettings = package.Settings.ExtendedSettings as RecurChargePackageSettings;
            //            if (recurChargePackageSettings != null && recurChargePackageDefinitionSettings != null)
            //            {
            //                var evaluateRecurringCharge = recurringChargeManager.EvaluateRecurringCharge(recurChargePackageSettings.Evaluator, recurChargePackageDefinitionSettings.EvaluatorDefinitionSettings, fromDate, toDate.AddDays(1).Date, this._acountBEDefinitionId, accountPackage);
            //                if (evaluateRecurringCharge != null && evaluateRecurringCharge.Count > 0)
            //                {
            //                    foreach (var output in evaluateRecurringCharge)
            //                    {
            //                        BranchSummaryItem branchSummary = new BranchSummaryItem()
            //                        {
            //                            AccountId = account.AccountId,
            //                            ChargeableEntityId = output.ChargeableEntityId,
            //                            UsageDescription = _genericLKUPManager.GetGenericLKUPItemName(output.ChargeableEntityId),
            //                            Quantity = 1,
            //                            NetAmount = output.Amount
            //                        };
            //                        billingSummaryItems.Add(branchSummary);
            //                    }
            //                }
            //            }

            //        }
            //    }
            //}
        }
        private void LoadTrafficData(MultiNetInvoiceGeneratorContext multiNetInvoiceGeneratorContext, long branchAccountId, DateTime fromDate, DateTime toDate, int currencyId, Guid branchTypeId)
        {
            if (multiNetInvoiceGeneratorContext.BillingSummariesByBranch == null)
            {
                List<string> listMeasures = new List<string> { "Amount", "CountCDRs", "TotalSaleDurationInSeconds" };
                List<string> listDimensions = new List<string> { "TrafficDirection", "ServiceType" };
                if (multiNetInvoiceGeneratorContext.FinancialAccount.TypeId == this._companyTypeId)
                {
                    listDimensions.Add("Branch");
                }
                string dimensionFilterName = "FinancialAccountId";

                Vanrise.Entities.DataRetrievalInput<AnalyticQuery> analyticQuery = new DataRetrievalInput<AnalyticQuery>()
                {
                    Query = new AnalyticQuery()
                    {
                        DimensionFields = listDimensions,
                        MeasureFields = listMeasures,
                        TableId = Guid.Parse("4F4C1DC0-6024-4AB9-933D-20F456360112"),
                        FromTime = fromDate,
                        ToTime = toDate,
                        ParentDimensions = new List<string>(),
                        Filters = new List<DimensionFilter>(),
                        CurrencyId = currencyId
                    },
                    SortByColumnName = "DimensionValues[0].Name"
                };
                DimensionFilter dimensionFilter = new DimensionFilter()
                {
                    Dimension = dimensionFilterName,
                    FilterValues = new List<object> { multiNetInvoiceGeneratorContext.FinancialAccount.AccountId }
                };
                analyticQuery.Query.Filters.Add(dimensionFilter);
                var analyticData = _analyticManager.GetFilteredRecords(analyticQuery) as Vanrise.Analytic.Entities.AnalyticSummaryBigResult<AnalyticRecord>;
                if (analyticData != null && analyticData.Data != null)
                {
                    if (multiNetInvoiceGeneratorContext.BillingSummariesByBranch == null)
                        multiNetInvoiceGeneratorContext.BillingSummariesByBranch = new Dictionary<long, List<BillingSummary>>();
                    foreach (var analyticRecord in analyticData.Data)
                    {

                        DimensionValue trafficDirection = analyticRecord.DimensionValues.ElementAtOrDefault(0);
                        DimensionValue serviceTypeIdDim = analyticRecord.DimensionValues.ElementAtOrDefault(1);
                        DimensionValue branch = null;
                        long branchId = -1;
                        if (multiNetInvoiceGeneratorContext.FinancialAccount.TypeId == this._companyTypeId)
                        {
                            branch = analyticRecord.DimensionValues.ElementAtOrDefault(2);
                            branchId = Convert.ToInt64(branch.Value);
                        }
                        else
                        {
                            branchId = branchAccountId;
                        }

                        List<BillingSummary> billingSummaries = multiNetInvoiceGeneratorContext.BillingSummariesByBranch.GetOrCreateItem(branchId);

                        if (trafficDirection.Value != null && serviceTypeIdDim.Value != null)
                        {
                            MeasureValue amountMeasure = GetMeasureValue(analyticRecord, "Amount");
                            MeasureValue countCDRsMeasure = GetMeasureValue(analyticRecord, "CountCDRs");
                            MeasureValue totalDurationMeasure = GetMeasureValue(analyticRecord, "TotalSaleDurationInSeconds");
                            decimal amount = Convert.ToDecimal(amountMeasure.Value ?? 0.0);
                            if(amount  != 0)
                            {
                                int countCDRs = Convert.ToInt32(countCDRsMeasure.Value);
                                Decimal totalDuration = Convert.ToDecimal(totalDurationMeasure.Value);
                                BillingSummary billingSummary = new BillingSummary
                                {
                                    Amount = Convert.ToDecimal(amountMeasure.Value ?? 0.0),
                                    CountCDRs = Convert.ToInt32(countCDRsMeasure.Value),
                                    TotalDuration = totalDuration,

                                    TrafficDirection = (TrafficDirection)trafficDirection.Value,
                                };
                                Guid serviceTypeId;
                                if (Guid.TryParse(serviceTypeIdDim.Value.ToString(), out serviceTypeId))
                                {
                                    billingSummary.ServiceTypeId = serviceTypeId;
                                }
                                billingSummaries.Add(billingSummary);
                            }
                        }
                    }
                }
            }

        }

        #region Load Traffic Data
        private MeasureValue GetMeasureValue(AnalyticRecord analyticRecord, string measureName)
        {
            MeasureValue measureValue;
            analyticRecord.MeasureValues.TryGetValue(measureName, out measureValue);
            return measureValue;
        }

        #endregion

        private string GetSubItemIdentifier(long accountId, Guid? serviceTypeId)
        {
            string subItemIdentifier = "";
            if (serviceTypeId.HasValue)
            {
                subItemIdentifier = string.Format("BranchCDROut_{0}_{1}", serviceTypeId, accountId);
            }
            else
            {
                subItemIdentifier = string.Format("BranchCDRIN_{0}", accountId);
            }
            return subItemIdentifier;
        }

        #endregion

        #region Build Generated Invoice Details

        private Decimal GetSaleTaxAmount(Account account, decimal amount, int currencyId, DateTime issueDate, out decimal percentage)
        {
            GenericRuleTarget ruleTarget = new GenericRuleTarget
            {
                Objects = new Dictionary<string, dynamic> { { "Account", account } },
                EffectiveOn = issueDate
            };
            TaxRuleContext taxRuleContext = new TaxRuleContext
            {
                Amount = amount,
                CurrencyId = currencyId,
                // TargetTime = ,
            };
            _taxRuleManager.ApplyTaxRule(taxRuleContext, this._salesTaxRuleDefinitionId, ruleTarget);
            percentage = taxRuleContext.Percentage;
            return taxRuleContext.TaxAmount;
        }
        private Decimal GetLatePaymentCharges(Account account, decimal amount, int currencyId, DateTime issueDate)
        {
            GenericRuleTarget ruleTarget = new GenericRuleTarget
            {
                Objects = new Dictionary<string, dynamic> { { "Account", account } },
                EffectiveOn = issueDate
            };
            var matchRule = _mappingRuleManager.GetMatchRule(this._latePaymentRuleDefinitionId, ruleTarget);
            if (matchRule != null)
            {
                var percentage = Convert.ToDecimal(matchRule.Settings.Value ?? 0.0);

                return percentage > 0 ? (percentage * amount) / 100 : 0;
            }
            else
            {
                return 0;
            }
        }
        private Decimal GetWHTaxAmount(Account account, decimal amount, int currencyId, DateTime issueDate, out decimal percentage)
        {
            GenericRuleTarget ruleTarget = new GenericRuleTarget
            {
                Objects = new Dictionary<string, dynamic> { { "Account", account } },
                EffectiveOn = issueDate
            };
            TaxRuleContext taxRuleContext = new TaxRuleContext
            {
                Amount = amount,
                CurrencyId = currencyId,
                // TargetTime = ,
            };
            _taxRuleManager.ApplyTaxRule(taxRuleContext, this._wHTaxRuleDefinitionId, ruleTarget);
            percentage = taxRuleContext.Percentage;
            return taxRuleContext.TaxAmount;
        }
        private InvoiceDetails BuildGeneratedInvoiceDetails(List<BranchSummary> branchesSummary, DateTime fromDate, DateTime toDate, DateTime issueDate, int currencyId, Account account, Guid financialAccountTypeId)
        {
            InvoiceDetails retailSubscriberInvoiceDetails = null;
            if (branchesSummary != null)
            {
                retailSubscriberInvoiceDetails = new InvoiceDetails();
                foreach (var branchSummary in branchesSummary)
                {
                    retailSubscriberInvoiceDetails.Quantity += branchSummary.Quantity;
                    retailSubscriberInvoiceDetails.CurrentCharges += branchSummary.CurrentCharges;
                    retailSubscriberInvoiceDetails.TotalCurrentCharges += branchSummary.TotalCurrentCharges;

                    retailSubscriberInvoiceDetails.OTC += branchSummary.OTC;
                    retailSubscriberInvoiceDetails.LineRent += branchSummary.LineRent;
                    retailSubscriberInvoiceDetails.InComing += branchSummary.InComing;
                    retailSubscriberInvoiceDetails.OutGoing += branchSummary.OutGoing;
                    retailSubscriberInvoiceDetails.IncomingDurationInSec += branchSummary.IncomingDurationInSec;
                    retailSubscriberInvoiceDetails.OutgoingDurationInSec += branchSummary.OutgoingDurationInSec;
                    retailSubscriberInvoiceDetails.SalesTaxAmount += branchSummary.SalesTaxAmount;
                    retailSubscriberInvoiceDetails.WHTaxAmount += branchSummary.WHTaxAmount;
                    retailSubscriberInvoiceDetails.SalesTax += branchSummary.SalesTax;
                    retailSubscriberInvoiceDetails.WHTax += branchSummary.WHTax;
                }
                retailSubscriberInvoiceDetails.PayableByDueDate = retailSubscriberInvoiceDetails.TotalCurrentCharges;
                retailSubscriberInvoiceDetails.LatePaymentCharges = GetLatePaymentCharges(account, retailSubscriberInvoiceDetails.TotalCurrentCharges, currencyId, issueDate);
                retailSubscriberInvoiceDetails.PayableAfterDueDate = retailSubscriberInvoiceDetails.TotalCurrentCharges + retailSubscriberInvoiceDetails.LatePaymentCharges;
                retailSubscriberInvoiceDetails.CurrencyId = currencyId;
                retailSubscriberInvoiceDetails.AccountTypeId = financialAccountTypeId;

                if (financialAccountTypeId == this._companyTypeId)
                {
                    retailSubscriberInvoiceDetails.CompanyId = account.AccountId;
                }
                else if (financialAccountTypeId == this._branchTypeId)
                {
                    var parentAccount = _accountBEManager.GetParentAccount(account);
                    retailSubscriberInvoiceDetails.CompanyId = parentAccount.AccountId;
                    retailSubscriberInvoiceDetails.BranchId = account.AccountId;
                    foreach (var accountPart in account.Settings.Parts)
                    {
                        var multiNetBranchExtendedInfo = accountPart.Value.Settings as MultiNetBranchExtendedInfo;
                        if (multiNetBranchExtendedInfo != null)
                        {
                            retailSubscriberInvoiceDetails.BranchCode = multiNetBranchExtendedInfo.BranchCode;
                            retailSubscriberInvoiceDetails.ContractReferenceNumber = multiNetBranchExtendedInfo.ContractReferenceNumber;
                        }
                    }
                }



            }
            return retailSubscriberInvoiceDetails;
        }

        #endregion

        #region Private Classes

        private class MultiNetInvoiceGeneratorContext
        {
            public Account FinancialAccount { get; set; }
            public Dictionary<long, List<BillingSummary>> BillingSummariesByBranch { get; set; }
            public Dictionary<long, List<BillingCDR>> BillingCDRByBranch { get; set; }
            public Dictionary<long, List<BranchSummaryItem>> SummaryItemsByBranch { get; set; }
            public List<GeneratedInvoiceItemSet> GeneratedInvoiceItemSets { get; set; }

            public DateTime IssueDate { get; set; }
        }

        private class BillingSummary
        {
            public Decimal Amount { get; set; }
            public int CountCDRs { get; set; }
            public Decimal TotalDuration { get; set; }
            public TrafficDirection TrafficDirection { get; set; }
            public Guid ServiceTypeId { get; set; }

        }

        private class BillingCDR
        {
            public TrafficDirection TrafficDirection { get; set; }
            public Guid ServiceTypeId { get; set; }
            public DateTime AttemptDateTime { get; set; }
            public Decimal SaleAmount { get; set; }
            public String CalledNumber { get; set; }
            public String CallingNumber { get; set; }
            public Decimal DurationInSeconds { get; set; }
            public long? ZoneId { get; set; }
            public string OperatorName { get; set; }
            public int SaleCurrencyId { get; set; }
        }

        private class AmountData
        {
            public decimal Amount { get; set; }
        }

        #endregion
    }

    #region Public Classes

    public class BranchSummary : IInvoiceItemAdditionalFields
    {
        public decimal WHTax { get; set; }
        public decimal WHTaxAmount { get; set; }
        public decimal SalesTax { get; set; }
        public decimal SalesTaxAmount { get; set; }
        public int CurrencyId { get; set; }
        public int Quantity { get; set; }
        public decimal CurrentCharges { get; set; }
        public decimal TotalCurrentCharges { get; set; }
        public long AccountId { get; set; }
        public string BranchName { get; set; }


        public decimal OTC { get; set; }
        public decimal LineRent { get; set; }
        public decimal InComing { get; set; }
        public decimal OutGoing { get; set; }
        public decimal IncomingDurationInSec { get; set; }
        public decimal OutgoingDurationInSec { get; set; }


        static AccountBEManager s_accountBEManager = new AccountBEManager();
        public void FillAdditionalFields(IInvoiceItemAdditionalFieldsContext context)
        {
            context.InvoiceType.ThrowIfNull("context.InvoiceType");
            context.InvoiceType.Settings.ThrowIfNull("context.InvoiceType.Settings");
            MultiNetSubscriberInvoiceSettings multiNetSubscriberInvoiceSettings = context.InvoiceType.Settings.ExtendedSettings.CastWithValidate<MultiNetSubscriberInvoiceSettings>("context.InvoiceType.Settings.ExtendedSettings");
            this.BranchName = s_accountBEManager.GetAccountName(multiNetSubscriberInvoiceSettings.AccountBEDefinitionId, this.AccountId);
        }

    }

    public class BranchSummaryItem
    {
        public long AccountId { get; set; }
        public string UsageDescription { get; set; }
        public int Quantity { get; set; }
        public Decimal NetAmount { get; set; }
        public Guid ChargeableEntityId { get; set; }
        public decimal DurationInSec { get; set; }
    }

    public class UsageSummary
    {
        public long AccountId { get; set; }
        public string SubItemIdentifier { get; set; }
        public string UsageDescription { get; set; }
        public int Quantity { get; set; }
        public Decimal TotalDuration { get; set; }
        public string TotalDurationDescription { get; set; }
        public Decimal NetAmount { get; set; }
    }

    public class MultiNetCDR : IInvoiceItemAdditionalFields
    {
        public string SubItemIdentifier { get; set; }
        public DateTime AttemptDateTime { get; set; }

        public string FormattedAttemptDate
        {
            get
            {
                return this.AttemptDateTime.ToString("yyyy-MM-dd");
            }
            set
            {
            }
        }

        public string FormattedAttemptTime
        {
            get
            {
                return this.AttemptDateTime.ToString("HH:mm:ss");
            }
            set
            {
            }
        }

        public decimal DurationInSeconds { get; set; }
        public string DurationDescription { get; set; }
        public string CallingNumber { get; set; }
        public string CalledNumber { get; set; }
        public decimal SaleAmount { get; set; }

        public long? ZoneId { get; set; }
        public string OperatorName { get; set; }
        public string ZoneName { get; set; }
        public int SaleCurrencyId { get; set; }
        public string SaleCurrencyName { get; set; }

        static SaleZoneManager s_saleZoneManager = new SaleZoneManager();
        static CurrencyManager s_currencyManager = new CurrencyManager();
        public void FillAdditionalFields(IInvoiceItemAdditionalFieldsContext context)
        {
            if (this.OperatorName != null)
            {
                this.ZoneName = this.OperatorName;
            }
            else if (this.ZoneId.HasValue)
            {
                this.ZoneName = s_saleZoneManager.GetSaleZoneName(this.ZoneId.Value);
            }

            this.SaleCurrencyName = s_currencyManager.GetCurrencySymbol(this.SaleCurrencyId);

        }
    }

    #endregion
}
