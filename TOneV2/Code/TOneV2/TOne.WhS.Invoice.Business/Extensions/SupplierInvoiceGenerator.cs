using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Invoice.Entities;
using Vanrise.Analytic.Business;
using Vanrise.Analytic.Entities;
using Vanrise.Common.Business;
using Vanrise.Entities;
using Vanrise.Invoice.Entities;
using Vanrise.Common;
namespace TOne.WhS.Invoice.Business.Extensions
{
    public class SupplierInvoiceGenerator : InvoiceGenerator
    {
        public override void GenerateInvoice(IInvoiceGenerationContext context)
        {
            List<string> listMeasures = new List<string> { "CostNetNotNULL", "NumberOfCalls", "CostDuration", "BillingPeriodTo", "BillingPeriodFrom", "CostNet_OrigCurr" };
            List<string> listDimensions = new List<string> { "SupplierZone", "Supplier", "CostCurrency", "CostRate", "CostRateType" };
            WHSFinancialAccountManager financialAccountManager = new WHSFinancialAccountManager();
            var financialAccount = financialAccountManager.GetFinancialAccount(Convert.ToInt32(context.PartnerId));
            var supplierGenerationCustomSectionPayload = context.CustomSectionPayload as SupplierGenerationCustomSectionPayload;

            var definitionSettings = new WHSFinancialAccountDefinitionManager().GetFinancialAccountDefinitionSettings(financialAccount.FinancialAccountDefinitionId);
            definitionSettings.ThrowIfNull("definitionSettings", financialAccount.FinancialAccountDefinitionId);
            definitionSettings.FinancialAccountInvoiceTypes.ThrowIfNull("definitionSettings.FinancialAccountInvoiceTypes", financialAccount.FinancialAccountDefinitionId);
            var financialAccountInvoiceType = definitionSettings.FinancialAccountInvoiceTypes.FindRecord(x => x.InvoiceTypeId == context.InvoiceTypeId);
            financialAccountInvoiceType.ThrowIfNull("financialAccountInvoiceType");

            int? timeZoneId = null;
            decimal? commission = null;
            CommissionType? commissionType = null;
            if (supplierGenerationCustomSectionPayload != null && supplierGenerationCustomSectionPayload.TimeZoneId.HasValue)
            {
                timeZoneId = supplierGenerationCustomSectionPayload.TimeZoneId;
                if (supplierGenerationCustomSectionPayload.Commission.HasValue)
                {
                    commission = supplierGenerationCustomSectionPayload.Commission.Value;
                    commissionType = supplierGenerationCustomSectionPayload.CommissionType;
                }
            }
            if (!timeZoneId.HasValue)
            {
                timeZoneId = financialAccountManager.GetSupplierTimeZoneId(financialAccount.FinancialAccountId);
            }
            string offset = null;
            DateTime fromDate = context.FromDate;
            DateTime toDate = context.ToDate;

            DateTime toDateForBillingTransaction = context.ToDate.Date.AddDays(1);
            if (timeZoneId.HasValue)
            {
                VRTimeZone timeZone = new VRTimeZoneManager().GetVRTimeZone(timeZoneId.Value);
                if (timeZone != null)
                {
                    offset = timeZone.Settings.Offset.ToString();
                    fromDate = context.FromDate.Add(-timeZone.Settings.Offset);
                    toDate = context.ToDate.Add(-timeZone.Settings.Offset);
                    toDateForBillingTransaction = toDateForBillingTransaction.Add(-timeZone.Settings.Offset);
                }
            }

            string dimentionName = "CostFinancialAccount";
            int dimensionValue = financialAccount.FinancialAccountId;
            int currencyId = financialAccountManager.GetFinancialAccountCurrencyId(financialAccount);
            string partnerType = null;
            if (financialAccount.CarrierProfileId.HasValue)
            {
                partnerType = "Profile";
            }
            else
            {
                partnerType = "Account";
            }
            IEnumerable<VRTaxItemDetail> taxItemDetails = financialAccountManager.GetFinancialAccountTaxItemDetails(financialAccount);
            var analyticResult = GetFilteredRecords(listDimensions, listMeasures, dimentionName, dimensionValue, fromDate, toDate, currencyId);
            if (analyticResult == null || analyticResult.Data == null || analyticResult.Data.Count() == 0)
            {
                context.GenerateInvoiceResult = GenerateInvoiceResult.NoData;
                return;
            }
            Dictionary<string, List<InvoiceBillingRecord>> itemSetNamesDic = ConvertAnalyticDataToDictionary(analyticResult.Data, currencyId, commission, commissionType, taxItemDetails);
            if (itemSetNamesDic.Count == 0)
            {
                context.GenerateInvoiceResult = GenerateInvoiceResult.NoData;
                return;
            }

            var supplierInvoiceBySaleCurrency = loadCurrencyItemSet(dimentionName, dimensionValue, fromDate, toDate, commission, commissionType, taxItemDetails);

            List<GeneratedInvoiceItemSet> generatedInvoiceItemSets = BuildGeneratedInvoiceItemSet(itemSetNamesDic, taxItemDetails, supplierInvoiceBySaleCurrency);
            #region BuildSupplierInvoiceDetails
            SupplierInvoiceDetails supplierInvoiceDetails = BuilSupplierInvoiceDetails(itemSetNamesDic, partnerType, context.FromDate, context.ToDate, commission, commissionType);
            if (supplierInvoiceDetails != null && supplierInvoiceDetails.CostAmount != 0)
            {
                supplierInvoiceDetails.TimeZoneId = timeZoneId;
                supplierInvoiceDetails.TotalAmount = supplierInvoiceDetails.CostAmount;
                supplierInvoiceDetails.TotalAmountAfterCommission = supplierInvoiceDetails.AmountAfterCommission;
                supplierInvoiceDetails.TotalOriginalAmountAfterCommission = supplierInvoiceDetails.OriginalAmountAfterCommission;

                supplierInvoiceDetails.Commission = commission;
                supplierInvoiceDetails.CommissionType = commissionType;
                supplierInvoiceDetails.Offset = offset;
                if (taxItemDetails != null)
                {
                    foreach (var tax in taxItemDetails)
                    {
                        supplierInvoiceDetails.TotalAmountAfterCommission += ((supplierInvoiceDetails.AmountAfterCommission * Convert.ToDecimal(tax.Value)) / 100);
                        supplierInvoiceDetails.TotalOriginalAmountAfterCommission += ((supplierInvoiceDetails.OriginalAmountAfterCommission * Convert.ToDecimal(tax.Value)) / 100);

                        supplierInvoiceDetails.TotalAmount += ((supplierInvoiceDetails.CostAmount * Convert.ToDecimal(tax.Value)) / 100);
                    }
                }

                if (!financialAccountInvoiceType.IgnoreFromBalance)
                {
                    SetInvoiceBillingTransactions(context, supplierInvoiceDetails, financialAccount, fromDate, toDateForBillingTransaction);
                }

                context.Invoice = new GeneratedInvoice
                {
                    InvoiceDetails = supplierInvoiceDetails,
                    InvoiceItemSets = generatedInvoiceItemSets,
                };
            }
            else
            {
                context.ErrorMessage = "No billing data available.";
                context.GenerateInvoiceResult = GenerateInvoiceResult.NoData;
                return;
            }
            #endregion

           
        }

