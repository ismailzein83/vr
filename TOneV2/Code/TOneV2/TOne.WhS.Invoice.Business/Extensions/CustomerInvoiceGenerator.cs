using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Invoice.Entities;
using Vanrise.Analytic.Business;
using Vanrise.Analytic.Entities;
using Vanrise.Entities;
using Vanrise.Invoice.Entities;

namespace TOne.WhS.Invoice.Business.Extensions
{
    public class CustomerInvoiceGenerator : InvoiceGenerator
    {
        public override void GenerateInvoice(IInvoiceGenerationContext context)
        {
            List<GeneratedInvoiceItemSet> generatedInvoiceItemSets = new List<GeneratedInvoiceItemSet>();
            CustomerInvoiceDetails customerInvoiceDetails = null;
            var analyticResultBySaleZone = GetFilteredRecordsBySaleZone(context.PartnerId, context.FromDate, context.ToDate);
            if (analyticResultBySaleZone != null && analyticResultBySaleZone.Data != null)
            {
                BuilInvoiceCustomerItemSet(analyticResultBySaleZone.Data, generatedInvoiceItemSets, out customerInvoiceDetails);
            }
            var analyticResultBySupplierZone = GetFilteredRecordsBySupplierZone(context.PartnerId, context.FromDate, context.ToDate);
            if (analyticResultBySupplierZone != null && analyticResultBySupplierZone.Data != null)
            {
                BuilInvoiceSupplierZoneItemSet(analyticResultBySupplierZone.Data, generatedInvoiceItemSets);
            }
            var analyticResultBySupplier = GetFilteredRecordsBySupplier(context.PartnerId, context.FromDate, context.ToDate);
            if (analyticResultBySupplier != null && analyticResultBySupplier.Data != null)
            {
                BuilInvoiceSupplierItemSet(analyticResultBySupplier.Data, generatedInvoiceItemSets);
            }
            context.Invoice = new GeneratedInvoice
            {
                InvoiceDetails = customerInvoiceDetails,
                InvoiceItemSets = generatedInvoiceItemSets
            };
        }
        private void BuilInvoiceCustomerItemSet(IEnumerable<AnalyticRecord> analyticRecords,List<GeneratedInvoiceItemSet> generatedInvoiceItemSets, out CustomerInvoiceDetails customerInvoiceDetails)
        {
              GeneratedInvoiceItemSet generatedInvoiceItemSet = new GeneratedInvoiceItemSet();
                generatedInvoiceItemSet.SetName = "SaleZone";
                generatedInvoiceItemSet.Items = new List<GeneratedInvoiceItem>();
                customerInvoiceDetails = new CustomerInvoiceDetails();
                foreach (var analyticRecord in analyticRecords)
                {

                    DimensionValue saleZone = analyticRecord.DimensionValues[0];
                    DimensionValue saleRate = analyticRecord.DimensionValues[1];
                    DimensionValue saleCurrency = analyticRecord.DimensionValues[2];
                    MeasureValue saleDuration;
                    analyticRecord.MeasureValues.TryGetValue("SaleDuration", out saleDuration);
                    customerInvoiceDetails.Duration += Convert.ToDecimal(saleDuration.Value ?? 0.0);

                    MeasureValue saleNet;
                    analyticRecord.MeasureValues.TryGetValue("SaleNet", out saleNet);
                    customerInvoiceDetails.SaleAmount += Convert.ToDouble(saleNet == null ? 0.0 : saleNet.Value ?? 0.0);

                    MeasureValue calls;
                    analyticRecord.MeasureValues.TryGetValue("NumberOfCalls", out calls);
                    customerInvoiceDetails.TotalNumberOfCalls += Convert.ToInt32(calls.Value ?? 0.0);



                    CustomerInvoiceItemDetails customerInvoiceItemDetails = new Entities.CustomerInvoiceItemDetails()
                    {
                        Duration = Convert.ToDecimal(saleDuration.Value ?? 0.0),
                        NumberOfCalls = Convert.ToInt32(calls.Value ?? 0.0),
                        SaleAmount = Convert.ToDouble(saleNet == null ? 0.0 : saleNet.Value ?? 0.0),
                        DimensionName = saleZone.Name.ToString(),
                        SaleRate = Convert.ToDecimal(saleRate.Value),
                        SaleCurrency = saleCurrency.Name.ToString(),
                    };
                    generatedInvoiceItemSet.Items.Add(new GeneratedInvoiceItem {
                        Details = customerInvoiceItemDetails,
                        Name = "SaleZone"
                    });
                }
                generatedInvoiceItemSets.Add(generatedInvoiceItemSet);
        }
        private AnalyticSummaryBigResult<AnalyticRecord> GetFilteredRecordsBySaleZone(string partnerId, DateTime fromDate, DateTime toDate)
        {
            AnalyticManager analyticManager = new AnalyticManager();
            List<string> listDimensions = new List<string> { "SaleZone", "SaleRate", "SaleCurrency" };
            List<string> listMeasures = new List<string> { "SaleNet", "NumberOfCalls", "SaleDuration"};
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
            string[] partner = partnerId.Split('_');
            string dimentionName = null;
            if(partner[0].Equals("Profile"))
            {
                dimentionName = "CustomerProfile";
            }
            else if (partner[0].Equals("Account"))
            {
                dimentionName = "Customer";
            }
            DimensionFilter dimensionFilter = new DimensionFilter()
            {
                Dimension = dimentionName,
                FilterValues = new List<object> { partner[1]}
            };
            analyticQuery.Query.Filters.Add(dimensionFilter);
            return analyticManager.GetFilteredRecords(analyticQuery) as Vanrise.Analytic.Entities.AnalyticSummaryBigResult<AnalyticRecord>;
        }
        private void BuilInvoiceSupplierZoneItemSet(IEnumerable<AnalyticRecord> analyticRecords, List<GeneratedInvoiceItemSet> generatedInvoiceItemSets)
        {
            GeneratedInvoiceItemSet generatedInvoiceItemSet = new GeneratedInvoiceItemSet();
            generatedInvoiceItemSet.SetName = "SupplierZone";
            generatedInvoiceItemSet.Items = new List<GeneratedInvoiceItem>();
            foreach (var analyticRecord in analyticRecords)
            {
                DimensionValue supplierZone = analyticRecord.DimensionValues[0];

                MeasureValue saleDuration;
                analyticRecord.MeasureValues.TryGetValue("SaleDuration", out saleDuration);
                MeasureValue saleNet;
                analyticRecord.MeasureValues.TryGetValue("SaleNet", out saleNet);
                MeasureValue calls;
                analyticRecord.MeasureValues.TryGetValue("NumberOfCalls", out calls);
                CustomerInvoiceItemDetails customerInvoiceItemDetails = new Entities.CustomerInvoiceItemDetails()
                {
                    Duration = Convert.ToDecimal(saleDuration.Value ?? 0.0),
                    NumberOfCalls = Convert.ToInt32(calls.Value ?? 0.0),
                    SaleAmount = Convert.ToDouble(saleNet == null ? 0.0 : saleNet.Value ?? 0.0),
                    DimensionName = supplierZone.Name.ToString()
                };
                generatedInvoiceItemSet.Items.Add(new GeneratedInvoiceItem
                {
                    Details = customerInvoiceItemDetails,
                    Name = "SupplierZone"
                });
            }
            generatedInvoiceItemSets.Add(generatedInvoiceItemSet);
        }
        private AnalyticSummaryBigResult<AnalyticRecord> GetFilteredRecordsBySupplierZone(string partnerId, DateTime fromDate, DateTime toDate)
        {
            AnalyticManager analyticManager = new AnalyticManager();
            List<string> listDimensions = new List<string> { "SupplierZone" };
            List<string> listMeasures = new List<string> { "SaleNet", "NumberOfCalls", "SaleDuration" };
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
            string[] partner = partnerId.Split('_');
            string dimentionName = null;
            if (partner[0].Equals("Profile"))
            {
                dimentionName = "CustomerProfile";
            }
            else if (partner[0].Equals("Account"))
            {
                dimentionName = "Customer";
            }
            DimensionFilter dimensionFilter = new DimensionFilter()
            {
                Dimension = dimentionName,
                FilterValues = new List<object> { partner[1] }
            };
            analyticQuery.Query.Filters.Add(dimensionFilter);
            return analyticManager.GetFilteredRecords(analyticQuery) as Vanrise.Analytic.Entities.AnalyticSummaryBigResult<AnalyticRecord>;
        }
        private void BuilInvoiceSupplierItemSet(IEnumerable<AnalyticRecord> analyticRecords, List<GeneratedInvoiceItemSet> generatedInvoiceItemSets)
        {
            GeneratedInvoiceItemSet generatedInvoiceItemSet = new GeneratedInvoiceItemSet();
            generatedInvoiceItemSet.SetName = "Supplier";
            generatedInvoiceItemSet.Items = new List<GeneratedInvoiceItem>();
            foreach (var analyticRecord in analyticRecords)
            {
                DimensionValue supplier = analyticRecord.DimensionValues[0];

                MeasureValue saleDuration;
                analyticRecord.MeasureValues.TryGetValue("SaleDuration", out saleDuration);
                MeasureValue saleNet;
                analyticRecord.MeasureValues.TryGetValue("SaleNet", out saleNet);
                MeasureValue calls;
                analyticRecord.MeasureValues.TryGetValue("NumberOfCalls", out calls);
                CustomerInvoiceItemDetails customerInvoiceItemDetails = new Entities.CustomerInvoiceItemDetails()
                {
                    Duration = Convert.ToDecimal(saleDuration.Value ?? 0.0),
                    NumberOfCalls = Convert.ToInt32(calls.Value ?? 0.0),
                    SaleAmount = Convert.ToDouble(saleNet == null ? 0.0 : saleNet.Value ?? 0.0),
                    DimensionName = supplier.Name.ToString()

                };
                generatedInvoiceItemSet.Items.Add(new GeneratedInvoiceItem
                {
                    Details = customerInvoiceItemDetails,
                    Name = "Supplier"
                });
            }
            generatedInvoiceItemSets.Add(generatedInvoiceItemSet);
        }
        private AnalyticSummaryBigResult<AnalyticRecord> GetFilteredRecordsBySupplier(string partnerId, DateTime fromDate, DateTime toDate)
        {
            AnalyticManager analyticManager = new AnalyticManager();
            List<string> listDimensions = new List<string> { "Supplier" };
            List<string> listMeasures = new List<string> { "SaleNet", "NumberOfCalls", "SaleDuration" };
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
            string[] partner = partnerId.Split('_');
            string dimentionName = null;
            if (partner[0].Equals("Profile"))
            {
                dimentionName = "CustomerProfile";
            }
            else if (partner[0].Equals("Account"))
            {
                dimentionName = "Customer";
            }
            DimensionFilter dimensionFilter = new DimensionFilter()
            {
                Dimension = dimentionName,
                FilterValues = new List<object> { partner[1] }
            };
            analyticQuery.Query.Filters.Add(dimensionFilter);
            return analyticManager.GetFilteredRecords(analyticQuery) as Vanrise.Analytic.Entities.AnalyticSummaryBigResult<AnalyticRecord>;
        }

    }
}
