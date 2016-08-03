using System;
using System.Collections.Generic;
using System.Linq;
using TOne.WhS.Analytics.Entities.BillingReport;
using Vanrise.Analytic.Business;
using Vanrise.Analytic.Entities;
using Vanrise.Entities;

namespace TOne.WhS.Analytics.Business.BillingReports
{
    public class DailySummaryReportGenerator : IReportGenerator
    {
        public Dictionary<string, System.Collections.IEnumerable> GenerateDataSources(ReportParameters parameters)
        {
            AnalyticManager analyticManager = new AnalyticManager();
            List<string> listGrouping = new List<string>();
            listGrouping.Add("Day");

            List<string> listMeasures = new List<string>();
            listMeasures.Add("NumberOfCalls");
            listMeasures.Add("DurationNet");
            listMeasures.Add("SaleDuration");
            listMeasures.Add("SaleNet");
            listMeasures.Add("CostNet");
            listMeasures.Add("Profit");


            Vanrise.Entities.DataRetrievalInput<AnalyticQuery> analyticQuery = new DataRetrievalInput<AnalyticQuery>()
            {
                Query = new AnalyticQuery()
                {
                    DimensionFields = listGrouping,
                    MeasureFields = listMeasures,
                    TableId = 8,
                    FromTime = parameters.FromTime,
                    ToTime = parameters.ToTime,
                    CurrencyId = parameters.CurrencyId,
                    ParentDimensions = new List<string>(),
                    Filters = new List<DimensionFilter>()
                },
                SortByColumnName = "DimensionValues[0].Name"
            };

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

            List<DailySummaryFormatted> listDailySummary = new List<DailySummaryFormatted>();

            var result = analyticManager.GetFilteredRecords(analyticQuery) as AnalyticSummaryBigResult<AnalyticRecord>;
            if (result != null)
                foreach (var analyticRecord in result.Data)
                {
                    DailySummaryFormatted dailySummary = new DailySummaryFormatted();

                    var dayValue = analyticRecord.DimensionValues[0];
                    if (dayValue != null)
                        dailySummary.Day = dayValue.Name;

                    MeasureValue calls;

                    analyticRecord.MeasureValues.TryGetValue("NumberOfCalls", out calls);
                    dailySummary.Calls = Convert.ToInt32(calls.Value ?? 0.0);

                    dailySummary.CallsFormatted = ReportHelpers.FormatNumber(dailySummary.Calls);

                    MeasureValue durationNet;
                    analyticRecord.MeasureValues.TryGetValue("DurationNet", out durationNet);
                    dailySummary.DurationNet = Convert.ToDecimal(durationNet.Value ?? 0.0);
                    dailySummary.DurationNetFormatted = ReportHelpers.FormatNumber(dailySummary.DurationNet);

                    MeasureValue saleDuration;
                    analyticRecord.MeasureValues.TryGetValue("SaleDuration", out saleDuration);
                    dailySummary.SaleDuration = Convert.ToDecimal(saleDuration.Value ?? 0.0);
                    dailySummary.SaleDurationFormatted = dailySummary.SaleNet == 0 ? "" : (dailySummary.SaleDuration.HasValue) ? ReportHelpers.FormatNumber(dailySummary.SaleDuration) : "0.00";

                    MeasureValue saleNet;
                    analyticRecord.MeasureValues.TryGetValue("SaleNet", out saleNet);
                    dailySummary.SaleNet = Convert.ToDouble(saleNet == null ? 0.0 : saleNet.Value ?? 0.0);
                    dailySummary.SaleNetFormatted = dailySummary.SaleNet == 0 ? "" : (dailySummary.SaleNet.HasValue) ? ReportHelpers.FormatNumber(dailySummary.SaleNet) : "0.00";

                    MeasureValue costNet;
                    analyticRecord.MeasureValues.TryGetValue("CostNet", out costNet);
                    dailySummary.CostNet = Convert.ToDouble(costNet == null ? 0.0 : costNet.Value ?? 0.0);
                    dailySummary.CostNetFormatted = (dailySummary.CostNet.HasValue) ? ReportHelpers.FormatNumber(dailySummary.CostNet) : "0.00";

                    MeasureValue profit;
                    analyticRecord.MeasureValues.TryGetValue("Profit", out profit);
                    dailySummary.Profit = Convert.ToDouble(profit.Value ?? 0.0);
                    dailySummary.ProfitFormatted = ReportHelpers.FormatNumber(dailySummary.Profit);


                    dailySummary.ProfitPercentageFormatted = dailySummary.SaleNet == 0 ? "" : (dailySummary.SaleNet.HasValue) ? ReportHelpers.FormatNumberPercentage(((1 - dailySummary.CostNet / dailySummary.SaleNet))) : "-100%";

                    listDailySummary.Add(dailySummary);
                }

            Dictionary<string, System.Collections.IEnumerable> dataSources = new Dictionary<string, System.Collections.IEnumerable>();
            dataSources.Add("DailySummary", listDailySummary);
            return dataSources;
        }

        public Dictionary<string, RdlcParameter> GetRdlcReportParameters(ReportParameters parameters)
        {
            Dictionary<string, RdlcParameter> list = new Dictionary<string, RdlcParameter>();

            list.Add("FromDate", new RdlcParameter { Value = parameters.FromTime.ToString(), IsVisible = true });
            list.Add("ToDate", new RdlcParameter { Value = (parameters.ToTime.HasValue)?parameters.ToTime.ToString():null, IsVisible = true });
            list.Add("Title", new RdlcParameter { Value = "Summary by Day", IsVisible = true });
            list.Add("Currency", new RdlcParameter { Value = parameters.CurrencyDescription, IsVisible = true });
            list.Add("LogoPath", new RdlcParameter { Value = "logo", IsVisible = true });
            list.Add("DigitRate", new RdlcParameter { Value = "4", IsVisible = true });

            return list;
        }
    }
}
