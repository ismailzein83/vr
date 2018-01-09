using System;
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
                    DimensionFields = new List<string>(),
                    MeasureFields = new List<string> { "SaleDuration", "CostDuration", "SaleNetNotNULL", "CostNetNotNULL", "ProfitNotNULL" },
                    TableId = Guid.Parse("4C1AAA1B-675B-420F-8E60-26B0747CA79B"),
                    FromTime = parameters.FromTime,
                    ToTime = parameters.ToTime,
                    CurrencyId = parameters.CurrencyId,
                    ParentDimensions = new List<string>(),
                    Filters = new List<DimensionFilter>(),
                    OrderType = AnalyticQueryOrderType.ByAllDimensions
                },
                SortByColumnName = "DimensionValues[0].Name"
            };

            if (parameters.GroupByProfile)
                analyticQuery.Query.DimensionFields.Add("CustomerProfile");
            else
                analyticQuery.Query.DimensionFields.Add("Customer");

            if (parameters.GroupByProfile)
                analyticQuery.Query.DimensionFields.Add("SupplierProfile");
            else
                analyticQuery.Query.DimensionFields.Add("Supplier");

            analyticQuery.Query.DimensionFields.Add("SaleZoneName");
            analyticQuery.Query.MeasureFields.Add("SaleRate_DurAvg");
            analyticQuery.Query.MeasureFields.Add("CostRate_DurAvg");

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
                    var customer = analyticRecord.DimensionValues[0];
                    if (customer != null)
                        customerFormatted.Customer = customer.Name;
                    var supplier = analyticRecord.DimensionValues[1];
                    if (supplier != null)
                        customerFormatted.Supplier = supplier.Name;
                    var zoneValue = analyticRecord.DimensionValues[2];
                    if (zoneValue != null)
                        customerFormatted.Destination = zoneValue.Name;

                    //var saleRateValue = analyticRecord.DimensionValues[3];
                    //if (saleRateValue != null)
                    //    customerFormatted.SaleRate = ReportHelpers.FormatLongNumberDigit(Convert.ToDecimal(saleRateValue.Value ?? 0.0));

                    //var costRateValue = analyticRecord.DimensionValues[4];
                    //if (costRateValue != null)
                    //    customerFormatted.CostRate = ReportHelpers.FormatLongNumberDigit(Convert.ToDecimal(costRateValue.Value ?? 0.0));


                    MeasureValue saleRate;
                    analyticRecord.MeasureValues.TryGetValue("SaleRate_DurAvg", out saleRate);
                    decimal saleRateValue = Convert.ToDecimal(saleRate.Value ?? 0.0);
                    customerFormatted.SaleRate = ReportHelpers.FormatLongNumberDigit(saleRateValue);

                    MeasureValue costRate;
                    analyticRecord.MeasureValues.TryGetValue("CostRate_DurAvg", out costRate);
                    decimal costRateValue = Convert.ToDecimal(costRate.Value ?? 0.0);
                    customerFormatted.CostRate = ReportHelpers.FormatLongNumberDigit(costRateValue);

                    MeasureValue saleDuration;
                    analyticRecord.MeasureValues.TryGetValue("SaleDuration", out saleDuration);
                    decimal saleDurationValue = Convert.ToDecimal(saleDuration.Value ?? 0.0);
                    customerFormatted.SaleDuration = ReportHelpers.FormatNormalNumberDigit(saleDurationValue);


                    MeasureValue costDuration;
                    analyticRecord.MeasureValues.TryGetValue("CostDuration", out costDuration);
                    decimal costDurationValue = Convert.ToDecimal(costDuration.Value ?? 0.0);
                    customerFormatted.CostDuration = ReportHelpers.FormatNormalNumberDigit(costDurationValue);

                    MeasureValue profit;
                    analyticRecord.MeasureValues.TryGetValue("ProfitNotNULL", out profit);
                    customerFormatted.Profit = Convert.ToDouble(profit.Value ?? 0.0);
                    customerFormatted.ProfitFormatted = ReportHelpers.FormatNormalNumberDigit(customerFormatted.Profit);

                    MeasureValue costNet;
                    analyticRecord.MeasureValues.TryGetValue("CostNetNotNULL", out costNet);
                    customerFormatted.CostNet = Convert.ToDouble(costNet.Value ?? 0.0);
                    customerFormatted.CostNetFormatted = ReportHelpers.FormatNormalNumberDigit(customerFormatted.CostNet);

                    MeasureValue saleNet;
                    analyticRecord.MeasureValues.TryGetValue("SaleNetNotNULL", out saleNet);
                    customerFormatted.SaleNet = Convert.ToDouble(saleNet == null ? 0.0 : saleNet.Value ?? 0.0);
                    customerFormatted.SaleNetFormatted = ReportHelpers.FormatNormalNumberDigit(customerFormatted.SaleNet);

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
                {"ToDate", new RdlcParameter {Value =  parameters.ToTime.HasValue ? parameters.ToTime.ToString() : null, IsVisible = true}},
                {"Customer", new RdlcParameter {Value = ReportHelpers.GetCarrierName(parameters.CustomersId,"Customers"), IsVisible = true}},
                {"Supplier", new RdlcParameter {Value = ReportHelpers.GetCarrierName(parameters.SuppliersId,"Suppliers"), IsVisible = true}},
                {"Title", new RdlcParameter {Value = "Routing by Customer", IsVisible = true}},
                {"Currency", new RdlcParameter {Value = parameters.CurrencyDescription, IsVisible = true}},
                {"LogoPath", new RdlcParameter {Value = "logo", IsVisible = true}},
                {"Digit", new RdlcParameter {Value = ReportHelpers.GetNormalNumberDigit(), IsVisible = true}}
            };

            return list;
        }
    }
}
