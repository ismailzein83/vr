using System;
using System.Collections.Generic;
using System.Linq;
using TOne.WhS.Analytics.Entities.BillingReport;
using TOne.WhS.Analytics.Entities.BillingReport.RoutingAnalysis;
using Vanrise.Analytic.Business;
using Vanrise.Analytic.Entities;
using Vanrise.Entities;

namespace TOne.WhS.Analytics.Business.BillingReports
{
    public class RoutingAnalysisReportGenerator : IReportGenerator
    {
        public Dictionary<string, System.Collections.IEnumerable> GenerateDataSources(ReportParameters parameters)
        {
            AnalyticManager analyticManager = new AnalyticManager();

            #region BillingStats
            DataRetrievalInput<AnalyticQuery> analyticQuery = new DataRetrievalInput<AnalyticQuery>
            {
                Query = new AnalyticQuery
                {
                    DimensionFields = new List<string> { "SaleZone", "Supplier" },
                    MeasureFields = new List<string> { "SaleDuration", "CostNet", "SaleNet", "Profit" },
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
                DimensionFilter dimensionFilter = new DimensionFilter
                {
                    Dimension = "Customer",
                    FilterValues = parameters.CustomersId.Split(',').ToList().Cast<object>().ToList()
                };
                analyticQuery.Query.Filters.Add(dimensionFilter);
            }

            if (!String.IsNullOrEmpty(parameters.SuppliersId))
            {
                DimensionFilter dimensionFilter = new DimensionFilter
                {
                    Dimension = "Supplier",
                    FilterValues = parameters.SuppliersId.Split(',').ToList().Cast<object>().ToList()
                };
                analyticQuery.Query.Filters.Add(dimensionFilter);
            }
            #endregion
            #region TrafficStats

            DataRetrievalInput<AnalyticQuery> trafficDataRetrievalInput = new DataRetrievalInput<AnalyticQuery>
            {
                Query = new AnalyticQuery
                {
                    DimensionFields = new List<string> { "SaleZone", "Supplier" },
                    MeasureFields = new List<string> { "ASR", "ACD", "DurationInMinutes" },
                    TableId = 4,
                    FromTime = parameters.FromTime,
                    ToTime = parameters.ToTime,
                    CurrencyId = parameters.CurrencyId,
                    ParentDimensions = new List<string>(),
                    Filters = new List<DimensionFilter>()
                },
                SortByColumnName = "DimensionValues[0].Name"
            };
            if (!String.IsNullOrEmpty(parameters.ZonesId))
            {
                DimensionFilter dimensionFilter = new DimensionFilter
                {
                    Dimension = "ZoneID",
                    FilterValues = parameters.ZonesId.Split(',').ToList().Cast<object>().ToList()
                };
                trafficDataRetrievalInput.Query.Filters.Add(dimensionFilter);
            }
            #endregion

            List<RoutingAnalysisFormatted> listRoutingAnalysisFormatteds = new List<RoutingAnalysisFormatted>();

            Dictionary<string, RoutingAnalysisFormatted> routingAnalysisFormatteds = new Dictionary<string, RoutingAnalysisFormatted>();
            var result = analyticManager.GetFilteredRecords(analyticQuery) as AnalyticSummaryBigResult<AnalyticRecord>;
            if (result != null)
                foreach (var analyticRecord in result.Data)
                {
                    RoutingAnalysisFormatted routingAnalysis = new RoutingAnalysisFormatted();

                    var zoneValue = analyticRecord.DimensionValues[0];
                    if (zoneValue != null)
                        routingAnalysis.SaleZone = zoneValue.Name;

                    var supplierValue = analyticRecord.DimensionValues[1];
                    if (supplierValue != null)
                        routingAnalysis.Supplier = supplierValue.Name;

                    MeasureValue saleDuration;
                    analyticRecord.MeasureValues.TryGetValue("SaleDuration", out saleDuration);
                    routingAnalysis.Duration = Convert.ToDecimal(saleDuration.Value ?? 0.0);
                    routingAnalysis.DurationFormatted = ReportHelpers.FormatNumber(routingAnalysis.Duration);

                    MeasureValue saleNet;
                    analyticRecord.MeasureValues.TryGetValue("SaleNet", out saleNet);
                    routingAnalysis.SaleNet = Convert.ToDouble(saleNet.Value ?? 0.0);
                    routingAnalysis.SaleNetFormatted = ReportHelpers.FormatNumber(routingAnalysis.SaleNet);


                    MeasureValue costNet;
                    analyticRecord.MeasureValues.TryGetValue("CostNet", out costNet);
                    routingAnalysis.CostNet = Convert.ToDouble(costNet.Value ?? 0.0);
                    routingAnalysis.CostNetFormatted = ReportHelpers.FormatNumber(routingAnalysis.CostNet);


                    MeasureValue profit;
                    analyticRecord.MeasureValues.TryGetValue("Profit", out profit);
                    routingAnalysis.Profit = Convert.ToDouble(profit.Value ?? 0.0);
                    routingAnalysis.ProfitFormatted = ReportHelpers.FormatNumber(routingAnalysis.Profit);

                    routingAnalysis.AVGCostFormatted = (routingAnalysis.Duration == 0 || routingAnalysis.CostNet == 0)
                        ? "0"
                        : ReportHelpers.FormatNumberDigitRate((routingAnalysis.CostNet / (double)routingAnalysis.Duration));
                    routingAnalysis.AVGSaleFormatted = (routingAnalysis.Duration == 0 || routingAnalysis.SaleNet == 0)
                        ? "0"
                        : ReportHelpers.FormatNumberDigitRate((routingAnalysis.SaleNet / (double)routingAnalysis.Duration));

                    if (!routingAnalysisFormatteds.ContainsKey(routingAnalysis.SaleZone + routingAnalysis.Supplier))
                        routingAnalysisFormatteds[routingAnalysis.SaleZone + routingAnalysis.Supplier] = routingAnalysis;
                    listRoutingAnalysisFormatteds.Add(routingAnalysis);
                }
            result = analyticManager.GetFilteredRecords(trafficDataRetrievalInput) as AnalyticSummaryBigResult<AnalyticRecord>;
            if (result != null)
                foreach (var analyticRecord in result.Data)
                {
                    RoutingAnalysisFormatted routingAnalysis;
                    string zoneName = "", supplierName = "";
                    var zoneValue = analyticRecord.DimensionValues[0];
                    if (zoneValue != null)
                        zoneName = zoneValue.Name;

                    var supplierValue = analyticRecord.DimensionValues[1];
                    if (supplierValue != null)
                        supplierName = supplierValue.Name;
                    routingAnalysisFormatteds.TryGetValue(zoneName + supplierName, out routingAnalysis);
                    if (routingAnalysis != null)
                    {
                        MeasureValue asrMeasure;
                        analyticRecord.MeasureValues.TryGetValue("ASR", out asrMeasure);
                        routingAnalysis.ASR = Convert.ToDecimal(asrMeasure.Value ?? 0.0);
                        routingAnalysis.ASRFormatted = ReportHelpers.FormatNumber(routingAnalysis.ASR);

                        MeasureValue acdMeasure;
                        analyticRecord.MeasureValues.TryGetValue("ACD", out acdMeasure);
                        routingAnalysis.ACD = Convert.ToDecimal(acdMeasure.Value ?? 0.0);
                        routingAnalysis.ACDFormatted = ReportHelpers.FormatNumber(routingAnalysis.ACD);
                    }
                }
            Dictionary<string, System.Collections.IEnumerable> dataSources =
                new Dictionary<string, System.Collections.IEnumerable>
                {
                    {"RoutingAnalysis", listRoutingAnalysisFormatteds}
                };
            return dataSources;
        }

        public Dictionary<string, RdlcParameter> GetRdlcReportParameters(ReportParameters parameters)
        {
            Dictionary<string, RdlcParameter> list = new Dictionary<string, RdlcParameter>
            {
                {"FromDate", new RdlcParameter {Value = parameters.FromTime.ToString(), IsVisible = true}},
                {"ToDate", new RdlcParameter {Value = parameters.ToTime.ToString(), IsVisible = true}},
                {"Title", new RdlcParameter {Value = "Routing Analysis Report", IsVisible = true}},
                {"Currency", new RdlcParameter {Value = parameters.CurrencyDescription, IsVisible = true}},
                {"LogoPath", new RdlcParameter {Value = "logo", IsVisible = true}},
                {"DigitRate", new RdlcParameter {Value = "2", IsVisible = true}},
                {"TotalDuration", new RdlcParameter {Value = parameters.TotalDuration.ToString(), IsVisible = true}},
                {"TotalSale", new RdlcParameter {Value = parameters.TotalSale.ToString(), IsVisible = true}},
                {"TotalCost", new RdlcParameter {Value = parameters.TotalCost.ToString(), IsVisible = true}},
                {"TotalProfit", new RdlcParameter {Value = parameters.TotalProfit.ToString(), IsVisible = true}},
                {"PageBreak", new RdlcParameter {Value = parameters.PageBreak.ToString(), IsVisible = true}}
            };




            return list;
        }
    }

}
