﻿using System;
using System.Collections.Generic;
using System.Linq;
using TOne.WhS.Analytics.Entities.BillingReport;
using TOne.WhS.BusinessEntity.Business;
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
            CarrierAccountManager carrierAccountManager = new CarrierAccountManager();
            var carrierAccounts = carrierAccountManager.GetAllCarriers();
            var exchangeAccounts = carrierAccounts.Where(it => it.AccountType == CarrierAccountType.Exchange).Select(it => it.CarrierAccountId);
            #region customer part

            List<string> listCustomerDimensions = new List<string> { parameters.GroupByProfile ? "CustomerProfile" : "Customer" };

            var analyticCustomerQuery = new AnalyticQuery
            {
                DimensionFields = listCustomerDimensions,
                MeasureFields = new List<string> { "TotalSaleNet", "TotalCostNet", "TotalProfit" },
                TableId = Guid.Parse("4C1AAA1B-675B-420F-8E60-26B0747CA79B"),
                FromTime = parameters.FromTime,
                ToTime = parameters.ToTime,
                CurrencyId = parameters.CurrencyId,
                ParentDimensions = new List<string>(),
                Filters = new List<DimensionFilter>(),
                OrderType = AnalyticQueryOrderType.ByAllDimensions
            };

            if (exchangeAccounts.Any())
            {
                DimensionFilter dimensionFilter = new DimensionFilter
                {
                    Dimension = "Customer",
                    FilterValues = exchangeAccounts.Cast<object>().ToList()
                };
                analyticCustomerQuery.Filters.Add(dimensionFilter);
            }

            #endregion
            #region supplier part
            List<string> listSupplierDimensions = new List<string> { parameters.GroupByProfile ? "SupplierProfile" : "Supplier" };

            var analyticSupplierQuery = new AnalyticQuery
            {
                DimensionFields = listSupplierDimensions,
                MeasureFields = new List<string> { "TotalSaleNet", "TotalCostNet", "TotalProfit" },
                TableId = Guid.Parse("4C1AAA1B-675B-420F-8E60-26B0747CA79B"),
                FromTime = parameters.FromTime,
                ToTime = parameters.ToTime,
                CurrencyId = parameters.CurrencyId,
                ParentDimensions = new List<string>(),
                Filters = new List<DimensionFilter>(),
                OrderType = AnalyticQueryOrderType.ByAllDimensions

            };
            if (exchangeAccounts.Any())
            {
                DimensionFilter dimensionFilter = new DimensionFilter
                {
                    Dimension = "Supplier",
                    FilterValues = exchangeAccounts.Cast<object>().ToList()
                };
                analyticSupplierQuery.Filters.Add(dimensionFilter);
            }
            #endregion
            List<ProfitByCarrier> listprofitByCarrier = new List<ProfitByCarrier>();
            Dictionary<string, ProfitByCarrier> dictionaryprofitByCustomer = new Dictionary<string, ProfitByCarrier>();

            var result = analyticManager.GetAllFilteredRecords(analyticCustomerQuery);
            if (result != null)
                foreach (var analyticRecord in result)
                {
                    ProfitByCarrier profitByCarrier = new ProfitByCarrier();
                    var customerValue = analyticRecord.DimensionValues[0];
                    if (customerValue != null)
                        profitByCarrier.Customer = customerValue.Name;

                    MeasureValue profit;
                    analyticRecord.MeasureValues.TryGetValue("TotalProfit", out profit);
                    profitByCarrier.CustomerProfit = Convert.ToDouble(profit.Value ?? 0.0);
                    profitByCarrier.FormattedCustomerProfit = ReportHelpers.FormatNormalNumberDigit(profitByCarrier.CustomerProfit);

                    profitByCarrier.TotalBase = profitByCarrier.CustomerProfit;
                    profitByCarrier.Total = ReportHelpers.FormatNormalNumberDigit(profitByCarrier.TotalBase);

                    if (!dictionaryprofitByCustomer.ContainsKey(profitByCarrier.Customer))
                        dictionaryprofitByCustomer[profitByCarrier.Customer] = profitByCarrier;
                }

            result = analyticManager.GetAllFilteredRecords(analyticSupplierQuery) ;
            if (result != null)
                foreach (var analyticRecord in result)
                {
                    ProfitByCarrier profitByCarrier = new ProfitByCarrier();
                    var customerValue = analyticRecord.DimensionValues[0];
                    if (customerValue != null)
                        profitByCarrier.Customer = customerValue.Name;

                    MeasureValue profit;
                    analyticRecord.MeasureValues.TryGetValue("TotalProfit", out profit);
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
            listprofitByCarrier = dictionaryprofitByCustomer.Values.OrderByDescending(it => it.TotalBase).ToList();
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
