using Retail.BusinessEntity.Business;
using Retail.BusinessEntity.Entities;
using Retail.Zajil.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Analytic.Business;
using Vanrise.Analytic.Entities;
using Vanrise.Entities;
using Vanrise.Invoice.Entities;
using Vanrise.Common;
namespace Retail.Zajil.MainExtensions
{
    public class ZajilSubscriberInvoiceGenerator : InvoiceGenerator
    {
        Guid _acountBEDefinitionId;
        Guid _companyExtendedInfoPartdefinitionId;

         Guid _invoiceTransactionTypeId;
         List<Guid> _usageTransactionTypeIds;
        public ZajilSubscriberInvoiceGenerator(Guid acountBEDefinitionId, Guid companyExtendedInfoPartdefinitionId, Guid invoiceTransactionTypeId, List<Guid> usageTransactionTypeIds)
        {
            this._acountBEDefinitionId = acountBEDefinitionId;
            this._companyExtendedInfoPartdefinitionId = companyExtendedInfoPartdefinitionId;
            this._invoiceTransactionTypeId = invoiceTransactionTypeId;
            this._usageTransactionTypeIds = usageTransactionTypeIds;
        }

        public override void GenerateInvoice(IInvoiceGenerationContext context)
        {
            List<string> listMeasures = new List<string> { "Amount", "CountCDRs", "TotalDuration" };
            List<string> listDimensions = new List<string> { "FinancialAccountId", "ServiceType", "Zone", "Operator" };

            string dimensionName = "FinancialAccountId";

            AccountBEManager accountBEManager = new AccountBEManager();
            IAccountPayment accountPayment;
            long accountId = Convert.ToInt32(context.PartnerId);
            if (!accountBEManager.HasAccountPayment(this._acountBEDefinitionId, accountId, false, out accountPayment))
            {
                context.ErrorMessage = string.Format("Account Id: {0} is not a financial account", accountId);
                context.GenerateInvoiceResult = GenerateInvoiceResult.Failed;
            }

            int currencyId = accountPayment.CurrencyId;

            var analyticResult = GetFilteredRecords(listDimensions, listMeasures, dimensionName, accountId, context.FromDate, context.ToDate, currencyId);
            if (analyticResult == null || analyticResult.Data == null || analyticResult.Data.Count() == 0)
            {
                context.GenerateInvoiceResult = GenerateInvoiceResult.NoData;
                return;
            }

            Dictionary<string, List<InvoiceBillingRecord>> itemSetNamesDic = ConvertAnalyticDataToDictionary(analyticResult.Data);
            List<GeneratedInvoiceItemSet> generatedInvoiceItemSets = BuildGeneratedInvoiceItemSet(itemSetNamesDic);

            InvoiceDetails retailSubscriberInvoiceDetails = BuildInvoiceDetails(itemSetNamesDic, context.FromDate, context.ToDate, currencyId);
            retailSubscriberInvoiceDetails.DuePeriod = context.GetDuePeriod();
            AccountPart accountPart;
            if (accountBEManager.TryGetAccountPart(_acountBEDefinitionId, accountId, _companyExtendedInfoPartdefinitionId, false, out accountPart))
            {
                ZajilCompanyExtendedInfo zajilCompanyExtendedInfo = null;
                zajilCompanyExtendedInfo = accountPart.Settings as ZajilCompanyExtendedInfo;
                if (zajilCompanyExtendedInfo != null)
                {
                    retailSubscriberInvoiceDetails.VoiceCustomerNo = zajilCompanyExtendedInfo.GPVoiceCustomerNo;
                    retailSubscriberInvoiceDetails.SalesAgent = zajilCompanyExtendedInfo.SalesAgent;
                    retailSubscriberInvoiceDetails.CustomerPO = zajilCompanyExtendedInfo.CustomerPO;
                }
            }
            SetInvoiceBillingTransactions(context, retailSubscriberInvoiceDetails);
            context.Invoice = new GeneratedInvoice
            {
                InvoiceDetails = retailSubscriberInvoiceDetails,
                InvoiceItemSets = generatedInvoiceItemSets,
            };

        }

        private InvoiceDetails BuildInvoiceDetails(Dictionary<string, List<InvoiceBillingRecord>> itemSetNamesDic, DateTime fromDate, DateTime toDate, int currencyId)
        {
            InvoiceDetails retailSubscriberInvoiceDetails = null;
            if (itemSetNamesDic != null)
            {
                List<InvoiceBillingRecord> invoiceBillingRecordList = null;
                if (itemSetNamesDic.TryGetValue("GroupedByServiceType", out invoiceBillingRecordList))
                {
                    retailSubscriberInvoiceDetails = new InvoiceDetails();
                    foreach (var invoiceBillingRecord in invoiceBillingRecordList)
                    {
                        retailSubscriberInvoiceDetails.TotalAmount += invoiceBillingRecord.Amount;
                        retailSubscriberInvoiceDetails.CountCDRs += invoiceBillingRecord.CountCDRs;
                        retailSubscriberInvoiceDetails.TotalDuration += invoiceBillingRecord.TotalDuration;
                        retailSubscriberInvoiceDetails.CurrencyId = currencyId;
                    }
                };
            }
            return retailSubscriberInvoiceDetails;
        }

