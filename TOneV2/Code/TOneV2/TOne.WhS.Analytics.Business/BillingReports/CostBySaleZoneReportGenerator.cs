using System;
using System.Collections.Generic;
using System.Linq;
using TOne.WhS.Analytics.Entities.BillingReport;
using Vanrise.Analytic.Business;
using Vanrise.Analytic.Entities;
using Vanrise.Entities;


namespace TOne.WhS.Analytics.Business.BillingReports
{
    public class CostBySaleZoneReportGenerator : IReportGenerator
    {
        public Dictionary<string, System.Collections.IEnumerable> GenerateDataSources(ReportParameters parameters)
        {
            //

            AnalyticManager analyticManager = new AnalyticManager();

            Vanrise.Entities.DataRetrievalInput<AnalyticQuery> analyticQuery = new DataRetrievalInput<AnalyticQuery>()
            {
                Query = new AnalyticQuery()
                {
                    DimensionFields = new List<string> { "SaleZone" },
                    MeasureFields = new List<string>() { "AverageCost", "CostDuration" },
                    TableId = 8,
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

            List<CostBySaleZone> listCostBySaleZone = new List<CostBySaleZone>();

            var result = analyticManager.GetFilteredRecords(analyticQuery) as AnalyticSummaryBigResult<AnalyticRecord>;

            if (result != null)
                foreach (var analyticRecord in result.Data)
                {
                    CostBySaleZone costBySaleZone = new CostBySaleZone();

                    var saleZoneValue = analyticRecord.DimensionValues[0];
                    if (saleZoneValue != null)
                        costBySaleZone.salezoneIDFormatted = saleZoneValue.Name;

                    MeasureValue averageCost;
                    analyticRecord.MeasureValues.TryGetValue("AverageCost", out averageCost);
                    costBySaleZone.AvgCost = Convert.ToDouble(averageCost.Value ?? 0.0);
                    costBySaleZone.AvgCostFormatted = costBySaleZone.AvgCost == 0 ? "" : (costBySaleZone.AvgCost.HasValue) ?
                        ReportHelpers.FormatLongNumberDigit(costBySaleZone.AvgCost) : "0.00000";

                    MeasureValue costDuration;
                    analyticRecord.MeasureValues.TryGetValue("CostDuration", out costDuration);
                    costBySaleZone.AvgDuration = Convert.ToDecimal(costDuration.Value ?? 0.0);
                    costBySaleZone.AvgDurationFormatted = ReportHelpers.FormatNormalNumberDigit(costBySaleZone.AvgDuration);

                    listCostBySaleZone.Add(costBySaleZone);
                }
            Dictionary<string, System.Collections.IEnumerable> dataSources = new Dictionary<string, System.Collections.IEnumerable>();
            dataSources.Add("SaleZoneCostSummary", listCostBySaleZone);
            return dataSources;
        }

        public Dictionary<string, RdlcParameter> GetRdlcReportParameters(ReportParameters parameters)
        {
            Dictionary<string, RdlcParameter> list = new Dictionary<string, RdlcParameter>();
            list.Add("FromDate", new RdlcParameter { Value = parameters.FromTime.ToString(), IsVisible = true });
            list.Add("ToDate", new RdlcParameter { Value = parameters.ToTime.HasValue ?parameters.ToTime.ToString():null, IsVisible = true });
            list.Add("Title", new RdlcParameter { Value = "Cost By Sale Zone", IsVisible = true });
            list.Add("LogoPath", new RdlcParameter { Value = "logo", IsVisible = true });
            list.Add("DigitRate", new RdlcParameter { Value = ReportHelpers.GetNormalNumberDigit(), IsVisible = true });
            list.Add("Currency", new RdlcParameter { Value = parameters.CurrencyDescription, IsVisible = true });

            return list;
        }
    }
}