        private List<SupplierInvoiceBySaleCurrencyItemDetails> loadCurrencyItemSet(string dimentionName, int dimensionValue, DateTime fromDate, DateTime toDate, decimal? commission, CommissionType? commissionType, IEnumerable<VRTaxItemDetail> taxItemDetails)
        {

            List<string> listMeasures = new List<string> {  "NumberOfCalls", "CostDuration", "BillingPeriodTo", "BillingPeriodFrom", "CostNet_OrigCurr" };
            List<string> listDimensions = new List<string> {  "CostCurrency" };
            var analyticResult = GetFilteredRecords(listDimensions, listMeasures, dimentionName, dimensionValue, fromDate, toDate, null);
            if (analyticResult != null && analyticResult.Data != null && analyticResult.Data.Count() != 0)
            {
                return BuildCurrencyItemSetNameFromAnalytic(analyticResult.Data, commission, commissionType, taxItemDetails);
            }
            return null;
        }
        private List<SupplierInvoiceBySaleCurrencyItemDetails> BuildCurrencyItemSetNameFromAnalytic(IEnumerable<AnalyticRecord> analyticRecords, decimal? commission, CommissionType? commissionType, IEnumerable<VRTaxItemDetail> taxItemDetails)
        {
            List<SupplierInvoiceBySaleCurrencyItemDetails> supplierInvoiceBySaleCurrencies = null;

            if (analyticRecords != null)
            {
                supplierInvoiceBySaleCurrencies = new List<SupplierInvoiceBySaleCurrencyItemDetails>();
                foreach (var analyticRecord in analyticRecords)
                {
                    #region ReadDataFromAnalyticResult
                    DimensionValue costCurrencyId = analyticRecord.DimensionValues.ElementAtOrDefault(0);

                    MeasureValue costNet_OrigCurr = GetMeasureValue(analyticRecord, "CostNet_OrigCurr");
                    MeasureValue costDuration = GetMeasureValue(analyticRecord, "CostDuration");
                    MeasureValue calls = GetMeasureValue(analyticRecord, "NumberOfCalls");
                    MeasureValue billingPeriodTo = GetMeasureValue(analyticRecord, "BillingPeriodTo");
                    MeasureValue billingPeriodFrom = GetMeasureValue(analyticRecord, "BillingPeriodFrom");
                    #endregion

                    var costNet = Convert.ToDecimal(costNet_OrigCurr == null ? 0.0 : costNet_OrigCurr.Value ?? 0.0);
                    if (costNet != 0)
                    {
                        var supplierInvoiceBySaleCurrencyItemDetails = new SupplierInvoiceBySaleCurrencyItemDetails
                        {
                            CurrencyId = Convert.ToInt32(costCurrencyId.Value),
                            FromDate = billingPeriodFrom != null ? Convert.ToDateTime(billingPeriodFrom.Value) : default(DateTime),
                            ToDate = billingPeriodTo != null ? Convert.ToDateTime(billingPeriodTo.Value) : default(DateTime),
                            Duration = Convert.ToDecimal(costDuration.Value ?? 0.0),
                            NumberOfCalls = Convert.ToInt32(calls.Value ?? 0.0),
                            Amount = costNet,
                        };
                        if (commission.HasValue)
                        {
                            supplierInvoiceBySaleCurrencyItemDetails.AmountAfterCommission = supplierInvoiceBySaleCurrencyItemDetails.Amount + ((supplierInvoiceBySaleCurrencyItemDetails.Amount * commission.Value) / 100);
                        }
                        else
                        {
                            supplierInvoiceBySaleCurrencyItemDetails.AmountAfterCommission = supplierInvoiceBySaleCurrencyItemDetails.Amount;
                        }

                        supplierInvoiceBySaleCurrencyItemDetails.AmountAfterCommissionWithTaxes = supplierInvoiceBySaleCurrencyItemDetails.AmountAfterCommission;

                        if (taxItemDetails != null)
                        {
                            foreach (var tax in taxItemDetails)
                            {
                                supplierInvoiceBySaleCurrencyItemDetails.AmountAfterCommissionWithTaxes += ((supplierInvoiceBySaleCurrencyItemDetails.Amount * Convert.ToDecimal(tax.Value)) / 100);
                            }
                        }
                        supplierInvoiceBySaleCurrencies.Add(supplierInvoiceBySaleCurrencyItemDetails);
                    }

                }
            }
            return supplierInvoiceBySaleCurrencies;
        }



