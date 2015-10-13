using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.Analytics.Entities;
using TOne.BusinessEntity.Business;
using TOne.Entities;

namespace TOne.Analytics.Business.BillingReports
{
    public class DailySummaryForcastingReportGenerator : TOne.Entities.IReportGenerator
    {
        public Dictionary<string, System.Collections.IEnumerable> GenerateDataSources(TOne.Entities.ReportParameters parameters)
        {
            AccountManagerManager am = new AccountManagerManager();
            List<string> suppliersIds = am.GetMyAssignedSupplierIds();
            List<string> customersIds = am.GetMyAssignedCustomerIds();



            BillingStatisticManager manager = new BillingStatisticManager();
            List<DailyForcastingFormatted> dailyForcasting = manager.GetDailyForcasting(parameters.FromTime, parameters.ToTime,customersIds, suppliersIds , parameters.CurrencyId);
           
            List<double> days = dailyForcasting.Select(r => Dayof(r.Day.ToString())).OrderBy(d => d).ToList();
            List<double> SaleAmounts = dailyForcasting.OrderBy(r => Dayof(r.Day.ToString())).Select(r => !string.IsNullOrEmpty(r.SaleNet.ToString()) ? double.Parse(r.SaleNet.ToString()) : 0).ToList();
            List<double> CostAmounts = dailyForcasting.OrderBy(r => Dayof(r.Day.ToString())).Select(r => !string.IsNullOrEmpty(r.CostNet.ToString()) ? double.Parse(r.CostNet.ToString()) : 0).ToList();

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

                DateTime date = dailyForcasting.Select(r => DateTime.Parse(r.Day.ToString())).Max();
                int startDay =  parameters.FromTime.Day;


                int endDay = parameters.ToTime.Day;

                int maxDays = (endDay - startDay) + 1;
                int year = date.Date.Year;
                int month = date.Month;
                int numDays = DateTime.DaysInMonth(year, month);
                numDays = numDays - maxDays;

                DateTime InterpolatedDate = date.AddDays(1);
                int interpolatedDay = InterpolatedDate.Day;


                double SaleInterpolatedValue = SaleInterpolation.Interpolate(interpolatedDay);
                double CostInterpolatedValue = CostInterpolation.Interpolate(interpolatedDay);

                parameters.AverageSaleNet = (dailyForcasting.Sum(r => !string.IsNullOrEmpty(r.SaleNet.ToString()) ? (double)r.SaleNet : 0) / maxDays).ToString("#,##0.00");
                parameters.AverageCostNet = (dailyForcasting.Sum(r => !string.IsNullOrEmpty(r.CostNet.ToString()) ? (double)r.CostNet : 0) / maxDays).ToString("#,##0.00");
                parameters.AverageProfit = (dailyForcasting.Sum(r => (!string.IsNullOrEmpty(r.SaleNet.ToString()) ? (double)r.SaleNet : 0) - (!string.IsNullOrEmpty(r.CostNet.ToString()) ? (double)r.CostNet : 0)) / maxDays).ToString("#,##0.00");
                parameters.AveragePercProfit = (dailyForcasting.Sum(r => ((!string.IsNullOrEmpty(r.SaleNet.ToString()) ? (double)r.SaleNet : 0) - (!string.IsNullOrEmpty(r.CostNet.ToString()) ? (double)r.CostNet : 0)) / ((double)r.SaleNet>0?(double)r.SaleNet:1)) / maxDays).ToString("P2");

                //MTD
                parameters.MTDSaleNet = dailyForcasting.Sum(r => !string.IsNullOrEmpty(r.SaleNet.ToString()) ? (double)r.SaleNet : 0).ToString("#,##0.00");
                parameters.MTDCostNet = dailyForcasting.Sum(r => !string.IsNullOrEmpty(r.CostNet.ToString()) ? (double)r.CostNet : 0).ToString("#,##0.00");
                parameters.MTDProfit = dailyForcasting.Sum(r => (!string.IsNullOrEmpty(r.SaleNet.ToString()) ? (double)r.SaleNet : 0) - (!string.IsNullOrEmpty(r.CostNet.ToString()) ? (double)r.CostNet : 0)).ToString("#,##0.00");

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
            dataSources.Add("DailyForcasting", dailyForcasting);
            return dataSources;
        }

        public Dictionary<string, RdlcParameter> GetRdlcReportParameters(TOne.Entities.ReportParameters parameters)
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

        protected double Dayof(string datestring)
        {
            double day;
            DateTime date = DateTime.Parse(datestring);
            day = (double)date.Day;
            return day;
        }
    }
}
