using Retail.BusinessEntity.Business;
using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Invoice.Entities;
using Retail.Interconnect.Entities;
using Vanrise.Entities;
using Vanrise.Common.Business;
using Vanrise.Analytic.Entities;
using Vanrise.Analytic.Business;
using Vanrise.Common;
using Vanrise.InvToAccBalanceRelation.Entities;
namespace Retail.Interconnect.Business
{
    public class InterconnectInvoiceGenerator : InvoiceGenerator
    {
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
       
        #region Fields

        private FinancialAccountManager _financialAccountManager = new FinancialAccountManager();
        private AccountBEManager _accountBEManager = new AccountBEManager();

        #endregion

        public override void GenerateInvoice(IInvoiceGenerationContext context)
        {
            List<string> listMeasures = new List<string> { "TotalBillingDuration", "Amount", "CountCDRs", "BillingPeriodTo", "BillingPeriodFrom", "Amount_OrigCurr" };
            List<string> listDimensions = new List<string> { "DestinationZone", "OriginationZone", "Operator", "Rate", "RateType", "BillingType", "Currency" };

            FinancialAccountData financialAccountData = _financialAccountManager.GetFinancialAccountData(_acountBEDefinitionId, context.PartnerId);

            if (context.FromDate < financialAccountData.FinancialAccount.BED || context.ToDate > financialAccountData.FinancialAccount.EED)
            {
                context.ErrorMessage = "From date and To date should be within the effective date of financial account.";
                context.GenerateInvoiceResult = GenerateInvoiceResult.Failed;
                return;

            }
                   
            DateTime fromDate = context.FromDate;
            DateTime toDate = context.ToDate;
            DateTime toDateForBillingTransaction = context.ToDate.Date.AddDays(1);

            string dimentionName = "BillingAccountId";
            string dimensionValue = financialAccountData.FinancialAccountId;
            int currencyId = _accountBEManager.GetCurrencyId(this._acountBEDefinitionId, financialAccountData.Account.AccountId);
           
            var analyticResult = GetFilteredRecords(listDimensions, listMeasures, dimentionName, dimensionValue, fromDate, toDate, currencyId);
            if (analyticResult == null || analyticResult.Data == null || analyticResult.Data.Count() == 0)
            {
                context.GenerateInvoiceResult = GenerateInvoiceResult.NoData;
                return;
            }

            Dictionary<string, List<InvoiceBillingRecord>> itemSetNamesDic = ConvertAnalyticDataToDictionary(analyticResult.Data, currencyId);
            if (itemSetNamesDic.Count == 0)
            {
                context.GenerateInvoiceResult = GenerateInvoiceResult.NoData;
                return;
            }
            IEnumerable<VRTaxItemDetail> taxItemDetails = _financialAccountManager.GetFinancialAccountTaxItemDetails(context.InvoiceTypeId, _acountBEDefinitionId, context.PartnerId);

            List<GeneratedInvoiceItemSet> generatedInvoiceItemSets = BuildGeneratedInvoiceItemSet(itemSetNamesDic, taxItemDetails);


            #region BuildInterconnectInvoiceDetails
            InterconnectInvoiceDetails interconnectInvoiceDetails = BuildInterconnectInvoiceDetails(itemSetNamesDic, context.FromDate, context.ToDate);
            if (interconnectInvoiceDetails != null)
            {
                interconnectInvoiceDetails.AmountWithTaxes = interconnectInvoiceDetails.Amount;

                if (taxItemDetails != null)
                {
                    foreach (var tax in taxItemDetails)
                    {
                        interconnectInvoiceDetails.AmountWithTaxes += ((interconnectInvoiceDetails.Amount * Convert.ToDecimal(tax.Value)) / 100);
                    }
                }

                if (interconnectInvoiceDetails.AmountWithTaxes != 0)
                {
                    context.Invoice = new GeneratedInvoice
                    {
                        InvoiceDetails = interconnectInvoiceDetails,
                        InvoiceItemSets = generatedInvoiceItemSets,
                    };
                    SetInvoiceBillingTransactions(context, interconnectInvoiceDetails);
                }
                else
                {
                    context.ErrorMessage = "No billing data available.";
                    context.GenerateInvoiceResult = GenerateInvoiceResult.NoData;
                    return;
                }
              
            }
            else
            {
                context.ErrorMessage = "No billing data available.";
                context.GenerateInvoiceResult = GenerateInvoiceResult.NoData;
                return;
            }



            #endregion
        }

