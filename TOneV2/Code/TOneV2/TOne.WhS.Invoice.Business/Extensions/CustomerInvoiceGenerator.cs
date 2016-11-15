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

namespace TOne.WhS.Invoice.Business.Extensions
{
    public class CustomerInvoiceGenerator : InvoiceGenerator
    {
        public override Guid ConfigId { get { return  new Guid("BD4F7B2C-1C07-4037-8730-92768BD28900"); } }
        public override void GenerateInvoice(IInvoiceGenerationContext context)
        {
            List<string> listMeasures = new List<string> { "SaleNet", "NumberOfCalls", "SaleDuration", "BillingPeriodTo", "BillingPeriodFrom", "SaleNet_OrigCurr" };
            List<string> listDimensions = new List<string> {"Customer","SaleZone", "SaleCurrency", "SaleRate", "SaleRateType" };
            string[] partner = context.PartnerId.Split('_');
            string dimentionName = null;

            string partnerType = partner[0];
            int parterId = Convert.ToInt32(partner[1]);
            int currencyId = -1;
            bool isGroupedByCustomer = false;
           CarrierProfileManager carrierProfileManager = new CarrierProfileManager();

            CarrierProfile carrierProfile = null;
            if (partnerType.Equals("Profile"))
            {
                dimentionName = "CustomerProfile";
                carrierProfile = carrierProfileManager.GetCarrierProfile(parterId);
                currencyId = carrierProfileManager.GetCarrierProfileCurrencyId(parterId);
                isGroupedByCustomer = true;
            }
            else if (partnerType.Equals("Account"))
            {
                dimentionName = "Customer";
                CarrierAccountManager carrierAccountManager = new CarrierAccountManager();
                currencyId = carrierAccountManager.GetCarrierAccountCurrencyId(parterId);
                var carrierAccount  = carrierAccountManager.GetCarrierAccount(parterId);
                carrierProfile = carrierProfileManager.GetCarrierProfile(carrierAccount.CarrierProfileId);
                isGroupedByCustomer = false;
            }

            var analyticResult = GetFilteredRecords(listDimensions, listMeasures, dimentionName, partner[1], context.FromDate, context.ToDate);
            if (analyticResult == null || analyticResult.Data == null || analyticResult.Data.Count() == 0)
            {
                throw new InvoiceGeneratorException("No data available between the selected period.");
            }
            Dictionary<string, List<InvoiceBillingRecord>> itemSetNamesDic = ConvertAnalyticDataToDictionary(analyticResult.Data, currencyId, isGroupedByCustomer);
            List<GeneratedInvoiceItemSet> generatedInvoiceItemSets = BuildGeneratedInvoiceItemSet(itemSetNamesDic);
            #region BuildCustomerInvoiceDetails
            CustomerInvoiceDetails customerInvoiceDetails = BuilCustomerInvoiceDetails(itemSetNamesDic, partner[0],context.FromDate,context.ToDate);
            if (customerInvoiceDetails != null)
            {
                customerInvoiceDetails.TotalAmount = customerInvoiceDetails.SaleAmount;
                if (carrierProfile.Settings.TaxSetting != null)
                {
                    if (carrierProfile.Settings.TaxSetting.Items != null)
                    {
                        foreach (var tax in carrierProfile.Settings.TaxSetting.Items)
                        {
                            customerInvoiceDetails.TotalAmount += ((customerInvoiceDetails.SaleAmount * Convert.ToDouble(tax.Value)) / 100);
                        }
                    }
                    customerInvoiceDetails.TotalAmount += (customerInvoiceDetails.SaleAmount * Convert.ToDouble(carrierProfile.Settings.TaxSetting.VAT)) / 100;
                }
            }
           
            #endregion

            context.Invoice = new GeneratedInvoice
            {
                InvoiceDetails = customerInvoiceDetails,
                InvoiceItemSets = generatedInvoiceItemSets,
            };
        }
        private CustomerInvoiceDetails BuilCustomerInvoiceDetails(Dictionary<string, List<InvoiceBillingRecord>> itemSetNamesDic, string partnerType,DateTime fromDate,DateTime toDate)
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
                        }
                        customerInvoiceDetails.ToDate = toDate;
                        customerInvoiceDetails.FromDate = fromDate;
                    };
                }
            }
            customerInvoiceDetails.OriginalSaleCurrency = currencyManager.GetCurrencySymbol(customerInvoiceDetails.OriginalSaleCurrencyId);
            customerInvoiceDetails.SaleCurrency = currencyManager.GetCurrencySymbol(customerInvoiceDetails.SaleCurrencyId);
            return customerInvoiceDetails;
        }
        private List<GeneratedInvoiceItemSet> BuildGeneratedInvoiceItemSet(Dictionary<string, List<InvoiceBillingRecord>> itemSetNamesDic)
        {
            CurrencyManager currencyManager = new CurrencyManager();
            CarrierAccountManager carrierAccountManager = new CarrierAccountManager();
            SaleZoneManager saleZoneManager = new SaleZoneManager();
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
                            SaleCurrency = currencyManager.GetCurrencySymbol(item.SaleCurrencyId),
                            OriginalSaleCurrency = currencyManager.GetCurrencySymbol(item.OriginalSaleCurrencyId),
                            CustomerName = carrierAccountManager.GetCarrierAccountName(item.CustomerId),
                            SaleZoneName = saleZoneManager.GetSaleZoneName(item.SaleZoneId),
                        };
                        generatedInvoiceItemSet.Items.Add(new GeneratedInvoiceItem
                        {
                            Details = customerInvoiceItemDetails,
                            Name = " "
                        });
                    }
                    generatedInvoiceItemSets.Add(generatedInvoiceItemSet);

                }
            }
            return generatedInvoiceItemSets;
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
        private Dictionary<string, List<InvoiceBillingRecord>> ConvertAnalyticDataToDictionary(IEnumerable<AnalyticRecord> analyticRecords, int currencyId,bool isGroupedByCustomer)
        {
            Dictionary<string, List<InvoiceBillingRecord>> itemSetNamesDic = new Dictionary<string, List<InvoiceBillingRecord>>();
            if (analyticRecords != null)
            {
                Dictionary<int, InvoiceBillingRecord> groupedByCustomerDic = new Dictionary<int, InvoiceBillingRecord>();
                foreach (var analyticRecord in analyticRecords)
                {

                    #region ReadDataFromAnalyticResult
                    DimensionValue customerId = analyticRecord.DimensionValues.ElementAtOrDefault(0);
                    DimensionValue saleZoneId = analyticRecord.DimensionValues.ElementAtOrDefault(1);
                    DimensionValue saleCurrencyId = analyticRecord.DimensionValues.ElementAtOrDefault(2);
                    DimensionValue saleRate = analyticRecord.DimensionValues.ElementAtOrDefault(3);
                    DimensionValue saleRateTypeId = analyticRecord.DimensionValues.ElementAtOrDefault(4);

                    MeasureValue saleNet_OrigCurr = GetMeasureValue(analyticRecord, "SaleNet_OrigCurr");
                    MeasureValue saleDuration = GetMeasureValue(analyticRecord, "SaleDuration");
                    MeasureValue saleNet = GetMeasureValue(analyticRecord, "SaleNet");
                    MeasureValue calls = GetMeasureValue(analyticRecord, "NumberOfCalls");
                    MeasureValue billingPeriodTo = GetMeasureValue(analyticRecord, "BillingPeriodTo");
                    MeasureValue billingPeriodFrom = GetMeasureValue(analyticRecord, "BillingPeriodFrom");

                    #endregion
                
                    InvoiceBillingRecord invoiceBillingRecord = new InvoiceBillingRecord
                    {
                        CustomerId = Convert.ToInt32(customerId.Value),
                        SaleCurrencyId = currencyId,
                        OriginalSaleCurrencyId = Convert.ToInt32(saleCurrencyId.Value),
                        SaleRate = saleRate != null ? Convert.ToDecimal(saleRate.Value) : default(Decimal),
                        SaleRateTypeId = saleRateTypeId != null && saleRateTypeId.Value != null ? Convert.ToInt32(saleRateTypeId.Value) : default(int),
                        SaleZoneId = Convert.ToInt64(saleZoneId.Value),
                        InvoiceMeasures = new InvoiceMeasures
                        {
                            BillingPeriodFrom = billingPeriodFrom != null ? Convert.ToDateTime(billingPeriodFrom.Value) : default(DateTime),
                            BillingPeriodTo = billingPeriodTo != null ? Convert.ToDateTime(billingPeriodTo.Value) : default(DateTime),
                            SaleDuration = Convert.ToDecimal(saleDuration.Value ?? 0.0),
                            SaleNet = Convert.ToDouble(saleNet == null ? 0.0 : saleNet.Value ?? 0.0),
                            NumberOfCalls = Convert.ToInt32(calls.Value ?? 0.0),
                            SaleNet_OrigCurr = Convert.ToDouble(saleNet_OrigCurr == null ? 0.0 : saleNet_OrigCurr.Value ?? 0.0),
                        }

                    };

                    AddItemToDictionary(itemSetNamesDic, "GroupedBySaleZone", invoiceBillingRecord);

                    #region Grouping By Customer
                    if (isGroupedByCustomer)
                    {
                        AddItemToDictionary(itemSetNamesDic, string.Format("GroupedByCustomer_{0}", invoiceBillingRecord.CustomerId), invoiceBillingRecord);

                        InvoiceBillingRecord customerInvoiceBillingRecord = null;
                        if (!groupedByCustomerDic.TryGetValue(invoiceBillingRecord.CustomerId, out customerInvoiceBillingRecord))
                        {
                            customerInvoiceBillingRecord = new InvoiceBillingRecord
                            {
                                SaleRateTypeId = invoiceBillingRecord.SaleRateTypeId,
                                SaleRate = invoiceBillingRecord.SaleRate,
                                SaleZoneId = invoiceBillingRecord.SaleZoneId,
                                SaleCurrencyId = invoiceBillingRecord.SaleCurrencyId,
                                OriginalSaleCurrencyId = invoiceBillingRecord.OriginalSaleCurrencyId,
                                CustomerId = invoiceBillingRecord.CustomerId,
                                InvoiceMeasures = new InvoiceMeasures
                                {
                                    SaleNet = invoiceBillingRecord.InvoiceMeasures.SaleNet,
                                    SaleDuration = invoiceBillingRecord.InvoiceMeasures.SaleDuration,
                                    BillingPeriodFrom = invoiceBillingRecord.InvoiceMeasures.BillingPeriodFrom,
                                    BillingPeriodTo = invoiceBillingRecord.InvoiceMeasures.BillingPeriodTo,
                                    NumberOfCalls = invoiceBillingRecord.InvoiceMeasures.NumberOfCalls,
                                    SaleNet_OrigCurr = invoiceBillingRecord.InvoiceMeasures.SaleNet_OrigCurr,
                                }
                            };
                            groupedByCustomerDic.Add(invoiceBillingRecord.CustomerId, customerInvoiceBillingRecord);
                        }
                        else
                        {
                            customerInvoiceBillingRecord.InvoiceMeasures.SaleDuration += invoiceBillingRecord.InvoiceMeasures.SaleDuration;
                            customerInvoiceBillingRecord.InvoiceMeasures.SaleNet += invoiceBillingRecord.InvoiceMeasures.SaleNet;
                            customerInvoiceBillingRecord.InvoiceMeasures.NumberOfCalls += invoiceBillingRecord.InvoiceMeasures.NumberOfCalls;

                            customerInvoiceBillingRecord.InvoiceMeasures.BillingPeriodTo = customerInvoiceBillingRecord.InvoiceMeasures.BillingPeriodTo > invoiceBillingRecord.InvoiceMeasures.BillingPeriodTo ? customerInvoiceBillingRecord.InvoiceMeasures.BillingPeriodTo : invoiceBillingRecord.InvoiceMeasures.BillingPeriodTo;
                            customerInvoiceBillingRecord.InvoiceMeasures.BillingPeriodFrom = customerInvoiceBillingRecord.InvoiceMeasures.BillingPeriodFrom < invoiceBillingRecord.InvoiceMeasures.BillingPeriodFrom ? customerInvoiceBillingRecord.InvoiceMeasures.BillingPeriodFrom : invoiceBillingRecord.InvoiceMeasures.BillingPeriodFrom;
                            groupedByCustomerDic[invoiceBillingRecord.CustomerId] = customerInvoiceBillingRecord;
                        }

                    }
                    #endregion

                }
                if (isGroupedByCustomer)
                {
                    foreach (var item in groupedByCustomerDic)
                    {
                        AddItemToDictionary(itemSetNamesDic, "GroupedByCustomer", item.Value);
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
            public Double SaleNet { get; set; }
            public Double SaleNet_OrigCurr { get; set; }
            public int NumberOfCalls { get; set; }
            public Decimal SaleDuration { get; set; }
            public DateTime BillingPeriodTo { get; set; }
            public DateTime BillingPeriodFrom { get; set; }


        } 
        public  class InvoiceBillingRecord
        {
            public InvoiceMeasures InvoiceMeasures { get; set; }
            public long SaleZoneId { get; set; }
            public int CustomerId { get; set; }
            public int OriginalSaleCurrencyId { get; set; }
            public Decimal SaleRate { get; set; }
            public int SaleRateTypeId { get; set; }
            public int SaleCurrencyId { get; set; }

        }
    }
}
