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
            List<string> listMeasures = new List<string> { "Amount", "CountCDRs", "TotalDuration" };
            List<string> listDimensions = new List<string> { "TrafficDirection", "ServiceType" };

            string dimensionName = "FinancialAccountId";
            var financialAccountData = _financialAccountManager.GetFinancialAccountData(_acountBEDefinitionId, context.PartnerId);


            var accountPackages = _accountPackageManager.GetPackageIdsAssignedToAccount(financialAccountData.Account.AccountId, DateTime.Now);
            IEnumerable<Package> packages = null;
            if(accountPackages != null)
            {
                packages = _packageManager.GetPackagesByIds(accountPackages);
            }
            
            IAccountPayment accountPayment;
            if (!_accountBEManager.HasAccountPayment(this._acountBEDefinitionId, financialAccountData.Account.AccountId, true, out accountPayment))
                throw new InvoiceGeneratorException(string.Format("Account Id: {0} is not a financial account", financialAccountData.Account.AccountId));
            int currencyId = accountPayment.CurrencyId;

            var analyticResult = GetFilteredRecords(listDimensions, listMeasures, dimensionName, financialAccountData.Account.AccountId, context.FromDate, context.GeneratedToDate, currencyId);
            //if (analyticResult == null || analyticResult.Data == null || analyticResult.Data.Count() == 0)
            //{
            //    throw new InvoiceGeneratorException("No data available between the selected period.");
            //}

            Dictionary<string, List<dynamic>> itemSetNamesDic = ConvertAnalyticDataToDictionary(analyticResult.Data, packages, financialAccountData.Account, currencyId);
            List<GeneratedInvoiceItemSet> generatedInvoiceItemSets = BuildGeneratedInvoiceItemSet(itemSetNamesDic);

            InvoiceDetails retailSubscriberInvoiceDetails = BuildInvoiceDetails(itemSetNamesDic, context.FromDate, context.ToDate, currencyId, financialAccountData.Account);

            context.Invoice = new GeneratedInvoice
            {
                InvoiceDetails = retailSubscriberInvoiceDetails,
                InvoiceItemSets = generatedInvoiceItemSets,
            };
        }
        private Dictionary<string, List<dynamic>> ConvertAnalyticDataToDictionary(IEnumerable<AnalyticRecord> analyticRecords, IEnumerable<Package> packages, Account account, int currencyId)
        {
            Dictionary<string, List<dynamic>> itemSetNamesDic = new Dictionary<string, List<dynamic>>();
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
                    AddItemToDictionary(itemSetNamesDic, "UsageSummary", item.Value);
                }
                AddItemToDictionary(itemSetNamesDic, "UsageSummary", callAggregationUsageSummary);


                #region Adding Package Usage
                if (packages != null)
                {

                    foreach (var package in packages)
                    {
                        var invoiceRecurChargePackageSettings = package.Settings.ExtendedSettings as RecurChargePackageSettings;
                        if (invoiceRecurChargePackageSettings != null)
                        {
                            var packageDefinition = _packageDefinitionManager.GetPackageDefinitionById(package.Settings.PackageDefinitionId);
                            if (packageDefinition != null)
                            {
                                var invoiceRecurChargePackageDefinitionSettings = packageDefinition.Settings.ExtendedSettings as RecurChargePackageDefinitionSettings;
                                if (invoiceRecurChargePackageDefinitionSettings != null)
                                {
                                    Summary summary = new Summary();
                                  //  summary.ChargeableEntityId = invoiceRecurChargePackageDefinitionSettings.ChargeableEntityId;
                                    //summary.UsageDescription = _genericLKUPManager.GetGenericLKUPItemName(invoiceRecurChargePackageDefinitionSettings.ChargeableEntityId);
                                    summary.Quantity = 1;
                                    //summary.NetAmount = invoiceRecurChargePackageSettings.Price;
                                    summary.SalesTaxAmount = GetSaleTaxAmount(account, summary.NetAmount, currencyId);
                                    summary.AmountWithTaxes = summary.NetAmount + summary.SalesTaxAmount;
                                    AddItemToDictionary(itemSetNamesDic, "Summary", summary);
                                }
                            }

                        }
                    }
                }
                #endregion

                callAggregationSummary.SalesTaxAmount = GetSaleTaxAmount(account, callAggregationSummary.NetAmount, currencyId);
                callAggregationSummary.AmountWithTaxes = callAggregationSummary.NetAmount + callAggregationSummary.SalesTaxAmount;

                outgoingCallsSummary.SalesTaxAmount = GetSaleTaxAmount(account, outgoingCallsSummary.NetAmount, currencyId);
                outgoingCallsSummary.AmountWithTaxes = outgoingCallsSummary.NetAmount + outgoingCallsSummary.SalesTaxAmount; 

                AddItemToDictionary(itemSetNamesDic, "Summary", callAggregationSummary);
                AddItemToDictionary(itemSetNamesDic, "Summary", outgoingCallsSummary);

            }
            return itemSetNamesDic;
        }
        private Decimal GetSaleTaxAmount(Account account, decimal amount, int currencyId)
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
        private Decimal GetWHTaxAmount(Account account, decimal amount, int currencyId)
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
            return taxRuleContext.TaxAmount;
        }
        private List<GeneratedInvoiceItemSet> BuildGeneratedInvoiceItemSet(Dictionary<string, List<dynamic>> itemSetNamesDic)
        {
            List<GeneratedInvoiceItemSet> generatedInvoiceItemSets = new List<GeneratedInvoiceItemSet>();
            if (itemSetNamesDic != null)
            {
                foreach (var itemSet in itemSetNamesDic)
                {
                    GeneratedInvoiceItemSet generatedInvoiceItemSet = new GeneratedInvoiceItemSet();
                    generatedInvoiceItemSet.SetName = itemSet.Key;
                    var itemSetValues = itemSet.Value;
                    generatedInvoiceItemSet.Items = new List<GeneratedInvoiceItem>();

                    foreach (var item in itemSetValues)
                    {
                        generatedInvoiceItemSet.Items.Add(new GeneratedInvoiceItem
                        {
                            Details = item,
                            Name = "Summary"
                        });
                    }
                    generatedInvoiceItemSets.Add(generatedInvoiceItemSet);

                }
            }
            return generatedInvoiceItemSets;
        }
        private InvoiceDetails BuildInvoiceDetails(Dictionary<string, List<dynamic>> itemSetNamesDic, DateTime fromDate, DateTime toDate, int currencyId, Account account)
        {
            InvoiceDetails retailSubscriberInvoiceDetails = null;
            if (itemSetNamesDic != null)
            {
                List<dynamic> invoiceBillingRecordList = null;
                if (itemSetNamesDic.TryGetValue("Summary", out invoiceBillingRecordList))
                {

                    retailSubscriberInvoiceDetails = new InvoiceDetails();
                    decimal whAmountWithTaxes = 0;
                    foreach (var invoiceBillingRecord in invoiceBillingRecordList)
                    {
                        retailSubscriberInvoiceDetails.Quantity += invoiceBillingRecord.Quantity;
                        retailSubscriberInvoiceDetails.CurrentCharges += invoiceBillingRecord.NetAmount;
                        if(this._salesTaxChargeableEntities.Contains(invoiceBillingRecord.ChargeableEntityId))
                        {
                           retailSubscriberInvoiceDetails.SalesTaxAmount += invoiceBillingRecord.SalesTaxAmount;
                        }
                        retailSubscriberInvoiceDetails.PayableByDueDate += invoiceBillingRecord.AmountWithTaxes;
                        if (this._wHTaxChargeableEntities.Contains(invoiceBillingRecord.ChargeableEntityId))
                        {
                            whAmountWithTaxes += invoiceBillingRecord.AmountWithTaxes;
                        }
                        retailSubscriberInvoiceDetails.TotalCurrentCharges += invoiceBillingRecord.AmountWithTaxes;
                    }
                    retailSubscriberInvoiceDetails.WHTaxAmount =  GetWHTaxAmount(account, whAmountWithTaxes, currencyId);
                    retailSubscriberInvoiceDetails.LatePaymentCharges = GetLatePaymentCharges(account, retailSubscriberInvoiceDetails.TotalCurrentCharges, currencyId);
                    retailSubscriberInvoiceDetails.PayableAfterDueDate = retailSubscriberInvoiceDetails.TotalCurrentCharges + retailSubscriberInvoiceDetails.LatePaymentCharges;
                    retailSubscriberInvoiceDetails.CurrencyId = currencyId;
                   
                };
            }
            return retailSubscriberInvoiceDetails;
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
        private void AddItemToDictionary<T>(Dictionary<string, List<T>> itemSetNamesDic, string key, T itemDetail)
        {
            if (itemSetNamesDic == null)
                itemSetNamesDic = new Dictionary<string, List<T>>();
            List<T> itemDetailRecordList = null;

            if (!itemSetNamesDic.TryGetValue(key, out itemDetailRecordList))
            {
                itemDetailRecordList = new List<T>();
                itemDetailRecordList.Add(itemDetail);
                itemSetNamesDic.Add(key, itemDetailRecordList);
            }
            else
            {
                itemDetailRecordList.Add(itemDetail);
                itemSetNamesDic[key] = itemDetailRecordList;
            }
        }
        private MeasureValue GetMeasureValue(AnalyticRecord analyticRecord, string measureName)
        {
            MeasureValue measureValue;
            analyticRecord.MeasureValues.TryGetValue(measureName, out measureValue);
            return measureValue;
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
