using System;
using System.Collections.Generic;
using System.Linq;
using TOne.WhS.Analytics.Entities.BillingReport;
using Vanrise.Analytic.Business;
using Vanrise.Analytic.Entities;
using Vanrise.Entities;

namespace TOne.WhS.Analytics.Business.BillingReports
{
    public class SalesAndProfitsByCustomerReportGenerator : IReportGenerator
    {
        public Dictionary<string, System.Collections.IEnumerable> GenerateDataSources(ReportParameters parameters)
        {

            AnalyticManager analyticManager = new AnalyticManager();

            Vanrise.Entities.DataRetrievalInput<AnalyticQuery> analyticQuery = new DataRetrievalInput<AnalyticQuery>()
            {
                Query = new AnalyticQuery()
                {
                    DimensionFields = new List<string> { "Customer" },
                    MeasureFields = new List<string>() { "SaleNetNotNULL", "CostNetNotNULL", "SaleDuration", "CostDuration" },
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

            List<SalesAndProfitsByCustomer> listSalesAndProfitsByCustomer = new List<SalesAndProfitsByCustomer>();

            var result = analyticManager.GetFilteredRecords(analyticQuery) as AnalyticSummaryBigResult<AnalyticRecord>;

            if (result != null)
                foreach (var analyticRecord in result.Data)
                {
                    SalesAndProfitsByCustomer salesAndProfitsByCustomer = new SalesAndProfitsByCustomer();

                    var customerValue = analyticRecord.DimensionValues[0];
                    if (customerValue != null)
                        salesAndProfitsByCustomer.Customer = customerValue.Name;

                    MeasureValue saleNet;
                    analyticRecord.MeasureValues.TryGetValue("SaleNetNotNULL", out saleNet);

                    salesAndProfitsByCustomer.SaleNet = Convert.ToDouble(saleNet == null ? 0.0 : saleNet.Value ?? 0.0);
                    salesAndProfitsByCustomer.SaleNetFormatted = salesAndProfitsByCustomer.SaleNet == 0 ? "" : (salesAndProfitsByCustomer.SaleNet.HasValue) ?
                        ReportHelpers.FormatNumberDigitRate(salesAndProfitsByCustomer.SaleNet) : "0.00";

                    MeasureValue costNet;
                    analyticRecord.MeasureValues.TryGetValue("CostNetNotNULL", out costNet);
                    salesAndProfitsByCustomer.CostNet = Convert.ToDouble(costNet == null ? 0.0 : costNet.Value ?? 0.0);
                    salesAndProfitsByCustomer.CostNetFormatted = (salesAndProfitsByCustomer.CostNet.HasValue)
                        ? ReportHelpers.FormatNumberDigitRate(salesAndProfitsByCustomer.CostNet)
                        : "0.00";

                    MeasureValue saleDuration;
                    analyticRecord.MeasureValues.TryGetValue("SaleDuration", out saleDuration);
                    salesAndProfitsByCustomer.SaleDuration = Convert.ToDecimal(saleDuration.Value ?? 0.0);
                    salesAndProfitsByCustomer.SaleDurationFormatted = salesAndProfitsByCustomer.SaleNet == 0 ? "" : (salesAndProfitsByCustomer.SaleDuration.HasValue) ?
                        ReportHelpers.FormatNormalNumberDigit(salesAndProfitsByCustomer.SaleDuration) : "0.00";

                    MeasureValue costDuration;
                    analyticRecord.MeasureValues.TryGetValue("CostDuration", out costDuration);
                    salesAndProfitsByCustomer.CostDuration = Convert.ToDecimal(costDuration.Value ?? 0.0);
                    salesAndProfitsByCustomer.CostDurationFormatted = ReportHelpers.FormatNormalNumberDigit(salesAndProfitsByCustomer.CostDuration);

                    salesAndProfitsByCustomer.Profit = (salesAndProfitsByCustomer.SaleNet > 0) ? ((salesAndProfitsByCustomer.SaleNet - salesAndProfitsByCustomer.CostNet)) : 0;
                    salesAndProfitsByCustomer.ProfitFormatted = ReportHelpers.FormatNormalNumberDigit((salesAndProfitsByCustomer.SaleNet > 0) ? ((salesAndProfitsByCustomer.SaleNet - salesAndProfitsByCustomer.CostNet)) : 0);
                    salesAndProfitsByCustomer.ProfitPercentageFormatted = (salesAndProfitsByCustomer.SaleNet > 0)
                        ? ReportHelpers.FormatNumberPercentage(((salesAndProfitsByCustomer.SaleNet - salesAndProfitsByCustomer.CostNet) / salesAndProfitsByCustomer.SaleNet))
                        : ReportHelpers.FormatNumberPercentage(0);

                    listSalesAndProfitsByCustomer.Add(salesAndProfitsByCustomer);
                }

            List<ProfitSummary> profitSummary = GetCustomerProfitSummary(listSalesAndProfitsByCustomer);
            List<SaleAmountSummary> saleAmount = GetCustomerSaleAmountSummary(listSalesAndProfitsByCustomer);
            Dictionary<string, System.Collections.IEnumerable> dataSources = new Dictionary<string, System.Collections.IEnumerable>();
            dataSources.Add("CustomerSummary", listSalesAndProfitsByCustomer);
            dataSources.Add("SaleAmount", saleAmount);
            dataSources.Add("ProfitSummary", profitSummary);
            return dataSources;
        }

        public Dictionary<string, RdlcParameter> GetRdlcReportParameters(ReportParameters parameters)
        {
            Dictionary<string, RdlcParameter> list = new Dictionary<string, RdlcParameter>();
            list.Add("FromDate", new RdlcParameter { Value = parameters.FromTime.ToString(), IsVisible = true });
            list.Add("ToDate", new RdlcParameter { Value = parameters.ToTime.HasValue ? parameters.ToTime.ToString() : null, IsVisible = true });
            list.Add("Title", new RdlcParameter { Value = "Sales and Profits by Customer", IsVisible = true });
            list.Add("Currency", new RdlcParameter { Value = parameters.CurrencyDescription, IsVisible = true });
            list.Add("LogoPath", new RdlcParameter { Value = "logo", IsVisible = true });
            list.Add("Customer", new RdlcParameter { Value = ReportHelpers.GetCarrierName(parameters.CustomersId, "Customers"), IsVisible = true });
            list.Add("DigitRate", new RdlcParameter { Value = ReportHelpers.GetLongNumberDigit(), IsVisible = true });
            list.Add("Digit", new RdlcParameter { Value = ReportHelpers.GetNormalNumberDigit(), IsVisible = true });
            list.Add("ShowProfit", new RdlcParameter { Value = parameters.IsService.ToString(), IsVisible = true });
            list.Add("ServicesPerCustomer", new RdlcParameter { Value = parameters.ServicesForCustomer.ToString(), IsVisible = true });

            return list;
        }
        private List<ProfitSummary> GetCustomerProfitSummary(List<SalesAndProfitsByCustomer> summarieslist)
        {
            List<ProfitSummary> lstProfitSummary = new List<ProfitSummary>();
            lstProfitSummary = summarieslist.Select(r => new ProfitSummary
            {
                Profit = (r.SaleNet > 0) ? Math.Truncate((double)((r.SaleNet - r.CostNet) * 100)) / 100 : 0,
                FormattedProfit = ReportHelpers.FormatNormalNumberDigit((r.SaleNet > 0) ? ((r.SaleNet - r.CostNet)) : 0),
                Customer = r.Customer.ToString()
            }).OrderByDescending(r => r.Profit).Take(10).ToList();
            return lstProfitSummary;
        }
        private List<SaleAmountSummary> GetCustomerSaleAmountSummary(List<SalesAndProfitsByCustomer> summarieslist)
        {
            List<SaleAmountSummary> saleAmountSummary = new List<SaleAmountSummary>();

            foreach (var cs in summarieslist)
            {
                SaleAmountSummary s = new SaleAmountSummary()
                {
                    SaleAmount = (double)cs.SaleNet,
                    FormattedSaleAmount = ReportHelpers.FormatNormalNumberDigit(cs.SaleNet != null ? (double)cs.SaleNet : 0),
                    Customer = cs.Customer
                };
                saleAmountSummary.Add(s);
            }
            return saleAmountSummary.OrderByDescending(s => s.SaleAmount).Take(10).ToList();
        }
    }
}
