using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.Invoice.Entities;

using Vanrise.Analytic.Business;
using Vanrise.Analytic.Entities;
using Vanrise.Common.Business;
using Vanrise.Entities;
using Vanrise.Invoice.Entities;

namespace TOne.WhS.Invoice.Business.Extensions
{
    public class CustomerInvoiceGenerator : InvoiceGenerator
    {
        public override Guid ConfigId { get { return  new Guid("BD4F7B2C-1C07-4037-8730-92768BD28900"); } }
        public override void GenerateInvoice(IInvoiceGenerationContext context)
        {
            List<GeneratedInvoiceItemSet> generatedInvoiceItemSets = new List<GeneratedInvoiceItemSet>();

            List<string> listMeasures = new List<string> { "SaleNet", "NumberOfCalls", "SaleDuration", "BillingPeriodTo", "BillingPeriodFrom" };
            List<string> listDimensions = new List<string> {"SaleZone", "SaleCurrency", "SaleRate", "SaleRateType" };
            string[] partner = context.PartnerId.Split('_');
            string dimentionName = null;

            string partnerType = partner[0];
            if (partnerType.Equals("Profile"))
            {
                dimentionName = "CustomerProfile";
            }
            else if (partnerType.Equals("Account"))
            {
                dimentionName = "Customer";
            }

            var analyticResult = GetFilteredRecords(listDimensions, listMeasures, dimentionName, partner[1], context.FromDate, context.ToDate);

            int currencyId = -1;
            Dictionary<string, List<InvoiceBillingRecord>> itemSetNamesDic = new Dictionary<string,List<InvoiceBillingRecord>>();

            if (partnerType.Equals("Profile"))
            {
                CarrierProfileManager carrierProfileManager = new CarrierProfileManager();
                currencyId = carrierProfileManager.GetCarrierProfileCurrencyId(Convert.ToInt32(partner[1]));



            }
            else if (partnerType.Equals("Account"))
            {
                CarrierAccountManager carrierAccountManager = new CarrierAccountManager();
                currencyId = carrierAccountManager.GetCarrierAccountCurrencyId(Convert.ToInt32(partner[1]));

                foreach (var analyticRecord in analyticResult.Data)
                {

                    DimensionValue dimensionId = analyticRecord.DimensionValues.ElementAtOrDefault(0);
                    DimensionValue saleCurrencyId = analyticRecord.DimensionValues.ElementAtOrDefault(1);
                    DimensionValue saleRate = analyticRecord.DimensionValues.ElementAtOrDefault(2);
                    DimensionValue saleRateTypeId = analyticRecord.DimensionValues.ElementAtOrDefault(3);
                    DimensionValue originalSaleRate = analyticRecord.DimensionValues.ElementAtOrDefault(4);


                    MeasureValue saleDuration = GetMeasureValue(analyticRecord, "SaleDuration");
                    MeasureValue saleNet = GetMeasureValue(analyticRecord, "SaleNet");
                    MeasureValue calls = GetMeasureValue(analyticRecord, "NumberOfCalls");
                    MeasureValue billingPeriodTo = GetMeasureValue(analyticRecord, "BillingPeriodTo");
                    MeasureValue billingPeriodFrom = GetMeasureValue(analyticRecord, "BillingPeriodFrom");

                    InvoiceBillingRecord invoiceBillingRecord = new InvoiceBillingRecord
                    {
                        SaleCurrencyId = saleCurrencyId != null ? Convert.ToInt64(saleCurrencyId.Value) : currencyId,
                        SaleRate = saleRate != null ? Convert.ToDecimal(saleRate.Value) : default(Decimal),
                        SaleRateTypeId = saleRateTypeId != null && saleRateTypeId.Value != null ? Convert.ToInt32(saleRateTypeId.Value) : default(int),
                        SaleZoneId = Convert.ToInt64(dimensionId.Value),
                        InvoiceMeasures = new InvoiceMeasures
                        {
                            BillingPeriodFrom = billingPeriodFrom != null ? Convert.ToDateTime(billingPeriodFrom.Value) : default(DateTime),
                            BillingPeriodTo = billingPeriodTo != null ? Convert.ToDateTime(billingPeriodTo.Value) : default(DateTime),
                            SaleDuration = Convert.ToDecimal(saleDuration.Value ?? 0.0),
                            SaleNet = Convert.ToDouble(saleNet == null ? 0.0 : saleNet.Value ?? 0.0),
                            NumberOfCalls = Convert.ToInt32(calls.Value ?? 0.0),
                        }

                    };
                    AddItemToDictionary(itemSetNamesDic, "GroupedBySaleZone", invoiceBillingRecord);
                }
            }




            //List<string> listDimensions = new List<string> { "SaleZone", "SaleCurrency", "SaleRate", "SaleRateType" };
            //var analyticResultBySaleZone = GetFilteredRecords(listDimensions, listMeasures, dimentionName, partner[1], context.FromDate, context.ToDate);
            //BuilInvoiceCustomerItemSet(analyticResultBySaleZone.Data, generatedInvoiceItemSets, "GroupedBySaleZone", context.FromDate, context.ToDate, currencyId);
            //BuilInvoiceCustomerItemSet(analyticResultForCustomer.Data, generatedInvoiceItemSets, "GroupedByCustomer", context.FromDate, context.ToDate, currencyId);




            //if (partner[0].Equals("Profile"))
            //{
            //    dimentionName = "CustomerProfile";
            //    CarrierProfileManager carrierProfileManager= new CarrierProfileManager();
            //    currencyId = carrierProfileManager.GetCarrierProfileCurrencyId(Convert.ToInt32(partner[1]));
            //    List<string> listProfileDimensions = new List<string> { "Customer" };
            //    var analyticResultForCustomer = GetFilteredRecords(listProfileDimensions, listMeasures, dimentionName, partner[1], context.FromDate, context.ToDate);
            //    BuilInvoiceCustomerItemSet(analyticResultForCustomer.Data, generatedInvoiceItemSets, "GroupedByCustomer",context.FromDate,context.ToDate,currencyId);
            //    foreach(var customer in analyticResultForCustomer.Data)
            //    {
            //        DimensionValue customerDimension = customer.DimensionValues[0];

            //        List<string> listCustomerDimensions = new List<string> { "SaleZone", "SaleCurrency", "SaleRate",  "SaleRateType" };
            //        var analyticResultByCustomerSaleZone = GetFilteredRecords(listCustomerDimensions, listMeasures, "Customer", customerDimension.Value, context.FromDate, context.ToDate);
            //        BuilInvoiceCustomerItemSet(analyticResultByCustomerSaleZone.Data, generatedInvoiceItemSets, string.Format("GroupedByCustomer_{0}", customerDimension.Value.ToString()), context.FromDate, context.ToDate,currencyId);
            //    }
            //}
    

            #region BuildCustomerInvoiceDetails
            List<string> listDimensionsForCustomerInvoice = new List<string> { dimentionName };
            var analyticResultForCustomerInvoice = GetFilteredRecords(listDimensionsForCustomerInvoice, listMeasures, dimentionName, partner[1], context.FromDate, context.ToDate);
            CustomerInvoiceDetails customerInvoiceDetails = BuilCustomerInvoiceDetails(analyticResultForCustomerInvoice.Data, partner[0]);
            #endregion

            context.Invoice = new GeneratedInvoice
            {
                InvoiceDetails = customerInvoiceDetails,
                InvoiceItemSets = generatedInvoiceItemSets,
            };
        }

