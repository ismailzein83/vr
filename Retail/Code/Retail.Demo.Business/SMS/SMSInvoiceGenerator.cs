using Retail.BusinessEntity.Business;
using Retail.BusinessEntity.Entities;
using Retail.Demo.Data;
using Retail.Demo.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Invoice.Entities;
using Vanrise.Common.Business;
using Vanrise.Analytic.Entities;
using Vanrise.Analytic.Business;
using Vanrise.Entities;

namespace Retail.Demo.Business
{
    public class SMSInvoiceGenerator : InvoiceGenerator
    {
        Guid _acountBEDefinitionId { get; set; }
        SMSInvoiceType _type;

        public SMSInvoiceGenerator(Guid acountBEDefinitionId, SMSInvoiceType type)
        {
            this._acountBEDefinitionId = acountBEDefinitionId;
            this._type = type;
        }

        #region Fields

        private FinancialAccountManager _financialAccountManager = new FinancialAccountManager();
        private AccountBEManager _accountBEManager = new AccountBEManager();
        private CurrencyManager _currencyManager = new CurrencyManager();

        #endregion
        public override void GenerateInvoice(IInvoiceGenerationContext context)
        {
            List<string> listMeasures = new List<string> { "CustomerAmount", "TotalAttempts" };
            List<string> listDimensions = new List<string> { "Customer", "CustomerCurrencyId", "DestinationZone" };

            FinancialAccountData financialAccountData = _financialAccountManager.GetFinancialAccountData(_acountBEDefinitionId, context.PartnerId);

            if (context.FromDate < financialAccountData.FinancialAccount.BED || context.ToDate > financialAccountData.FinancialAccount.EED)
            {
                context.ErrorMessage = "From date and To date should be within the effective date of financial account.";
                context.GenerateInvoiceResult = GenerateInvoiceResult.Failed;
                return;
            }

            DateTime fromDate = context.FromDate;
            DateTime toDate = context.ToDate;

            string dimensionName = "CustomerBillingAccountId";
            string dimensionValue = financialAccountData.FinancialAccountId;

            int currencyId = _accountBEManager.GetCurrencyId(this._acountBEDefinitionId, financialAccountData.Account.AccountId);

            var analyticResult = GetFilteredRecords(listDimensions, listMeasures, dimensionName, dimensionValue, fromDate, toDate, currencyId);
            if (analyticResult == null || analyticResult.Data == null || analyticResult.Data.Count() == 0)
            {
                context.GenerateInvoiceResult = GenerateInvoiceResult.NoData;
                return;
            }

            Dictionary<string, List<InvoiceMeasures>> itemSetNamesDic = ConvertAnalyticDataToDictionary(analyticResult.Data, currencyId);
            if (itemSetNamesDic.Count == 0)
            {
                context.GenerateInvoiceResult = GenerateInvoiceResult.NoData;
                return;
            }

            List<GeneratedInvoiceItemSet> generatedInvoiceItemSets = BuildGeneratedInvoiceItemSet(itemSetNamesDic);
            decimal amount = 0;
            int numberOfSMS = 0;
            if (generatedInvoiceItemSets != null && generatedInvoiceItemSets.Count > 0)
                foreach (var generatedInvoiceItemSet in generatedInvoiceItemSets)
                {
                    foreach (var item in generatedInvoiceItemSet.Items)
                    {
                        amount += item.Details.Amount;
                        numberOfSMS += item.Details.NumberOfSMS;
                    }
                }
            context.Invoice = new GeneratedInvoice
            {
                InvoiceItemSets = generatedInvoiceItemSets,
                InvoiceDetails = new CustomerSMSInvoiceDetails()
                {
                    Amount = amount,
                    CurrencyId = currencyId,
                    CurrencyIdDescription = _currencyManager.GetCurrencySymbol(currencyId),
                    NumberOfSMS = numberOfSMS
                }
            };
        }

        private List<GeneratedInvoiceItemSet> BuildGeneratedInvoiceItemSet(Dictionary<string, List<InvoiceMeasures>> itemSetNamesDic)
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
                        CustomerSMSInvoiceItemDetails smsInvoiceItemDetails = new Entities.CustomerSMSInvoiceItemDetails()
                        {
                            Amount = item.CustomerAmount,
                            CustomerId = item.CustomerId,
                            NumberOfSMS = item.TotalAttempts,
                            CurrencyId = item.CurrencyId,
                            DestinationZoneId = item.DestinationZoneId
                        };
                        generatedInvoiceItemSet.Items.Add(new GeneratedInvoiceItem
                        {
                            Details = smsInvoiceItemDetails,
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
                    TableId = Guid.Parse("2d09f33c-8b52-44d8-ae23-ca8c7f518bdb"),
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
        private Dictionary<string, List<InvoiceMeasures>> ConvertAnalyticDataToDictionary(IEnumerable<AnalyticRecord> analyticRecords, int currencyId)
        {
            Dictionary<string, List<InvoiceMeasures>> itemSetNamesDic = new Dictionary<string, List<InvoiceMeasures>>();
            if (analyticRecords != null)
            {
                foreach (var analyticRecord in analyticRecords)
                {
                    #region ReadDataFromAnalyticResult
                    DimensionValue customer = analyticRecord.DimensionValues.ElementAtOrDefault(0);
                    DimensionValue currency = analyticRecord.DimensionValues.ElementAtOrDefault(1);
                    DimensionValue destinationZoneId = analyticRecord.DimensionValues.ElementAtOrDefault(2);


                    MeasureValue customerAmount = GetMeasureValue(analyticRecord, "CustomerAmount");
                    MeasureValue totalAttempts = GetMeasureValue(analyticRecord, "TotalAttempts");
                    #endregion

                    var customerAmountValue = Convert.ToDecimal(customerAmount == null ? 0.0 : customerAmount.Value ?? 0.0);
                    if (customerAmountValue != 0)
                    {

                        var invoiceMeasures = new InvoiceMeasures
                        {
                            CustomerId = Convert.ToInt64(customer.Value),
                            CustomerAmount = customerAmountValue,
                            TotalAttempts = Convert.ToInt32(totalAttempts.Value ?? 00),
                            CurrencyId = currency != null && currency.Value != null ? Convert.ToInt32(currency.Value) : default(int),
                            DestinationZoneId = Convert.ToInt64(destinationZoneId.Value)
                        };
                        AddItemToDictionary(itemSetNamesDic, "CustomerSMSInvoiceItemDetails", invoiceMeasures);
                    }

                }
            }
            return itemSetNamesDic;
        }
        private void AddItemToDictionary<T>(Dictionary<T, List<InvoiceMeasures>> itemSetNamesDic, T key, InvoiceMeasures invoiceMeasures)
        {
            if (itemSetNamesDic == null)
                itemSetNamesDic = new Dictionary<T, List<InvoiceMeasures>>();
            List<InvoiceMeasures> invoiceMeasuresList = null;

            if (!itemSetNamesDic.TryGetValue(key, out invoiceMeasuresList))
            {
                invoiceMeasuresList = new List<InvoiceMeasures>();
                invoiceMeasuresList.Add(invoiceMeasures);
                itemSetNamesDic.Add(key, invoiceMeasuresList);
            }
            else
            {
                invoiceMeasuresList.Add(invoiceMeasures);
                itemSetNamesDic[key] = invoiceMeasuresList;
            }
        }
        public class InvoiceMeasures
        {
            public long CustomerId { get; set; }
            public decimal CustomerAmount { get; set; }
            public int TotalAttempts { get; set; }
            public long DestinationZoneId { get; set; }
            public int CurrencyId { get; set; }

        }
    }
}
