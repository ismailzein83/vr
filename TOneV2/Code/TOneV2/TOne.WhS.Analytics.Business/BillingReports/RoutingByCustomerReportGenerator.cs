﻿using System;
using System.Collections.Generic;
using System.Linq;
using TOne.WhS.Analytics.Entities.BillingReport;
using TOne.WhS.Analytics.Entities.BillingReport.RoutingByCustomer;
using Vanrise.Analytic.Business;
using Vanrise.Analytic.Entities;
using Vanrise.Entities;

namespace TOne.WhS.Analytics.Business.BillingReports
{
    public class RoutingByCustomerReportGenerator : IReportGenerator
    {
        public Dictionary<string, System.Collections.IEnumerable> GenerateDataSources(ReportParameters parameters)
        {
            AnalyticManager analyticManager = new AnalyticManager();
            DataRetrievalInput<AnalyticQuery> analyticQuery = new DataRetrievalInput<AnalyticQuery>()
            {
                Query = new AnalyticQuery()
                {
                    DimensionFields = new List<string> { "Day", "Customer", "Supplier", "SaleZone", "SupplierZone", "CostRate", "SaleRate" },
                    MeasureFields = new List<string> { "SaleDuration", "CostDuration", "SaleNet", "CostNet", "Profit" },
                    TableId = 8,
                    FromTime = parameters.FromTime,
                    ToTime = parameters.ToTime,
                    CurrencyId = parameters.CurrencyId,
                    ParentDimensions = new List<string>(),
                    Filters = new List<DimensionFilter>(),
                    OrderType = AnalyticQueryOrderType.ByAllDimensions
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
            List<RoutingByCustomerFormatted> listByCustomerFormatteds = new List<RoutingByCustomerFormatted>();

            var result = analyticManager.GetFilteredRecords(analyticQuery) as AnalyticSummaryBigResult<AnalyticRecord>;
            if (result != null)
                foreach (var analyticRecord in result.Data)
                {
                    RoutingByCustomerFormatted customerFormatted = new RoutingByCustomerFormatted();
                    var customer = analyticRecord.DimensionValues[1];
                    if (customer != null)
                        customerFormatted.Customer = customer.Name;
                    var supplier = analyticRecord.DimensionValues[2];
                    if (supplier != null)
                        customerFormatted.Supplier = supplier.Name;
                    var zoneValue = analyticRecord.DimensionValues[3];
                    if (zoneValue != null)
                        customerFormatted.Destination = zoneValue.Name;
                    var saleRateValue = analyticRecord.DimensionValues[5];
                    if (saleRateValue != null)
                        customerFormatted.SaleRate = ReportHelpers.FormatNumberDigitRate(Convert.ToDecimal(saleRateValue.Value ?? 0.0));

                    var costRateValue = analyticRecord.DimensionValues[6];
                    if (costRateValue != null)
                        customerFormatted.CostRate = ReportHelpers.FormatNumberDigitRate(Convert.ToDecimal(costRateValue.Value ?? 0.0));

                    MeasureValue saleDuration;
                    analyticRecord.MeasureValues.TryGetValue("SaleDuration", out saleDuration);
                    decimal saleDurationValue = Convert.ToDecimal(saleDuration.Value ?? 0.0);
                    customerFormatted.SaleDuration = ReportHelpers.FormatNumber(saleDurationValue);


                    MeasureValue costDuration;
                    analyticRecord.MeasureValues.TryGetValue("CostDuration", out costDuration);
                    decimal costDurationValue = Convert.ToDecimal(costDuration.Value ?? 0.0);
                    customerFormatted.CostDuration = ReportHelpers.FormatNumber(costDurationValue);

                    MeasureValue profit;
                    analyticRecord.MeasureValues.TryGetValue("Profit", out profit);
                    customerFormatted.Profit = Convert.ToDouble(profit.Value ?? 0.0);
                    customerFormatted.ProfitFormatted = ReportHelpers.FormatNumber(customerFormatted.Profit);

                    MeasureValue costNet;
                    analyticRecord.MeasureValues.TryGetValue("CostNet", out costNet);
                    customerFormatted.CostNet = Convert.ToDouble(costNet.Value ?? 0.0);

                    MeasureValue saleNet;
                    analyticRecord.MeasureValues.TryGetValue("SaleNet", out saleNet);
                    customerFormatted.SaleNet = Convert.ToDouble(saleNet == null ? 0.0 : saleNet.Value ?? 0.0);

                    customerFormatted.ProfitPerc = customerFormatted.SaleNet == 0 ? "" : (customerFormatted.SaleNet != 0) ? ReportHelpers.FormatNumberPercentage(((1 - customerFormatted.CostNet / customerFormatted.SaleNet))) : "-100%";

                    listByCustomerFormatteds.Add(customerFormatted);
                }

            Dictionary<string, System.Collections.IEnumerable> dataSources =
                new Dictionary<string, System.Collections.IEnumerable> { { "CustomerRouting", listByCustomerFormatteds } };
            return dataSources;
        }

        public Dictionary<string, RdlcParameter> GetRdlcReportParameters(ReportParameters parameters)
        {
            Dictionary<string, RdlcParameter> list = new Dictionary<string, RdlcParameter>
            {
                {"FromDate", new RdlcParameter {Value = parameters.FromTime.ToString(), IsVisible = true}},
                {"ToDate", new RdlcParameter {Value = parameters.ToTime.ToString(), IsVisible = true}},
                {"Customer", new RdlcParameter {Value = "", IsVisible = true}},
                {"Supplier", new RdlcParameter {Value = "", IsVisible = true}},
                {"Title", new RdlcParameter {Value = "Routing by Customer Report", IsVisible = true}},
                {"Currency", new RdlcParameter {Value = parameters.CurrencyDescription, IsVisible = true}},
                {"LogoPath", new RdlcParameter {Value = "logo", IsVisible = true}},
                {"DigitRate", new RdlcParameter {Value = "2", IsVisible = true}}
            };

            return list;
        }
    }
}
