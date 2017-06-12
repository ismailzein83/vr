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
using Vanrise.Common.Business;
using Vanrise.Entities;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;
using Vanrise.GenericData.Pricing;
using Vanrise.GenericData.Transformation;
using Vanrise.Invoice.Entities;
using Vanrise.Common;
using Vanrise.NumberingPlan.Business;
namespace Retail.MultiNet.Business
{
    public enum TrafficDirection { InComming = 1 , OutGoing = 2 }
    public class MultiNetSubscriberInvoiceGenerator : InvoiceGenerator
    {
        Guid _acountBEDefinitionId;
        List<Guid> _salesTaxChargeableEntities { get; set; }
        List<Guid> _wHTaxChargeableEntities { get; set; }
        Guid _inComingChargeableEntity { get; set; }
        Guid _outGoingChargeableEntity { get; set; }
        Guid _salesTaxRuleDefinitionId { get; set; }
        Guid _wHTaxRuleDefinitionId { get; set; }
        Guid _latePaymentRuleDefinitionId { get; set; }
        Guid _mainDataRecordStorageId { get; set; }

        Guid _branchTypeId { get; set; }
        Guid _companyTypeId { get; set; }


        TaxRuleManager _taxRuleManager = new TaxRuleManager();
        MappingRuleManager _mappingRuleManager = new MappingRuleManager();
        GenericLKUPManager _genericLKUPManager = new GenericLKUPManager();
        ServiceTypeManager _serviceTypeManager = new ServiceTypeManager();
        AccountBEManager _accountBEManager = new AccountBEManager();
        PackageManager _packageManager = new PackageManager();
        AccountPackageManager _accountPackageManager = new AccountPackageManager(); 
        PackageDefinitionManager _packageDefinitionManager = new PackageDefinitionManager();
        AnalyticManager _analyticManager = new AnalyticManager();
        FinancialAccountManager _financialAccountManager = new FinancialAccountManager();
        GeneralSettingsManager _generalSettingsManager = new GeneralSettingsManager();
        DataRecordStorageManager _dataRecordStorageManager = new DataRecordStorageManager();
        CurrencyExchangeRateManager _currencyExchangeRateManager = new CurrencyExchangeRateManager();
        public MultiNetSubscriberInvoiceGenerator(Guid acountBEDefinitionId, List<Guid> salesTaxChargeableEntities, List<Guid> wHTaxChargeableEntities, Guid inComingChargeableEntity, Guid outGoingChargeableEntity, Guid salesTaxRuleDefinitionId, Guid wHTaxRuleDefinitionId, Guid latePaymentRuleDefinitionId, Guid mainDataRecordStorageId,Guid branchTypeId,Guid companyTypeId)
        {
            this._acountBEDefinitionId = acountBEDefinitionId;
            this._salesTaxChargeableEntities = salesTaxChargeableEntities;
            this._wHTaxChargeableEntities = wHTaxChargeableEntities ;
            this._inComingChargeableEntity = inComingChargeableEntity;
            this._outGoingChargeableEntity = outGoingChargeableEntity;
            this._salesTaxRuleDefinitionId = salesTaxRuleDefinitionId;
            this._wHTaxRuleDefinitionId = wHTaxRuleDefinitionId;
            this._latePaymentRuleDefinitionId = latePaymentRuleDefinitionId;
            this._mainDataRecordStorageId = mainDataRecordStorageId;
            this._branchTypeId = branchTypeId;
            this._companyTypeId = companyTypeId;
        }

