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
        public override void GenerateInvoice(IInvoiceGenerationContext context)
        {
            List<string> listMeasures = new List<string> { "SaleNet", "NumberOfCalls", "SaleDuration", "BillingPeriodTo", "BillingPeriodFrom", "SaleNet_OrigCurr" };
            List<string> listDimensions = new List<string> {"Customer","SaleZone", "SaleCurrency", "SaleRate", "SaleRateType" };
            string[] partner = context.PartnerId.Split('_');
            string dimentionName = null;

            string partnerType = partner[0];
            int parterId = Convert.ToInt32(partner[1]);
            int currencyId = -1;
            CarrierProfileManager carrierProfileManager = new CarrierProfileManager();
            IEnumerable<VRTaxItemDetail> taxItemDetails = null;
            CarrierProfile carrierProfile = null;
            
            if (partnerType.Equals("Profile"))
            {
                dimentionName = "CustomerProfile";
                carrierProfile = carrierProfileManager.GetCarrierProfile(parterId);
                currencyId = carrierProfileManager.GetCarrierProfileCurrencyId(parterId);
                taxItemDetails = carrierProfileManager.GetTaxItemDetails(parterId);
            }
            else if (partnerType.Equals("Account"))
            {
                dimentionName = "Customer";
                CarrierAccountManager carrierAccountManager = new CarrierAccountManager();
                currencyId = carrierAccountManager.GetCarrierAccountCurrencyId(parterId);
                var carrierAccount  = carrierAccountManager.GetCarrierAccount(parterId);
                carrierProfile = carrierProfileManager.GetCarrierProfile(carrierAccount.CarrierProfileId);
                taxItemDetails = carrierProfileManager.GetTaxItemDetails(carrierAccount.CarrierProfileId);
            }

            var analyticResult = GetFilteredRecords(listDimensions, listMeasures, dimentionName, partner[1], context.FromDate, context.GeneratedToDate,currencyId);
            if (analyticResult == null || analyticResult.Data == null || analyticResult.Data.Count() == 0)
            {
                throw new InvoiceGeneratorException("No data available between the selected period.");
            }
            Dictionary<string, List<InvoiceBillingRecord>> itemSetNamesDic = ConvertAnalyticDataToDictionary(analyticResult.Data, currencyId);

            List<GeneratedInvoiceItemSet> generatedInvoiceItemSets = BuildGeneratedInvoiceItemSet(itemSetNamesDic, taxItemDetails);
            #region BuildCustomerInvoiceDetails
            CustomerInvoiceDetails customerInvoiceDetails = BuilCustomerInvoiceDetails(itemSetNamesDic, partner[0],context.FromDate,context.ToDate);
            if (customerInvoiceDetails != null)
            {
                customerInvoiceDetails.TotalAmount = customerInvoiceDetails.SaleAmount;
                if (taxItemDetails != null)
                {
                    foreach (var tax in taxItemDetails)
                    {
                        customerInvoiceDetails.TotalAmount += ((customerInvoiceDetails.SaleAmount * Convert.ToDecimal(tax.Value)) / 100);
                    }
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
                        };
                        generatedInvoiceItemSet.Items.Add(new GeneratedInvoiceItem
                        {
                            Details = customerInvoiceItemDetails,
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
                    TableId = 8,
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
        private Dictionary<string, List<InvoiceBillingRecord>> ConvertAnalyticDataToDictionary(IEnumerable<AnalyticRecord> analyticRecords, int currencyId)
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
                        SaleRateTypeId = saleRateTypeId != null && saleRateTypeId.Value != null ? Convert.ToInt32(saleRateTypeId.Value) : default(int?),
                        SaleZoneId = Convert.ToInt64(saleZoneId.Value),
                        InvoiceMeasures = new InvoiceMeasures
                        {
                            BillingPeriodFrom = billingPeriodFrom != null ? Convert.ToDateTime(billingPeriodFrom.Value) : default(DateTime),
                            BillingPeriodTo = billingPeriodTo != null ? Convert.ToDateTime(billingPeriodTo.Value) : default(DateTime),
                            SaleDuration = Convert.ToDecimal(saleDuration.Value ?? 0.0),
                            SaleNet = Convert.ToDecimal(saleNet == null ? 0.0 : saleNet.Value ?? 0.0),
                            NumberOfCalls = Convert.ToInt32(calls.Value ?? 0.0),
                            SaleNet_OrigCurr = Convert.ToDecimal(saleNet_OrigCurr == null ? 0.0 : saleNet_OrigCurr.Value ?? 0.0),
                        }

                    };
                    AddItemToDictionary(itemSetNamesDic, "GroupedBySaleZone", invoiceBillingRecord);
                }
            }
            return itemSetNamesDic;
        }


        //private void GroupingData(Dictionary<int, InvoiceBillingRecord> groupedDic, int key, InvoiceBillingRecord newInvoiceBillingRecord)
        //{
        //    InvoiceBillingRecord invoiceBillingRecord = null;
        //    if (!groupedDic.TryGetValue(key, out invoiceBillingRecord))
        //    {
        //        invoiceBillingRecord = new InvoiceBillingRecord
        //        {
        //            SaleRateTypeId = newInvoiceBillingRecord.SaleRateTypeId,
        //            SaleRate = newInvoiceBillingRecord.SaleRate,
        //            SaleZoneId = newInvoiceBillingRecord.SaleZoneId,
        //            SaleCurrencyId = newInvoiceBillingRecord.SaleCurrencyId,
        //            OriginalSaleCurrencyId = newInvoiceBillingRecord.OriginalSaleCurrencyId,
        //            CustomerId = newInvoiceBillingRecord.CustomerId,
        //            InvoiceMeasures = new InvoiceMeasures
        //            {
        //                SaleNet = newInvoiceBillingRecord.InvoiceMeasures.SaleNet,
        //                SaleDuration = newInvoiceBillingRecord.InvoiceMeasures.SaleDuration,
        //                BillingPeriodFrom = newInvoiceBillingRecord.InvoiceMeasures.BillingPeriodFrom,
        //                BillingPeriodTo = newInvoiceBillingRecord.InvoiceMeasures.BillingPeriodTo,
        //                NumberOfCalls = newInvoiceBillingRecord.InvoiceMeasures.NumberOfCalls,
        //                SaleNet_OrigCurr = newInvoiceBillingRecord.InvoiceMeasures.SaleNet_OrigCurr,
        //            }
        //        };
        //        groupedDic.Add(key, invoiceBillingRecord);
        //    }
        //    else
        //    {
        //        invoiceBillingRecord.InvoiceMeasures.SaleDuration += newInvoiceBillingRecord.InvoiceMeasures.SaleDuration;
        //        invoiceBillingRecord.InvoiceMeasures.SaleNet += newInvoiceBillingRecord.InvoiceMeasures.SaleNet;
        //        invoiceBillingRecord.InvoiceMeasures.NumberOfCalls += newInvoiceBillingRecord.InvoiceMeasures.NumberOfCalls;
        //        invoiceBillingRecord.InvoiceMeasures.BillingPeriodTo = invoiceBillingRecord.InvoiceMeasures.BillingPeriodTo > newInvoiceBillingRecord.InvoiceMeasures.BillingPeriodTo ? invoiceBillingRecord.InvoiceMeasures.BillingPeriodTo : newInvoiceBillingRecord.InvoiceMeasures.BillingPeriodTo;
        //        invoiceBillingRecord.InvoiceMeasures.BillingPeriodFrom = invoiceBillingRecord.InvoiceMeasures.BillingPeriodFrom < newInvoiceBillingRecord.InvoiceMeasures.BillingPeriodFrom ? invoiceBillingRecord.InvoiceMeasures.BillingPeriodFrom : newInvoiceBillingRecord.InvoiceMeasures.BillingPeriodFrom;
        //        groupedDic[key] = invoiceBillingRecord;
        //    }
        //}
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
            public decimal SaleNet { get; set; }
            public decimal SaleNet_OrigCurr { get; set; }
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
            public int? SaleRateTypeId { get; set; }
            public int SaleCurrencyId { get; set; }

        }
    }
}
