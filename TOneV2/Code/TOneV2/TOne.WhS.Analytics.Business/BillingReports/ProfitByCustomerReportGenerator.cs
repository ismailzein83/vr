using System;
using System.Collections.Generic;
using System.Linq;
using TOne.WhS.Analytics.Entities.BillingReport;
using Vanrise.Analytic.Business;
using Vanrise.Analytic.Entities;
using Vanrise.Entities;

namespace TOne.WhS.Analytics.Business.BillingReports
{
    public class ProfitByCustomerReportGenerator : IReportGenerator
    {
        private string totalCostDuration = "";
        private string totalSaleDuration = "";
        private string totalCostNet = "";
        private string totalSaleNet = "";
        private string totalProfit = "";
        private string totalProfitPerc = "";
        private string totalAvgMinutes = "";
        private string totalCostExtraCharge = "";
        private string totalSaleExtraCharge = "";
        private string netProfit = "";

        public Dictionary<string, System.Collections.IEnumerable> GenerateDataSources(ReportParameters parameters)
        {
            AnalyticManager analyticManager = new AnalyticManager();

            DataRetrievalInput<AnalyticQuery> analyticQuery = new DataRetrievalInput<AnalyticQuery>
            {
                Query = new AnalyticQuery
                {
                    DimensionFields = new List<string> { "Customer", "Supplier" },
                    MeasureFields = new List<string> { "SaleDuration", "SaleNet", "CostDuration", "CostNet", "CostExtraCharges", "SaleExtraCharges", "Profit", "PercentageProfit" },
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

            List<ProfitByCustomerFormatted> listCarrierSummaryDetailed = new List<ProfitByCustomerFormatted>();
            decimal? totalCostDur = 0;
            decimal? totalSaleDur = 0;
            double? totalCostNt = 0;
            double? totalSaleNt = 0;
            double? totalProft = 0;
            double? totalProftPerc = 0;
            decimal? totalAvgMin = 0;
            double? totalCostExtraChrg = 0;
            double? totalSaleExtraChrg = 0;
            double? netProft = 0;
            var result = analyticManager.GetFilteredRecords(analyticQuery) as AnalyticSummaryBigResult<AnalyticRecord>;
            if (result != null)
                foreach (var analyticRecord in result.Data)
                {
                    ProfitByCustomerFormatted carrierSummary = new ProfitByCustomerFormatted();

                    var customerValue = analyticRecord.DimensionValues[0];
                    if (customerValue != null)
                        carrierSummary.Customer = customerValue.Name;
                    var supplierValue = analyticRecord.DimensionValues[1];
                    if (supplierValue != null)
                        carrierSummary.Supplier = supplierValue.Name;

                    MeasureValue saleDuration;
                    analyticRecord.MeasureValues.TryGetValue("SaleDuration", out saleDuration);
                    carrierSummary.SaleDuration = Convert.ToDecimal(saleDuration.Value ?? 0.0);
                    carrierSummary.SaleDurationFormatted = ReportHelpers.FormatNormalNumberDigit(carrierSummary.SaleDuration);
                    totalSaleDur = totalSaleDur + carrierSummary.SaleDuration;

                    MeasureValue costDuration;
                    analyticRecord.MeasureValues.TryGetValue("CostDuration", out costDuration);
                    carrierSummary.CostDuration = Convert.ToDecimal(costDuration.Value ?? 0.0);
                    carrierSummary.CostDurationFormatted = ReportHelpers.FormatNormalNumberDigit(carrierSummary.CostDuration);
                    totalCostDur = totalCostDur + carrierSummary.CostDuration;

                    MeasureValue costNet;
                    analyticRecord.MeasureValues.TryGetValue("CostNet", out costNet);
                    carrierSummary.CostNet = Convert.ToDouble(costNet.Value ?? 0.0);
                    carrierSummary.CostNetFormatted = ReportHelpers.FormatNormalNumberDigit(carrierSummary.CostNet);
                    totalCostNt = totalCostNt + carrierSummary.CostNet;

                    MeasureValue saleNet;
                    analyticRecord.MeasureValues.TryGetValue("SaleNet", out saleNet);
                    carrierSummary.SaleNet = Convert.ToDouble(saleNet == null ? 0.0 : saleNet.Value ?? 0.0);
                    carrierSummary.SaleNetFormatted = ReportHelpers.FormatNormalNumberDigit(carrierSummary.SaleNet);
                    totalSaleNt = totalSaleNt + carrierSummary.SaleNet;

                    MeasureValue profit;
                    analyticRecord.MeasureValues.TryGetValue("Profit", out profit);
                    carrierSummary.Profit = Convert.ToDouble(profit.Value ?? 0.0);
                    carrierSummary.ProfitFormatted = ReportHelpers.FormatNormalNumberDigit(carrierSummary.Profit);
                    totalProft = totalProft + carrierSummary.Profit;

                    MeasureValue costChargesValue;
                    analyticRecord.MeasureValues.TryGetValue("CostExtraCharges", out costChargesValue);
                    carrierSummary.CostExtraChargeValue = Convert.ToDouble(costChargesValue.Value ?? 0.0);
                    carrierSummary.CostExtraChargeValueFormatted = ReportHelpers.FormatLongNumberDigit(carrierSummary.CostExtraChargeValue);
                    totalCostExtraChrg = totalCostExtraChrg + Math.Abs(carrierSummary.CostExtraChargeValue.Value);

                    MeasureValue saleChargesValue;
                    analyticRecord.MeasureValues.TryGetValue("SaleExtraCharges", out saleChargesValue);
                    carrierSummary.SaleExtraChargeValue = Convert.ToDouble(saleChargesValue.Value ?? 0.0);
                    carrierSummary.SaleExtraChargeValueFormatted = ReportHelpers.FormatLongNumberDigit(carrierSummary.SaleExtraChargeValue);
                    totalSaleExtraChrg = totalSaleExtraChrg + Math.Abs(carrierSummary.SaleExtraChargeValue.Value);

                    MeasureValue percentageProfit;
                    analyticRecord.MeasureValues.TryGetValue("PercentageProfit", out percentageProfit);
                    carrierSummary.PercentageProfit = Convert.ToDouble(percentageProfit.Value ?? 0.0);
                    carrierSummary.ProfitPercentageFormatted = ReportHelpers.FormatNumberPercentage(carrierSummary.PercentageProfit);

                    carrierSummary.AvgMin = (carrierSummary.SaleDuration.Value != 0) ? (decimal)(((double)carrierSummary.Profit.Value) / (double)carrierSummary.SaleDuration.Value) : 0;
                    totalAvgMin = totalAvgMin + carrierSummary.AvgMin;
                      
                    netProft = netProft + (carrierSummary.SaleNet - carrierSummary.CostNet);

                    carrierSummary.AvgMinFormatted = (carrierSummary.SaleDuration.Value != 0) ? ReportHelpers.FormatLongNumberDigit((decimal)carrierSummary.Profit / carrierSummary.SaleDuration) : "0.00";

                    listCarrierSummaryDetailed.Add(carrierSummary);
                }
            if (parameters.IsCommission)
                netProft = netProft - totalSaleExtraChrg - totalCostExtraChrg;

            totalProftPerc = (totalSaleNt - totalCostNt)/totalSaleNt;
            totalCostDuration = ReportHelpers.FormatNormalNumberDigit(totalCostDur);
            totalSaleDuration = ReportHelpers.FormatNormalNumberDigit(totalSaleDur);
            totalSaleNet = ReportHelpers.FormatNormalNumberDigit(totalSaleNt);
            totalCostNet = ReportHelpers.FormatNormalNumberDigit(totalCostNt);
            totalProfit = ReportHelpers.FormatNormalNumberDigit(totalProft);
            totalProfitPerc = ReportHelpers.FormatNormalNumberDigit(totalProftPerc);
            totalAvgMinutes = ReportHelpers.FormatLongNumberDigit(totalAvgMin);
            totalCostExtraCharge = ReportHelpers.FormatLongNumberDigit(totalCostExtraChrg);
            totalSaleExtraCharge = ReportHelpers.FormatLongNumberDigit(totalSaleExtraChrg);
            netProfit = ReportHelpers.FormatNormalNumberDigit(netProft);

            Dictionary<string, System.Collections.IEnumerable> dataSources =
                new Dictionary<string, System.Collections.IEnumerable> { { "CarrierSummary", listCarrierSummaryDetailed } };
            return dataSources;

        }

        public Dictionary<string, RdlcParameter> GetRdlcReportParameters(ReportParameters parameters)
        {
            Dictionary<string, RdlcParameter> list = new Dictionary<string, RdlcParameter>();


            list.Add("FromDate", new RdlcParameter { Value = parameters.FromTime.ToString(), IsVisible = true });
            list.Add("ToDate", new RdlcParameter { Value = parameters.ToTime.HasValue ? parameters.ToTime.ToString() : null, IsVisible = true });
            list.Add("Customer", new RdlcParameter {Value = ReportHelpers.GetCarrierName(parameters.CustomersId,"Customers"), IsVisible = true});
            list.Add("Supplier", new RdlcParameter { Value = ReportHelpers.GetCarrierName(parameters.SuppliersId, "Suppliers"), IsVisible = true });
            list.Add("Title", new RdlcParameter { Value = "Profit by Customer", IsVisible = true });
            list.Add("Currency", new RdlcParameter { Value = parameters.CurrencyDescription, IsVisible = true });
            list.Add("LogoPath", new RdlcParameter { Value = "logo", IsVisible = true });
            list.Add("DigitRate", new RdlcParameter { Value = ReportHelpers.GetLongNumberDigit(), IsVisible = true });
            list.Add("Digit", new RdlcParameter { Value = ReportHelpers.GetNormalNumberDigit(), IsVisible = true });
            list.Add("ShowProfit", new RdlcParameter { Value = parameters.IsCommission.ToString(), IsVisible = true });
            list.Add("PageBreak", new RdlcParameter { Value = parameters.PageBreak.ToString(), IsVisible = true });
            list.Add("TotalCostDuration", new RdlcParameter { Value = totalCostDuration, IsVisible = true });
            list.Add("TotalSaleDuration", new RdlcParameter { Value = totalSaleDuration, IsVisible = true });
            list.Add("TotalSaleNet", new RdlcParameter { Value = totalSaleNet, IsVisible = true });
            list.Add("TotalCostNet", new RdlcParameter { Value = totalCostNet, IsVisible = true });
            list.Add("TotalProfit", new RdlcParameter { Value = totalProfit, IsVisible = true });
            list.Add("TotalProfitPerc", new RdlcParameter { Value = totalProfitPerc, IsVisible = true });
            list.Add("TotalAvgMinutes", new RdlcParameter { Value = totalAvgMinutes, IsVisible = true });
            list.Add("TotalCostExtraCharge", new RdlcParameter { Value = totalCostExtraCharge, IsVisible = true });
            list.Add("TotalSaleExtraCharge", new RdlcParameter { Value = totalSaleExtraCharge, IsVisible = true });
            list.Add("NetProfit", new RdlcParameter { Value = netProfit, IsVisible = true });

            return list;
        }
    }
}
