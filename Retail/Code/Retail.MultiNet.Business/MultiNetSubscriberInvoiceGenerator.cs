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
using Vanrise.GenericData.Entities;
using Vanrise.GenericData.Pricing;
using Vanrise.GenericData.Transformation;
using Vanrise.Invoice.Entities;

namespace Retail.MultiNet.Business
{
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
        public MultiNetSubscriberInvoiceGenerator(Guid acountBEDefinitionId, List<Guid> salesTaxChargeableEntities, List<Guid> wHTaxChargeableEntities, Guid inComingChargeableEntity, Guid outGoingChargeableEntity, Guid salesTaxRuleDefinitionId, Guid wHTaxRuleDefinitionId, Guid latePaymentRuleDefinitionId)
        {
            this._acountBEDefinitionId = acountBEDefinitionId;
            this._salesTaxChargeableEntities = new List<Guid> { new Guid("fc8a8acc-5c10-49f0-95fd-b7e95ed5db80"), new Guid("711039b3-92ee-4cb9-80e5-ac6354452c8e"), new Guid("f062a145-a311-4629-a96d-d770c34c7da6") };
            this._wHTaxChargeableEntities = new List<Guid> { new Guid("fc8a8acc-5c10-49f0-95fd-b7e95ed5db80"), new Guid("711039b3-92ee-4cb9-80e5-ac6354452c8e") };
            this._inComingChargeableEntity = new Guid("f062a145-a311-4629-a96d-d770c34c7da6");
            this._outGoingChargeableEntity = new Guid("fc8a8acc-5c10-49f0-95fd-b7e95ed5db80");
            this._salesTaxRuleDefinitionId = new Guid("d15d107e-64f0-4caa-bc46-ba58ed30e848");
            this._wHTaxRuleDefinitionId = new Guid("3f6002b5-3e4b-4300-b676-e11176b54782");
            this._latePaymentRuleDefinitionId = new Guid("631518f4-e35f-40a8-8ac9-7d25532f7a36");
        }

