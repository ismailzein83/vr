using System;
using System.Collections.Generic;
using System.Linq;
using TOne.WhS.Analytics.Entities.BillingReport;
using Vanrise.Analytic.Business;
using Vanrise.Analytic.Entities;
using Vanrise.Entities;

namespace TOne.WhS.Analytics.Business.BillingReports
{
    public class DailySummaryForcastingReportGenerator : IReportGenerator
    {
        public Dictionary<string, System.Collections.IEnumerable> GenerateDataSources(ReportParameters parameters)
        {
            AnalyticManager analyticManager = new AnalyticManager();
            List<string> listDimensions = new List<string>() { "Day" };
            List<string> listMeasures = new List<string> { "SaleNet", "CostNet" };

            Vanrise.Entities.DataRetrievalInput<AnalyticQuery> analyticQuery = new DataRetrievalInput<AnalyticQuery>()
            {
                Query = new AnalyticQuery()
                {
                    DimensionFields = listDimensions,
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

            var result = analyticManager.GetFilteredRecords(analyticQuery) as Vanrise.Analytic.Entities.AnalyticSummaryBigResult<AnalyticRecord>;
            if (result != null)
                foreach (var analyticRecord in result.Data)
                {
                    DailySummaryFormatted dailySummary = new DailySummaryFormatted();

                    var dayValue = analyticRecord.DimensionValues[0];
                    if (dayValue != null)
                        dailySummary.Day = dayValue.Name;

                    MeasureValue saleNet;    
                    analyticRecord.MeasureValues.TryGetValue("SaleNet", out saleNet);
                    dailySummary.SaleNet = (saleNet == null) ? 0 : Convert.ToDouble(saleNet.Value ?? 0.0);
                    dailySummary.SaleNetFormatted = ReportHelpers.FormatNormalNumberDigit(dailySummary.SaleNet);


                    MeasureValue costNet;    
                    analyticRecord.MeasureValues.TryGetValue("CostNet", out costNet);
                    dailySummary.CostNet = (costNet == null) ? 0 : Convert.ToDouble(costNet.Value ?? 0.0);
                    dailySummary.CostNetFormatted = ReportHelpers.FormatNormalNumberDigit(dailySummary.CostNet);

                    listDailySummary.Add(dailySummary);
                }

            List<double> days = listDailySummary.Select(r => Dayof(r.Day.ToString())).OrderBy(d => d).ToList();
            List<double> SaleAmounts = listDailySummary.OrderBy(r => Dayof(r.Day.ToString())).Select(r => !string.IsNullOrEmpty(r.SaleNet.ToString()) ? double.Parse(r.SaleNet.ToString()) : 0).ToList();
            List<double> CostAmounts = listDailySummary.OrderBy(r => Dayof(r.Day.ToString())).Select(r => !string.IsNullOrEmpty(r.CostNet.ToString()) ? double.Parse(r.CostNet.ToString()) : 0).ToList();

            if (days.Count == 0)
            {
                parameters.AverageSaleNet = "";
                parameters.AverageCostNet = "";
                parameters.AverageProfit = "";
                parameters.AveragePercProfit = "";
                parameters.MTDSaleNet = "";
                parameters.MTDCostNet = "";
                parameters.MTDProfit = "";
                parameters.ForcastSaleNet = "";
                parameters.ForcastCostNet = "";
                parameters.ForcastProfit = "";
                parameters.InterpolatedDay = "";
                parameters.InterpolatedSaleNet = "";
                parameters.InterpolatedCostNet = "";
                parameters.InterpolatedProfit = "";
                parameters.InterpolatedPercProfit = "";
            }
            else
            {
                MyInterpolation SaleInterpolation = new MyInterpolation(days, SaleAmounts);
                MyInterpolation CostInterpolation = new MyInterpolation(days, CostAmounts);

                DateTime date = listDailySummary.Select(r => DateTime.Parse(r.Day.ToString())).Max();
                int startDay = parameters.FromTime.Day;

                int endDay = 0;
                if(parameters.ToTime.HasValue)
                 endDay = parameters.ToTime.Value.Day ;

                int maxDays = (endDay - startDay) + 1;
                int year = date.Date.Year;
                int month = date.Month;
                int numDays = DateTime.DaysInMonth(year, month);
                numDays = numDays - maxDays;

                DateTime InterpolatedDate = date.AddDays(1);
                int interpolatedDay = InterpolatedDate.Day;


                double SaleInterpolatedValue = SaleInterpolation.Interpolate(interpolatedDay);
                double CostInterpolatedValue = CostInterpolation.Interpolate(interpolatedDay);

                parameters.AverageSaleNet = (listDailySummary.Sum(r => !string.IsNullOrEmpty(r.SaleNet.ToString()) ? (double)r.SaleNet : 0) / maxDays).ToString("#,##0.00");
                parameters.AverageCostNet = (listDailySummary.Sum(r => !string.IsNullOrEmpty(r.CostNet.ToString()) ? (double)r.CostNet : 0) / maxDays).ToString("#,##0.00");
                parameters.AverageProfit = (listDailySummary.Sum(r => (!string.IsNullOrEmpty(r.SaleNet.ToString()) ? (double)r.SaleNet : 0) - (!string.IsNullOrEmpty(r.CostNet.ToString()) ? (double)r.CostNet : 0)) / maxDays).ToString("#,##0.00");
                parameters.AveragePercProfit = (listDailySummary.Sum(r => ((!string.IsNullOrEmpty(r.SaleNet.ToString()) ? (double)r.SaleNet : 0) - (!string.IsNullOrEmpty(r.CostNet.ToString()) ? (double)r.CostNet : 0)) / ((double)r.SaleNet > 0 ? (double)r.SaleNet : 1)) / maxDays).ToString("P2");

                //MTD
                parameters.MTDSaleNet = listDailySummary.Sum(r => !string.IsNullOrEmpty(r.SaleNet.ToString()) ? (double)r.SaleNet : 0).ToString("#,##0.00");
                parameters.MTDCostNet = listDailySummary.Sum(r => !string.IsNullOrEmpty(r.CostNet.ToString()) ? (double)r.CostNet : 0).ToString("#,##0.00");
                parameters.MTDProfit = listDailySummary.Sum(r => (!string.IsNullOrEmpty(r.SaleNet.ToString()) ? (double)r.SaleNet : 0) - (!string.IsNullOrEmpty(r.CostNet.ToString()) ? (double)r.CostNet : 0)).ToString("#,##0.00");

                //Forcast
                parameters.ForcastSaleNet = (double.Parse(parameters.AverageSaleNet) * numDays + double.Parse(parameters.MTDSaleNet)).ToString("#,##0.00");
                parameters.ForcastCostNet = (double.Parse(parameters.AverageCostNet) * numDays + double.Parse(parameters.MTDCostNet)).ToString("#,##0.00");
                parameters.ForcastProfit = (double.Parse(parameters.AverageProfit) * numDays + double.Parse(parameters.MTDProfit)).ToString("#,##0.00");
                //interpolated 
                parameters.InterpolatedDay = InterpolatedDate.Date.ToString("MMM dd yyyy");
                parameters.InterpolatedSaleNet = SaleInterpolatedValue > 0 ? SaleInterpolatedValue.ToString("#,##0.00") : "0.0";
                parameters.InterpolatedCostNet = CostInterpolatedValue > 0 ? CostInterpolatedValue.ToString("#,##0.00") : "0.0";
                parameters.InterpolatedProfit = (SaleInterpolatedValue - CostInterpolatedValue) > 0 ? (SaleInterpolatedValue - CostInterpolatedValue).ToString("#,##0.00") : "0.00";
                parameters.InterpolatedPercProfit = (SaleInterpolatedValue - CostInterpolatedValue) > 0 ? ((SaleInterpolatedValue - CostInterpolatedValue) / SaleInterpolatedValue).ToString("P2") : "0.00 %";
            }

            Dictionary<string, System.Collections.IEnumerable> dataSources = new Dictionary<string, System.Collections.IEnumerable>();
            dataSources.Add("DailyForcasting", listDailySummary);
            return dataSources;
        }

        public Dictionary<string, RdlcParameter> GetRdlcReportParameters(ReportParameters parameters)
        {
            Dictionary<string, RdlcParameter> list = new Dictionary<string, RdlcParameter>();
            list.Add("FromDate", new RdlcParameter { Value = parameters.FromTime.ToString(), IsVisible = true });
            list.Add("ToDate", new RdlcParameter { Value = parameters.ToTime.ToString(), IsVisible = true });
            list.Add("Title", new RdlcParameter { Value = "Daily Summary Forcasting", IsVisible = true });
            list.Add("LogoPath", new RdlcParameter { Value = "logo", IsVisible = true });
            list.Add("DigitRate", new RdlcParameter { Value = "2", IsVisible = true });
            list.Add("Currency", new RdlcParameter { Value = parameters.CurrencyDescription, IsVisible = true });

            list.Add("AverageSaleNet", new RdlcParameter { Value = parameters.AverageSaleNet, IsVisible = true });
            list.Add("AverageCostNet", new RdlcParameter { Value = parameters.AverageCostNet, IsVisible = true });
            list.Add("AverageProfit", new RdlcParameter { Value = parameters.AverageProfit, IsVisible = true });
            list.Add("AveragePercProfit", new RdlcParameter { Value = parameters.AveragePercProfit, IsVisible = true });

            list.Add("MTDSaleNet", new RdlcParameter { Value = parameters.MTDSaleNet, IsVisible = true });
            list.Add("MTDCostNet", new RdlcParameter { Value = parameters.MTDCostNet, IsVisible = true });
            list.Add("MTDProfit", new RdlcParameter { Value = parameters.MTDProfit, IsVisible = true });

            list.Add("ForcastSaleNet", new RdlcParameter { Value = parameters.ForcastSaleNet, IsVisible = true });
            list.Add("ForcastCostNet", new RdlcParameter { Value = parameters.ForcastCostNet, IsVisible = true });
            list.Add("ForcastProfit", new RdlcParameter { Value = parameters.ForcastProfit, IsVisible = true });


            list.Add("InterpolatedDay", new RdlcParameter { Value = parameters.InterpolatedDay, IsVisible = true });
            list.Add("InterpolatedSaleNet", new RdlcParameter { Value = parameters.InterpolatedSaleNet, IsVisible = true });
            list.Add("InterpolatedCostNet", new RdlcParameter { Value = parameters.InterpolatedCostNet, IsVisible = true });
            list.Add("InterpolatedProfit", new RdlcParameter { Value = parameters.InterpolatedProfit, IsVisible = true });
            list.Add("InterpolatedPercProfit", new RdlcParameter { Value = parameters.InterpolatedPercProfit, IsVisible = true });


            return list;
        }

        private double Dayof(string datestring)
        {
            double day;
            DateTime date = DateTime.Parse(datestring);
            day = (double)date.Day;
            return day;
        }
    }
}
