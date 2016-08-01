﻿using System;
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
                    MeasureFields = new List<string> { "DurationInMinutes", "ASR", "ACD" },
                    TableId = 4,
                    FromTime = parameters.FromTime,
                    ToTime = parameters.ToTime,
                    CurrencyId = parameters.CurrencyId,
                    ParentDimensions = new List<string>(),
                    Filters = new List<DimensionFilter>(),
                    OrderType = AnalyticQueryOrderType.ByAllMeasures
                }
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

                    routingAnalysis.AVGCost = (routingAnalysis.Duration == 0 || routingAnalysis.CostNet == 0)
                        ? 0
                        : (routingAnalysis.SaleNet / (double)routingAnalysis.Duration);
                    routingAnalysis.AVGCostFormatted = routingAnalysis.AVGCost == 0
                        ? "0"
                        : ReportHelpers.FormatNumberDigitRate(routingAnalysis.AVGCost);

                    routingAnalysis.AVGSale = (routingAnalysis.Duration == 0 || routingAnalysis.SaleNet == 0)
                        ? 0
                        : (routingAnalysis.SaleNet / (double)routingAnalysis.Duration);
                    routingAnalysis.AVGSaleFormatted = routingAnalysis.AVGSale == 0
                        ? "0"
                        : ReportHelpers.FormatNumberDigitRate(routingAnalysis.AVGSale);

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
            parameters.TotalCost = TotalSale;
            parameters.TotalProfit = TotalProfit;

            start = DateTime.Now;
            result = analyticManager.GetFilteredRecords(trafficDataRetrievalInput) as AnalyticSummaryBigResult<AnalyticRecord>;
            spent = DateTime.Now.Subtract(start);
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
                {"ToDate", new RdlcParameter {Value =(parameters.ToTime.HasValue)?parameters.ToTime.ToString():null, IsVisible = true}},
                {"Title", new RdlcParameter {Value = "Routing Analysis Report", IsVisible = true}},
                {"Currency", new RdlcParameter {Value = parameters.CurrencyDescription, IsVisible = true}},
                {"Customer", new RdlcParameter { Value = ReportHelpers.GetCarrierName(parameters.CustomersId, "Customers"), IsVisible = true }},
                {"LogoPath", new RdlcParameter {Value = "logo", IsVisible = true}},
                {"DigitRate", new RdlcParameter {Value = "2", IsVisible = true}},
                {"TotalDuration", new RdlcParameter {Value =  ReportHelpers.FormatNumber(parameters.TotalDuration), IsVisible = true}},
                {"TotalSale", new RdlcParameter {Value =  ReportHelpers.FormatNumber(parameters.TotalSale), IsVisible = true}},
                {"TotalCost", new RdlcParameter {Value = ReportHelpers.FormatNumber(parameters.TotalCost), IsVisible = true}},
                {"TotalProfit", new RdlcParameter {Value =  ReportHelpers.FormatNumber(parameters.TotalProfit), IsVisible = true}},
                {"PageBreak", new RdlcParameter {Value = parameters.PageBreak.ToString(), IsVisible = true}}
            };
            return list;
        }
    }

}
