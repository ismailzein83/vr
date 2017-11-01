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
    public class CustomerInvoiceGenerator : InvoiceGenerator
    {
        public override void GenerateInvoice(IInvoiceGenerationContext context)
        {
            List<string> listMeasures = new List<string> { "SaleNetNotNULL", "NumberOfCalls", "SaleDuration", "BillingPeriodTo", "BillingPeriodFrom", "SaleNet_OrigCurr" };
            List<string> listDimensions = new List<string> { "Customer", "SaleZone", "SaleCurrency", "SaleRate", "SaleRateType", "Supplier", "Country", "SupplierZone" };
            WHSFinancialAccountManager financialAccountManager = new WHSFinancialAccountManager();
            var financialAccount = financialAccountManager.GetFinancialAccount(Convert.ToInt32(context.PartnerId));

            var customerGenerationCustomSectionPayload = context.CustomSectionPayload as CustomerGenerationCustomSectionPayload;
            int? timeZoneId = null;
            decimal? commission = null;
            CommissionType? commissionType = null;
            if (customerGenerationCustomSectionPayload != null && customerGenerationCustomSectionPayload.TimeZoneId.HasValue)
            {
                timeZoneId = customerGenerationCustomSectionPayload.TimeZoneId;
                if (customerGenerationCustomSectionPayload.Commission.HasValue)
                {
                    commission = customerGenerationCustomSectionPayload.Commission.Value;
                    commissionType = customerGenerationCustomSectionPayload.CommissionType;
                }
               
            }
            if (!timeZoneId.HasValue)
            {
                timeZoneId = financialAccountManager.GetCustomerTimeZoneId(financialAccount.FinancialAccountId);
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
            string partnerType = null;
            string dimentionName = "SaleFinancialAccount";
            int dimensionValue = financialAccount.FinancialAccountId;
            int currencyId = financialAccountManager.GetFinancialAccountCurrencyId(financialAccount);

            if (financialAccount.CarrierProfileId.HasValue)
            {
                partnerType = "Profile";
            }
            else
            {
                partnerType = "Account";
            }
            IEnumerable<VRTaxItemDetail>  taxItemDetails = financialAccountManager.GetFinancialAccountTaxItemDetails(financialAccount);
            var analyticResult = GetFilteredRecords(listDimensions, listMeasures, dimentionName, dimensionValue, fromDate, toDate, currencyId);
            if (analyticResult == null || analyticResult.Data == null || analyticResult.Data.Count() == 0)
            {
                context.ErrorMessage = "No data available between the selected period."; 
                throw new InvoiceGeneratorException("No data available between the selected period.");
            }
            Dictionary<string, List<InvoiceBillingRecord>> itemSetNamesDic = ConvertAnalyticDataToDictionary(analyticResult.Data, currencyId, commission, commissionType, taxItemDetails);
            List<GeneratedInvoiceItemSet> generatedInvoiceItemSets = BuildGeneratedInvoiceItemSet(itemSetNamesDic, taxItemDetails);
            #region BuildCustomerInvoiceDetails
            CustomerInvoiceDetails customerInvoiceDetails = BuilCustomerInvoiceDetails(itemSetNamesDic, partnerType, context.FromDate, context.ToDate, commission, commissionType);
            if (customerInvoiceDetails != null && customerInvoiceDetails.SaleAmount != 0)
            {
                customerInvoiceDetails.TimeZoneId = timeZoneId;
                customerInvoiceDetails.Commission = commission;
                customerInvoiceDetails.TotalAmount = customerInvoiceDetails.SaleAmount;
                customerInvoiceDetails.CommissionType = commissionType;
                customerInvoiceDetails.Offset = offset;

                customerInvoiceDetails.TotalAmountAfterCommission = customerInvoiceDetails.AmountAfterCommission;
                customerInvoiceDetails.TotalOriginalAmountAfterCommission = customerInvoiceDetails.OriginalAmountAfterCommission;
                if (taxItemDetails != null)
                {
                    foreach (var tax in taxItemDetails)
                    {
                        customerInvoiceDetails.TotalAmountAfterCommission += ((customerInvoiceDetails.AmountAfterCommission * Convert.ToDecimal(tax.Value)) / 100);
                        customerInvoiceDetails.TotalOriginalAmountAfterCommission += ((customerInvoiceDetails.OriginalAmountAfterCommission * Convert.ToDecimal(tax.Value)) / 100);
                        customerInvoiceDetails.TotalAmount += ((customerInvoiceDetails.SaleAmount * Convert.ToDecimal(tax.Value)) / 100);
                    }
                }
                SetInvoiceBillingTransactions(context, customerInvoiceDetails, financialAccount, fromDate, toDateForBillingTransaction);
                context.Invoice = new GeneratedInvoice
                {
                    InvoiceDetails = customerInvoiceDetails,
                    InvoiceItemSets = generatedInvoiceItemSets,
                };

            }

            #endregion

           
        }
        private void SetInvoiceBillingTransactions(IInvoiceGenerationContext context, CustomerInvoiceDetails invoiceDetails, WHSFinancialAccount financialAccount, DateTime fromDate, DateTime toDate)
        {
            var financialAccountDefinitionManager = new WHSFinancialAccountDefinitionManager();
            var balanceAccountTypeId = financialAccountDefinitionManager.GetBalanceAccountTypeId(financialAccount.FinancialAccountDefinitionId);
            if (balanceAccountTypeId.HasValue)
            {
                Vanrise.Invoice.Entities.InvoiceType invoiceType = new Vanrise.Invoice.Business.InvoiceTypeManager().GetInvoiceType(context.InvoiceTypeId);
                invoiceType.ThrowIfNull("invoiceType", context.InvoiceTypeId);
                invoiceType.Settings.ThrowIfNull("invoiceType.Settings", context.InvoiceTypeId);
                CustomerInvoiceSettings invoiceSettings = invoiceType.Settings.ExtendedSettings.CastWithValidate<CustomerInvoiceSettings>("invoiceType.Settings.ExtendedSettings");

                var billingTransaction = new GeneratedInvoiceBillingTransaction()
                {
                    AccountTypeId = balanceAccountTypeId.Value,
                    AccountId = context.PartnerId,
                    TransactionTypeId = invoiceSettings.InvoiceTransactionTypeId,
                    Amount = invoiceDetails.TotalAmount,
                    CurrencyId = invoiceDetails.SaleCurrencyId,
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
        private CustomerInvoiceDetails BuilCustomerInvoiceDetails(Dictionary<string, List<InvoiceBillingRecord>> itemSetNamesDic, string partnerType, DateTime fromDate, DateTime toDate,decimal? commission, CommissionType? commissionType)
        {
            CurrencyManager currencyManager = new CurrencyManager();
            CustomerInvoiceDetails customerInvoiceDetails = null;
            if (partnerType != null)
            {

                if (itemSetNamesDic != null)
                {
                    List<InvoiceBillingRecord> invoiceBillingRecordList = null;
                    if (itemSetNamesDic.TryGetValue("GroupedBySaleZone", out invoiceBillingRecordList))
                    {
                        customerInvoiceDetails = new CustomerInvoiceDetails();
                        customerInvoiceDetails.PartnerType = partnerType;
                        foreach (var invoiceBillingRecord in invoiceBillingRecordList)
                        {

                            customerInvoiceDetails.Duration += invoiceBillingRecord.InvoiceMeasures.SaleDuration;
                            customerInvoiceDetails.SaleAmount += invoiceBillingRecord.InvoiceMeasures.SaleNet;
                            customerInvoiceDetails.OriginalSaleAmount += invoiceBillingRecord.InvoiceMeasures.SaleNet_OrigCurr;
                            customerInvoiceDetails.TotalNumberOfCalls += invoiceBillingRecord.InvoiceMeasures.NumberOfCalls;
                            customerInvoiceDetails.OriginalSaleCurrencyId = invoiceBillingRecord.OriginalSaleCurrencyId;
                            customerInvoiceDetails.SaleCurrencyId = invoiceBillingRecord.SaleCurrencyId;
                            customerInvoiceDetails.CountryId = invoiceBillingRecord.CountryId;
                            customerInvoiceDetails.SupplierId = invoiceBillingRecord.SupplierId;
                            customerInvoiceDetails.SupplierZoneId = invoiceBillingRecord.SupplierZoneId;
                            customerInvoiceDetails.AmountAfterCommission += invoiceBillingRecord.InvoiceMeasures.AmountAfterCommission;
                            customerInvoiceDetails.OriginalAmountAfterCommission += invoiceBillingRecord.InvoiceMeasures.OriginalAmountAfterCommission;
                        }
                        if (commissionType.HasValue)
                        {
                            switch (commissionType.Value)
                            {
                                case CommissionType.Display:
                                    customerInvoiceDetails.DisplayComission = true;
                                    break;
                            }
                        }
                        else
                        {
                            customerInvoiceDetails.DisplayComission = false;
                        }
                    };
                }
            }
            if (customerInvoiceDetails != null && customerInvoiceDetails.SaleAmount > 0)
            {
                customerInvoiceDetails.OriginalSaleCurrency = currencyManager.GetCurrencySymbol(customerInvoiceDetails.OriginalSaleCurrencyId);
                customerInvoiceDetails.SaleCurrency = currencyManager.GetCurrencySymbol(customerInvoiceDetails.SaleCurrencyId);
            }
            return customerInvoiceDetails;
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
                        CustomerInvoiceItemDetails customerInvoiceItemDetails = new Entities.CustomerInvoiceItemDetails()
                        {
                            Duration = item.InvoiceMeasures.SaleDuration,
                            NumberOfCalls = item.InvoiceMeasures.NumberOfCalls,
                            OriginalSaleCurrencyId = item.OriginalSaleCurrencyId,
                            OriginalSaleAmount = item.InvoiceMeasures.SaleNet_OrigCurr,
                            SaleAmount = item.InvoiceMeasures.SaleNet,
                            SaleZoneId = item.SaleZoneId,
                            CustomerId = item.CustomerId,
                            SaleCurrencyId = item.SaleCurrencyId,
                            SaleRate = item.SaleRate,
                            SaleRateTypeId = item.SaleRateTypeId,
                            FromDate = item.InvoiceMeasures.BillingPeriodFrom,
                            ToDate = item.InvoiceMeasures.BillingPeriodTo,
                            CountryId = item.CountryId,
                            SupplierId = item.SupplierId,
                            SupplierZoneId = item.SupplierZoneId,
                            AmountAfterCommission = item.InvoiceMeasures.AmountAfterCommission,
                            OriginalAmountAfterCommission = item.InvoiceMeasures.OriginalAmountAfterCommission,
                            AmountAfterCommissionWithTaxes = item.InvoiceMeasures.AmountAfterCommissionWithTaxes,
                            OriginalAmountAfterCommissionWithTaxes =item.InvoiceMeasures.OriginalAmountAfterCommissionWithTaxes,
                            OriginalSaleAmountWithTaxes=item.InvoiceMeasures.SaleNet_OrigCurrWithTaxes,
                            SaleAmountWithTaxes = item.InvoiceMeasures.SaleNetWithTaxes,
                        };
                        generatedInvoiceItemSet.Items.Add(new GeneratedInvoiceItem
                        {
                            Details = customerInvoiceItemDetails,
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
        private AnalyticSummaryBigResult<AnalyticRecord> GetFilteredRecords(List<string> listDimensions, List<string> listMeasures, string dimentionFilterName, object dimentionFilterValue, DateTime fromDate, DateTime toDate, int currencyId)
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
        private MeasureValue GetMeasureValue(AnalyticRecord analyticRecord, string measureName)
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
                    DimensionValue customerId = analyticRecord.DimensionValues.ElementAtOrDefault(0);
                    DimensionValue saleZoneId = analyticRecord.DimensionValues.ElementAtOrDefault(1);
                    DimensionValue saleCurrencyId = analyticRecord.DimensionValues.ElementAtOrDefault(2);
                    DimensionValue saleRate = analyticRecord.DimensionValues.ElementAtOrDefault(3);
                    DimensionValue saleRateTypeId = analyticRecord.DimensionValues.ElementAtOrDefault(4);
                    DimensionValue supplier = analyticRecord.DimensionValues.ElementAtOrDefault(5);
                    DimensionValue country = analyticRecord.DimensionValues.ElementAtOrDefault(6);
                    DimensionValue supplierZone = analyticRecord.DimensionValues.ElementAtOrDefault(7);


                    MeasureValue saleNet_OrigCurr = GetMeasureValue(analyticRecord, "SaleNet_OrigCurr");
                    MeasureValue saleDuration = GetMeasureValue(analyticRecord, "SaleDuration");
                    MeasureValue saleNet = GetMeasureValue(analyticRecord, "SaleNetNotNULL");
                    MeasureValue calls = GetMeasureValue(analyticRecord, "NumberOfCalls");
                    MeasureValue billingPeriodTo = GetMeasureValue(analyticRecord, "BillingPeriodTo");
                    MeasureValue billingPeriodFrom = GetMeasureValue(analyticRecord, "BillingPeriodFrom");
                    #endregion
                    var saleNetValue = Convert.ToDecimal(saleNet == null ? 0.0 : saleNet.Value ?? 0.0);
                    if(saleNetValue != 0)
                    {
                        InvoiceBillingRecord invoiceBillingRecord = new InvoiceBillingRecord
                        {
                            CustomerId = Convert.ToInt32(customerId.Value),
                            SaleCurrencyId = currencyId,
                            OriginalSaleCurrencyId = Convert.ToInt32(saleCurrencyId.Value),
                            SaleRate = saleRate != null ? Convert.ToDecimal(saleRate.Value) : default(Decimal),
                            SaleRateTypeId = saleRateTypeId != null && saleRateTypeId.Value != null ? Convert.ToInt32(saleRateTypeId.Value) : default(int?),
                            SaleZoneId = Convert.ToInt64(saleZoneId.Value),
                            InvoiceMeasures = new InvoiceMeasures
                            {
                                BillingPeriodFrom = billingPeriodFrom != null ? Convert.ToDateTime(billingPeriodFrom.Value) : default(DateTime),
                                BillingPeriodTo = billingPeriodTo != null ? Convert.ToDateTime(billingPeriodTo.Value) : default(DateTime),
                                SaleDuration = Convert.ToDecimal(saleDuration.Value ?? 0.0),
                                SaleNet = saleNetValue,
                                NumberOfCalls = Convert.ToInt32(calls.Value ?? 0.0),
                                SaleNet_OrigCurr = Convert.ToDecimal(saleNet_OrigCurr == null ? 0.0 : saleNet_OrigCurr.Value ?? 0.0),
                            },
                            CountryId = Convert.ToInt32(country.Value),
                            SupplierId = Convert.ToInt32(supplier.Value),
                            SupplierZoneId = Convert.ToInt32(supplierZone.Value),

                        };
                        if (commission.HasValue)
                        {
                            if (commissionType.HasValue && commissionType.Value == CommissionType.DoNotDisplay)
                            {
                                invoiceBillingRecord.SaleRate = invoiceBillingRecord.SaleRate + ((invoiceBillingRecord.SaleRate * commission.Value) / 100);
                            }

                            invoiceBillingRecord.InvoiceMeasures.OriginalAmountAfterCommission = invoiceBillingRecord.InvoiceMeasures.SaleNet_OrigCurr + ((invoiceBillingRecord.InvoiceMeasures.SaleNet_OrigCurr * commission.Value) / 100);
                            invoiceBillingRecord.InvoiceMeasures.AmountAfterCommission = invoiceBillingRecord.InvoiceMeasures.SaleNet + ((invoiceBillingRecord.InvoiceMeasures.SaleNet * commission.Value) / 100);
                        }else
                        {
                            invoiceBillingRecord.InvoiceMeasures.OriginalAmountAfterCommission = invoiceBillingRecord.InvoiceMeasures.SaleNet_OrigCurr;
                            invoiceBillingRecord.InvoiceMeasures.AmountAfterCommission = invoiceBillingRecord.InvoiceMeasures.SaleNet;
                        }
                        if (taxItemDetails != null)
                        {
                            foreach (var tax in taxItemDetails)
                            {
                                invoiceBillingRecord.InvoiceMeasures.AmountAfterCommissionWithTaxes = invoiceBillingRecord.InvoiceMeasures.AmountAfterCommission + ((invoiceBillingRecord.InvoiceMeasures.AmountAfterCommission * Convert.ToDecimal(tax.Value)) / 100);

                                invoiceBillingRecord.InvoiceMeasures.OriginalAmountAfterCommissionWithTaxes = invoiceBillingRecord.InvoiceMeasures.OriginalAmountAfterCommission + ((invoiceBillingRecord.InvoiceMeasures.OriginalAmountAfterCommission * Convert.ToDecimal(tax.Value)) / 100);

                                invoiceBillingRecord.InvoiceMeasures.SaleNet_OrigCurrWithTaxes = invoiceBillingRecord.InvoiceMeasures.SaleNet_OrigCurr + ((invoiceBillingRecord.InvoiceMeasures.SaleNet_OrigCurr * Convert.ToDecimal(tax.Value)) / 100);

                                invoiceBillingRecord.InvoiceMeasures.SaleNetWithTaxes = invoiceBillingRecord.InvoiceMeasures.SaleNet +((invoiceBillingRecord.InvoiceMeasures.SaleNet * Convert.ToDecimal(tax.Value)) / 100);
                            }
                        }

                        AddItemToDictionary(itemSetNamesDic, "GroupedBySaleZone", invoiceBillingRecord);
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
            }
            else
            {
                invoiceBillingRecordList.Add(invoiceBillingRecord);
                itemSetNamesDic[key] = invoiceBillingRecordList;
            }
        }
        public class InvoiceMeasures
        {
            public decimal SaleNet { get; set; }
            public decimal SaleNetWithTaxes { get; set; }

            public decimal SaleNet_OrigCurr { get; set; }
            public decimal SaleNet_OrigCurrWithTaxes { get; set; }

            public int NumberOfCalls { get; set; }
            public Decimal SaleDuration { get; set; }
            public DateTime BillingPeriodTo { get; set; }
            public DateTime BillingPeriodFrom { get; set; }
            public decimal AmountAfterCommission { get; set; }
            public decimal OriginalAmountAfterCommission { get; set; }
            public decimal AmountAfterCommissionWithTaxes { get; set; }
            public decimal OriginalAmountAfterCommissionWithTaxes { get; set; }

        }
        public class InvoiceBillingRecord
        {
            public InvoiceMeasures InvoiceMeasures { get; set; }
            public long SaleZoneId { get; set; }
            public int CustomerId { get; set; }
            public int OriginalSaleCurrencyId { get; set; }
            public Decimal SaleRate { get; set; }
            public int? SaleRateTypeId { get; set; }
            public int SaleCurrencyId { get; set; }
            public int CountryId { get; set; }
            public int SupplierId { get; set; }
            public int SupplierZoneId { get; set; }
        }
    }
}
