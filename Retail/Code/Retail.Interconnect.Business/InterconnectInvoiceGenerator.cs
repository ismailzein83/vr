using Retail.BusinessEntity.Business;
using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Invoice.Entities;
using Retail.Interconnect.Entities;
using Retail.Invoice.Entities;
using Vanrise.Entities;
using Vanrise.Common.Business;
using Vanrise.Analytic.Entities;
using Vanrise.Analytic.Business;

namespace Retail.Interconnect.Business
{
    public class InterconnectInvoiceGenerator : InvoiceGenerator
    {
        #region Fields

        private FinancialAccountManager _financialAccountManager = new FinancialAccountManager();
        private AccountBEManager _accountBEManager = new AccountBEManager();

        #endregion

        #region Constructors
         Guid _acountBEDefinitionId;
         Guid _invoiceTransactionTypeId;
         List<Guid> _usageTransactionTypeIds;
         public InterconnectInvoiceGenerator(Guid acountBEDefinitionId, Guid invoiceTransactionTypeId, List<Guid> usageTransactionTypeIds)
         {
             this._acountBEDefinitionId = acountBEDefinitionId;
             this._invoiceTransactionTypeId = invoiceTransactionTypeId;
             this._usageTransactionTypeIds = usageTransactionTypeIds;
         }

        #endregion

