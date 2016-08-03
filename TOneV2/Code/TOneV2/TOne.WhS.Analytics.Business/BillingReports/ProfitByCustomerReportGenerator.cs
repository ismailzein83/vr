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

                    MeasureValue costDuration;
                    analyticRecord.MeasureValues.TryGetValue("CostDuration", out costDuration);
                    carrierSummary.CostDuration = Convert.ToDecimal(costDuration.Value ?? 0.0);
                    carrierSummary.CostDurationFormatted = ReportHelpers.FormatNormalNumberDigit(carrierSummary.CostDuration);

                    MeasureValue costNet;
                    analyticRecord.MeasureValues.TryGetValue("CostNet", out costNet);
                    carrierSummary.CostNet = Convert.ToDouble(costNet.Value ?? 0.0);
                    carrierSummary.CostNetFormatted = ReportHelpers.FormatNormalNumberDigit(carrierSummary.CostNet);

                    MeasureValue saleNet;
                    analyticRecord.MeasureValues.TryGetValue("SaleNet", out saleNet);

                    carrierSummary.SaleNet = Convert.ToDouble(saleNet == null ? 0.0 : saleNet.Value ?? 0.0);
                    carrierSummary.SaleNetFormatted = ReportHelpers.FormatNormalNumberDigit(carrierSummary.SaleNet);

                    MeasureValue profit;
                    analyticRecord.MeasureValues.TryGetValue("Profit", out profit);
                    carrierSummary.Profit = Convert.ToDouble(profit.Value ?? 0.0);
                    carrierSummary.ProfitFormatted = ReportHelpers.FormatNormalNumberDigit(carrierSummary.Profit);

                    MeasureValue costChargesValue;
                    analyticRecord.MeasureValues.TryGetValue("CostExtraCharges", out costChargesValue);
                    carrierSummary.CostExtraChargeValue = Convert.ToDouble(costChargesValue.Value ?? 0.0);
                    carrierSummary.CostExtraChargeValueFormatted = ReportHelpers.FormatLongNumberDigit(carrierSummary.CostExtraChargeValue);


                    MeasureValue saleChargesValue;
                    analyticRecord.MeasureValues.TryGetValue("SaleExtraCharges", out saleChargesValue);
                    carrierSummary.SaleExtraChargeValue = Convert.ToDouble(saleChargesValue.Value ?? 0.0);
                    carrierSummary.SaleExtraChargeValueFormatted = ReportHelpers.FormatLongNumberDigit(carrierSummary.SaleExtraChargeValue);

                    MeasureValue percentageProfit;
                    analyticRecord.MeasureValues.TryGetValue("PercentageProfit", out percentageProfit);
                    carrierSummary.PercentageProfit = Convert.ToDouble(percentageProfit.Value ?? 0.0);
                    carrierSummary.ProfitPercentageFormatted = ReportHelpers.FormatNumberPercentage(carrierSummary.PercentageProfit);



                    //MeasureValue costCommissionValue;
                    //analyticRecord.MeasureValues.TryGetValue("CostCommissions", out costCommissionValue);
                    //carrierSummary.CostCommissionValue = Convert.ToDouble(costCommissionValue.Value ?? 0.0);
                    //carrierSummary.CostCommissionValueFormatted = ReportHelpers.FormatNumber(carrierSummary.CostCommissionValue);

                    //MeasureValue saleCommissionValue;
                    //analyticRecord.MeasureValues.TryGetValue("SaleCommissions", out saleCommissionValue);
                    //carrierSummary.SaleCommissionValue = Convert.ToDouble(saleCommissionValue.Value ?? 0.0);
                    //carrierSummary.SaleCommissionValueFormatted = ReportHelpers.FormatNumber(carrierSummary.SaleCommissionValue);


                    carrierSummary.AvgMin = (carrierSummary.SaleDuration.Value != 0) ? (decimal)(((double)carrierSummary.SaleNet.Value - (double)carrierSummary.CostNet.Value) / (double)carrierSummary.SaleDuration.Value) : 0;

                    carrierSummary.AvgMinFormatted = (carrierSummary.SaleDuration.Value != 0) ? ReportHelpers.FormatNormalNumberDigit((decimal)carrierSummary.SaleNet / carrierSummary.SaleDuration - (decimal)carrierSummary.CostNet / carrierSummary.SaleDuration) : "0.00";

                    //carrierSummary.ProfitPercentageFormatted = carrierSummary.SaleNet == 0 ? "" : (carrierSummary.SaleNet.HasValue) ? ReportHelpers.FormatNumberPercentage(carrierSummary.Profit / carrierSummary.SaleNet) : "-100%";


                    listCarrierSummaryDetailed.Add(carrierSummary);
                }

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

            return list;
        }
    }
}
