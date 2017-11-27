using System;
using System.Collections.Generic;
using System.Linq;
using TOne.WhS.Analytics.Entities.BillingReport;
using Vanrise.Analytic.Business;
using Vanrise.Analytic.Entities;
using Vanrise.Entities;

namespace TOne.WhS.Analytics.Business.BillingReports
{
    public class CostBySaleZoneSupplierReportGenerator : IReportGenerator
    {
        public Dictionary<string, System.Collections.IEnumerable> GenerateDataSources(ReportParameters parameters)
        {
            //

            AnalyticManager analyticManager = new AnalyticManager();

            Vanrise.Entities.DataRetrievalInput<AnalyticQuery> analyticQuery = new DataRetrievalInput<AnalyticQuery>()
            {
                Query = new AnalyticQuery()
                {
                    DimensionFields = new List<string> { "SaleZone", "Supplier" },
                    MeasureFields = new List<string>() { "CostRate_DurAvg", "SaleDuration" },
                    TableId = Guid.Parse("4C1AAA1B-675B-420F-8E60-26B0747CA79B"),
                    FromTime = parameters.FromTime,
                    ToTime = parameters.ToTime,
                    CurrencyId = parameters.CurrencyId,
                    ParentDimensions = new List<string>(),
                    Filters = new List<DimensionFilter>()
                },
                SortByColumnName = "DimensionValues[0].Name"
            };

            if (parameters.GroupByCustomer)
                analyticQuery.Query.DimensionFields.Add("Customer");

            if (!String.IsNullOrEmpty(parameters.CustomersId))
            {
                DimensionFilter dimensionFilter = new DimensionFilter()
                {
                    Dimension = "Customer",
                    FilterValues = parameters.CustomersId.Split(',').ToList().Cast<object>().ToList()
                };
                analyticQuery.Query.Filters.Add(dimensionFilter);
            }

            if (!String.IsNullOrEmpty(parameters.SuppliersId))
            {
                DimensionFilter dimensionFilter = new DimensionFilter()
                {
                    Dimension = "Supplier",
                    FilterValues = parameters.SuppliersId.Split(',').ToList().Cast<object>().ToList()
                };
                analyticQuery.Query.Filters.Add(dimensionFilter);
            }

            List<CostBySaleZoneSupplier> listCostBySaleZoneSupplier = new List<CostBySaleZoneSupplier>();

            var result = analyticManager.GetFilteredRecords(analyticQuery) as AnalyticSummaryBigResult<AnalyticRecord>;

            if (result != null)
                foreach (var analyticRecord in result.Data)
                {
                    CostBySaleZoneSupplier costBySaleZoneSupplier = new CostBySaleZoneSupplier();

                    var saleZoneValue = analyticRecord.DimensionValues[0];
                    if (saleZoneValue != null)
                        costBySaleZoneSupplier.salezoneIDFormatted = saleZoneValue.Name;

                    var supplierValue = analyticRecord.DimensionValues[1];
                    if (supplierValue != null)
                        costBySaleZoneSupplier.SupplierID = supplierValue.Name;

                    MeasureValue averageCost;
                    analyticRecord.MeasureValues.TryGetValue("CostRate_DurAvg", out averageCost);
                    costBySaleZoneSupplier.HighestRate = Convert.ToDouble(averageCost.Value ?? 0.0);
                    costBySaleZoneSupplier.HighestRateFormatted = costBySaleZoneSupplier.HighestRate == 0 ? "" : (costBySaleZoneSupplier.HighestRate.HasValue) ?
                        ReportHelpers.FormatLongNumberDigit(costBySaleZoneSupplier.HighestRate) : "0.00";

                    MeasureValue saleDuration;
                    analyticRecord.MeasureValues.TryGetValue("SaleDuration", out saleDuration);
                    costBySaleZoneSupplier.AvgDuration = Convert.ToDecimal(saleDuration.Value ?? 0.0);
                    costBySaleZoneSupplier.AvgDurationFormatted = ReportHelpers.FormatNormalNumberDigit(costBySaleZoneSupplier.AvgDuration);

                    listCostBySaleZoneSupplier.Add(costBySaleZoneSupplier);
                }
            Dictionary<string, System.Collections.IEnumerable> dataSources = new Dictionary<string, System.Collections.IEnumerable>();
            dataSources.Add("SaleZoneCostSummarySupplier", listCostBySaleZoneSupplier);
            return dataSources;
        }

        public Dictionary<string, RdlcParameter> GetRdlcReportParameters(ReportParameters parameters)
        {
            Dictionary<string, RdlcParameter> list = new Dictionary<string, RdlcParameter>();
            list.Add("FromDate", new RdlcParameter { Value = parameters.FromTime.ToString(), IsVisible = true });
            list.Add("ToDate", new RdlcParameter { Value = (parameters.ToTime.HasValue) ? parameters.ToTime.ToString() : null, IsVisible = true });
            list.Add("Title", new RdlcParameter { Value = "Cost by Sale Zone - Grouped by Supplier", IsVisible = true });
            list.Add("LogoPath", new RdlcParameter { Value = "logo", IsVisible = true });
            list.Add("DigitRate", new RdlcParameter { Value = ReportHelpers.GetNormalNumberDigit(), IsVisible = true });
            list.Add("Currency", new RdlcParameter { Value = parameters.CurrencyDescription, IsVisible = true });

            return list;
        }
    }
}
