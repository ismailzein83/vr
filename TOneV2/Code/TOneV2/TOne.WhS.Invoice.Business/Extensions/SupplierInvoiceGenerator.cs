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
    //public class SupplierInvoiceGenerator : InvoiceGenerator
    //{
    //    public override Guid ConfigId { get { return  new Guid("432AF409-47C6-4CA2-9D1A-6D9D947F29F4"); } }
    //    public override void GenerateInvoice(IInvoiceGenerationContext context)
    //    {
    //        List<GeneratedInvoiceItemSet> generatedInvoiceItemSets = new List<GeneratedInvoiceItemSet>();

    //        List<string> listMeasures = new List<string> { "CostNet", "NumberOfCalls", "CostDuration" };


    //        string[] partner = context.PartnerId.Split('_');
    //        string dimentionName = null;
    //        if (partner[0].Equals("Profile"))
    //        {
    //            dimentionName = "SupplierProfile";

    //            List<string> listProfileDimensions = new List<string> { "Supplier" };
    //            var analyticResultForSupplier = GetFilteredRecords(listProfileDimensions, listMeasures, dimentionName, partner[1], context.FromDate, context.ToDate);
    //            BuilInvoiceSupplierItemSet(analyticResultForSupplier.Data, generatedInvoiceItemSets, "Supplier");
    //            foreach (var supplier in analyticResultForSupplier.Data)
    //            {
    //                DimensionValue supplierDimension = supplier.DimensionValues[0];

    //                List<string> listSupplierDimensions = new List<string> { "SupplierZone" };
    //                var analyticResultBySupplierZone = GetFilteredRecords(listSupplierDimensions, listMeasures, "Supplier", supplierDimension.Value, context.FromDate, context.ToDate);
    //                BuilInvoiceSupplierItemSet(analyticResultBySupplierZone.Data, generatedInvoiceItemSets, string.Format("Supplier_{0}", supplierDimension.Name.ToString()));
    //            }


    //        }
    //        else if (partner[0].Equals("Account"))
    //        {
    //            dimentionName = "Supplier";

    //            List<string> listDimensions = new List<string> { "SupplierZone" };
    //            var analyticResultBySupplierZone = GetFilteredRecords(listDimensions, listMeasures, dimentionName, partner[1], context.FromDate, context.ToDate);
    //            BuilInvoiceSupplierItemSet(analyticResultBySupplierZone.Data, generatedInvoiceItemSets, "Supplier");

    //        }

    //        #region BuildSupplierInvoiceDetails
    //        List<string> listDimensionsForSupplierInvoice = new List<string> { dimentionName };
    //        var analyticResultForSupplierInvoice = GetFilteredRecords(listDimensionsForSupplierInvoice, listMeasures, dimentionName, partner[1], context.FromDate, context.ToDate);
    //        SupplierInvoiceDetails supplierInvoiceDetails = BuilSupplierInvoiceDetails(analyticResultForSupplierInvoice.Data, partner[0]);
    //        #endregion

    //        context.Invoice = new GeneratedInvoice
    //        {
    //            InvoiceDetails = supplierInvoiceDetails,
    //            InvoiceItemSets = generatedInvoiceItemSets
    //        };
    //    }
    //    private SupplierInvoiceDetails BuilSupplierInvoiceDetails(IEnumerable<AnalyticRecord> analyticRecords, string partnerType)
    //    {
    //        SupplierInvoiceDetails supplierInvoiceDetails = null;
    //        if (partnerType != null)
    //        {
    //            supplierInvoiceDetails = new SupplierInvoiceDetails();
    //            supplierInvoiceDetails.PartnerType = partnerType;

    //            foreach (var analyticRecord in analyticRecords)
    //            {
    //                MeasureValue costDuration;
    //                analyticRecord.MeasureValues.TryGetValue("CostDuration", out costDuration);
    //                supplierInvoiceDetails.Duration += Convert.ToDecimal(costDuration.Value ?? 0.0);

    //                MeasureValue costNet;
    //                analyticRecord.MeasureValues.TryGetValue("CostNet", out costNet);
    //                supplierInvoiceDetails.SupplierAmount += Convert.ToDouble(costNet == null ? 0.0 : costNet.Value ?? 0.0);

    //                MeasureValue calls;
    //                analyticRecord.MeasureValues.TryGetValue("NumberOfCalls", out calls);
    //                supplierInvoiceDetails.TotalNumberOfCalls += Convert.ToInt32(calls.Value ?? 0.0);

    //            }
    //        }
    //        return supplierInvoiceDetails;
    //    }
    //    private void BuilInvoiceSupplierItemSet(IEnumerable<AnalyticRecord> analyticRecords, List<GeneratedInvoiceItemSet> generatedInvoiceItemSets, string itemSetName)
    //    {
    //        GeneratedInvoiceItemSet generatedInvoiceItemSet = new GeneratedInvoiceItemSet();
    //        generatedInvoiceItemSet.SetName = itemSetName;
    //        generatedInvoiceItemSet.Items = new List<GeneratedInvoiceItem>();
    //        foreach (var analyticRecord in analyticRecords)
    //        {

    //            DimensionValue dimensionName = analyticRecord.DimensionValues[0];
    //            MeasureValue costDuration;
    //            analyticRecord.MeasureValues.TryGetValue("CostDuration", out costDuration);

    //            MeasureValue costNet;
    //            analyticRecord.MeasureValues.TryGetValue("CostrNet", out costNet);

    //            MeasureValue calls;
    //            analyticRecord.MeasureValues.TryGetValue("NumberOfCalls", out calls);

    //            SupplierInvoiceItemDetails supplierInvoiceItemDetails = new Entities.SupplierInvoiceItemDetails()
    //            {
    //                Duration = Convert.ToDecimal(costDuration.Value ?? 0.0),
    //                NumberOfCalls = Convert.ToInt32(calls.Value ?? 0.0),
    //                SupplierAmount = Convert.ToDouble(costNet == null ? 0.0 : costNet.Value ?? 0.0),
    //                DimensionName = dimensionName.Name.ToString(),
    //            };
    //            generatedInvoiceItemSet.Items.Add(new GeneratedInvoiceItem
    //            {
    //                Details = supplierInvoiceItemDetails,
    //                Name = "SupplierZone"
    //            });
    //        }
    //        generatedInvoiceItemSets.Add(generatedInvoiceItemSet);
    //    }
    //    private AnalyticSummaryBigResult<AnalyticRecord> GetFilteredRecords(List<string> listDimensions, List<string> listMeasures, string dimentionFilterName, object dimentionFilterValue, DateTime fromDate, DateTime toDate)
    //    {
    //        AnalyticManager analyticManager = new AnalyticManager();
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
    //        DimensionFilter dimensionFilter = new DimensionFilter()
    //        {
    //            Dimension = dimentionFilterName,
    //            FilterValues = new List<object> { dimentionFilterValue }
    //        };
    //        analyticQuery.Query.Filters.Add(dimensionFilter);
    //        return analyticManager.GetFilteredRecords(analyticQuery) as Vanrise.Analytic.Entities.AnalyticSummaryBigResult<AnalyticRecord>;
    //    }

    //}
}
