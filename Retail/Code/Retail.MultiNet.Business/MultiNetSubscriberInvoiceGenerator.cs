using Retail.BusinessEntity.Business;
using Retail.BusinessEntity.Entities;
using Retail.MultiNet.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Analytic.Business;
using Vanrise.Analytic.Entities;
using Vanrise.Entities;
using Vanrise.Invoice.Entities;

namespace Retail.MultiNet.Business
{
    public class MultiNetSubscriberInvoiceGenerator : InvoiceGenerator
    {
        Guid _acountBEDefinitionId;
        Guid _companyExtendedInfoPartdefinitionId;
        public MultiNetSubscriberInvoiceGenerator(Guid acountBEDefinitionId, Guid companyExtendedInfoPartdefinitionId)
        {
            this._acountBEDefinitionId = acountBEDefinitionId;
            this._companyExtendedInfoPartdefinitionId = companyExtendedInfoPartdefinitionId;
        }

        public override void GenerateInvoice(IInvoiceGenerationContext context)
        {
            List<string> listMeasures = new List<string> { "CountIN", "CountOUT" };
            List<string> listDimensions = new List<string> { "FinancialAccountId", "ServiceType"};

            string dimensionName = "FinancialAccountId";

            AccountBEManager accountBEManager = new AccountBEManager();
            IAccountPayment accountPayment;
            long accountId = Convert.ToInt32(context.PartnerId);
            if (!accountBEManager.HasAccountPayment(this._acountBEDefinitionId, accountId, false, out accountPayment))
                throw new InvoiceGeneratorException(string.Format("Account Id: {0} is not a financial account", accountId));

            int currencyId = accountPayment.CurrencyId;

            var analyticResult = GetFilteredRecords(listDimensions, listMeasures, dimensionName, accountId, context.FromDate, context.GeneratedToDate, currencyId);
            if (analyticResult == null || analyticResult.Data == null || analyticResult.Data.Count() == 0)
            {
                throw new InvoiceGeneratorException("No data available between the selected period.");
            }

            Dictionary<string, List<InvoiceBillingRecord>> itemSetNamesDic = ConvertAnalyticDataToDictionary(analyticResult.Data);
            List<GeneratedInvoiceItemSet> generatedInvoiceItemSets = BuildGeneratedInvoiceItemSet(itemSetNamesDic);

            InvoiceDetails retailSubscriberInvoiceDetails = BuildInvoiceDetails(itemSetNamesDic, context.FromDate, context.ToDate, currencyId);
           
            context.Invoice = new GeneratedInvoice
            {
                InvoiceDetails = retailSubscriberInvoiceDetails,
                InvoiceItemSets = generatedInvoiceItemSets,
            };

        }
        private Dictionary<string, List<InvoiceBillingRecord>> ConvertAnalyticDataToDictionary(IEnumerable<AnalyticRecord> analyticRecords)
        {
            Dictionary<string, List<InvoiceBillingRecord>> itemSetNamesDic = new Dictionary<string, List<InvoiceBillingRecord>>();
            if (analyticRecords != null)
            {
                foreach (var analyticRecord in analyticRecords)
                {
                    DimensionValue serviceTypeId = analyticRecord.DimensionValues.ElementAtOrDefault(1);
                    DimensionValue financialAccountId = analyticRecord.DimensionValues.ElementAtOrDefault(2);

                    if (serviceTypeId.Value != null)
                    {
                        MeasureValue countIN = GetMeasureValue(analyticRecord, "CountIN");
                        MeasureValue countOUT = GetMeasureValue(analyticRecord, "CountOUT");

                        InvoiceBillingRecord invoiceBillingRecord = new InvoiceBillingRecord
                        {
                            ServiceTypeId = new Guid(serviceTypeId.Value.ToString()),
                            CountIN = Convert.ToDecimal(countIN.Value ?? 0.0),
                            CountOUT = Convert.ToInt32(countOUT.Value),
                        };
                        if (financialAccountId.Value != null)
                            invoiceBillingRecord.FinancialAccountId = Convert.ToInt64(financialAccountId.Value);
                        AddItemToDictionary(itemSetNamesDic, "GroupedByServiceType", invoiceBillingRecord);
                    }
                }
            }
            return itemSetNamesDic;
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
                        InvoiceItemDetails subscriberInvoiceItemDetails = new InvoiceItemDetails()
                        {
                            CountIN = item.CountIN,
                            CountOUT = item.CountOUT,
                            ServiceTypeId = item.ServiceTypeId,
                            FinancialAccountId = item.FinancialAccountId,

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
                        retailSubscriberInvoiceDetails.CountIN += invoiceBillingRecord.CountIN;
                        retailSubscriberInvoiceDetails.CountOUT += invoiceBillingRecord.CountOUT;
                    }
                };
            }
            return retailSubscriberInvoiceDetails;
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
            return analyticManager.GetFilteredRecords(analyticQuery) as Vanrise.Analytic.Entities.AnalyticSummaryBigResult<AnalyticRecord>;
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
        public class InvoiceBillingRecord
        {
            public Guid? ServiceTypeId { get; set; }
            public long? FinancialAccountId { get; set; }
            public Decimal CountOUT { get; set; }
            public Decimal CountIN { get; set; }
        }

    }
}
