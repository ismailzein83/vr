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
            AnalyticManager analyticManager = new AnalyticManager();
            List<string> listDimensions = new List<string> { "SaleZone" };
            List<string> listMeasures = new List<string> { "SaleNet", "NumberOfCalls", "SaleDuration" };

            Vanrise.Entities.DataRetrievalInput<AnalyticQuery> analyticQuery = new DataRetrievalInput<AnalyticQuery>()
            {
                Query = new AnalyticQuery()
                {
                    DimensionFields = listDimensions,
                    MeasureFields = listMeasures,
                    TableId = 8,
                    FromTime = context.FromDate,
                    ToTime = context.ToDate,
                    ParentDimensions = new List<string>(),
                    Filters = new List<DimensionFilter>(),
                },
                SortByColumnName = "DimensionValues[0].Name"
            };

            DimensionFilter dimensionFilter = new DimensionFilter()
            {
                Dimension = "Customer",
                FilterValues = new List<object> {context.PartnerId}
            };
            analyticQuery.Query.Filters.Add(dimensionFilter);

            var result = analyticManager.GetFilteredRecords(analyticQuery) as Vanrise.Analytic.Entities.AnalyticSummaryBigResult<AnalyticRecord>;
            if (result != null && result.Data != null)
            {
                List<GeneratedInvoiceItemSet> generatedInvoiceItemSets = new List<GeneratedInvoiceItemSet>();
                GeneratedInvoiceItemSet generatedInvoiceItemSet = new GeneratedInvoiceItemSet();
                generatedInvoiceItemSet.SetName = "Customer";
                generatedInvoiceItemSet.Items = new List<GeneratedInvoiceItem>();
                CustomerInvoiceDetails customerInvoiceDetails = new CustomerInvoiceDetails();
                foreach (var analyticRecord in result.Data)
                {

                    MeasureValue saleDuration;
                    analyticRecord.MeasureValues.TryGetValue("SaleDuration", out saleDuration);
                    customerInvoiceDetails.Duration += Convert.ToDecimal(saleDuration.Value ?? 0.0);

                    MeasureValue saleNet;
                    analyticRecord.MeasureValues.TryGetValue("SaleNet", out saleNet);
                    customerInvoiceDetails.SaleAmount += Convert.ToDouble(saleNet == null ? 0.0 : saleNet.Value ?? 0.0);

                    MeasureValue calls;
                    analyticRecord.MeasureValues.TryGetValue("NumberOfCalls", out calls);
                    customerInvoiceDetails.TotalNumberOfCalls = Convert.ToInt32(calls.Value ?? 0.0);
                    CustomerInvoiceItemDetails customerInvoiceItemDetails = new Entities.CustomerInvoiceItemDetails()
                    {
                        Duration = Convert.ToDecimal(saleDuration.Value ?? 0.0),
                        NumberOfCalls = Convert.ToInt32(calls.Value ?? 0.0),
                        SaleAmount = Convert.ToDouble(saleNet == null ? 0.0 : saleNet.Value ?? 0.0)
                    };
                    generatedInvoiceItemSet.Items.Add(new GeneratedInvoiceItem {
                        Details = customerInvoiceItemDetails,
                        Name = "Customer"
                    });
                }
                generatedInvoiceItemSets.Add(generatedInvoiceItemSet);
                context.Invoice = new GeneratedInvoice
                {
                    InvoiceDetails = customerInvoiceDetails,
                    InvoiceItemSets = generatedInvoiceItemSets
                };
            }
        }
    }
}
