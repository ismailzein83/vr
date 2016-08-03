using System;
using System.Collections.Generic;
using System.Linq;
using TOne.WhS.Analytics.Entities.BillingReport;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Analytic.Business;
using Vanrise.Analytic.Entities;
using Vanrise.Entities;

namespace TOne.WhS.Analytics.Business.BillingReports
{
    public class ProfitByCarrierReportGenerator : IReportGenerator
    {
        public Dictionary<string, System.Collections.IEnumerable> GenerateDataSources(ReportParameters parameters)
        {
            AnalyticManager analyticManager = new AnalyticManager();

            #region customer part

            List<string> listCustomerDimensions = new List<string> { parameters.GroupByProfile ? "CustomerProfile" : "Customer" };

            DataRetrievalInput<AnalyticQuery> analyticCustomerQuery = new DataRetrievalInput<AnalyticQuery>
            {
                Query = new AnalyticQuery
                {
                    DimensionFields = listCustomerDimensions,
                    MeasureFields = new List<string> { "SaleNet", "CostNet", "Profit" },
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
                analyticCustomerQuery.Query.Filters.Add(dimensionFilter);
            }

            if (!String.IsNullOrEmpty(parameters.SuppliersId))
            {
                DimensionFilter dimensionFilter = new DimensionFilter
                {
                    Dimension = "Supplier",
                    FilterValues = parameters.SuppliersId.Split(',').ToList().Cast<object>().ToList()
                };
                analyticCustomerQuery.Query.Filters.Add(dimensionFilter);
            }
            #endregion
            #region supplier part
            List<string> listSupplierDimensions = new List<string> { parameters.GroupByProfile ? "SupplierProfile" : "Supplier" };

            DataRetrievalInput<AnalyticQuery> analyticSupplierQuery = new DataRetrievalInput<AnalyticQuery>
            {
                Query = new AnalyticQuery
                {
                    DimensionFields = listSupplierDimensions,
                    MeasureFields = new List<string> { "SaleNet", "CostNet", "Profit" },
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
                analyticSupplierQuery.Query.Filters.Add(dimensionFilter);
            }

            if (!String.IsNullOrEmpty(parameters.SuppliersId))
            {
                DimensionFilter dimensionFilter = new DimensionFilter
                {
                    Dimension = "Supplier",
                    FilterValues = parameters.SuppliersId.Split(',').ToList().Cast<object>().ToList()
                };
                analyticSupplierQuery.Query.Filters.Add(dimensionFilter);
            }
            #endregion
            List<ProfitByCarrier> listprofitByCarrier = new List<ProfitByCarrier>();
            Dictionary<string, ProfitByCarrier> dictionaryprofitByCustomer = new Dictionary<string, ProfitByCarrier>();

            var result = analyticManager.GetFilteredRecords(analyticCustomerQuery) as AnalyticSummaryBigResult<AnalyticRecord>;
            if (result != null)
                foreach (var analyticRecord in result.Data)
                {
                    ProfitByCarrier profitByCarrier = new ProfitByCarrier();
                    var customerValue = analyticRecord.DimensionValues[0];
                    if (customerValue != null)
                        profitByCarrier.Customer = customerValue.Name;

                    MeasureValue profit;
                    analyticRecord.MeasureValues.TryGetValue("Profit", out profit);
                    profitByCarrier.CustomerProfit = Convert.ToDouble(profit.Value ?? 0.0);
                    profitByCarrier.FormattedCustomerProfit = ReportHelpers.FormatNormalNumberDigit(profitByCarrier.CustomerProfit);

                    profitByCarrier.TotalBase = profitByCarrier.CustomerProfit;
                    profitByCarrier.Total = ReportHelpers.FormatNormalNumberDigit(profitByCarrier.TotalBase);

                    if (!dictionaryprofitByCustomer.ContainsKey(profitByCarrier.Customer))
                        dictionaryprofitByCustomer[profitByCarrier.Customer] = profitByCarrier;
                }

            result = analyticManager.GetFilteredRecords(analyticSupplierQuery) as AnalyticSummaryBigResult<AnalyticRecord>;
            if (result != null)
                foreach (var analyticRecord in result.Data)
                {
                    ProfitByCarrier profitByCarrier = new ProfitByCarrier();
                    var customerValue = analyticRecord.DimensionValues[0];
                    if (customerValue != null)
                        profitByCarrier.Customer = customerValue.Name;

                    MeasureValue profit;
                    analyticRecord.MeasureValues.TryGetValue("Profit", out profit);
                    profitByCarrier.SupplierProfit = Convert.ToDouble(profit.Value ?? 0.0);
                    profitByCarrier.FormattedSupplierProfit = ReportHelpers.FormatNormalNumberDigit(profitByCarrier.SupplierProfit);
                    if (dictionaryprofitByCustomer.ContainsKey(profitByCarrier.Customer))
                    {
                        var profitBycust = dictionaryprofitByCustomer[profitByCarrier.Customer];
                        profitBycust.SupplierProfit = profitByCarrier.SupplierProfit;
                        profitBycust.FormattedSupplierProfit = profitByCarrier.FormattedSupplierProfit;
                        profitBycust.TotalBase += profitByCarrier.SupplierProfit;
                        profitBycust.Total = ReportHelpers.FormatNormalNumberDigit(profitBycust.TotalBase);
                    }
                    else
                    {
                        dictionaryprofitByCustomer[profitByCarrier.Customer] = profitByCarrier;
                        profitByCarrier.TotalBase = profitByCarrier.SupplierProfit;
                        profitByCarrier.Total = ReportHelpers.FormatNormalNumberDigit(profitByCarrier.TotalBase);
                    }
                }

            listprofitByCarrier = dictionaryprofitByCustomer.Values.ToList();
            Dictionary<string, System.Collections.IEnumerable> dataSources =
                new Dictionary<string, System.Collections.IEnumerable>
                {
                    {"ExchangeCarrier", listprofitByCarrier},
                    {"ExchangeChart", listprofitByCarrier.Take(10).ToList()}
                };
            return dataSources;
        }

        public Dictionary<string, RdlcParameter> GetRdlcReportParameters(ReportParameters parameters)
        {
            Dictionary<string, RdlcParameter> list = new Dictionary<string, RdlcParameter>
            {
                {"FromDate", new RdlcParameter {Value = parameters.FromTime.ToString(), IsVisible = true}},
                {"ToDate", new RdlcParameter { Value = parameters.ToTime.HasValue ?parameters.ToTime.ToString():null, IsVisible = true }},
                {"Title", new RdlcParameter {Value =parameters.GroupByProfile ? "Carrier Profile Profit": "Profit by Carrier", IsVisible = true}},
                {"Currency", new RdlcParameter {Value = parameters.CurrencyDescription, IsVisible = true}},
                {"LogoPath", new RdlcParameter {Value = "logo", IsVisible = true}},
                {"DigitRate", new RdlcParameter {Value = ReportHelpers.GetLongNumberDigit(), IsVisible = true}},
                {"Digit", new RdlcParameter {Value = ReportHelpers.GetNormalNumberDigit(), IsVisible = true}}

            };

            return list;
        }
    }
}
