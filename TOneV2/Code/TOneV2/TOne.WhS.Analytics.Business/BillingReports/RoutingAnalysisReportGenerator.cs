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
            List<long> topZoneIds = new List<long>();

            DataRetrievalInput<AnalyticQuery> topNSaleZoneQuery = new DataRetrievalInput<AnalyticQuery>
            {
                Query = new AnalyticQuery
                {
                    DimensionFields = new List<string> { "SaleZone"},
                    MeasureFields = new List<string> { "DurationNet"},
                    TableId = 8,
                    FromTime = parameters.FromTime,
                    ToTime = parameters.ToTime,
                    CurrencyId = parameters.CurrencyId,
                    ParentDimensions = new List<string>(),
                    Filters = new List<DimensionFilter>(),
                    TopRecords = parameters.Top
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
                topNSaleZoneQuery.Query.Filters.Add(dimensionFilter);
            }

            if (!String.IsNullOrEmpty(parameters.SuppliersId))
            {
                DimensionFilter dimensionFilter = new DimensionFilter
                {
                    Dimension = "Supplier",
                    FilterValues = parameters.SuppliersId.Split(',').ToList().Cast<object>().ToList()
                };
                topNSaleZoneQuery.Query.Filters.Add(dimensionFilter);
            }

            var resultN = analyticManager.GetFilteredRecords(topNSaleZoneQuery) as AnalyticSummaryBigResult<AnalyticRecord>;
            List<RoutingAnalysisFormatted> listRoutingAnalysisFormatteds = new List<RoutingAnalysisFormatted>();
   
            if (resultN != null)
            {
                foreach (var analyticRecord in resultN.Data)
                {
                    RoutingAnalysisFormatted routingAnalysis = new RoutingAnalysisFormatted();

                    var zoneValue = analyticRecord.DimensionValues[0];
                    if (zoneValue != null) {
                        topZoneIds.Add((long)zoneValue.Value);
                    }
                }

                DataRetrievalInput<AnalyticQuery> analyticQuery = new DataRetrievalInput<AnalyticQuery>
                {
                    Query = new AnalyticQuery
                    {
                        DimensionFields = new List<string> { "SaleZone", "Supplier" },
                        MeasureFields = new List<string> { "DurationNet", "CostNet", "SaleNet", "Profit" },
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
               
                DimensionFilter zoneFilter = new DimensionFilter
                {
                    Dimension = "SaleZone",
                    FilterValues = topZoneIds.Cast<object>().ToList()
                };
                analyticQuery.Query.Filters.Add(zoneFilter);

            #endregion
                #region TrafficStats

                DataRetrievalInput<AnalyticQuery> trafficDataRetrievalInput = new DataRetrievalInput<AnalyticQuery>
                {
                    Query = new AnalyticQuery
                    {
                        DimensionFields = new List<string> { "SaleZone", "Supplier" },
                        MeasureFields = new List<string> { "DurationInMinutes", "ASR", "ACD" },
                        TableId = 4,
                        FromTime = parameters.FromTime,
                        ToTime = parameters.ToTime,
                        CurrencyId = parameters.CurrencyId,
                        ParentDimensions = new List<string>(),
                        Filters = new List<DimensionFilter>()
                    },
                    SortByColumnName = "DimensionValues[0].Name"
                };

                trafficDataRetrievalInput.Query.Filters.Add(zoneFilter);
                #endregion


                Dictionary<string, RoutingAnalysisFormatted> routingAnalysisFormatteds = new Dictionary<string, RoutingAnalysisFormatted>();

                decimal TotalDuration = 0;
                double TotalSale = 0;
                double TotalCost = 0;
                double TotalProfit = 0;
                DateTime start = DateTime.Now;
                var result = analyticManager.GetFilteredRecords(analyticQuery) as AnalyticSummaryBigResult<AnalyticRecord>;
                TimeSpan spent = DateTime.Now.Subtract(start);
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

                        MeasureValue duration;
                        analyticRecord.MeasureValues.TryGetValue("DurationNet", out duration);
                        routingAnalysis.Duration = Convert.ToDecimal(duration.Value ?? 0.0);
                        routingAnalysis.DurationFormatted = ReportHelpers.FormatNormalNumberDigit(routingAnalysis.Duration);

                        MeasureValue saleNet;
                        analyticRecord.MeasureValues.TryGetValue("SaleNet", out saleNet);
                        routingAnalysis.SaleNet = Convert.ToDouble(saleNet.Value ?? 0.0);
                        routingAnalysis.SaleNetFormatted = ReportHelpers.FormatNormalNumberDigit(routingAnalysis.SaleNet);


                        MeasureValue costNet;
                        analyticRecord.MeasureValues.TryGetValue("CostNet", out costNet);
                        routingAnalysis.CostNet = Convert.ToDouble(costNet.Value ?? 0.0);
                        routingAnalysis.CostNetFormatted = ReportHelpers.FormatNormalNumberDigit(routingAnalysis.CostNet);


                        MeasureValue profit;
                        analyticRecord.MeasureValues.TryGetValue("Profit", out profit);
                        routingAnalysis.Profit = Convert.ToDouble(profit.Value ?? 0.0);
                        routingAnalysis.ProfitFormatted = ReportHelpers.FormatNormalNumberDigit(routingAnalysis.Profit);

                        routingAnalysis.AVGCost = (routingAnalysis.Duration == 0 || routingAnalysis.CostNet == 0)
                            ? 0
                            : (routingAnalysis.CostNet / (double)routingAnalysis.Duration);
                        routingAnalysis.AVGCostFormatted = routingAnalysis.AVGCost == 0
                            ? "0"
                            : ReportHelpers.FormatNormalNumberDigit(routingAnalysis.AVGCost);

                        routingAnalysis.AVGSale = (routingAnalysis.Duration == 0 || routingAnalysis.SaleNet == 0)
                            ? 0
                            : (routingAnalysis.SaleNet / (double)routingAnalysis.Duration);
                        routingAnalysis.AVGSaleFormatted = routingAnalysis.AVGSale == 0
                            ? "0"
                            : ReportHelpers.FormatNormalNumberDigit(routingAnalysis.AVGSale);

                        if (!routingAnalysisFormatteds.ContainsKey(routingAnalysis.SaleZone + routingAnalysis.Supplier))
                            routingAnalysisFormatteds[routingAnalysis.SaleZone + routingAnalysis.Supplier] = routingAnalysis;
                        listRoutingAnalysisFormatteds.Add(routingAnalysis);

                        TotalDuration += routingAnalysis.Duration;
                        TotalCost += routingAnalysis.CostNet;
                        TotalSale += routingAnalysis.SaleNet;
                        TotalProfit += routingAnalysis.SaleNet - routingAnalysis.CostNet;
                 }

                parameters.TotalDuration = TotalDuration;
                parameters.TotalSale = TotalSale;
                parameters.TotalCost = TotalCost;
                parameters.TotalProfit = TotalProfit;

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
                            routingAnalysis.ASRFormatted = ReportHelpers.FormatNormalNumberDigit(routingAnalysis.ASR);

                            MeasureValue acdMeasure;
                            analyticRecord.MeasureValues.TryGetValue("ACD", out acdMeasure);
                            routingAnalysis.ACD = Convert.ToDecimal(acdMeasure.Value ?? 0.0);
                            routingAnalysis.ACDFormatted = ReportHelpers.FormatNormalNumberDigit(routingAnalysis.ACD);
                        }
                    }

            }
            var list = listRoutingAnalysisFormatteds;
            Dictionary<string, System.Collections.IEnumerable> dataSources =
                new Dictionary<string, System.Collections.IEnumerable>
                {
                    {"RoutingAnalysis", listRoutingAnalysisFormatteds}//.OrderByDescending(r=>r.SaleZone).Take(parameters.Top)}
                };

            return dataSources;
        }

        public Dictionary<string, RdlcParameter> GetRdlcReportParameters(ReportParameters parameters)
        {
            Dictionary<string, RdlcParameter> list = new Dictionary<string, RdlcParameter>
            {
                {"FromDate", new RdlcParameter {Value = parameters.FromTime.ToString(), IsVisible = true}},
                {"ToDate", new RdlcParameter {Value =(parameters.ToTime.HasValue)?parameters.ToTime.ToString():null, IsVisible = true}},
                {"Title", new RdlcParameter {Value = "Routing Analysis", IsVisible = true}},
                {"Currency", new RdlcParameter {Value = parameters.CurrencyDescription, IsVisible = true}},
                {"Customer", new RdlcParameter { Value = ReportHelpers.GetCarrierName(parameters.CustomersId, "Customers"), IsVisible = true }},
                {"Supplier", new RdlcParameter { Value = ReportHelpers.GetCarrierName(parameters.SuppliersId, "Suppliers"), IsVisible = true }},

                {"LogoPath", new RdlcParameter {Value = "logo", IsVisible = true}},
                {"DigitRate", new RdlcParameter {Value = ReportHelpers.GetLongNumberDigit(), IsVisible = true}},
                {"Digit", new RdlcParameter {Value = ReportHelpers.GetNormalNumberDigit(), IsVisible = true}},

                {"TotalDuration", new RdlcParameter {Value =  ReportHelpers.FormatNormalNumberDigit(parameters.TotalDuration), IsVisible = true}},
                {"TotalSale", new RdlcParameter {Value =  ReportHelpers.FormatNormalNumberDigit(parameters.TotalSale), IsVisible = true}},
                {"TotalCost", new RdlcParameter {Value = ReportHelpers.FormatNormalNumberDigit(parameters.TotalCost), IsVisible = true}},
                {"TotalProfit", new RdlcParameter {Value =  ReportHelpers.FormatNormalNumberDigit(parameters.TotalProfit), IsVisible = true}},
                {"PageBreak", new RdlcParameter {Value = parameters.PageBreak.ToString(), IsVisible = true}}
            };
            return list;
        }
    }

}