        private List<GeneratedInvoiceItemSet> BuildGeneratedInvoiceItemSet(Dictionary<string, List<InvoiceBillingRecord>> itemSetNamesDic)
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
                        if (item.Amount == 0)
                            continue;

                        InvoiceItemDetails subscriberInvoiceItemDetails = new InvoiceItemDetails()
                        {
                            Amount = item.Amount,
                            ServiceTypeId = item.ServiceTypeId,
                            CountCDRs = item.CountCDRs,
                            TotalDuration = item.TotalDuration,
                            ZoneId = item.ZoneId,
                            InterconnectOperatorId = item.InterconnectOperatorId
                        };
                        generatedInvoiceItemSet.Items.Add(new GeneratedInvoiceItem
                        {
                            Details = subscriberInvoiceItemDetails,
                            Name = "GroupedByServiceType"
                        });
                    }
                    generatedInvoiceItemSets.Add(generatedInvoiceItemSet);

                }
            }
            return generatedInvoiceItemSets;
        }

        private AnalyticSummaryBigResult<AnalyticRecord> GetFilteredRecords(List<string> listDimensions, List<string> listMeasures, string dimensionFilterName, object dimensionFilterValue, DateTime fromDate, DateTime toDate, int currencyId)
        {
            AnalyticManager analyticManager = new AnalyticManager();
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
                FilterValues = new List<object> { dimensionFilterValue }
            };
            analyticQuery.Query.Filters.Add(dimensionFilter);
            return analyticManager.GetFilteredRecords(analyticQuery) as Vanrise.Analytic.Entities.AnalyticSummaryBigResult<AnalyticRecord>;
        }

        private Dictionary<string, List<InvoiceBillingRecord>> ConvertAnalyticDataToDictionary(IEnumerable<AnalyticRecord> analyticRecords)
        {
            Dictionary<string, List<InvoiceBillingRecord>> itemSetNamesDic = new Dictionary<string, List<InvoiceBillingRecord>>();
            if (analyticRecords != null)
            {
                foreach (var analyticRecord in analyticRecords)
                {
                    DimensionValue serviceTypeId = analyticRecord.DimensionValues.ElementAtOrDefault(1);
                    DimensionValue zoneId = analyticRecord.DimensionValues.ElementAtOrDefault(2);
                    DimensionValue operatorId = analyticRecord.DimensionValues.ElementAtOrDefault(3);

                    if (serviceTypeId.Value != null)
                    {
                        MeasureValue totalAmount = GetMeasureValue(analyticRecord, "Amount");
                        MeasureValue countCDRs = GetMeasureValue(analyticRecord, "CountCDRs");
                        MeasureValue totalDuration = GetMeasureValue(analyticRecord, "TotalDuration");

                        InvoiceBillingRecord invoiceBillingRecord = new InvoiceBillingRecord
                        {
                            ServiceTypeId = new Guid(serviceTypeId.Value.ToString()),
                            Amount = Convert.ToDecimal(totalAmount.Value ?? 0.0),
                            CountCDRs = Convert.ToInt32(countCDRs.Value),
                            TotalDuration = Convert.ToDecimal(totalDuration.Value ?? 0.0)
                        };

                        if (zoneId.Value != null)
                            invoiceBillingRecord.ZoneId = Convert.ToInt32(zoneId.Value);

                        if (operatorId.Value != null)
                            invoiceBillingRecord.InterconnectOperatorId = Convert.ToInt32(operatorId.Value);

                        AddItemToDictionary(itemSetNamesDic, "GroupedByServiceType", invoiceBillingRecord);
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

        private MeasureValue GetMeasureValue(AnalyticRecord analyticRecord, string measureName)
        {
            MeasureValue measureValue;
            analyticRecord.MeasureValues.TryGetValue(measureName, out measureValue);
            return measureValue;
        }
        private void SetInvoiceBillingTransactions(IInvoiceGenerationContext context, InvoiceDetails invoiceDetails)
        {
            var accountBalanceManager = new AccountBalanceManager();
            Guid accountTypeId = accountBalanceManager.GetAccountBalanceTypeId(_acountBEDefinitionId);
            var billingTransaction = new GeneratedInvoiceBillingTransaction()
            {
                AccountTypeId = accountTypeId,
                AccountId = context.PartnerId,
                TransactionTypeId = this._invoiceTransactionTypeId,
                Amount = invoiceDetails.TotalAmount,
                CurrencyId = invoiceDetails.CurrencyId
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
        public class InvoiceBillingRecord
        {
            public decimal Amount { get; set; }

            public Guid? ServiceTypeId { get; set; }

            public long? ZoneId { get; set; }

            public long? InterconnectOperatorId { get; set; }

            public int CountCDRs { get; set; }

            public Decimal TotalDuration { get; set; }
        }

    }
}