        private void SetInvoiceBillingTransactions(IInvoiceGenerationContext context, InterconnectInvoiceDetails invoiceDetails)
        {
            var relationManager = new Vanrise.InvToAccBalanceRelation.Business.InvToAccBalanceRelationDefinitionManager();
            List<BalanceAccountInfo> invoiceBalanceAccounts = relationManager.GetInvoiceBalanceAccounts(context.InvoiceTypeId, context.PartnerId, context.IssueDate);

            invoiceBalanceAccounts.ThrowIfNull("invoiceBalanceAccounts", context.PartnerId);
            if (invoiceBalanceAccounts.Count == 0)
                throw new Vanrise.Entities.DataIntegrityValidationException("invoiceBalanceAccounts.Count == 0");

            var billingTransaction = new GeneratedInvoiceBillingTransaction()
            {
                AccountTypeId = invoiceBalanceAccounts.FirstOrDefault().AccountTypeId,
                AccountId = context.PartnerId,
                TransactionTypeId = this._invoiceTransactionTypeId,
                Amount = invoiceDetails.Amount,
                CurrencyId = invoiceDetails.InterconnectCurrencyId
            };
        
            billingTransaction.Settings = new GeneratedInvoiceBillingTransactionSettings();
            billingTransaction.Settings.UsageOverrides = new List<GeneratedInvoiceBillingTransactionUsageOverride>();
            if (this._usageTransactionTypeIds != null)
            {
                foreach (Guid usageTransactionTypeId in this._usageTransactionTypeIds)
                {
                    billingTransaction.Settings.UsageOverrides.Add(new GeneratedInvoiceBillingTransactionUsageOverride()
                    {
                        TransactionTypeId = usageTransactionTypeId
                    });
                }
            }
            context.BillingTransactions = new List<GeneratedInvoiceBillingTransaction>() { billingTransaction };
        }
        private InterconnectInvoiceDetails BuildInterconnectInvoiceDetails(Dictionary<string, List<InvoiceBillingRecord>> itemSetNamesDic, DateTime fromDate, DateTime toDate)
        {
            CurrencyManager currencyManager = new CurrencyManager();
            InterconnectInvoiceDetails interconnectInvoiceDetails = null;
            
                if (itemSetNamesDic != null)
                {
                    List<InvoiceBillingRecord> invoiceBillingRecordList = null;
                    if (itemSetNamesDic.TryGetValue("GroupedByDestinationZone", out invoiceBillingRecordList))
                    {
                        interconnectInvoiceDetails = new InterconnectInvoiceDetails();
                        foreach (var invoiceBillingRecord in invoiceBillingRecordList)
                        {
                            interconnectInvoiceDetails.Duration += invoiceBillingRecord.InvoiceMeasures.TotalBillingDuration;
                            interconnectInvoiceDetails.Amount += invoiceBillingRecord.InvoiceMeasures.Amount;
                            interconnectInvoiceDetails.InterconnectCurrencyId = invoiceBillingRecord.CurrencyId;
                            interconnectInvoiceDetails.TotalNumberOfCalls += invoiceBillingRecord.InvoiceMeasures.NumberOfCalls;
                        }
                    };
                }
            
            if (interconnectInvoiceDetails != null)
            {
                interconnectInvoiceDetails.InterconnectCurrency = currencyManager.GetCurrencySymbol(interconnectInvoiceDetails.InterconnectCurrencyId);
            }
            return interconnectInvoiceDetails;
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
                        InterconnectInvoiceItemDetails interconnectInvoiceItemDetails = new Entities.InterconnectInvoiceItemDetails()
                        {
                            Duration = item.InvoiceMeasures.TotalBillingDuration,
                            Amount = item.InvoiceMeasures.Amount,
                            DestinationZoneId = item.DestinationZoneId,
                            OriginationZoneId = item.OriginationZoneId,
                            OperatorId = item.OperatorId,
                            Rate = item.Rate,
                            RateTypeId = item.RateTypeId,
                            TrafficDirection = item.TrafficDirection,
                            CurrencyId = item.CurrencyId,
                            NumberOfCalls = item.InvoiceMeasures.NumberOfCalls,
                            FromDate = item.InvoiceMeasures.FromDate,
                            ToDate = item.InvoiceMeasures.ToDate,
                            OriginalAmount = item.InvoiceMeasures.Amount_OrigCurr,
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
                    TableId = Guid.Parse("6cd535c0-ac49-46bb-aecf-0eae33823b20"),
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
            DimensionFilter billingTypeFilter = new DimensionFilter()
            {
                Dimension = "BillingType",
                FilterValues = new List<object> { 1 }
            };
            analyticQuery.Query.Filters.Add(billingTypeFilter);
            return analyticManager.GetFilteredRecords(analyticQuery) as Vanrise.Analytic.Entities.AnalyticSummaryBigResult<AnalyticRecord>;
        }
        private MeasureValue GetMeasureValue(AnalyticRecord analyticRecord, string measureName)
        {
            MeasureValue measureValue;
            analyticRecord.MeasureValues.TryGetValue(measureName, out measureValue);
            return measureValue;
        }
        private Dictionary<string, List<InvoiceBillingRecord>> ConvertAnalyticDataToDictionary(IEnumerable<AnalyticRecord> analyticRecords, int currencyId)
        {
            Dictionary<string, List<InvoiceBillingRecord>> itemSetNamesDic = new Dictionary<string, List<InvoiceBillingRecord>>();
            if (analyticRecords != null)
            {
                foreach (var analyticRecord in analyticRecords)
                {
                    #region ReadDataFromAnalyticResult
                    DimensionValue destinationZoneId = analyticRecord.DimensionValues.ElementAtOrDefault(0);
                    DimensionValue originationZoneId = analyticRecord.DimensionValues.ElementAtOrDefault(1);
                    DimensionValue operatorId = analyticRecord.DimensionValues.ElementAtOrDefault(2);
                    DimensionValue rate = analyticRecord.DimensionValues.ElementAtOrDefault(3);
                    DimensionValue rateTypeId = analyticRecord.DimensionValues.ElementAtOrDefault(4);
                    DimensionValue trafficDirection = analyticRecord.DimensionValues.ElementAtOrDefault(5);
                    DimensionValue currency = analyticRecord.DimensionValues.ElementAtOrDefault(6);

                    MeasureValue totalBillingDuration = GetMeasureValue(analyticRecord, "TotalBillingDuration");
                    MeasureValue amount = GetMeasureValue(analyticRecord, "Amount");
                    MeasureValue countCDRs = GetMeasureValue(analyticRecord, "CountCDRs");
                    MeasureValue billingPeriodTo = GetMeasureValue(analyticRecord, "BillingPeriodTo");
                    MeasureValue billingPeriodFrom = GetMeasureValue(analyticRecord, "BillingPeriodFrom");
                    MeasureValue amount_OrigCurr = GetMeasureValue(analyticRecord, "Amount_OrigCurr");

                    #endregion

                    var amountValue = Convert.ToDecimal(amount == null ? 0.0 : amount.Value ?? 0.0);
                    if (amountValue != 0)
                    {
                        InvoiceBillingRecord invoiceBillingRecord = new InvoiceBillingRecord
                        {
                            DestinationZoneId = Convert.ToInt64(destinationZoneId.Value),
                            OriginationZoneId = Convert.ToInt64(originationZoneId.Value),
                            OperatorId = Convert.ToInt64(operatorId.Value),

                            Rate = rate != null ? Convert.ToDecimal(rate.Value) : default(decimal),
                            RateTypeId = rateTypeId != null && rateTypeId.Value != null ? Convert.ToInt32(rateTypeId.Value) : default(int),
                            TrafficDirection = Convert.ToInt32(trafficDirection.Value),
                            CurrencyId = Convert.ToInt32(currency.Value),
                            InvoiceMeasures = new InvoiceMeasures
                            {
                                FromDate = billingPeriodFrom != null ? Convert.ToDateTime(billingPeriodFrom.Value) : default(DateTime),
                                ToDate = billingPeriodTo != null ? Convert.ToDateTime(billingPeriodTo.Value) : default(DateTime),
                                TotalBillingDuration = Convert.ToDecimal(totalBillingDuration.Value ?? 0.0),
                                Amount = amountValue,
                                Amount_OrigCurr = Convert.ToDecimal(amount_OrigCurr == null ? 0.0 : amount_OrigCurr.Value ?? 0.0),
                                NumberOfCalls = Convert.ToInt32(countCDRs.Value ?? 00),
                            }
                        };
                        AddItemToDictionary(itemSetNamesDic, "GroupedByDestinationZone", invoiceBillingRecord);
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
            public DateTime FromDate { get; set; }
            public DateTime ToDate { get; set; }
            public Decimal TotalBillingDuration { get; set; }
            public decimal Amount { get; set; }
            public decimal Amount_OrigCurr { get; set; }
            public int NumberOfCalls { get; set; }
        }
        public class InvoiceBillingRecord
        {
            public InvoiceMeasures InvoiceMeasures { get; set; }
            public long DestinationZoneId { get; set; }
            public long OriginationZoneId { get; set; }
            public long OperatorId { get; set; }
            public decimal Rate { get; set; }
            public int RateTypeId { get; set; }
            public int TrafficDirection { get; set; }
            public int CurrencyId { get; set; }

        }
    }
}