        public override void GenerateInvoice(IInvoiceGenerationContext context)
        {
            List<string> listMeasures = new List<string> { "TotalDuration", "Amount"};
            List<string> listDimensions = new List<string> { "DestinationZone", "Operator", "Rate", "RateType", "TrafficDirection", "Currency" };

            FinancialAccountData financialAccountData = _financialAccountManager.GetFinancialAccountData(_acountBEDefinitionId, context.PartnerId);
           
            var interconnectGenerationCustomSectionPayload = context.CustomSectionPayload as InterconnectGenerationCustomSectionPayload;
            
            int? timeZoneId = null;
            decimal? commission = null;
            CommissionType? commissionType = null;

            if (interconnectGenerationCustomSectionPayload != null)
            {
                timeZoneId = interconnectGenerationCustomSectionPayload.TimeZoneId;
                if (interconnectGenerationCustomSectionPayload.Commission.HasValue)
                {
                    commission = interconnectGenerationCustomSectionPayload.Commission.Value;
                    commissionType = interconnectGenerationCustomSectionPayload.CommissionType;
                }
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
            int dimensionValue = Int32.Parse(financialAccountData.FinancialAccountId);
            int currencyId = _accountBEManager.GetCurrencyId(this._acountBEDefinitionId, financialAccountData.Account.AccountId);
           
            var analyticResult = GetFilteredRecords(listDimensions, listMeasures, dimentionName, dimensionValue, fromDate, toDate, currencyId);
            if (analyticResult == null || analyticResult.Data == null || analyticResult.Data.Count() == 0)
            {
                context.GenerateInvoiceResult = GenerateInvoiceResult.NoData;
                return;
            }

            Dictionary<string, List<InvoiceBillingRecord>> itemSetNamesDic = ConvertAnalyticDataToDictionary(analyticResult.Data, currencyId, commission, commissionType);
            if (itemSetNamesDic.Count == 0)
            {
                context.GenerateInvoiceResult = GenerateInvoiceResult.NoData;
                return;
            }

            var interconnectInvoiceBySaleCurrency = loadCurrencyItemSet(dimentionName, dimensionValue, fromDate, toDate, commission, commissionType);

            List<GeneratedInvoiceItemSet> generatedInvoiceItemSets = BuildGeneratedInvoiceItemSet(itemSetNamesDic, interconnectInvoiceBySaleCurrency);
            
            #region BuildInterconnectInvoiceDetails
            InterconnectInvoiceDetails interconnectInvoiceDetails = BuildInterconnectInvoiceDetails(itemSetNamesDic, context.FromDate, context.ToDate, commission, commissionType);
            if (interconnectInvoiceDetails != null && interconnectInvoiceDetails.CostAmount != 0)
            {
                interconnectInvoiceDetails.TimeZoneId = timeZoneId;
                interconnectInvoiceDetails.TotalAmount = interconnectInvoiceDetails.CostAmount;

                interconnectInvoiceDetails.Commission = commission;
                interconnectInvoiceDetails.CommissionType = commissionType;
                interconnectInvoiceDetails.Offset = offset;
                
                context.Invoice = new GeneratedInvoice
                {
                    InvoiceDetails = interconnectInvoiceDetails,
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
        private List<InterconnectInvoiceBySaleCurrencyItemDetails> loadCurrencyItemSet(string dimentionName, int dimensionValue, DateTime fromDate, DateTime toDate, decimal? commission, CommissionType? commissionType)
        {
            List<string> listMeasures = new List<string> { "NumberOfCalls", "CostDuration", "BillingPeriodTo", "BillingPeriodFrom", "CostNet_OrigCurr" };
            List<string> listDimensions = new List<string> { "CostCurrency" };
            var analyticResult = GetFilteredRecords(listDimensions, listMeasures, dimentionName, dimensionValue, fromDate, toDate, null);
            if (analyticResult != null && analyticResult.Data != null && analyticResult.Data.Count() != 0)
            {
                return BuildCurrencyItemSetNameFromAnalytic(analyticResult.Data, commission, commissionType);
            }
            return null;
        }
        private List<InterconnectInvoiceBySaleCurrencyItemDetails> BuildCurrencyItemSetNameFromAnalytic(IEnumerable<AnalyticRecord> analyticRecords, decimal? commission, CommissionType? commissionType)
        {
            List<InterconnectInvoiceBySaleCurrencyItemDetails> interconnectInvoiceBySaleCurrencies = null;

            if (analyticRecords != null)
            {
                interconnectInvoiceBySaleCurrencies = new List<InterconnectInvoiceBySaleCurrencyItemDetails>();
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
                        var interconnectInvoiceBySaleCurrencyItemDetails = new InterconnectInvoiceBySaleCurrencyItemDetails
                        {
                            CurrencyId = Convert.ToInt32(costCurrencyId.Value),
                            FromDate = billingPeriodFrom != null ? Convert.ToDateTime(billingPeriodFrom.Value) : default(DateTime),
                            ToDate = billingPeriodTo != null ? Convert.ToDateTime(billingPeriodTo.Value) : default(DateTime),
                            Duration = Convert.ToDecimal(costDuration.Value ?? 0.0),
                            NumberOfCalls = Convert.ToInt32(calls.Value ?? 0.0),
                            Amount = costNet,
                        };
                            interconnectInvoiceBySaleCurrencies.Add(interconnectInvoiceBySaleCurrencyItemDetails);
                    }
                }
            }
            return interconnectInvoiceBySaleCurrencies;
        }
        private InterconnectInvoiceDetails BuildInterconnectInvoiceDetails(Dictionary<string, List<InvoiceBillingRecord>> itemSetNamesDic, DateTime fromDate, DateTime toDate, decimal? commission, CommissionType? commissionType)
        {
            CurrencyManager currencyManager = new CurrencyManager();
            InterconnectInvoiceDetails interconnectInvoiceDetails = null;
            
                if (itemSetNamesDic != null)
                {
                    List<InvoiceBillingRecord> invoiceBillingRecordList = null;
                    if (itemSetNamesDic.TryGetValue("GroupedByCostZone", out invoiceBillingRecordList))
                    {
                        interconnectInvoiceDetails = new InterconnectInvoiceDetails();
                        foreach (var invoiceBillingRecord in invoiceBillingRecordList)
                        {
                            interconnectInvoiceDetails.Duration += invoiceBillingRecord.InvoiceMeasures.TotalDuration;
                            interconnectInvoiceDetails.CostAmount += invoiceBillingRecord.InvoiceMeasures.Amount;
                            interconnectInvoiceDetails.InterconnectCurrencyId = invoiceBillingRecord.CurrencyId;
                        }
                        if (commissionType.HasValue)
                        {
                            switch (commissionType.Value)
                            {
                                case CommissionType.Display:
                                    interconnectInvoiceDetails.DisplayComission = true;
                                    break;
                            }
                        }
                        else
                        {
                            interconnectInvoiceDetails.DisplayComission = false;
                        }
                    };
                }
            
            if (interconnectInvoiceDetails != null)
            {
                interconnectInvoiceDetails.InterconnectCurrency = currencyManager.GetCurrencySymbol(interconnectInvoiceDetails.InterconnectCurrencyId);
            }
            return interconnectInvoiceDetails;
        }
        private List<GeneratedInvoiceItemSet> BuildGeneratedInvoiceItemSet(Dictionary<string, List<InvoiceBillingRecord>> itemSetNamesDic, List<InterconnectInvoiceBySaleCurrencyItemDetails> interconnectInvoicesBySaleCurrency)
        {
            List<GeneratedInvoiceItemSet> generatedInvoiceItemSets = new List<GeneratedInvoiceItemSet>();

            if (interconnectInvoicesBySaleCurrency != null && interconnectInvoicesBySaleCurrency.Count > 0)
            {
                GeneratedInvoiceItemSet generatedInvoiceItemSet = new GeneratedInvoiceItemSet();
                generatedInvoiceItemSet.SetName = "GroupingByCurrency";
                generatedInvoiceItemSet.Items = new List<GeneratedInvoiceItem>();

                foreach (var interconnectInvoiceBySaleCurrency in interconnectInvoicesBySaleCurrency)
                {
                    generatedInvoiceItemSet.Items.Add(new GeneratedInvoiceItem
                    {
                        Details = interconnectInvoiceBySaleCurrency,
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
                        InterconnectInvoiceItemDetails interconnectInvoiceItemDetails = new Entities.InterconnectInvoiceItemDetails()
                        {
                            Duration = item.InvoiceMeasures.TotalDuration,
                            Amount = item.InvoiceMeasures.Amount,
                            DestinationZoneId = item.DestinationZoneId,
                            OperatorId = item.OperatorId,
                            Rate = item.Rate,
                            RateTypeId = item.RateTypeID,
                            TrafficDirection = item.TrafficDirection,
                            CurrencyId = item.CurrencyId,
                        };
                        generatedInvoiceItemSet.Items.Add(new GeneratedInvoiceItem
                        {
                            Details = interconnectInvoiceItemDetails,
                            Name = " "
                        });
                    }
                    if (generatedInvoiceItemSet.Items.Count > 0)
                    {
                        generatedInvoiceItemSets.Add(generatedInvoiceItemSet);
                    }
                }
            }
            return generatedInvoiceItemSets;
        }
        private AnalyticSummaryBigResult<AnalyticRecord> GetFilteredRecords(List<string> listDimensions, List<string> listMeasures, string dimentionFilterName, object dimentionFilterValue, DateTime fromDate, DateTime toDate, int? currencyId)
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
        private MeasureValue GetMeasureValue(AnalyticRecord analyticRecord, string measureName)
        {
            MeasureValue measureValue;
            analyticRecord.MeasureValues.TryGetValue(measureName, out measureValue);
            return measureValue;
        }
        private Dictionary<string, List<InvoiceBillingRecord>> ConvertAnalyticDataToDictionary(IEnumerable<AnalyticRecord> analyticRecords, int currencyId, decimal? commission, CommissionType? commissionType)
        {
            Dictionary<string, List<InvoiceBillingRecord>> itemSetNamesDic = new Dictionary<string, List<InvoiceBillingRecord>>();
            if (analyticRecords != null)
            {
                foreach (var analyticRecord in analyticRecords)
                {

                    #region ReadDataFromAnalyticResult
                    DimensionValue destinationZoneId = analyticRecord.DimensionValues.ElementAtOrDefault(0);
                    DimensionValue operatorId = analyticRecord.DimensionValues.ElementAtOrDefault(1);
                    DimensionValue rate = analyticRecord.DimensionValues.ElementAtOrDefault(2);
                    DimensionValue rateTypeId = analyticRecord.DimensionValues.ElementAtOrDefault(3);
                    DimensionValue trafficDirection = analyticRecord.DimensionValues.ElementAtOrDefault(4);
                    DimensionValue currency = analyticRecord.DimensionValues.ElementAtOrDefault(5);

                    MeasureValue totalDuration = GetMeasureValue(analyticRecord, "TotalDuration");
                    MeasureValue amount = GetMeasureValue(analyticRecord, "Amount");
                    #endregion

                    var amountValue = Convert.ToDecimal(amount == null ? 0.0 : amount.Value ?? 0.0);
                    if (amountValue != 0)
                    {
                        InvoiceBillingRecord invoiceBillingRecord = new InvoiceBillingRecord
                        {
                            DestinationZoneId = Convert.ToInt64(destinationZoneId.Value),
                            OperatorId = Convert.ToInt64(operatorId.Value),
                            Rate = rate != null ? Convert.ToDecimal(rate.Value) : default(decimal),
                            RateTypeID = rateTypeId != null && rateTypeId.Value != null ? Convert.ToInt32(rateTypeId.Value) : default(int),
                            TrafficDirection = Convert.ToInt32(trafficDirection.Value),
                            CurrencyId = Convert.ToInt32(currency.Value),
                            InvoiceMeasures = new InvoiceMeasures
                            {
                                TotalDuration = Convert.ToDecimal(totalDuration.Value ?? 0.0),
                                Amount = amountValue,
                            }
                        };
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
            }
            else
            {
                invoiceBillingRecordList.Add(invoiceBillingRecord);
                itemSetNamesDic[key] = invoiceBillingRecordList;
            }
        }
        public class InvoiceMeasures
        {
            public Decimal TotalDuration { get; set; }
            public decimal Amount { get; set; }
        }
        public class InvoiceBillingRecord
        {
            public InvoiceMeasures InvoiceMeasures { get; set; }
            public long DestinationZoneId { get; set; }
            public long OperatorId { get; set; }
            public decimal Rate { get; set; }
            public int RateTypeID { get; set; }
            public int TrafficDirection { get; set; }
            public int CurrencyId { get; set; }

        }
    }
}
