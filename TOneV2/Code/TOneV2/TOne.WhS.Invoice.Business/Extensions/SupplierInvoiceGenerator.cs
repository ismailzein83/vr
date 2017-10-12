﻿using System;
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
            List<string> listDimensions = new List<string> { "Supplier", "SupplierZone", "CostCurrency", "CostRate", "CostRateType" };
            WHSFinancialAccountManager financialAccountManager = new WHSFinancialAccountManager();
            var financialAccount = financialAccountManager.GetFinancialAccount(Convert.ToInt32(context.PartnerId));
            var supplierGenerationCustomSectionPayload = context.CustomSectionPayload as SupplierGenerationCustomSectionPayload;

            int? timeZoneId = null;
            decimal commission = 0;
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
            if (timeZoneId.HasValue)
            {
                VRTimeZone timeZone = new VRTimeZoneManager().GetVRTimeZone(timeZoneId.Value);
                if (timeZone != null)
                {
                    offset = timeZone.Settings.Offset.ToString();
                    fromDate = context.FromDate.Add(-timeZone.Settings.Offset);
                    toDate = context.ToDate.Add(-timeZone.Settings.Offset);
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
                throw new InvoiceGeneratorException("No data available between the selected period.");
            }
            Dictionary<string, List<InvoiceBillingRecord>> itemSetNamesDic = ConvertAnalyticDataToDictionary(analyticResult.Data, currencyId, commission, commissionType);
            List<GeneratedInvoiceItemSet> generatedInvoiceItemSets = BuildGeneratedInvoiceItemSet(itemSetNamesDic, taxItemDetails);
            #region BuildSupplierInvoiceDetails
            SupplierInvoiceDetails supplierInvoiceDetails = BuilSupplierInvoiceDetails(itemSetNamesDic, partnerType, context.FromDate, context.ToDate, commission, commissionType);
            if (supplierInvoiceDetails != null)
            {
                supplierInvoiceDetails.TimeZoneId = timeZoneId;
                supplierInvoiceDetails.TotalAmount = supplierInvoiceDetails.CostAmount;
                supplierInvoiceDetails.TotalAmountAfterCommission = supplierInvoiceDetails.AmountAfterCommission;
                supplierInvoiceDetails.Commission = commission;
                supplierInvoiceDetails.CommissionType = commissionType;
                if (taxItemDetails != null)
                {
                    foreach (var tax in taxItemDetails)
                    {
                        supplierInvoiceDetails.TotalAmountAfterCommission += ((supplierInvoiceDetails.AmountAfterCommission * Convert.ToDecimal(tax.Value)) / 100);
                        supplierInvoiceDetails.TotalAmount += ((supplierInvoiceDetails.CostAmount * Convert.ToDecimal(tax.Value)) / 100);
                    }
                }
                SetInvoiceBillingTransactions(context, supplierInvoiceDetails, financialAccount);
            }
            #endregion

            context.Invoice = new GeneratedInvoice
            {
                InvoiceDetails = supplierInvoiceDetails,
                InvoiceItemSets = generatedInvoiceItemSets,
            };
        }
        private void SetInvoiceBillingTransactions(IInvoiceGenerationContext context, SupplierInvoiceDetails invoiceDetails, WHSFinancialAccount financialAccount)
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
                    Amount = invoiceDetails.TotalAmount,
                    CurrencyId = invoiceDetails.SupplierCurrencyId
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
        private SupplierInvoiceDetails BuilSupplierInvoiceDetails(Dictionary<string, List<InvoiceBillingRecord>> itemSetNamesDic, string partnerType, DateTime fromDate, DateTime toDate, decimal commission, CommissionType? commissionType)
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
            supplierInvoiceDetails.OriginalSupplierCurrency = currencyManager.GetCurrencySymbol(supplierInvoiceDetails.OriginalSupplierCurrencyId);
            supplierInvoiceDetails.SupplierCurrency = currencyManager.GetCurrencySymbol(supplierInvoiceDetails.SupplierCurrencyId);
            return supplierInvoiceDetails;
        }

        private List<GeneratedInvoiceItemSet> BuildGeneratedInvoiceItemSet(Dictionary<string, List<InvoiceBillingRecord>> itemSetNamesDic, IEnumerable<VRTaxItemDetail> taxItemDetails)
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

                        };
                        generatedInvoiceItemSet.Items.Add(new GeneratedInvoiceItem
                        {
                            Details = supplierInvoiceItemDetails,
                            Name = " "
                        });
                    }
                    generatedInvoiceItemSets.Add(generatedInvoiceItemSet);

                }
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
            return generatedInvoiceItemSets;
        }
        private AnalyticSummaryBigResult<AnalyticRecord> GetFilteredRecords(List<string> listDimensions, List<string> listMeasures, string dimentionFilterName, object dimentionFilterValue, DateTime fromDate, DateTime toDate,int currencyId)
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
                    CurrencyId = currencyId
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
        private Dictionary<string, List<InvoiceBillingRecord>> ConvertAnalyticDataToDictionary(IEnumerable<AnalyticRecord> analyticRecords, int currencyId, decimal commission, CommissionType? commissionType)
        {
            Dictionary<string, List<InvoiceBillingRecord>> itemSetNamesDic = new Dictionary<string, List<InvoiceBillingRecord>>();
            if (analyticRecords != null)
            {
                foreach (var analyticRecord in analyticRecords)
                {

                    #region ReadDataFromAnalyticResult
                    DimensionValue supplierId = analyticRecord.DimensionValues.ElementAtOrDefault(0);
                    DimensionValue supplierZoneId = analyticRecord.DimensionValues.ElementAtOrDefault(1);
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
                            CostNet = Convert.ToDecimal(costNet == null ? 0.0 : costNet.Value ?? 0.0),
                            NumberOfCalls = Convert.ToInt32(calls.Value ?? 0.0),
                            CostNet_OrigCurr = Convert.ToDecimal(costNet_OrigCurr == null ? 0.0 : costNet_OrigCurr.Value ?? 0.0),
                        }

                    };
                    invoiceBillingRecord.SupplierRate = invoiceBillingRecord.SupplierRate + ((invoiceBillingRecord.SupplierRate * commission) / 100);
                    invoiceBillingRecord.InvoiceMeasures.AmountAfterCommission = invoiceBillingRecord.InvoiceMeasures.CostNet_OrigCurr + ((invoiceBillingRecord.InvoiceMeasures.CostNet_OrigCurr * commission) / 100);

                    AddItemToDictionary(itemSetNamesDic, "GroupedByCostZone", invoiceBillingRecord);
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
            public decimal CostNet_OrigCurr { get; set; }
            public int NumberOfCalls { get; set; }
            public Decimal CostDuration { get; set; }
            public DateTime BillingPeriodTo { get; set; }
            public DateTime BillingPeriodFrom { get; set; }
            public decimal AmountAfterCommission { get; set; }


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