        private void SetInvoiceBillingTransactions(IInvoiceGenerationContext context, SupplierInvoiceDetails invoiceDetails, WHSFinancialAccount financialAccount, DateTime fromDate,DateTime toDate)
        {
            var financialAccountDefinitionManager = new WHSFinancialAccountDefinitionManager();
            var balanceAccountTypeId = financialAccountDefinitionManager.GetBalanceAccountTypeId(financialAccount.FinancialAccountDefinitionId);
            if (balanceAccountTypeId.HasValue)
            {
                Vanrise.Invoice.Entities.InvoiceType invoiceType = new Vanrise.Invoice.Business.InvoiceTypeManager().GetInvoiceType(context.InvoiceTypeId);
                invoiceType.ThrowIfNull("invoiceType", context.InvoiceTypeId);
                invoiceType.Settings.ThrowIfNull("invoiceType.Settings", context.InvoiceTypeId);
                SupplierInvoiceSettings invoiceSettings = invoiceType.Settings.ExtendedSettings.CastWithValidate<SupplierInvoiceSettings>("invoiceType.Settings.ExtendedSettings");

                var billingTransaction = new GeneratedInvoiceBillingTransaction()
                {
                    AccountTypeId = balanceAccountTypeId.Value,
                    AccountId = context.PartnerId,
                    TransactionTypeId = invoiceSettings.InvoiceTransactionTypeId,
                    Amount = invoiceDetails.TotalAmountAfterCommission,
                    CurrencyId = invoiceDetails.SupplierCurrencyId,
                    FromDate = fromDate,
                    ToDate = toDate
                };

                billingTransaction.Settings = new GeneratedInvoiceBillingTransactionSettings();
                billingTransaction.Settings.UsageOverrides = new List<GeneratedInvoiceBillingTransactionUsageOverride>();
                invoiceSettings.UsageTransactionTypeIds.ThrowIfNull("invoiceSettings.UsageTransactionTypeIds");
                foreach (Guid usageTransactionTypeId in invoiceSettings.UsageTransactionTypeIds)
                {
                    billingTransaction.Settings.UsageOverrides.Add(new GeneratedInvoiceBillingTransactionUsageOverride()
                    {
                        TransactionTypeId = usageTransactionTypeId
                    });
                }
                context.BillingTransactions = new List<GeneratedInvoiceBillingTransaction>() { billingTransaction };
            }

        }
        private SupplierInvoiceDetails BuilSupplierInvoiceDetails(Dictionary<string, List<InvoiceBillingRecord>> itemSetNamesDic, string partnerType, DateTime fromDate, DateTime toDate, decimal? commission, CommissionType? commissionType)
        {
            CurrencyManager currencyManager = new CurrencyManager();
            SupplierInvoiceDetails supplierInvoiceDetails = null;
            if (partnerType != null)
            {

                if (itemSetNamesDic != null)
                {
                    List<InvoiceBillingRecord> invoiceBillingRecordList = null;
                    if (itemSetNamesDic.TryGetValue("GroupedByCostZone", out invoiceBillingRecordList))
                    {
                         supplierInvoiceDetails = new SupplierInvoiceDetails();
                         supplierInvoiceDetails.PartnerType = partnerType;
                        foreach (var invoiceBillingRecord in invoiceBillingRecordList)
                        {

                            supplierInvoiceDetails.Duration += invoiceBillingRecord.InvoiceMeasures.CostDuration;
                            supplierInvoiceDetails.CostAmount += invoiceBillingRecord.InvoiceMeasures.CostNet;
                            supplierInvoiceDetails.OriginalCostAmount += invoiceBillingRecord.InvoiceMeasures.CostNet_OrigCurr;
                            supplierInvoiceDetails.TotalNumberOfCalls += invoiceBillingRecord.InvoiceMeasures.NumberOfCalls;
                            supplierInvoiceDetails.OriginalSupplierCurrencyId = invoiceBillingRecord.OriginalSupplierCurrencyId;
                            supplierInvoiceDetails.SupplierCurrencyId = invoiceBillingRecord.SupplierCurrencyId;
                            supplierInvoiceDetails.AmountAfterCommission += invoiceBillingRecord.InvoiceMeasures.AmountAfterCommission;
                            supplierInvoiceDetails.OriginalAmountAfterCommission += invoiceBillingRecord.InvoiceMeasures.OriginalAmountAfterCommission;

                        }
                        if (commissionType.HasValue)
                        {
                            switch (commissionType.Value)
                            {
                                case CommissionType.Display:
                                    supplierInvoiceDetails.DisplayComission = true;
                                    break;
                            }
                        }
                        else
                        {
                            supplierInvoiceDetails.DisplayComission = false;
                        }
                    };
                }
            }
            if (supplierInvoiceDetails != null )
            {
                supplierInvoiceDetails.OriginalSupplierCurrency = currencyManager.GetCurrencySymbol(supplierInvoiceDetails.OriginalSupplierCurrencyId);
                supplierInvoiceDetails.SupplierCurrency = currencyManager.GetCurrencySymbol(supplierInvoiceDetails.SupplierCurrencyId);
            }
            return supplierInvoiceDetails;
        }