        public override void GenerateInvoice(IInvoiceGenerationContext context)
        {

            var financialAccountData = _financialAccountManager.GetFinancialAccountData(_acountBEDefinitionId, context.PartnerId);
            if (context.FromDate < financialAccountData.FinancialAccount.BED || context.ToDate > financialAccountData.FinancialAccount.EED)
            {
                throw new InvoiceGeneratorException("From date and To date should be within the effective date of financial account.");
            }
            int currencyId = _accountBEManager.GetCurrencyId(this._acountBEDefinitionId, financialAccountData.Account.AccountId);
            MultiNetInvoiceGeneratorContext multiNetInvoiceGeneratorContext = new MultiNetInvoiceGeneratorContext();
            multiNetInvoiceGeneratorContext.IssueDate = context.IssueDate;
            multiNetInvoiceGeneratorContext.FinancialAccount = financialAccountData.Account;
            List<BranchSummary> branchesSummary = new List<BranchSummary>();
            if (financialAccountData.Account.TypeId == this._branchTypeId)
            {
               var branchSummary = BuildBranchSummary(multiNetInvoiceGeneratorContext, financialAccountData.Account, currencyId, context.FromDate, context.GeneratedToDate, _branchTypeId);
               if (branchSummary != null)
                   branchesSummary.Add(branchSummary);
                branchesSummary.Add(branchSummary);
            }
            else if (financialAccountData.Account.TypeId == this._companyTypeId)
            {
                var childAccounts = _accountBEManager.GetChildAccounts(this._acountBEDefinitionId, financialAccountData.Account.AccountId, false);
                foreach(var branchAccount in childAccounts)
                {
                    var branchSummary = BuildBranchSummary(multiNetInvoiceGeneratorContext, branchAccount, currencyId, context.FromDate, context.GeneratedToDate, branchAccount.TypeId);
                    if (branchSummary != null)
                     branchesSummary.Add(branchSummary);
                }
                if (branchesSummary .Count > 0)
                {
                    BuildGeneratedBranchSummaryItemSet(multiNetInvoiceGeneratorContext, branchesSummary);
                }
            }
            if(branchesSummary.Count > 0)
            {
                InvoiceDetails retailSubscriberInvoiceDetails = BuildGeneratedInvoiceDetails(branchesSummary, context.FromDate, context.ToDate, context.IssueDate, currencyId, financialAccountData.Account, multiNetInvoiceGeneratorContext.FinancialAccount.TypeId);
               
                context.Invoice = new GeneratedInvoice
                {
                    InvoiceDetails = retailSubscriberInvoiceDetails,
                    InvoiceItemSets = multiNetInvoiceGeneratorContext.GeneratedInvoiceItemSets,
                };
            }else
            {
                throw new InvoiceGeneratorException("No data available between the selected period.");
            }
        }

        #region Build Branch Summary
        private BranchSummary BuildBranchSummary(MultiNetInvoiceGeneratorContext multiNetInvoiceGeneratorContext, Account account, int currencyId, DateTime fromDate, DateTime toDate, Guid branchTypeId)
        {

            BuildTrafficData(multiNetInvoiceGeneratorContext, account, currencyId, fromDate, toDate, branchTypeId);
            AddRecurringChargeToBranchSummary(multiNetInvoiceGeneratorContext, account, currencyId, fromDate, toDate);
            if (multiNetInvoiceGeneratorContext.SummaryItemsByBranch != null && multiNetInvoiceGeneratorContext.SummaryItemsByBranch.Count > 0)
            {
                BuildGeneratedBranchItemSummaryItemSet(multiNetInvoiceGeneratorContext, account.AccountId);
                return BuildBranchSummary(multiNetInvoiceGeneratorContext, account, currencyId);
            }
            return null;
        }
        private  BranchSummary  BuildBranchSummary(MultiNetInvoiceGeneratorContext multiNetInvoiceGeneratorContext, Account account,int currencyId )
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
                    branchSummary.Quantity += summaryItem.Quantity;
                    branchSummary.CurrentCharges += summaryItem.NetAmount;
                    if (this._salesTaxChargeableEntities.Contains(summaryItem.ChargeableEntityId))
                        saleAmount += summaryItem.NetAmount;
                    if (this._wHTaxChargeableEntities.Contains(summaryItem.ChargeableEntityId))
                        whAmount += summaryItem.NetAmount;
                }
                decimal whAmountSaleTaxPercentage = 0;
                whAmount += GetSaleTaxAmount(account, whAmount, currencyId, multiNetInvoiceGeneratorContext.IssueDate, out whAmountSaleTaxPercentage);

