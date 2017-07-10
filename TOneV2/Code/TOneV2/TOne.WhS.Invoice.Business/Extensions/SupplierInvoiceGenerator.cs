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
    public class SupplierInvoiceGenerator : InvoiceGenerator
    {
        public override void GenerateInvoice(IInvoiceGenerationContext context)
        {
            List<string> listMeasures = new List<string> { "CostNet", "NumberOfCalls", "CostDuration", "BillingPeriodTo", "BillingPeriodFrom", "CostNet_OrigCurr" };
            List<string> listDimensions = new List<string> { "Supplier", "SupplierZone", "SupplierCurrency", "SupplierRate", "SupplierRateType" };
            string dimentionName = null;
            int currencyId = -1;
            IEnumerable<VRTaxItemDetail> taxItemDetails = null;
            CarrierProfileManager carrierProfileManager = new CarrierProfileManager();
            InvoiceAccountManager invoiceAccountManager = new Business.InvoiceAccountManager();
            var invoiceAccount = invoiceAccountManager.GetInvoiceAccount(Convert.ToInt32(context.PartnerId));
            string partnerType = null;
            int dimensionValue;
            int carrierProfileId;
            if(invoiceAccount.CarrierProfileId.HasValue)
            {
                partnerType = "Profile";
                dimensionValue = invoiceAccount.CarrierProfileId.Value;
                carrierProfileId = invoiceAccount.CarrierProfileId.Value;
                dimentionName = "SupplierProfile";
                currencyId = carrierProfileManager.GetCarrierProfileCurrencyId(dimensionValue);
            }
            else
            {
                partnerType = "Account";
                dimentionName = "Supplier";
                dimensionValue = invoiceAccount.CarrierAccountId.Value;
                CarrierAccountManager carrierAccountManager = new CarrierAccountManager();
                currencyId = carrierAccountManager.GetCarrierAccountCurrencyId(dimensionValue);
                var carrierAccount = carrierAccountManager.GetCarrierAccount(dimensionValue);
                carrierProfileId = carrierAccount.CarrierProfileId;
            }
            taxItemDetails = carrierProfileManager.GetTaxItemDetails(carrierProfileId);
            var analyticResult = GetFilteredRecords(listDimensions, listMeasures, dimentionName, dimensionValue, context.FromDate, context.GeneratedToDate, currencyId);
            if (analyticResult == null || analyticResult.Data == null || analyticResult.Data.Count() == 0)
            {
                throw new InvoiceGeneratorException("No data available between the selected period.");
            }
            Dictionary<string, List<InvoiceBillingRecord>> itemSetNamesDic = ConvertAnalyticDataToDictionary(analyticResult.Data, currencyId);
            List<GeneratedInvoiceItemSet> generatedInvoiceItemSets = BuildGeneratedInvoiceItemSet(itemSetNamesDic, taxItemDetails);
            #region BuildSupplierInvoiceDetails
            SupplierInvoiceDetails supplierInvoiceDetails = BuilSupplierInvoiceDetails(itemSetNamesDic, partnerType, context.FromDate, context.ToDate);
            if (supplierInvoiceDetails != null)
            {
                supplierInvoiceDetails.TotalAmount = supplierInvoiceDetails.CostAmount;
                if (taxItemDetails != null)
                {
                    foreach (var tax in taxItemDetails)
                    {
                        supplierInvoiceDetails.TotalAmount += ((supplierInvoiceDetails.CostAmount * Convert.ToDecimal(tax.Value)) / 100);
                    }
                }
            }
           
            #endregion

            context.Invoice = new GeneratedInvoice
            {
                InvoiceDetails = supplierInvoiceDetails,
                InvoiceItemSets = generatedInvoiceItemSets,
            };
        }
        private SupplierInvoiceDetails BuilSupplierInvoiceDetails(Dictionary<string, List<InvoiceBillingRecord>> itemSetNamesDic, string partnerType,DateTime fromDate,DateTime toDate)
        {
            CurrencyManager currencyManager = new CurrencyManager();
            SupplierInvoiceDetails supplierInvoiceDetails = null;
            if (partnerType != null)
            {

                if (itemSetNamesDic != null)
                {
                    List<InvoiceBillingRecord> invoiceBillingRecordList = null;
                    if (itemSetNamesDic.TryGetValue("GroupedByCostZone", out invoiceBillingRecordList))
                    {
                         supplierInvoiceDetails = new SupplierInvoiceDetails();
                         supplierInvoiceDetails.PartnerType = partnerType;
                        foreach (var invoiceBillingRecord in invoiceBillingRecordList)
                        {

                            supplierInvoiceDetails.Duration += invoiceBillingRecord.InvoiceMeasures.CostDuration;
                            supplierInvoiceDetails.CostAmount += invoiceBillingRecord.InvoiceMeasures.CostNet;
                            supplierInvoiceDetails.OriginalCostAmount += invoiceBillingRecord.InvoiceMeasures.CostNet_OrigCurr;
                            supplierInvoiceDetails.TotalNumberOfCalls += invoiceBillingRecord.InvoiceMeasures.NumberOfCalls;
                            supplierInvoiceDetails.OriginalSupplierCurrencyId = invoiceBillingRecord.OriginalSupplierCurrencyId;
                            supplierInvoiceDetails.SupplierCurrencyId = invoiceBillingRecord.SupplierCurrencyId;
                        }
                    };
                }
            }
            supplierInvoiceDetails.OriginalSupplierCurrency = currencyManager.GetCurrencySymbol(supplierInvoiceDetails.OriginalSupplierCurrencyId);
            supplierInvoiceDetails.SupplierCurrency = currencyManager.GetCurrencySymbol(supplierInvoiceDetails.SupplierCurrencyId);
            return supplierInvoiceDetails;
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
                        SupplierInvoiceItemDetails supplierInvoiceItemDetails = new Entities.SupplierInvoiceItemDetails()
                        {
                            Duration = item.InvoiceMeasures.CostDuration,
                            NumberOfCalls = item.InvoiceMeasures.NumberOfCalls,
                            OriginalSupplierCurrencyId = item.OriginalSupplierCurrencyId,
                            OriginalCostAmount = item.InvoiceMeasures.CostNet_OrigCurr,
                            CostAmount = item.InvoiceMeasures.CostNet,
                            SupplierZoneId = item.SupplierZoneId,
                            SupplierId = item.SupplierId,
                            SupplierCurrencyId = item.SupplierCurrencyId,
                            SupplierRate = item.SupplierRate,
                            SupplierRateTypeId = item.SupplierRateTypeId,
                            FromDate = item.InvoiceMeasures.BillingPeriodFrom,
                            ToDate = item.InvoiceMeasures.BillingPeriodTo,
                        };
                        generatedInvoiceItemSet.Items.Add(new GeneratedInvoiceItem
                        {
                            Details = supplierInvoiceItemDetails,
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
                    DimensionValue supplierId = analyticRecord.DimensionValues.ElementAtOrDefault(0);
                    DimensionValue saleZoneId = analyticRecord.DimensionValues.ElementAtOrDefault(1);
                    DimensionValue saleCurrencyId = analyticRecord.DimensionValues.ElementAtOrDefault(2);
                    DimensionValue saleRate = analyticRecord.DimensionValues.ElementAtOrDefault(3);
                    DimensionValue saleRateTypeId = analyticRecord.DimensionValues.ElementAtOrDefault(4);

                    MeasureValue saleNet_OrigCurr = GetMeasureValue(analyticRecord, "CostNet_OrigCurr");
                    MeasureValue saleDuration = GetMeasureValue(analyticRecord, "CostDuration");
                    MeasureValue saleNet = GetMeasureValue(analyticRecord, "CostNet");
                    MeasureValue calls = GetMeasureValue(analyticRecord, "NumberOfCalls");
                    MeasureValue billingPeriodTo = GetMeasureValue(analyticRecord, "BillingPeriodTo");
                    MeasureValue billingPeriodFrom = GetMeasureValue(analyticRecord, "BillingPeriodFrom");
                    #endregion
                    InvoiceBillingRecord invoiceBillingRecord = new InvoiceBillingRecord
                    {
                        SupplierId = Convert.ToInt32(supplierId.Value),
                        SupplierCurrencyId = currencyId,
                        OriginalSupplierCurrencyId = Convert.ToInt32(saleCurrencyId.Value),
                        SupplierRate = saleRate != null ? Convert.ToDecimal(saleRate.Value) : default(Decimal),
                        SupplierRateTypeId = saleRateTypeId != null && saleRateTypeId.Value != null ? Convert.ToInt32(saleRateTypeId.Value) : default(int?),
                        SupplierZoneId = Convert.ToInt64(saleZoneId.Value),
                        InvoiceMeasures = new InvoiceMeasures
                        {
                            BillingPeriodFrom = billingPeriodFrom != null ? Convert.ToDateTime(billingPeriodFrom.Value) : default(DateTime),
                            BillingPeriodTo = billingPeriodTo != null ? Convert.ToDateTime(billingPeriodTo.Value) : default(DateTime),
                            CostDuration = Convert.ToDecimal(saleDuration.Value ?? 0.0),
                            CostNet = Convert.ToDecimal(saleNet == null ? 0.0 : saleNet.Value ?? 0.0),
                            NumberOfCalls = Convert.ToInt32(calls.Value ?? 0.0),
                            CostNet_OrigCurr = Convert.ToDecimal(saleNet_OrigCurr == null ? 0.0 : saleNet_OrigCurr.Value ?? 0.0),
                        }

                    };
                    AddItemToDictionary(itemSetNamesDic, "GroupedByCostZone", invoiceBillingRecord);
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
        //            SupplierRateTypeId = newInvoiceBillingRecord.SupplierRateTypeId,
        //            SupplierRate = newInvoiceBillingRecord.SupplierRate,
        //            SupplierZoneId = newInvoiceBillingRecord.SupplierZoneId,
        //            SupplierCurrencyId = newInvoiceBillingRecord.SupplierCurrencyId,
        //            OriginalSupplierCurrencyId = newInvoiceBillingRecord.OriginalSupplierCurrencyId,
        //            SupplierId = newInvoiceBillingRecord.SupplierId,
        //            InvoiceMeasures = new InvoiceMeasures
        //            {
        //                CostNet = newInvoiceBillingRecord.InvoiceMeasures.CostNet,
        //                CostDuration = newInvoiceBillingRecord.InvoiceMeasures.CostDuration,
        //                BillingPeriodFrom = newInvoiceBillingRecord.InvoiceMeasures.BillingPeriodFrom,
        //                BillingPeriodTo = newInvoiceBillingRecord.InvoiceMeasures.BillingPeriodTo,
        //                NumberOfCalls = newInvoiceBillingRecord.InvoiceMeasures.NumberOfCalls,
        //                CostNet_OrigCurr = newInvoiceBillingRecord.InvoiceMeasures.CostNet_OrigCurr,
        //            }
        //        };
        //        groupedDic.Add(key, invoiceBillingRecord);
        //    }
        //    else
        //    {
        //        invoiceBillingRecord.InvoiceMeasures.CostDuration += newInvoiceBillingRecord.InvoiceMeasures.CostDuration;
        //        invoiceBillingRecord.InvoiceMeasures.CostNet += newInvoiceBillingRecord.InvoiceMeasures.CostNet;
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
            public decimal CostNet { get; set; }
            public decimal CostNet_OrigCurr { get; set; }
            public int NumberOfCalls { get; set; }
            public Decimal CostDuration { get; set; }
            public DateTime BillingPeriodTo { get; set; }
            public DateTime BillingPeriodFrom { get; set; }


        } 
        public  class InvoiceBillingRecord
        {
            public InvoiceMeasures InvoiceMeasures { get; set; }
            public long SupplierZoneId { get; set; }
            public int SupplierId { get; set; }
            public int OriginalSupplierCurrencyId { get; set; }
            public Decimal SupplierRate { get; set; }
            public int? SupplierRateTypeId { get; set; }
            public int SupplierCurrencyId { get; set; }

        }
    }
}