        private List<GeneratedInvoiceItemSet> BuildGeneratedInvoiceItemSet(Dictionary<string, List<InvoiceBillingRecord>> itemSetNamesDic, IEnumerable<VRTaxItemDetail> taxItemDetails, List<SupplierInvoiceBySaleCurrencyItemDetails> supplierInvoicesBySaleCurrency)
        {
            List<GeneratedInvoiceItemSet> generatedInvoiceItemSets = new List<GeneratedInvoiceItemSet>();

            if (supplierInvoicesBySaleCurrency != null && supplierInvoicesBySaleCurrency.Count > 0)
            {
                GeneratedInvoiceItemSet generatedInvoiceItemSet = new GeneratedInvoiceItemSet();
                generatedInvoiceItemSet.SetName = "GroupingBySaleCurrency";
                generatedInvoiceItemSet.Items = new List<GeneratedInvoiceItem>();

                foreach (var supplierInvoiceBySaleCurrency in supplierInvoicesBySaleCurrency)
                {
                    generatedInvoiceItemSet.Items.Add(new GeneratedInvoiceItem
                    {
                        Details = supplierInvoiceBySaleCurrency,
                        Name = " "
                    });
                }
              
                generatedInvoiceItemSets.Add(generatedInvoiceItemSet);

            }

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
                        SupplierInvoiceItemDetails supplierInvoiceItemDetails = new Entities.SupplierInvoiceItemDetails()
                        {
                            Duration = item.InvoiceMeasures.CostDuration,
                            NumberOfCalls = item.InvoiceMeasures.NumberOfCalls,
                            OriginalSupplierCurrencyId = item.OriginalSupplierCurrencyId,
                            OriginalCostAmount = item.InvoiceMeasures.CostNet_OrigCurr,
                            CostAmount = item.InvoiceMeasures.CostNet,
                            SupplierZoneId = item.SupplierZoneId,
                            SupplierId = item.SupplierId,
                            SupplierCurrencyId = item.SupplierCurrencyId,
                            SupplierRate = item.SupplierRate,
                            SupplierRateTypeId = item.SupplierRateTypeId,
                            FromDate = item.InvoiceMeasures.BillingPeriodFrom,
                            ToDate = item.InvoiceMeasures.BillingPeriodTo,
                            AmountAfterCommission = item.InvoiceMeasures.AmountAfterCommission,
                            OriginalAmountAfterCommission = item.InvoiceMeasures.OriginalAmountAfterCommission,
                            AmountAfterCommissionWithTaxes = item.InvoiceMeasures.AmountAfterCommissionWithTaxes,
                            OriginalAmountAfterCommissionWithTaxes = item.InvoiceMeasures.OriginalAmountAfterCommissionWithTaxes,
                            OriginalSupplierAmountWithTaxes = item.InvoiceMeasures.CostNet_OrigCurrWithTaxes,
                            SupplierAmountWithTaxes = item.InvoiceMeasures.CostNetWithTaxes,
                        };
                        generatedInvoiceItemSet.Items.Add(new GeneratedInvoiceItem
                        {
                            Details = supplierInvoiceItemDetails,
                            Name = " "
                        });
                    }
                    if (generatedInvoiceItemSet.Items.Count > 0)
                    {
                        generatedInvoiceItemSets.Add(generatedInvoiceItemSet);
                    }
                }
                if (generatedInvoiceItemSets.Count > 0)
                {
                    if (taxItemDetails != null)
                    {
                        GeneratedInvoiceItemSet generatedInvoiceItemSet = new GeneratedInvoiceItemSet();
                        generatedInvoiceItemSet.SetName = "Taxes";
                        generatedInvoiceItemSet.Items = new List<GeneratedInvoiceItem>();
                        foreach (var item in taxItemDetails)
                        {
                            generatedInvoiceItemSet.Items.Add(new GeneratedInvoiceItem
                            {
                                Details = item,
                                Name = " "
                            });
                        }
                        generatedInvoiceItemSets.Add(generatedInvoiceItemSet);
                    }
                }
            }
            return generatedInvoiceItemSets;
        }
        private AnalyticSummaryBigResult<AnalyticRecord> GetFilteredRecords(List<string> listDimensions, List<string> listMeasures, string dimentionFilterName, object dimentionFilterValue, DateTime fromDate, DateTime toDate,int? currencyId)
        {
            AnalyticManager analyticManager = new AnalyticManager();
            Vanrise.Entities.DataRetrievalInput<AnalyticQuery> analyticQuery = new DataRetrievalInput<AnalyticQuery>()
            {
                Query = new AnalyticQuery()
                {
                    DimensionFields = listDimensions,
                    MeasureFields = listMeasures,
                    TableId = Guid.Parse("4C1AAA1B-675B-420F-8E60-26B0747CA79B"),
                    FromTime = fromDate,
                    ToTime = toDate,
                    ParentDimensions = new List<string>(),
                    Filters = new List<DimensionFilter>(),
                    CurrencyId = currencyId,
                  //  OrderType = AnalyticQueryOrderType.ByAllDimensions
                },
                SortByColumnName = "DimensionValues[0].Name"
            };
            DimensionFilter dimensionFilter = new DimensionFilter()
            {
                Dimension = dimentionFilterName,
                FilterValues = new List<object> { dimentionFilterValue }
            };
            analyticQuery.Query.Filters.Add(dimensionFilter);
            return analyticManager.GetFilteredRecords(analyticQuery) as Vanrise.Analytic.Entities.AnalyticSummaryBigResult<AnalyticRecord>;
        }
        private MeasureValue GetMeasureValue(AnalyticRecord analyticRecord,string measureName)
        {
            MeasureValue measureValue;
            analyticRecord.MeasureValues.TryGetValue(measureName, out measureValue);
            return measureValue;
        }
        private Dictionary<string, List<InvoiceBillingRecord>> ConvertAnalyticDataToDictionary(IEnumerable<AnalyticRecord> analyticRecords, int currencyId, decimal? commission, CommissionType? commissionType, IEnumerable<VRTaxItemDetail> taxItemDetails)
        {
            Dictionary<string, List<InvoiceBillingRecord>> itemSetNamesDic = new Dictionary<string, List<InvoiceBillingRecord>>();
            if (analyticRecords != null)
            {
                foreach (var analyticRecord in analyticRecords)
                {

                    #region ReadDataFromAnalyticResult
                    DimensionValue supplierZoneId = analyticRecord.DimensionValues.ElementAtOrDefault(0);
                    DimensionValue supplierId = analyticRecord.DimensionValues.ElementAtOrDefault(1);
                    DimensionValue supplierCurrencyId = analyticRecord.DimensionValues.ElementAtOrDefault(2);
                    DimensionValue supplierRate = analyticRecord.DimensionValues.ElementAtOrDefault(3);
                    DimensionValue supplierRateTypeId = analyticRecord.DimensionValues.ElementAtOrDefault(4);

                    MeasureValue costNet_OrigCurr = GetMeasureValue(analyticRecord, "CostNet_OrigCurr");
                    MeasureValue costDuration = GetMeasureValue(analyticRecord, "CostDuration");
                    MeasureValue costNet = GetMeasureValue(analyticRecord, "CostNetNotNULL");
                    MeasureValue calls = GetMeasureValue(analyticRecord, "NumberOfCalls");
                    MeasureValue billingPeriodTo = GetMeasureValue(analyticRecord, "BillingPeriodTo");
                    MeasureValue billingPeriodFrom = GetMeasureValue(analyticRecord, "BillingPeriodFrom");
                    #endregion

                    var costNetValue = Convert.ToDecimal(costNet == null ? 0.0 : costNet.Value ?? 0.0);
                    if (costNetValue != 0)
                    {
                        InvoiceBillingRecord invoiceBillingRecord = new InvoiceBillingRecord
                        {
                            SupplierId = Convert.ToInt32(supplierId.Value),
                            SupplierCurrencyId = currencyId,
                            OriginalSupplierCurrencyId = Convert.ToInt32(supplierCurrencyId.Value),
                            SupplierRate = supplierRate != null ? Convert.ToDecimal(supplierRate.Value) : default(Decimal),
                            SupplierRateTypeId = supplierRateTypeId != null && supplierRateTypeId.Value != null ? Convert.ToInt32(supplierRateTypeId.Value) : default(int?),
                            SupplierZoneId = Convert.ToInt64(supplierZoneId.Value),
                            InvoiceMeasures = new InvoiceMeasures
                            {
                                BillingPeriodFrom = billingPeriodFrom != null ? Convert.ToDateTime(billingPeriodFrom.Value) : default(DateTime),
                                BillingPeriodTo = billingPeriodTo != null ? Convert.ToDateTime(billingPeriodTo.Value) : default(DateTime),
                                CostDuration = Convert.ToDecimal(costDuration.Value ?? 0.0),
                                CostNet = costNetValue,
                                NumberOfCalls = Convert.ToInt32(calls.Value ?? 0.0),
                                CostNet_OrigCurr = Convert.ToDecimal(costNet_OrigCurr == null ? 0.0 : costNet_OrigCurr.Value ?? 0.0),
                            }

                        };
                        if(commission.HasValue)
                        {
                            if (commissionType.HasValue && commissionType.Value == CommissionType.DoNotDisplay)
                            {
                                invoiceBillingRecord.SupplierRate = invoiceBillingRecord.SupplierRate + ((invoiceBillingRecord.SupplierRate * commission.Value) / 100);
                            }
                            invoiceBillingRecord.InvoiceMeasures.OriginalAmountAfterCommission = invoiceBillingRecord.InvoiceMeasures.CostNet_OrigCurr + ((invoiceBillingRecord.InvoiceMeasures.CostNet_OrigCurr * commission.Value) / 100);
                            invoiceBillingRecord.InvoiceMeasures.AmountAfterCommission = invoiceBillingRecord.InvoiceMeasures.CostNet + ((invoiceBillingRecord.InvoiceMeasures.CostNet * commission.Value) / 100);
                        }
                        else
                        {
                            invoiceBillingRecord.InvoiceMeasures.OriginalAmountAfterCommission = invoiceBillingRecord.InvoiceMeasures.CostNet_OrigCurr;
                            invoiceBillingRecord.InvoiceMeasures.AmountAfterCommission = invoiceBillingRecord.InvoiceMeasures.CostNet;
                        }
                       
                        invoiceBillingRecord.InvoiceMeasures.AmountAfterCommissionWithTaxes = invoiceBillingRecord.InvoiceMeasures.AmountAfterCommission ;
                        invoiceBillingRecord.InvoiceMeasures.OriginalAmountAfterCommissionWithTaxes = invoiceBillingRecord.InvoiceMeasures.OriginalAmountAfterCommission;
                        invoiceBillingRecord.InvoiceMeasures.CostNet_OrigCurrWithTaxes = invoiceBillingRecord.InvoiceMeasures.CostNet_OrigCurr;
                        invoiceBillingRecord.InvoiceMeasures.CostNetWithTaxes = invoiceBillingRecord.InvoiceMeasures.CostNet;

                        if (taxItemDetails != null)
                        {
                            foreach (var tax in taxItemDetails)
                            {
                                invoiceBillingRecord.InvoiceMeasures.AmountAfterCommissionWithTaxes += ((invoiceBillingRecord.InvoiceMeasures.AmountAfterCommission * Convert.ToDecimal(tax.Value)) / 100);

                                invoiceBillingRecord.InvoiceMeasures.OriginalAmountAfterCommissionWithTaxes +=((invoiceBillingRecord.InvoiceMeasures.OriginalAmountAfterCommission * Convert.ToDecimal(tax.Value)) / 100);

                                invoiceBillingRecord.InvoiceMeasures.CostNet_OrigCurrWithTaxes += ((invoiceBillingRecord.InvoiceMeasures.CostNet_OrigCurr * Convert.ToDecimal(tax.Value)) / 100);

                                invoiceBillingRecord.InvoiceMeasures.CostNetWithTaxes +=((invoiceBillingRecord.InvoiceMeasures.CostNet * Convert.ToDecimal(tax.Value)) / 100);
                            }
                        }

                        AddItemToDictionary(itemSetNamesDic, "GroupedByCostZone", invoiceBillingRecord);

                    }
                    
                }
            }
            return itemSetNamesDic;
        }
        private void AddItemToDictionary<T>(Dictionary<T, List<InvoiceBillingRecord>> itemSetNamesDic, T key, InvoiceBillingRecord invoiceBillingRecord)
        {
            if (itemSetNamesDic == null)
                itemSetNamesDic = new Dictionary<T, List<InvoiceBillingRecord>>();
            List<InvoiceBillingRecord> invoiceBillingRecordList = null;

            if (!itemSetNamesDic.TryGetValue(key, out invoiceBillingRecordList))
            {
                invoiceBillingRecordList = new List<InvoiceBillingRecord>();
                invoiceBillingRecordList.Add(invoiceBillingRecord);
                itemSetNamesDic.Add(key, invoiceBillingRecordList);
            }else
            {
                invoiceBillingRecordList.Add(invoiceBillingRecord);
                itemSetNamesDic[key] = invoiceBillingRecordList;
            }
        }
        public class InvoiceMeasures
        {
            public decimal CostNet { get; set; }
            public decimal CostNetWithTaxes { get; set; }
            public decimal CostNet_OrigCurr { get; set; }
            public decimal CostNet_OrigCurrWithTaxes { get; set; }
            public int NumberOfCalls { get; set; }
            public Decimal CostDuration { get; set; }
            public DateTime BillingPeriodTo { get; set; }
            public DateTime BillingPeriodFrom { get; set; }
            public decimal AmountAfterCommission { get; set; }
            public decimal OriginalAmountAfterCommission { get; set; }
            public decimal AmountAfterCommissionWithTaxes { get; set; }
            public decimal OriginalAmountAfterCommissionWithTaxes { get; set; }

        } 
        public  class InvoiceBillingRecord
        {
            public InvoiceMeasures InvoiceMeasures { get; set; }
            public long SupplierZoneId { get; set; }
            public int SupplierId { get; set; }
            public int OriginalSupplierCurrencyId { get; set; }
            public Decimal SupplierRate { get; set; }
            public int? SupplierRateTypeId { get; set; }
            public int SupplierCurrencyId { get; set; }

        }
    }
}