                decimal saleTaxPercentage = 0;
                branchSummary.SalesTaxAmount = GetSaleTaxAmount(account, saleAmount, currencyId, multiNetInvoiceGeneratorContext.IssueDate, out saleTaxPercentage);
                branchSummary.SalesTax = saleTaxPercentage;

                decimal whTaxPercentage = 0;
                branchSummary.WHTaxAmount = GetWHTaxAmount(account, whAmount, currencyId, multiNetInvoiceGeneratorContext.IssueDate, out whTaxPercentage);
                branchSummary.WHTax = whTaxPercentage;

                branchSummary.TotalCurrentCharges = branchSummary.CurrentCharges + branchSummary.WHTaxAmount + branchSummary.SalesTaxAmount;

                branchSummary.CurrencyId = currencyId;
                branchSummary.AccountId = account.AccountId;
            }
            return branchSummary;

        }
        private void BuildGeneratedBranchSummaryItemSet(MultiNetInvoiceGeneratorContext multiNetInvoiceGeneratorContext,List<BranchSummary> branchesSummaries)
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

        private void BuildTrafficData(MultiNetInvoiceGeneratorContext multiNetInvoiceGeneratorContext, Account account, int currencyId, DateTime fromDate, DateTime toDate, Guid branchTypeId)
        {
            LoadTrafficData(multiNetInvoiceGeneratorContext, account.AccountId, fromDate, toDate, currencyId, branchTypeId);
            var billingSummaries = multiNetInvoiceGeneratorContext.BillingSummariesByBranch.GetOrCreateItem(account.AccountId);
            if (billingSummaries != null && billingSummaries.Count > 0)
            {
                Dictionary<string, UsageSummary> usagesSummariesBySubItemIdentifier = new Dictionary<string, UsageSummary>();
                BranchSummaryItem callAggregationSummary = new BranchSummaryItem
                {
                    AccountId = account.AccountId
                };
                BranchSummaryItem outgoingCallsSummary = new BranchSummaryItem
                {
                    AccountId = account.AccountId
                };
                foreach (var billingSummary in billingSummaries)
                {
                    string usageDescription = null;
                    string subItemIdentifier = null;
                    switch (billingSummary.TrafficDirection)
                    {
                        case TrafficDirection.InComming:
                            ModifySummaryItem(callAggregationSummary, billingSummary.CountCDRs, billingSummary.Amount, billingSummary.TotalDuration, this._inComingChargeableEntity);
                            subItemIdentifier = GetSubItemIdentifier(account.AccountId, null);
                            usageDescription = _genericLKUPManager.GetGenericLKUPItemName(this._inComingChargeableEntity);
                            AddUsageSummary(usagesSummariesBySubItemIdentifier, account.AccountId, subItemIdentifier, billingSummary.CountCDRs, billingSummary.Amount, billingSummary.TotalDuration, usageDescription);
                            break;
                        case TrafficDirection.OutGoing:
                            ModifySummaryItem(outgoingCallsSummary, billingSummary.CountCDRs, billingSummary.Amount, billingSummary.TotalDuration, this._outGoingChargeableEntity);
                            subItemIdentifier = GetSubItemIdentifier(account.AccountId, billingSummary.ServiceTypeId);
                            usageDescription = _serviceTypeManager.GetServiceTypeName(billingSummary.ServiceTypeId);
                            AddUsageSummary(usagesSummariesBySubItemIdentifier, account.AccountId, subItemIdentifier, billingSummary.CountCDRs, billingSummary.Amount, billingSummary.TotalDuration, usageDescription);
                            break;
                    }
                }
                if (multiNetInvoiceGeneratorContext.SummaryItemsByBranch == null)
                    multiNetInvoiceGeneratorContext.SummaryItemsByBranch = new Dictionary<long, List<BranchSummaryItem>>();
                var summaryItems = multiNetInvoiceGeneratorContext.SummaryItemsByBranch.GetOrCreateItem(account.AccountId);
                summaryItems.Add(callAggregationSummary);
                summaryItems.Add(outgoingCallsSummary);
                BuildGeneratedUsageSummaryItemSet(multiNetInvoiceGeneratorContext, usagesSummariesBySubItemIdentifier, account.AccountId);
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
                    generatedSummaryItemSet.Items.Add(new GeneratedInvoiceItem
                    {
                        Details = summaryItem,
                        Name = ""
                    });
                }
                multiNetInvoiceGeneratorContext.GeneratedInvoiceItemSets.Add(generatedSummaryItemSet);
            }
        }
        private void BuildGeneratedUsageSummaryItemSet(MultiNetInvoiceGeneratorContext multiNetInvoiceGeneratorContext, Dictionary<string, UsageSummary> usagesSummariesBySubItemIdentifier,long accountId)
        {
            GeneratedInvoiceItemSet generatedSummaryItemSet = new GeneratedInvoiceItemSet();
            generatedSummaryItemSet.SetName = string.Format("BranchUsageSummary_{0}",accountId);
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
        private void LoadAndBuildUsageCDRs(MultiNetInvoiceGeneratorContext multiNetInvoiceGeneratorContext, long accountId,int currencyId, DateTime fromDate, DateTime toDate, Guid branchTypeId)
        {

            LoadAndStructureCDRData(multiNetInvoiceGeneratorContext, accountId, fromDate, toDate, branchTypeId);
            if (multiNetInvoiceGeneratorContext.BillingCDRByBranch != null)
            {
                var cdrsBySubItemIdentifier = new Dictionary<string, List<MultiNetCDR>>();
                var billingCDRs = multiNetInvoiceGeneratorContext.BillingCDRByBranch.GetRecord(accountId);
                if (billingCDRs != null && billingCDRs.Count > 0)
                {
                    foreach (var billingCDR in billingCDRs)
                    {

                        MultiNetCDR multiNetCDR = new MultiNetCDR
                        {
                            AttemptDateTime = billingCDR.AttemptDateTime,
                            CalledNumber = billingCDR.CalledNumber,
                            CallingNumber = billingCDR.CallingNumber,
                            DurationInSeconds = billingCDR.DurationInSeconds,
                            ZoneId = billingCDR.ZoneId,
                            SaleCurrencyId = billingCDR.SaleCurrencyId
                        };
                        multiNetCDR.SaleAmount = multiNetCDR.SaleCurrencyId != currencyId ? _currencyExchangeRateManager.ConvertValueToCurrency(billingCDR.SaleAmount, multiNetCDR.SaleCurrencyId, currencyId, multiNetCDR.AttemptDateTime) : billingCDR.SaleAmount;

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
                    BuildGeneratedUsageCDRsItemSet(multiNetInvoiceGeneratorContext, cdrsBySubItemIdentifier);
                }
            }
        }

        #region  Load And Build Usage CDRs
        private void LoadAndStructureCDRData(MultiNetInvoiceGeneratorContext multiNetInvoiceGeneratorContext, long accountId, DateTime fromDate, DateTime toDate, Guid branchTypeId)
        {
            if (multiNetInvoiceGeneratorContext.BillingCDRByBranch == null)
            {
                var columns = new List<string> { "FinancialAccountId", "AttemptDateTime", "DurationInSeconds", "Calling", "Called", "SaleAmount", "TrafficDirection", "ServiceType", "SubscriberAccountId", "Zone", "SaleCurrencyId" };
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

                        DataRecordFieldValue subscriberAccount;
                        dataRecordDetail.FieldValues.TryGetValue("SubscriberAccountId", out subscriberAccount);
                        var subscriberAccountId = Convert.ToInt64(subscriberAccount.Value);
                        var account = _accountBEManager.GetSelfOrParentAccountOfType(this._acountBEDefinitionId, subscriberAccountId, branchTypeId);
                        var billingCDRs = multiNetInvoiceGeneratorContext.BillingCDRByBranch.GetOrCreateItem(account.AccountId);

                        DataRecordFieldValue attemptDateTime;
                        dataRecordDetail.FieldValues.TryGetValue("AttemptDateTime", out attemptDateTime);

                        DataRecordFieldValue durationInSeconds;
                        dataRecordDetail.FieldValues.TryGetValue("DurationInSeconds", out durationInSeconds);

                        DataRecordFieldValue calling;
                        dataRecordDetail.FieldValues.TryGetValue("Calling", out calling);

                        DataRecordFieldValue called;
                        dataRecordDetail.FieldValues.TryGetValue("Called", out called);

                        DataRecordFieldValue saleAmount;
                        dataRecordDetail.FieldValues.TryGetValue("SaleAmount", out saleAmount);

                        DataRecordFieldValue trafficDirection;
                        dataRecordDetail.FieldValues.TryGetValue("TrafficDirection", out trafficDirection);

                        DataRecordFieldValue serviceType;
                        dataRecordDetail.FieldValues.TryGetValue("ServiceType", out serviceType);
                       
                        DataRecordFieldValue zone;
                        dataRecordDetail.FieldValues.TryGetValue("Zone", out zone);

                        DataRecordFieldValue saleCurrencyId;
                        dataRecordDetail.FieldValues.TryGetValue("SaleCurrencyId", out saleCurrencyId);

                        BillingCDR billingCDR = new BillingCDR
                        {
                            AttemptDateTime = Convert.ToDateTime(attemptDateTime.Value),
                            CallingNumber = Convert.ToString(calling.Value),
                            CalledNumber = Convert.ToString(called.Value),
                            SaleAmount = Convert.ToDecimal(saleAmount.Value ?? 0.0),
                            DurationInSeconds = Convert.ToDecimal(durationInSeconds.Value ?? 0.0),
                            TrafficDirection = (TrafficDirection)trafficDirection.Value,
                            ZoneId = zone!= null && zone.Value != null? Convert.ToInt64(zone.Value):default(long?),
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
                    cdrsItemSet.Items.Add(new GeneratedInvoiceItem
                    {
                        Details = item,
                        Name = ""
                    });
                }
                multiNetInvoiceGeneratorContext.GeneratedInvoiceItemSets.Add(cdrsItemSet);
            }
        }

        #endregion

        private void ModifySummaryItem(BranchSummaryItem summary, int countCDRs, decimal netAmount, int totalDuration, Guid chargeableEntityId)
        {
            summary.Quantity += countCDRs;
            summary.NetAmount += netAmount;
            summary.ChargeableEntityId = chargeableEntityId;
            if (summary.UsageDescription == null)
                summary.UsageDescription = _genericLKUPManager.GetGenericLKUPItemName(chargeableEntityId);
        }
        private void AddUsageSummary(Dictionary<string, UsageSummary> usagesSummariesBySubItemIdentifier,long accountId, string subItemIdentifier, int countCDRs, decimal netAmount, int totalDuration, string usageDescription)
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
        }




        #endregion
       
        private void AddRecurringChargeToBranchSummary(MultiNetInvoiceGeneratorContext multiNetInvoiceGeneratorContext, Account account, int currencyId, DateTime fromDate, DateTime toDate)
        {
           
            var accountPackages = _accountPackageManager.GetAccountPackagesByAccountId(account.AccountId);
            List<int> accountPackagesIds = new List<int>();
            if (accountPackages != null && accountPackages.Count > 0)
            {
                if (multiNetInvoiceGeneratorContext.SummaryItemsByBranch == null)
                    multiNetInvoiceGeneratorContext.SummaryItemsByBranch = new Dictionary<long, List<BranchSummaryItem>>();

                var billingSummaryItems = multiNetInvoiceGeneratorContext.SummaryItemsByBranch.GetOrCreateItem(account.AccountId);
                RecurringChargeManager recurringChargeManager = new RecurringChargeManager();
                PackageDefinitionManager packageDefinitionManager = new PackageDefinitionManager();
                foreach (var accountPackage in accountPackages)
                {
                    if (Vanrise.Common.Utilities.AreTimePeriodsOverlapped(accountPackage.BED, accountPackage.EED, fromDate, toDate))
                    {
                        var package = _packageManager.GetPackage(accountPackage.PackageId);
                        var packageDefinition = packageDefinitionManager.GetPackageDefinitionById(package.Settings.PackageDefinitionId);
                        var recurChargePackageDefinitionSettings = packageDefinition.Settings.ExtendedSettings as RecurChargePackageDefinitionSettings;
                        var recurChargePackageSettings = package.Settings.ExtendedSettings as RecurChargePackageSettings;
                        if (recurChargePackageSettings != null && recurChargePackageDefinitionSettings != null)
                        {
                            var evaluateRecurringCharge = recurringChargeManager.EvaluateRecurringCharge(recurChargePackageSettings.Evaluator, recurChargePackageDefinitionSettings.EvaluatorDefinitionSettings, fromDate, toDate, this._acountBEDefinitionId, accountPackage);
                            if (evaluateRecurringCharge != null && evaluateRecurringCharge.Count > 0)
                            {
                                foreach (var output in evaluateRecurringCharge)
                                {
                                    BranchSummaryItem branchSummary = new BranchSummaryItem()
                                    {
                                        AccountId = account.AccountId,
                                        ChargeableEntityId = output.ChargeableEntityId,
                                        UsageDescription = _genericLKUPManager.GetGenericLKUPItemName(output.ChargeableEntityId),
                                        Quantity = 1,
                                        NetAmount = output.Amount
                                    };
                                    billingSummaryItems.Add(branchSummary);
                                }
                            }
                        }

                    }
                }
            }
        }
        private void LoadTrafficData(MultiNetInvoiceGeneratorContext multiNetInvoiceGeneratorContext, long branchAccountId, DateTime fromDate, DateTime toDate, int currencyId, Guid branchTypeId)
        {
            if(multiNetInvoiceGeneratorContext.BillingSummariesByBranch == null)
            {
                List<string> listMeasures = new List<string> { "Amount", "CountCDRs", "TotalDuration" };
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
                        TableId = 9,
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
                        }else
                        {
                            branchId = branchAccountId;
                        }

                        List<BillingSummary> billingSummaries = multiNetInvoiceGeneratorContext.BillingSummariesByBranch.GetOrCreateItem(branchId);
                       
                        if (trafficDirection.Value != null && serviceTypeIdDim.Value != null)
                        {
                            MeasureValue amountMeasure = GetMeasureValue(analyticRecord, "Amount");
                            MeasureValue countCDRsMeasure = GetMeasureValue(analyticRecord, "CountCDRs");
                            MeasureValue totalDurationMeasure = GetMeasureValue(analyticRecord, "CountCDRs");
                            decimal amount = Convert.ToDecimal(amountMeasure.Value ?? 0.0);
                            int countCDRs = Convert.ToInt32(countCDRsMeasure.Value);
                            int totalDuration = Convert.ToInt32(amountMeasure.Value);
                            BillingSummary billingSummary = new BillingSummary
                            {
                                Amount = Convert.ToDecimal(amountMeasure.Value ?? 0.0),
                                CountCDRs = Convert.ToInt32(countCDRsMeasure.Value),
                                TotalDuration = Convert.ToInt32(amountMeasure.Value),
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

        #region Load Traffic Data
        private MeasureValue GetMeasureValue(AnalyticRecord analyticRecord, string measureName)
        {
            MeasureValue measureValue;
            analyticRecord.MeasureValues.TryGetValue(measureName, out measureValue);
            return measureValue;
        }

        #endregion


        private string GetSubItemIdentifier(long accountId,Guid? serviceTypeId)
        {
            string subItemIdentifier = "";
            if(serviceTypeId.HasValue)
            {
                subItemIdentifier = string.Format("BranchCDROut_{0}_{1}", serviceTypeId, accountId);
            }else
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
        private InvoiceDetails BuildGeneratedInvoiceDetails(List<BranchSummary> branchesSummary, DateTime fromDate, DateTime toDate, DateTime issueDate, int currencyId, Account account,Guid financialAccountTypeId)
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
                }
                retailSubscriberInvoiceDetails.PayableByDueDate = retailSubscriberInvoiceDetails.TotalCurrentCharges;
                retailSubscriberInvoiceDetails.LatePaymentCharges = GetLatePaymentCharges(account, retailSubscriberInvoiceDetails.TotalCurrentCharges, currencyId, issueDate);
                retailSubscriberInvoiceDetails.PayableAfterDueDate = retailSubscriberInvoiceDetails.TotalCurrentCharges + retailSubscriberInvoiceDetails.LatePaymentCharges;
                retailSubscriberInvoiceDetails.CurrencyId = currencyId;
                retailSubscriberInvoiceDetails.AccountTypeId = financialAccountTypeId; 
            }
            return retailSubscriberInvoiceDetails;
        }

        #endregion

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
            public int TotalDuration { get; set; }
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
           public int SaleCurrencyId { get; set; }
        }
    }

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


        public void FillAdditionalFields(IInvoiceItemAdditionalFieldsContext context)
        {
            AccountBEManager accountBEManager = new AccountBEManager();
            context.InvoiceType.ThrowIfNull("context.InvoiceType");
            context.InvoiceType.Settings.ThrowIfNull("context.InvoiceType.Settings");
            MultiNetSubscriberInvoiceSettings multiNetSubscriberInvoiceSettings = context.InvoiceType.Settings.ExtendedSettings.CastWithValidate<MultiNetSubscriberInvoiceSettings>("context.InvoiceType.Settings.ExtendedSettings");
            this.BranchName = accountBEManager.GetAccountName(multiNetSubscriberInvoiceSettings.AccountBEDefinitionId, this.AccountId);
        }

    }
    public class BranchSummaryItem
    {
        public long AccountId { get; set; }
        public string UsageDescription { get; set; }
        public int Quantity { get; set; }
        public Decimal NetAmount { get; set; }
        public Guid ChargeableEntityId { get; set; }
    }
    public class UsageSummary
    {
        public long AccountId { get; set; }
        public string SubItemIdentifier { get; set; }
        public string UsageDescription { get; set; }
        public int Quantity { get; set; }
        public int TotalDuration { get; set; }
        public Decimal NetAmount { get; set; }
    }
    public class MultiNetCDR : IInvoiceItemAdditionalFields
    {
        public string SubItemIdentifier { get; set; }
        public DateTime AttemptDateTime { get; set; }
        public decimal DurationInSeconds { get; set; }
        public string CallingNumber { get; set; }
        public string CalledNumber { get; set; }
        public decimal SaleAmount { get; set; }
        public long? ZoneId { get; set; }
        public string ZoneName { get; set; }
        public int SaleCurrencyId { get; set; }
        public string SaleCurrencyName { get; set; }

        public void FillAdditionalFields(IInvoiceItemAdditionalFieldsContext context)
        {
            if(this.ZoneId.HasValue)
            {
                SaleZoneManager saleZoneManager = new SaleZoneManager();
                this.ZoneName = saleZoneManager.GetSaleZoneName(this.ZoneId.Value);
            }
            CurrencyManager currencyManager = new CurrencyManager();
            this.SaleCurrencyName = currencyManager.GetCurrencySymbol(this.SaleCurrencyId);
          
        }
    }


}
