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
        InterconnectInvoiceType _type;

        public InterconnectInvoiceGenerator(Guid acountBEDefinitionId, Guid invoiceTransactionTypeId, List<Guid> usageTransactionTypeIds ,InterconnectInvoiceType type)
        {
            this._acountBEDefinitionId = acountBEDefinitionId;
            this._invoiceTransactionTypeId = invoiceTransactionTypeId;
            this._usageTransactionTypeIds = usageTransactionTypeIds;
            this._type = type;
        }

        #endregion

        #region Fields

        private FinancialAccountManager _financialAccountManager = new FinancialAccountManager();
        private AccountBEManager _accountBEManager = new AccountBEManager();

        #endregion

        public override void GenerateInvoice(IInvoiceGenerationContext context)
        {
            Guid voiceAnalyticTableId = new Guid("6cd535c0-ac49-46bb-aecf-0eae33823b20");
            Guid smsAnalyticTableId = new Guid("c1bd3f2f-6213-44d1-9d58-99f81e169930");

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

            string dimensionName = "BillingAccountId";
            string dimensionValue = financialAccountData.FinancialAccountId;
            int currencyId = _accountBEManager.GetCurrencyId(this._acountBEDefinitionId, financialAccountData.Account.AccountId);

            RetailModuleManager retailModuleManager = new RetailModuleManager();
            AnalyticSummaryBigResult<AnalyticRecord> voiceAnalyticResult = new AnalyticSummaryBigResult<AnalyticRecord>();
            AnalyticSummaryBigResult<AnalyticRecord> smsAnalyticResult = new AnalyticSummaryBigResult<AnalyticRecord>();
            if (retailModuleManager.IsVoiceModuleEnabled(voiceAnalyticTableId))
            {
                List<string> voiceListMeasures = new List<string> { "TotalBillingDuration", "Amount", "CountCDRs", "BillingPeriodTo", "BillingPeriodFrom", "Amount_OrigCurr" };
                List<string> voiceListDimensions = new List<string> { "DestinationZone", "OriginationZone", "Operator", "Rate", "RateType", "BillingType", "Currency" };
                voiceAnalyticResult = GetFilteredRecords(voiceListDimensions, voiceListMeasures, dimensionName, dimensionValue, fromDate, toDate, currencyId, voiceAnalyticTableId);
            }
            if (retailModuleManager.IsSMSModuleEnabled(smsAnalyticTableId))
            {
                List<string> smsListMeasures = new List<string> { "Amount", "DeliveredSMS", "BillingPeriodTo", "BillingPeriodFrom", "Amount_OriginalCurrency" };
                List<string> smsListDimensions = new List<string> { "DestinationMobileNetwork", "OriginationMobileNetwork", "Operator", "Rate", "RateType", "BillingType", "Currency" };
                smsAnalyticResult = GetFilteredRecords(smsListDimensions, smsListMeasures, dimensionName, dimensionValue, fromDate, toDate, currencyId, smsAnalyticTableId);
            }
            if ((voiceAnalyticResult.Data == null || voiceAnalyticResult.Data.Count() == 0) && (smsAnalyticResult.Data == null || smsAnalyticResult.Data.Count() == 0))
            {
                context.GenerateInvoiceResult = GenerateInvoiceResult.NoData;
                return;
            }
            List<InvoiceBillingRecord> voiceItemSetNames = new List<InvoiceBillingRecord>();
            List<InvoiceSMSBillingRecord> smsItemSetNames = new List<InvoiceSMSBillingRecord>();

            ConvertAnalyticDataToList(voiceAnalyticResult.Data, smsAnalyticResult.Data, currencyId, voiceItemSetNames, smsItemSetNames);

            if (voiceItemSetNames.Count == 0 &&  smsItemSetNames.Count == 0)
            {
                context.GenerateInvoiceResult = GenerateInvoiceResult.NoData;
                return;
            }
            IEnumerable<VRTaxItemDetail> taxItemDetails = _financialAccountManager.GetFinancialAccountTaxItemDetails(context.InvoiceTypeId, _acountBEDefinitionId, context.PartnerId);

            List<GeneratedInvoiceItemSet> generatedInvoiceItemSets = BuildGeneratedInvoiceItemSet(voiceItemSetNames, smsItemSetNames, taxItemDetails);


            #region BuildInterconnectInvoiceDetails
            InterconnectInvoiceDetails interconnectInvoiceDetails = BuildInterconnectInvoiceDetails(voiceItemSetNames, smsItemSetNames, context.FromDate, context.ToDate);
            if (interconnectInvoiceDetails != null)
            {
                interconnectInvoiceDetails.AmountWithTaxes = interconnectInvoiceDetails.Amount;
                interconnectInvoiceDetails.SMSAmountWithTaxes = interconnectInvoiceDetails.SMSAmount;

                if (taxItemDetails != null)
                {
                    foreach (var tax in taxItemDetails)
                    {
                        interconnectInvoiceDetails.AmountWithTaxes += ((interconnectInvoiceDetails.Amount * Convert.ToDecimal(tax.Value)) / 100);
                        interconnectInvoiceDetails.SMSAmountWithTaxes += ((interconnectInvoiceDetails.SMSAmount * Convert.ToDecimal(tax.Value)) / 100);
                    }
                }

                if (interconnectInvoiceDetails.AmountWithTaxes != 0 || interconnectInvoiceDetails.SMSAmountWithTaxes != 0)
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
        private InterconnectInvoiceDetails BuildInterconnectInvoiceDetails(List<InvoiceBillingRecord> voiceItemSetNames, List<InvoiceSMSBillingRecord> smsItemSetNames, DateTime fromDate, DateTime toDate)
        {
            CurrencyManager currencyManager = new CurrencyManager();
            InterconnectInvoiceDetails interconnectInvoiceDetails = null;

            if (voiceItemSetNames != null && voiceItemSetNames.Count > 0)
            {
                interconnectInvoiceDetails = new InterconnectInvoiceDetails();
                foreach (var invoiceBillingRecord in voiceItemSetNames)
                {
                    interconnectInvoiceDetails.Duration += invoiceBillingRecord.InvoiceMeasures.TotalBillingDuration;
                    interconnectInvoiceDetails.Amount += invoiceBillingRecord.InvoiceMeasures.Amount;
                    interconnectInvoiceDetails.VoiceAmount += invoiceBillingRecord.InvoiceMeasures.Amount;
                    interconnectInvoiceDetails.InterconnectCurrencyId = invoiceBillingRecord.CurrencyId;
                    interconnectInvoiceDetails.TotalNumberOfCalls += invoiceBillingRecord.InvoiceMeasures.NumberOfCalls;
                }
            }
            if (smsItemSetNames != null && smsItemSetNames.Count > 0)
            {
                if (interconnectInvoiceDetails == null)
                    interconnectInvoiceDetails = new InterconnectInvoiceDetails();
                foreach (var invoiceBillingRecord in smsItemSetNames)
                {
                    interconnectInvoiceDetails.Amount += invoiceBillingRecord.InvoiceMeasures.Amount;
                    interconnectInvoiceDetails.SMSAmount += invoiceBillingRecord.InvoiceMeasures.Amount;
                    interconnectInvoiceDetails.InterconnectCurrencyId = invoiceBillingRecord.CurrencyId;
                    interconnectInvoiceDetails.TotalNumberOfSMS += invoiceBillingRecord.InvoiceMeasures.NumberOfSMS;
                }
            }

            if (interconnectInvoiceDetails != null)
            {
                interconnectInvoiceDetails.InterconnectCurrency = currencyManager.GetCurrencySymbol(interconnectInvoiceDetails.InterconnectCurrencyId);
            }
            return interconnectInvoiceDetails;
        }
        private List<GeneratedInvoiceItemSet> BuildGeneratedInvoiceItemSet(List<InvoiceBillingRecord> voiceItemSetNames, List<InvoiceSMSBillingRecord> smsItemSetNames, IEnumerable<VRTaxItemDetail> taxItemDetails)
        {
            List<GeneratedInvoiceItemSet> generatedInvoiceItemSets = new List<GeneratedInvoiceItemSet>();
            if (voiceItemSetNames != null && voiceItemSetNames.Count>0)
            {
                GeneratedInvoiceItemSet generatedInvoiceItemSet = new GeneratedInvoiceItemSet()
                {
                    SetName = "GroupedByDestinationZone",
                    Items = new List<GeneratedInvoiceItem>()
                };
                foreach (var item in voiceItemSetNames)
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
            if (smsItemSetNames != null && smsItemSetNames.Count > 0)
            {
                GeneratedInvoiceItemSet generatedInvoiceItemSet = new GeneratedInvoiceItemSet()
                {
                    SetName = "GroupedByDestinationMobileNetwork",
                    Items = new List<GeneratedInvoiceItem>()
                };
                foreach (var item in smsItemSetNames)
                {
                    InterconnectSMSInvoiceItemDetails interconnectInvoiceItemDetails = new Entities.InterconnectSMSInvoiceItemDetails()
                    {
                        Amount = item.InvoiceMeasures.Amount,
                        DestinationMobileNetworkId = item.DestinationMobileNetworkId,
                        OriginationMobileNetworkId = item.OriginationMobileNetworkId,
                        OperatorId = item.OperatorId,
                        Rate = item.Rate,
                        RateTypeId = item.RateTypeId,
                        BillingType = item.BillingType,
                        CurrencyId = item.CurrencyId,
                        NumberOfSMS = item.InvoiceMeasures.NumberOfSMS,
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

            if (generatedInvoiceItemSets.Count > 0)
            {
                if (taxItemDetails != null && taxItemDetails.Count() > 0)
                {
                    GeneratedInvoiceItemSet generatedInvoiceItemSet = new GeneratedInvoiceItemSet()
                    {
                        SetName = "Taxes",
                        Items = new List<GeneratedInvoiceItem>()
                    };
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
        private AnalyticSummaryBigResult<AnalyticRecord> GetFilteredRecords(List<string> listDimensions, List<string> listMeasures, string dimentionFilterName, object dimentionFilterValue, DateTime fromDate, DateTime toDate, int? currencyId, Guid analyticTableId)
        {
            AnalyticManager analyticManager = new AnalyticManager();
            Vanrise.Entities.DataRetrievalInput<AnalyticQuery> analyticQuery = new DataRetrievalInput<AnalyticQuery>()
            {
                Query = new AnalyticQuery()
                {
                    DimensionFields = listDimensions,
                    MeasureFields = listMeasures,
                    TableId = analyticTableId,
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

            DimensionFilter billingTypeFilter = new DimensionFilter();

            billingTypeFilter.Dimension = "BillingType";
            if (_type == InterconnectInvoiceType.Customer)
            { billingTypeFilter.FilterValues = new List<object> { 1 }; }
            else
            {
                billingTypeFilter.FilterValues = new List<object> { 2 };
            }

            analyticQuery.Query.Filters.Add(dimensionFilter);
            analyticQuery.Query.Filters.Add(billingTypeFilter);
            return analyticManager.GetFilteredRecords(analyticQuery) as Vanrise.Analytic.Entities.AnalyticSummaryBigResult<AnalyticRecord>;
        }
        private MeasureValue GetMeasureValue(AnalyticRecord analyticRecord, string measureName)
        {
            MeasureValue measureValue;
            analyticRecord.MeasureValues.TryGetValue(measureName, out measureValue);
            return measureValue;
        }
        private void ConvertAnalyticDataToList(IEnumerable<AnalyticRecord> voiceAnalyticRecords, IEnumerable<AnalyticRecord> smsAnalyticRecords, int currencyId, List<InvoiceBillingRecord> voiceItemSetNames, List<InvoiceSMSBillingRecord> smsItemSetNames)
        {
            if (voiceAnalyticRecords != null && voiceAnalyticRecords.Count()>0)
            {
                foreach (var analyticRecord in voiceAnalyticRecords)
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
                        voiceItemSetNames.Add(invoiceBillingRecord);
                    }
                }
            }
            if (smsAnalyticRecords != null && smsAnalyticRecords.Count() > 0)
            {
                foreach (var analyticRecord in smsAnalyticRecords)
                {
                    #region ReadDataFromAnalyticResult
                    DimensionValue destinationMobileNetworkId = analyticRecord.DimensionValues.ElementAtOrDefault(0);
                    DimensionValue originationMobileNetworkId = analyticRecord.DimensionValues.ElementAtOrDefault(1);
                    DimensionValue operatorId = analyticRecord.DimensionValues.ElementAtOrDefault(2);
                    DimensionValue rate = analyticRecord.DimensionValues.ElementAtOrDefault(3);
                    DimensionValue rateTypeId = analyticRecord.DimensionValues.ElementAtOrDefault(4);
                    DimensionValue billingType = analyticRecord.DimensionValues.ElementAtOrDefault(5);
                    DimensionValue currency = analyticRecord.DimensionValues.ElementAtOrDefault(6);

                    MeasureValue amount = GetMeasureValue(analyticRecord, "Amount");
                    MeasureValue deliveredSMS = GetMeasureValue(analyticRecord, "DeliveredSMS");
                    MeasureValue billingPeriodTo = GetMeasureValue(analyticRecord, "BillingPeriodTo");
                    MeasureValue billingPeriodFrom = GetMeasureValue(analyticRecord, "BillingPeriodFrom");
                    MeasureValue amount_OrigCurr = GetMeasureValue(analyticRecord, "Amount_OriginalCurrency");

                    #endregion

                    var amountValue = Convert.ToDecimal(amount == null ? 0.0 : amount.Value ?? 0.0);
                    if (amountValue != 0)
                    {
                        InvoiceSMSBillingRecord invoiceBillingRecord = new InvoiceSMSBillingRecord
                        {
                            DestinationMobileNetworkId = Convert.ToInt64(destinationMobileNetworkId.Value),
                            OriginationMobileNetworkId = Convert.ToInt64(originationMobileNetworkId.Value),
                            OperatorId = Convert.ToInt64(operatorId.Value),
                            Rate = rate != null ? Convert.ToDecimal(rate.Value) : default(decimal),
                            RateTypeId = rateTypeId != null && rateTypeId.Value != null ? Convert.ToInt32(rateTypeId.Value) : default(int),
                            BillingType = Convert.ToInt32(billingType.Value),
                            CurrencyId = Convert.ToInt32(currency.Value),
                            InvoiceMeasures = new InvoiceSMSMeasures
                            {
                                FromDate = billingPeriodFrom != null ? Convert.ToDateTime(billingPeriodFrom.Value) : default(DateTime),
                                ToDate = billingPeriodTo != null ? Convert.ToDateTime(billingPeriodTo.Value) : default(DateTime),
                                Amount = amountValue,
                                Amount_OrigCurr = Convert.ToDecimal(amount_OrigCurr == null ? 0.0 : amount_OrigCurr.Value ?? 0.0),
                                NumberOfSMS = Convert.ToInt32(deliveredSMS.Value ?? 00),
                            }
                        };
                        smsItemSetNames.Add(invoiceBillingRecord);
                    }
                }
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
        public class InvoiceSMSMeasures
        {
            public DateTime FromDate { get; set; }
            public DateTime ToDate { get; set; }
            public decimal Amount { get; set; }
            public decimal Amount_OrigCurr { get; set; }
            public int NumberOfSMS { get; set; }
        }
        public class InvoiceSMSBillingRecord
        {
            public InvoiceSMSMeasures InvoiceMeasures { get; set; }
            public long DestinationMobileNetworkId { get; set; }
            public long OriginationMobileNetworkId { get; set; }
            public long OperatorId { get; set; }
            public decimal Rate { get; set; }
            public int RateTypeId { get; set; }
            public int BillingType { get; set; }
            public int CurrencyId { get; set; }
        }
    }
}