        private CustomerInvoiceDetails BuilCustomerInvoiceDetails(IEnumerable<AnalyticRecord> analyticRecords, string partnerType)
        {
            CustomerInvoiceDetails customerInvoiceDetails = null;
            if (partnerType != null)
            {
                customerInvoiceDetails = new CustomerInvoiceDetails();
                customerInvoiceDetails.PartnerType = partnerType;

                foreach (var analyticRecord in analyticRecords)
                {
                    MeasureValue saleDuration = GetMeasureValue(analyticRecord, "SaleDuration");
                    customerInvoiceDetails.Duration += Convert.ToDecimal(saleDuration.Value ?? 0.0);
                    MeasureValue saleNet = GetMeasureValue(analyticRecord, "SaleNet");
                    customerInvoiceDetails.SaleAmount += Convert.ToDouble(saleNet == null ? 0.0 : saleNet.Value ?? 0.0);
                    MeasureValue calls = GetMeasureValue(analyticRecord, "NumberOfCalls");
                    customerInvoiceDetails.TotalNumberOfCalls += Convert.ToInt32(calls.Value ?? 0.0);
                    MeasureValue billingPeriodTo = GetMeasureValue(analyticRecord, "BillingPeriodTo");
                    customerInvoiceDetails.ToDate = Convert.ToDateTime(billingPeriodTo.Value ?? 0.0);
                    MeasureValue billingPeriodFrom = GetMeasureValue(analyticRecord, "BillingPeriodFrom");
                    customerInvoiceDetails.FromDate = Convert.ToDateTime(billingPeriodTo.Value ?? 0.0);
                }
            }
            return customerInvoiceDetails;
        }
        private void BuilInvoiceCustomerItemSet(IEnumerable<AnalyticRecord> analyticRecords, List<GeneratedInvoiceItemSet> generatedInvoiceItemSets, string itemSetName,DateTime fromDate,DateTime toDate,long currencyId)
        {
            GeneratedInvoiceItemSet generatedInvoiceItemSet = new GeneratedInvoiceItemSet();
            generatedInvoiceItemSet.SetName = itemSetName;
            generatedInvoiceItemSet.Items = new List<GeneratedInvoiceItem>();
            foreach (var analyticRecord in analyticRecords)
            {

                DimensionValue dimensionId = analyticRecord.DimensionValues.ElementAtOrDefault(0);
                DimensionValue saleCurrencyId = analyticRecord.DimensionValues.ElementAtOrDefault(1);
                DimensionValue saleRate = analyticRecord.DimensionValues.ElementAtOrDefault(2);
                DimensionValue saleRateTypeId = analyticRecord.DimensionValues.ElementAtOrDefault(3);
                DimensionValue originalSaleRate = analyticRecord.DimensionValues.ElementAtOrDefault(4);


                MeasureValue saleDuration = GetMeasureValue(analyticRecord,"SaleDuration");
                MeasureValue saleNet = GetMeasureValue(analyticRecord, "SaleNet");
                MeasureValue calls = GetMeasureValue(analyticRecord, "NumberOfCalls");
                MeasureValue billingPeriodTo = GetMeasureValue(analyticRecord, "BillingPeriodTo");
                MeasureValue billingPeriodFrom = GetMeasureValue(analyticRecord, "BillingPeriodFrom");

                CustomerInvoiceItemDetails customerInvoiceItemDetails = new Entities.CustomerInvoiceItemDetails()
                {
                    Duration = Convert.ToDecimal(saleDuration.Value ?? 0.0),
                    NumberOfCalls = Convert.ToInt32(calls.Value ?? 0.0),
                    SaleAmount = Convert.ToDouble(saleNet == null ? 0.0 : saleNet.Value ?? 0.0),
                    DimensionId = Convert.ToInt64(dimensionId.Value),
                    SaleCurrencyId = saleCurrencyId != null ? Convert.ToInt64(saleCurrencyId.Value) : currencyId,
                    SaleRate = saleRate != null?Convert.ToDecimal(saleRate.Value):default(Decimal),
                    SaleRateTypeId = saleRateTypeId != null && saleRateTypeId.Value != null? Convert.ToInt32(saleRateTypeId.Value) : default(int),
                    FromDate = billingPeriodFrom != null?Convert.ToDateTime(billingPeriodFrom.Value):default(DateTime),
                    ToDate = billingPeriodTo != null?Convert.ToDateTime(billingPeriodTo.Value):default(DateTime),
                    OriginalSaleRate = originalSaleRate != null ? Convert.ToDecimal(originalSaleRate.Value) : default(Decimal),
                };
                generatedInvoiceItemSet.Items.Add(new GeneratedInvoiceItem
                {
                    Details = customerInvoiceItemDetails,
                    Name = "SaleZone"
                });
            }
            generatedInvoiceItemSets.Add(generatedInvoiceItemSet);
        }
        private AnalyticSummaryBigResult<AnalyticRecord> GetFilteredRecords(List<string> listDimensions, List<string> listMeasures, string dimentionFilterName, object dimentionFilterValue, DateTime fromDate, DateTime toDate)
        {
            AnalyticManager analyticManager = new AnalyticManager();
            Vanrise.Entities.DataRetrievalInput<AnalyticQuery> analyticQuery = new DataRetrievalInput<AnalyticQuery>()
            {
                Query = new AnalyticQuery()
                {
                    DimensionFields = listDimensions,
                    MeasureFields = listMeasures,
                    TableId = 8,
                    FromTime = fromDate,
                    ToTime = toDate,
                    ParentDimensions = new List<string>(),
                    Filters = new List<DimensionFilter>(),
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


        private void AddItemToDictionary(Dictionary<string, List<InvoiceBillingRecord>> itemSetNamesDic, string itemSetName, InvoiceBillingRecord invoiceBillingRecord)
        {
            if (itemSetNamesDic == null)
                itemSetNamesDic = new Dictionary<string, List<InvoiceBillingRecord>>();
            List<InvoiceBillingRecord> invoiceBillingRecordList = null;

            if(!itemSetNamesDic.TryGetValue(itemSetName,out invoiceBillingRecordList))
            {
                invoiceBillingRecordList = new List<InvoiceBillingRecord>();
                invoiceBillingRecordList.Add(invoiceBillingRecord);
                itemSetNamesDic.Add(itemSetName, invoiceBillingRecordList);
            }else
            {
                invoiceBillingRecordList.Add(invoiceBillingRecord);
                itemSetNamesDic[itemSetName] = invoiceBillingRecordList;
            }
        }
        public class InvoiceMeasures
        {
            public Double SaleNet { get; set; }
            public int NumberOfCalls { get; set; }
            public Decimal SaleDuration { get; set; }
            public DateTime BillingPeriodTo { get; set; }
            public DateTime BillingPeriodFrom { get; set; }


        }
        public  class InvoiceBillingRecord
        {
            public InvoiceMeasures InvoiceMeasures { get; set; }
            public int CustomerId { get; set; }
            public int ProfileId { get; set; }
            public long SaleZoneId { get; set; }
            public long SaleCurrencyId { get; set; }
            public Decimal SaleRate { get; set; }
            public int SaleRateTypeId { get; set; }
            public long OriginalSaleCurrencyId { get; set; }
            public Decimal OriginalSaleRate { get; set; }
            public int OriginalSaleRateTypeId { get; set; }

        }
    }
}
