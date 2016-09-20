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
        public override Guid ConfigId { get { return  new Guid("BD4F7B2C-1C07-4037-8730-92768BD28900"); } }
        public override void GenerateInvoice(IInvoiceGenerationContext context)
        {
            List<GeneratedInvoiceItemSet> generatedInvoiceItemSets = new List<GeneratedInvoiceItemSet>();

            List<string> listMeasures = new List<string> { "SaleNet", "NumberOfCalls", "SaleDuration" };


            string[] partner = context.PartnerId.Split('_');
            string dimentionName = null;
            if (partner[0].Equals("Profile"))
            {
                dimentionName = "CustomerProfile";

                List<string> listProfileDimensions = new List<string> { "Customer" };
                var analyticResultForCustomer = GetFilteredRecords(listProfileDimensions, listMeasures, dimentionName, partner[1], context.FromDate, context.ToDate);
                BuilInvoiceCustomerItemSet(analyticResultForCustomer.Data, generatedInvoiceItemSets, "Customer");
                foreach(var customer in analyticResultForCustomer.Data)
                {
                    DimensionValue customerDimension = customer.DimensionValues[0];
                    
                    List<string> listCustomerDimensions = new List<string> { "SaleZone" };
                    var analyticResultBySaleZone = GetFilteredRecords(listCustomerDimensions, listMeasures, "Customer", customerDimension.Value, context.FromDate, context.ToDate);
                    BuilInvoiceCustomerItemSet(analyticResultBySaleZone.Data, generatedInvoiceItemSets, string.Format("Customer_{0}", customerDimension.Name.ToString()));
                }


            }
            else if (partner[0].Equals("Account"))
            {
                dimentionName = "Customer";

                List<string> listDimensions = new List<string> { "SaleZone" };
                var analyticResultBySaleZone = GetFilteredRecords(listDimensions, listMeasures, dimentionName, partner[1], context.FromDate, context.ToDate);
                BuilInvoiceCustomerItemSet(analyticResultBySaleZone.Data, generatedInvoiceItemSets, "Customer");

            }

            #region BuildCustomerInvoiceDetails
            List<string> listDimensionsForCustomerInvoice = new List<string> { dimentionName };
            var analyticResultForCustomerInvoice = GetFilteredRecords(listDimensionsForCustomerInvoice, listMeasures, dimentionName, partner[1], context.FromDate, context.ToDate);
            CustomerInvoiceDetails customerInvoiceDetails = BuilCustomerInvoiceDetails(analyticResultForCustomerInvoice.Data, partner[0]);
            #endregion


            //var analyticResultBySaleZone = GetFilteredRecordsBySaleZone(context.PartnerId, context.FromDate, context.ToDate);
            //if (analyticResultBySaleZone != null && analyticResultBySaleZone.Data != null)
            //{
            //    BuilInvoiceCustomerItemSet(partner[0],analyticResultBySaleZone.Data, generatedInvoiceItemSets, out customerInvoiceDetails);
            //}

            //var analyticResultBySaleCurrency = GetFilteredRecordsBySaleCurrency(context.PartnerId, context.FromDate, context.ToDate);
            //if (analyticResultBySaleCurrency != null && analyticResultBySaleCurrency.Data != null)
            //{
            //    BuilInvoiceSaleCurrencyItemSet(partner[0], analyticResultBySaleCurrency.Data, generatedInvoiceItemSets);
            //}
            //var analyticResultBySupplierZone = GetFilteredRecordsBySupplierZone(context.PartnerId, context.FromDate, context.ToDate);
            //if (analyticResultBySupplierZone != null && analyticResultBySupplierZone.Data != null)
            //{
            //    BuilInvoiceSupplierZoneItemSet(analyticResultBySupplierZone.Data, generatedInvoiceItemSets);
            //}
            //var analyticResultBySupplier = GetFilteredRecordsBySupplier(context.PartnerId, context.FromDate, context.ToDate);
            //if (analyticResultBySupplier != null && analyticResultBySupplier.Data != null)
            //{
            //    BuilInvoiceSupplierItemSet(analyticResultBySupplier.Data, generatedInvoiceItemSets);
            //}
            context.Invoice = new GeneratedInvoice
            {
                InvoiceDetails = customerInvoiceDetails,
                InvoiceItemSets = generatedInvoiceItemSets
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
                    MeasureValue saleDuration;
                    analyticRecord.MeasureValues.TryGetValue("SaleDuration", out saleDuration);
                    customerInvoiceDetails.Duration += Convert.ToDecimal(saleDuration.Value ?? 0.0);

                    MeasureValue saleNet;
                    analyticRecord.MeasureValues.TryGetValue("SaleNet", out saleNet);
                    customerInvoiceDetails.SaleAmount += Convert.ToDouble(saleNet == null ? 0.0 : saleNet.Value ?? 0.0);

                    MeasureValue calls;
                    analyticRecord.MeasureValues.TryGetValue("NumberOfCalls", out calls);
                    customerInvoiceDetails.TotalNumberOfCalls += Convert.ToInt32(calls.Value ?? 0.0);

                }
            }
            return customerInvoiceDetails;
        }
        private void BuilInvoiceCustomerItemSet(IEnumerable<AnalyticRecord> analyticRecords, List<GeneratedInvoiceItemSet> generatedInvoiceItemSets, string itemSetName)
        {
            GeneratedInvoiceItemSet generatedInvoiceItemSet = new GeneratedInvoiceItemSet();
            generatedInvoiceItemSet.SetName = itemSetName;
            generatedInvoiceItemSet.Items = new List<GeneratedInvoiceItem>();
            foreach (var analyticRecord in analyticRecords)
            {

                DimensionValue dimensionName = analyticRecord.DimensionValues[0];
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
                    DimensionName = dimensionName.Name.ToString(),
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


    //    private void BuilInvoiceCustomerItemSet(string partnerType,IEnumerable<AnalyticRecord> analyticRecords,List<GeneratedInvoiceItemSet> generatedInvoiceItemSets, out CustomerInvoiceDetails customerInvoiceDetails)
    //    {
    //          GeneratedInvoiceItemSet generatedInvoiceItemSet = new GeneratedInvoiceItemSet();
    //            generatedInvoiceItemSet.SetName = "SaleZone";
    //            generatedInvoiceItemSet.Items = new List<GeneratedInvoiceItem>();
    //            customerInvoiceDetails = new CustomerInvoiceDetails();
    //            customerInvoiceDetails.PartnerType = partnerType;
    //            foreach (var analyticRecord in analyticRecords)
    //            {

    //                DimensionValue saleZone = analyticRecord.DimensionValues[0];
    //                DimensionValue saleRate = analyticRecord.DimensionValues[1];
    //                DimensionValue saleCurrency = analyticRecord.DimensionValues[2];
    //                MeasureValue saleDuration;
    //                analyticRecord.MeasureValues.TryGetValue("SaleDuration", out saleDuration);
    //                customerInvoiceDetails.Duration += Convert.ToDecimal(saleDuration.Value ?? 0.0);

    //                MeasureValue saleNet;
    //                analyticRecord.MeasureValues.TryGetValue("SaleNet", out saleNet);
    //                customerInvoiceDetails.SaleAmount += Convert.ToDouble(saleNet == null ? 0.0 : saleNet.Value ?? 0.0);

    //                MeasureValue calls;
    //                analyticRecord.MeasureValues.TryGetValue("NumberOfCalls", out calls);
    //                customerInvoiceDetails.TotalNumberOfCalls += Convert.ToInt32(calls.Value ?? 0.0);

                  

    //                CustomerInvoiceItemDetails customerInvoiceItemDetails = new Entities.CustomerInvoiceItemDetails()
    //                {
    //                    Duration = Convert.ToDecimal(saleDuration.Value ?? 0.0),
    //                    NumberOfCalls = Convert.ToInt32(calls.Value ?? 0.0),
    //                    SaleAmount = Convert.ToDouble(saleNet == null ? 0.0 : saleNet.Value ?? 0.0),
    //                    DimensionName = saleZone.Name.ToString(),
    //                    SaleRate = Convert.ToDecimal(saleRate.Value),
    //                    SaleCurrency = saleCurrency.Value.ToString(),
    //                };
    //                generatedInvoiceItemSet.Items.Add(new GeneratedInvoiceItem {
    //                    Details = customerInvoiceItemDetails,
    //                    Name = "SaleZone"
    //                });
    //            }
    //            generatedInvoiceItemSets.Add(generatedInvoiceItemSet);
    //    }
    //    private AnalyticSummaryBigResult<AnalyticRecord> GetFilteredRecordsBySaleZone(string partnerId, DateTime fromDate, DateTime toDate)
    //    {
    //        AnalyticManager analyticManager = new AnalyticManager();
    //        List<string> listDimensions = new List<string> { "SaleZone", "SaleRate", "SaleCurrency" };
    //        List<string> listMeasures = new List<string> { "SaleNet", "NumberOfCalls", "SaleDuration"};
    //        Vanrise.Entities.DataRetrievalInput<AnalyticQuery> analyticQuery = new DataRetrievalInput<AnalyticQuery>()
    //        {
    //            Query = new AnalyticQuery()
    //            {
    //                DimensionFields = listDimensions,
    //                MeasureFields = listMeasures,
    //                TableId = 8,
    //                FromTime = fromDate,
    //                ToTime = toDate,
    //                ParentDimensions = new List<string>(),
    //                Filters = new List<DimensionFilter>(),
    //            },
    //            SortByColumnName = "DimensionValues[0].Name"
    //        };
    //        string[] partner = partnerId.Split('_');
    //        string dimentionName = null;
    //        if(partner[0].Equals("Profile"))
    //        {
    //            dimentionName = "CustomerProfile";
    //        }
    //        else if (partner[0].Equals("Account"))
    //        {
    //            dimentionName = "Customer";
    //        }
    //        DimensionFilter dimensionFilter = new DimensionFilter()
    //        {
    //            Dimension = dimentionName,
    //            FilterValues = new List<object> { partner[1]}
    //        };
    //        analyticQuery.Query.Filters.Add(dimensionFilter);
    //        return analyticManager.GetFilteredRecords(analyticQuery) as Vanrise.Analytic.Entities.AnalyticSummaryBigResult<AnalyticRecord>;
    //    }

    //    private void BuilInvoiceSaleCurrencyItemSet(string partnerType, IEnumerable<AnalyticRecord> analyticRecords, List<GeneratedInvoiceItemSet> generatedInvoiceItemSets)
    //    {
    //        GeneratedInvoiceItemSet generatedInvoiceItemSet = new GeneratedInvoiceItemSet();
    //        generatedInvoiceItemSet.SetName = "SaleCurrency";
    //        generatedInvoiceItemSet.Items = new List<GeneratedInvoiceItem>();
    //        foreach (var analyticRecord in analyticRecords)
    //        {

    //            DimensionValue saleCurrency = analyticRecord.DimensionValues[0];
    //            MeasureValue saleDuration;
    //            analyticRecord.MeasureValues.TryGetValue("SaleDuration", out saleDuration);
    //            MeasureValue saleNet;
    //            analyticRecord.MeasureValues.TryGetValue("SaleNet", out saleNet);
    //            MeasureValue calls;
    //            analyticRecord.MeasureValues.TryGetValue("NumberOfCalls", out calls);



    //            CustomerInvoiceItemDetails customerInvoiceItemDetails = new Entities.CustomerInvoiceItemDetails()
    //            {
    //                Duration = Convert.ToDecimal(saleDuration.Value ?? 0.0),
    //                NumberOfCalls = Convert.ToInt32(calls.Value ?? 0.0),
    //                SaleAmount = Convert.ToDouble(saleNet == null ? 0.0 : saleNet.Value ?? 0.0),
    //                DimensionName = saleCurrency.Name.ToString(),
    //                SaleCurrency = saleCurrency.Value.ToString(),
    //            };
    //            generatedInvoiceItemSet.Items.Add(new GeneratedInvoiceItem
    //            {
    //                Details = customerInvoiceItemDetails,
    //                Name = "SaleCurrency"
    //            });
    //        }
    //        generatedInvoiceItemSets.Add(generatedInvoiceItemSet);
    //    }
    //    private AnalyticSummaryBigResult<AnalyticRecord> GetFilteredRecordsBySaleCurrency(string partnerId, DateTime fromDate, DateTime toDate)
    //    {
    //        AnalyticManager analyticManager = new AnalyticManager();
    //        List<string> listDimensions = new List<string> { "SaleCurrency" };
    //        List<string> listMeasures = new List<string> { "SaleNet", "NumberOfCalls", "SaleDuration" };
    //        Vanrise.Entities.DataRetrievalInput<AnalyticQuery> analyticQuery = new DataRetrievalInput<AnalyticQuery>()
    //        {
    //            Query = new AnalyticQuery()
    //            {
    //                DimensionFields = listDimensions,
    //                MeasureFields = listMeasures,
    //                TableId = 8,
    //                FromTime = fromDate,
    //                ToTime = toDate,
    //                ParentDimensions = new List<string>(),
    //                Filters = new List<DimensionFilter>(),
    //            },
    //            SortByColumnName = "DimensionValues[0].Name"
    //        };
    //        string[] partner = partnerId.Split('_');
    //        string dimentionName = null;
    //        if (partner[0].Equals("Profile"))
    //        {
    //            dimentionName = "CustomerProfile";
    //        }
    //        else if (partner[0].Equals("Account"))
    //        {
    //            dimentionName = "Customer";
    //        }
    //        DimensionFilter dimensionFilter = new DimensionFilter()
    //        {
    //            Dimension = dimentionName,
    //            FilterValues = new List<object> { partner[1] }
    //        };
    //        analyticQuery.Query.Filters.Add(dimensionFilter);
    //        return analyticManager.GetFilteredRecords(analyticQuery) as Vanrise.Analytic.Entities.AnalyticSummaryBigResult<AnalyticRecord>;
    //    }
    //    private void BuilInvoiceSupplierZoneItemSet(IEnumerable<AnalyticRecord> analyticRecords, List<GeneratedInvoiceItemSet> generatedInvoiceItemSets)
    //    {
    //        GeneratedInvoiceItemSet generatedInvoiceItemSet = new GeneratedInvoiceItemSet();
    //        generatedInvoiceItemSet.SetName = "SupplierZone";
    //        generatedInvoiceItemSet.Items = new List<GeneratedInvoiceItem>();
    //        foreach (var analyticRecord in analyticRecords)
    //        {
    //            DimensionValue supplierZone = analyticRecord.DimensionValues[0];

    //            MeasureValue saleDuration;
    //            analyticRecord.MeasureValues.TryGetValue("SaleDuration", out saleDuration);
    //            MeasureValue saleNet;
    //            analyticRecord.MeasureValues.TryGetValue("SaleNet", out saleNet);
    //            MeasureValue calls;
    //            analyticRecord.MeasureValues.TryGetValue("NumberOfCalls", out calls);
    //            CustomerInvoiceItemDetails customerInvoiceItemDetails = new Entities.CustomerInvoiceItemDetails()
    //            {
    //                Duration = Convert.ToDecimal(saleDuration.Value ?? 0.0),
    //                NumberOfCalls = Convert.ToInt32(calls.Value ?? 0.0),
    //                SaleAmount = Convert.ToDouble(saleNet == null ? 0.0 : saleNet.Value ?? 0.0),
    //                DimensionName = supplierZone.Name.ToString()
    //            };
    //            generatedInvoiceItemSet.Items.Add(new GeneratedInvoiceItem
    //            {
    //                Details = customerInvoiceItemDetails,
    //                Name = "SupplierZone"
    //            });
    //        }
    //        generatedInvoiceItemSets.Add(generatedInvoiceItemSet);
    //    }
    //    private AnalyticSummaryBigResult<AnalyticRecord> GetFilteredRecordsBySupplierZone(string partnerId, DateTime fromDate, DateTime toDate)
    //    {
    //        AnalyticManager analyticManager = new AnalyticManager();
    //        List<string> listDimensions = new List<string> { "SupplierZone" };
    //        List<string> listMeasures = new List<string> { "SaleNet", "NumberOfCalls", "SaleDuration" };
    //        Vanrise.Entities.DataRetrievalInput<AnalyticQuery> analyticQuery = new DataRetrievalInput<AnalyticQuery>()
    //        {
    //            Query = new AnalyticQuery()
    //            {
    //                DimensionFields = listDimensions,
    //                MeasureFields = listMeasures,
    //                TableId = 8,
    //                FromTime = fromDate,
    //                ToTime = toDate,
    //                ParentDimensions = new List<string>(),
    //                Filters = new List<DimensionFilter>(),
    //            },
    //            SortByColumnName = "DimensionValues[0].Name"
    //        };
    //        string[] partner = partnerId.Split('_');
    //        string dimentionName = null;
    //        if (partner[0].Equals("Profile"))
    //        {
    //            dimentionName = "CustomerProfile";
    //        }
    //        else if (partner[0].Equals("Account"))
    //        {
    //            dimentionName = "Customer";
    //        }
    //        DimensionFilter dimensionFilter = new DimensionFilter()
    //        {
    //            Dimension = dimentionName,
    //            FilterValues = new List<object> { partner[1] }
    //        };
    //        analyticQuery.Query.Filters.Add(dimensionFilter);
    //        return analyticManager.GetFilteredRecords(analyticQuery) as Vanrise.Analytic.Entities.AnalyticSummaryBigResult<AnalyticRecord>;
    //    }
    //    private void BuilInvoiceSupplierItemSet(IEnumerable<AnalyticRecord> analyticRecords, List<GeneratedInvoiceItemSet> generatedInvoiceItemSets)
    //    {
    //        GeneratedInvoiceItemSet generatedInvoiceItemSet = new GeneratedInvoiceItemSet();
    //        generatedInvoiceItemSet.SetName = "Supplier";
    //        generatedInvoiceItemSet.Items = new List<GeneratedInvoiceItem>();
    //        foreach (var analyticRecord in analyticRecords)
    //        {
    //            DimensionValue supplier = analyticRecord.DimensionValues[0];

    //            MeasureValue saleDuration;
    //            analyticRecord.MeasureValues.TryGetValue("SaleDuration", out saleDuration);
    //            MeasureValue saleNet;
    //            analyticRecord.MeasureValues.TryGetValue("SaleNet", out saleNet);
    //            MeasureValue calls;
    //            analyticRecord.MeasureValues.TryGetValue("NumberOfCalls", out calls);
    //            CustomerInvoiceItemDetails customerInvoiceItemDetails = new Entities.CustomerInvoiceItemDetails()
    //            {
    //                Duration = Convert.ToDecimal(saleDuration.Value ?? 0.0),
    //                NumberOfCalls = Convert.ToInt32(calls.Value ?? 0.0),
    //                SaleAmount = Convert.ToDouble(saleNet == null ? 0.0 : saleNet.Value ?? 0.0),
    //                DimensionName = supplier.Name.ToString()

    //            };
    //            generatedInvoiceItemSet.Items.Add(new GeneratedInvoiceItem
    //            {
    //                Details = customerInvoiceItemDetails,
    //                Name = "Supplier"
    //            });
    //        }
    //        generatedInvoiceItemSets.Add(generatedInvoiceItemSet);
    //    }
    //    private AnalyticSummaryBigResult<AnalyticRecord> GetFilteredRecordsBySupplier(string partnerId, DateTime fromDate, DateTime toDate)
    //    {
    //        AnalyticManager analyticManager = new AnalyticManager();
    //        List<string> listDimensions = new List<string> { "Supplier" };
    //        List<string> listMeasures = new List<string> { "SaleNet", "NumberOfCalls", "SaleDuration" };
    //        Vanrise.Entities.DataRetrievalInput<AnalyticQuery> analyticQuery = new DataRetrievalInput<AnalyticQuery>()
    //        {
    //            Query = new AnalyticQuery()
    //            {
    //                DimensionFields = listDimensions,
    //                MeasureFields = listMeasures,
    //                TableId = 8,
    //                FromTime = fromDate,
    //                ToTime = toDate,
    //                ParentDimensions = new List<string>(),
    //                Filters = new List<DimensionFilter>(),
    //            },
    //            SortByColumnName = "DimensionValues[0].Name"
    //        };
    //        string[] partner = partnerId.Split('_');
    //        string dimentionName = null;
    //        if (partner[0].Equals("Profile"))
    //        {
    //            dimentionName = "CustomerProfile";
    //        }
    //        else if (partner[0].Equals("Account"))
    //        {
    //            dimentionName = "Customer";
    //        }
    //        DimensionFilter dimensionFilter = new DimensionFilter()
    //        {
    //            Dimension = dimentionName,
    //            FilterValues = new List<object> { partner[1] }
    //        };
    //        analyticQuery.Query.Filters.Add(dimensionFilter);
    //        return analyticManager.GetFilteredRecords(analyticQuery) as Vanrise.Analytic.Entities.AnalyticSummaryBigResult<AnalyticRecord>;
    //    }
    }
}