        public override void GenerateInvoice(IInvoiceGenerationContext context)
        {

            var financialAccountData = _financialAccountManager.GetFinancialAccountData(_acountBEDefinitionId, context.PartnerId);
            //if (context.FromDate < financialAccountData.FinancialAccount.BED || context.ToDate > financialAccountData.FinancialAccount.EED)
            //{
            //    throw new InvoiceGeneratorException("From date and To date should be within the effective date of financial account.");
            //}

            IAccountPayment accountPayment;
            if (!_accountBEManager.HasAccountPayment(this._acountBEDefinitionId, financialAccountData.Account.AccountId, true, out accountPayment))
                throw new InvoiceGeneratorException(string.Format("Account Id: {0} is not a financial account", financialAccountData.Account.AccountId));
            int currencyId = accountPayment.CurrencyId;

            List<Summary> summaryItemSet = new List<Summary>();
            List<UsageSummary> usageSummaryItemSet = new List<UsageSummary>();


            List<string> listMeasures = new List<string> { "Amount", "CountCDRs", "TotalDuration" };
            List<string> listDimensions = new List<string> { "TrafficDirection", "ServiceType" };
            string dimensionName = "FinancialAccountId";


            var analyticResult = GetFilteredRecords(listDimensions, listMeasures, dimensionName, financialAccountData.Account.AccountId, context.FromDate, context.GeneratedToDate, currencyId);

            BuildItemSetNames(analyticResult.Data, financialAccountData.Account, currencyId, summaryItemSet, usageSummaryItemSet);
            AddingRecurringCharge(summaryItemSet, financialAccountData.Account, currencyId, context.FromDate, context.ToDate);

            List<GeneratedInvoiceItemSet> generatedInvoiceItemSets = BuildGeneratedInvoiceItemSet(summaryItemSet, usageSummaryItemSet);
            InvoiceDetails retailSubscriberInvoiceDetails = BuildGeneratedInvoiceDetails(summaryItemSet, context.FromDate, context.ToDate, currencyId, financialAccountData.Account);

            context.Invoice = new GeneratedInvoice
            {
                InvoiceDetails = retailSubscriberInvoiceDetails,
                InvoiceItemSets = generatedInvoiceItemSets,
            };
        }
        private void AddingRecurringCharge(List<Summary> summaryItemSet, Account account, int currencyId,DateTime fromDate, DateTime toDate)
        {
            var accountPackages = _accountPackageManager.GetAccountPackagesByAccountId(account.AccountId);
            List<int> accountPackagesIds = new List<int>();
            if (accountPackages != null)
            {
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
                                decimal amountWithTaxes = 0;
                                foreach (var output in evaluateRecurringCharge)
                                {
                                    Summary summary = new Summary();
                                    summary.ChargeableEntityId = output.ChargeableEntityId;
                                    summary.UsageDescription = _genericLKUPManager.GetGenericLKUPItemName(output.ChargeableEntityId);
                                    summary.Quantity = 1;
                                    summary.NetAmount = output.Amount;
                                    summary.SalesTaxAmount = GetSaleTaxAmount(account, summary.NetAmount, currencyId, out amountWithTaxes);
                                    summary.AmountWithTaxes = summary.NetAmount + summary.SalesTaxAmount;
                                    summaryItemSet.Add(summary);
                                }
                            }
                        }

                    }
                }
            }
        }
        private void BuildItemSetNames(IEnumerable<AnalyticRecord> analyticRecords, Account account, int currencyId, List<Summary> summaryItemSet, List<UsageSummary> usageSummaryItemSet)
        {
            if (analyticRecords != null)
            {
                Summary callAggregationSummary = new Summary();
                Summary outgoingCallsSummary = new Summary();
                Dictionary<Guid, UsageSummary> usagesSummariesByServiceType = new Dictionary<Guid, UsageSummary>();
                UsageSummary callAggregationUsageSummary = new UsageSummary();

                List<Guid> serviceTypeIds = new List<Guid>();

                foreach (var analyticRecord in analyticRecords)
                {
                    DimensionValue trafficDirection = analyticRecord.DimensionValues.ElementAtOrDefault(0);
                    DimensionValue serviceTypeIdDim = analyticRecord.DimensionValues.ElementAtOrDefault(1);

                    if (trafficDirection.Value != null && serviceTypeIdDim.Value != null)
                    {
                        MeasureValue amountMeasure = GetMeasureValue(analyticRecord, "Amount");
                        MeasureValue countCDRsMeasure = GetMeasureValue(analyticRecord, "CountCDRs");
                        MeasureValue totalDurationMeasure = GetMeasureValue(analyticRecord, "CountCDRs");

                        decimal amount = Convert.ToDecimal(amountMeasure.Value ?? 0.0);
                        int countCDRs = Convert.ToInt32(countCDRsMeasure.Value);
                        int totalDuration = Convert.ToInt32(amountMeasure.Value);


                        int traficDirection = Convert.ToInt32(trafficDirection.Value);
                        if (traficDirection == 1)
                        {
                            if (callAggregationSummary.UsageDescription == null)
                                callAggregationSummary.UsageDescription = _genericLKUPManager.GetGenericLKUPItemName(this._inComingChargeableEntity);
                            callAggregationSummary.Quantity += countCDRs;
                            callAggregationSummary.NetAmount += amount;
                            callAggregationSummary.ChargeableEntityId = this._inComingChargeableEntity;

                            if (callAggregationUsageSummary.UsageDescription == null)
                                callAggregationUsageSummary.UsageDescription = _genericLKUPManager.GetGenericLKUPItemName(this._inComingChargeableEntity);
                            callAggregationUsageSummary.Quantity += countCDRs;
                            callAggregationUsageSummary.NetAmount += amount;
                            callAggregationUsageSummary.TotalDuration += totalDuration;

                        }
                        else
                        {
                            if (outgoingCallsSummary.UsageDescription == null)
                                outgoingCallsSummary.UsageDescription = _genericLKUPManager.GetGenericLKUPItemName(this._outGoingChargeableEntity);
                            outgoingCallsSummary.Quantity += countCDRs;
                            outgoingCallsSummary.NetAmount += amount;
                            outgoingCallsSummary.ChargeableEntityId = this._outGoingChargeableEntity;
                            Guid serviceTypeId;
                            if (Guid.TryParse(serviceTypeIdDim.Value.ToString(), out serviceTypeId))
                            {
                                UsageSummary usageSummary;
                                if (!usagesSummariesByServiceType.TryGetValue(serviceTypeId, out usageSummary))
                                {
                                    usageSummary = new UsageSummary();
                                    usageSummary.UsageDescription = _serviceTypeManager.GetServiceTypeName(serviceTypeId);
                                    usageSummary.Quantity = countCDRs;
                                    usageSummary.NetAmount = amount;
                                    usageSummary.TotalDuration = totalDuration;
                                    usagesSummariesByServiceType.Add(serviceTypeId, usageSummary);
                                }
                                else
                                {
                                    usageSummary.Quantity += countCDRs;
                                    usageSummary.NetAmount += amount;
                                    usageSummary.TotalDuration += totalDuration;
                                }
                            };
                        }
                    }
                }

                foreach (var item in usagesSummariesByServiceType)
                {
                    usageSummaryItemSet.Add(item.Value);
                }
                usageSummaryItemSet.Add(callAggregationUsageSummary);
                decimal salesTaxAmount = 0;
                callAggregationSummary.SalesTaxAmount = GetSaleTaxAmount(account, callAggregationSummary.NetAmount, currencyId, out salesTaxAmount);
                callAggregationSummary.AmountWithTaxes = callAggregationSummary.NetAmount + callAggregationSummary.SalesTaxAmount;

                outgoingCallsSummary.SalesTaxAmount = GetSaleTaxAmount(account, outgoingCallsSummary.NetAmount, currencyId, out salesTaxAmount);
                 outgoingCallsSummary.AmountWithTaxes = outgoingCallsSummary.NetAmount + outgoingCallsSummary.SalesTaxAmount;

                summaryItemSet.Add(callAggregationSummary);
                summaryItemSet.Add(outgoingCallsSummary);

            }
        }
        private Decimal GetSaleTaxAmount(Account account, decimal amount, int currencyId,out decimal percentage)
        {
            GenericRuleTarget ruleTarget = new GenericRuleTarget
            {
                Objects = new Dictionary<string, dynamic> { { "Account", account } },
                EffectiveOn = DateTime.Now
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
        private Decimal GetLatePaymentCharges(Account account, decimal amount, int currencyId)
        {
            GenericRuleTarget ruleTarget = new GenericRuleTarget
            {
                Objects = new Dictionary<string, dynamic> { { "Account", account } },
                EffectiveOn = DateTime.Now
            };
            var matchRule = _mappingRuleManager.GetMatchRule(this._latePaymentRuleDefinitionId, ruleTarget);
            if(matchRule == null)
               throw new InvoiceGeneratorException("Connot find late payment rule");
            var percentage = Convert.ToDecimal(matchRule.Settings.Value ?? 0.0);

            return percentage > 0 ? (percentage * amount) / 100 : 0;
        }
        private Decimal GetWHTaxAmount(Account account, decimal amount, int currencyId, out decimal percentage)
        {
            GenericRuleTarget ruleTarget = new GenericRuleTarget
            {
                Objects = new Dictionary<string, dynamic> { { "Account", account } },
                EffectiveOn = DateTime.Now
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
      
        private AnalyticSummaryBigResult<AnalyticRecord> GetFilteredRecords(List<string> listDimensions, List<string> listMeasures, string dimensionFilterName, object dimensionFilterValue, DateTime fromDate, DateTime toDate, int currencyId)
        {
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
                FilterValues = new List<object> { dimensionFilterValue }
            };
            analyticQuery.Query.Filters.Add(dimensionFilter);
            return _analyticManager.GetFilteredRecords(analyticQuery) as Vanrise.Analytic.Entities.AnalyticSummaryBigResult<AnalyticRecord>;
        }
        private MeasureValue GetMeasureValue(AnalyticRecord analyticRecord, string measureName)
        {
            MeasureValue measureValue;
            analyticRecord.MeasureValues.TryGetValue(measureName, out measureValue);
            return measureValue;
        }

        private List<GeneratedInvoiceItemSet> BuildGeneratedInvoiceItemSet(List<Summary> summaryItemSet, List<UsageSummary> usageSummaryItemSet)
        {
            List<GeneratedInvoiceItemSet> generatedInvoiceItemSets = new List<GeneratedInvoiceItemSet>();
            GeneratedInvoiceItemSet generatedSummaryItemSet = new GeneratedInvoiceItemSet();
            generatedSummaryItemSet.SetName = "Summary";
            generatedSummaryItemSet.Items = new List<GeneratedInvoiceItem>();
            foreach (var item in summaryItemSet)
            {
                generatedSummaryItemSet.Items.Add(new GeneratedInvoiceItem
                {
                    Details = item,
                    Name = ""
                });
            }
            generatedInvoiceItemSets.Add(generatedSummaryItemSet);

            GeneratedInvoiceItemSet generatedUsageSummaryItemSet = new GeneratedInvoiceItemSet();
            generatedUsageSummaryItemSet.SetName = "UsageSummary";
            generatedUsageSummaryItemSet.Items = new List<GeneratedInvoiceItem>();
            foreach (var item in usageSummaryItemSet)
            {
                generatedUsageSummaryItemSet.Items.Add(new GeneratedInvoiceItem
                {
                    Details = item,
                    Name = ""
                });
            }
            generatedInvoiceItemSets.Add(generatedUsageSummaryItemSet);
            return generatedInvoiceItemSets;
        }
        private InvoiceDetails BuildGeneratedInvoiceDetails(List<Summary> summaryItemSet, DateTime fromDate, DateTime toDate, int currencyId, Account account)
        {
            InvoiceDetails retailSubscriberInvoiceDetails = null;
            if (summaryItemSet != null)
            {
                retailSubscriberInvoiceDetails = new InvoiceDetails();
                decimal whAmount = 0;
                decimal saleAmount = 0;
                decimal totalSaleAmount = 0;
                foreach (var invoiceBillingRecord in summaryItemSet)
                {
                    retailSubscriberInvoiceDetails.Quantity += invoiceBillingRecord.Quantity;
                    retailSubscriberInvoiceDetails.CurrentCharges += invoiceBillingRecord.NetAmount;
                    if (this._salesTaxChargeableEntities.Contains(invoiceBillingRecord.ChargeableEntityId))
                    {
                        saleAmount += invoiceBillingRecord.NetAmount;
                    }
                    totalSaleAmount += invoiceBillingRecord.NetAmount;
                    if (this._wHTaxChargeableEntities.Contains(invoiceBillingRecord.ChargeableEntityId))
                    {
                        whAmount += invoiceBillingRecord.NetAmount;
                    }
                }
                decimal saleTaxPercentage = 0;
                retailSubscriberInvoiceDetails.SalesTaxAmount = GetSaleTaxAmount(account, saleAmount, currencyId, out saleTaxPercentage);
                retailSubscriberInvoiceDetails.SalesTax = saleTaxPercentage;

                decimal totalSaleTaxPercentage = 0;
                retailSubscriberInvoiceDetails.TotalCurrentCharges = totalSaleAmount + GetSaleTaxAmount(account, totalSaleAmount, currencyId, out totalSaleTaxPercentage);
                retailSubscriberInvoiceDetails.PayableByDueDate = retailSubscriberInvoiceDetails.TotalCurrentCharges;

                decimal whSaleTaxPercentage = 0;
                decimal whAmountWithTaxes = whAmount + GetSaleTaxAmount(account, whAmount, currencyId, out whSaleTaxPercentage);

                decimal whTaxPercentage = 0;
                retailSubscriberInvoiceDetails.WHTaxAmount =  GetWHTaxAmount(account, whAmountWithTaxes, currencyId, out whTaxPercentage);
                retailSubscriberInvoiceDetails.WHTax = whTaxPercentage;

                retailSubscriberInvoiceDetails.LatePaymentCharges = GetLatePaymentCharges(account, retailSubscriberInvoiceDetails.TotalCurrentCharges, currencyId);
                retailSubscriberInvoiceDetails.PayableAfterDueDate = retailSubscriberInvoiceDetails.TotalCurrentCharges + retailSubscriberInvoiceDetails.LatePaymentCharges;
                retailSubscriberInvoiceDetails.CurrencyId = currencyId;

            }
            return retailSubscriberInvoiceDetails;
        }
     
        public class Summary
        {
            public string UsageDescription { get; set; }
            public int Quantity { get; set; }
            public Decimal NetAmount { get; set; }
            public Guid ChargeableEntityId { get; set; }
            public Decimal SalesTaxAmount { get; set; }
            public Decimal AmountWithTaxes { get; set; }
        }
        public class UsageSummary
        {
            public string UsageDescription { get; set; }
            public int Quantity { get; set; }
            public int TotalDuration { get; set; }
            public Decimal NetAmount { get; set; }
        }
    }
}
