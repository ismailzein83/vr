using System;
using System.Collections.Generic;
using System.Linq;
using TOne.WhS.Analytics.Entities.BillingReport;
using Vanrise.Analytic.Business;
using Vanrise.Analytic.Entities;
using Vanrise.Entities;

namespace TOne.WhS.Analytics.Business.BillingReports
{
    public class LossesByZonesReportGenerator : IReportGenerator
    {
        public Dictionary<string, System.Collections.IEnumerable> GenerateDataSources(ReportParameters parameters)
        {
            AnalyticManager analyticManager = new AnalyticManager();
            List<string> listGrouping = new List<string>();
            List<string> listMeasures = new List<string>();




            listGrouping.Add("SupplierZone");
            listGrouping.Add("SaleZone");
            listGrouping.Add("Supplier");
            listGrouping.Add("Customer");



            listMeasures.Add("SaleRate_DurAvg");
            listMeasures.Add("CostRate_DurAvg");
            listMeasures.Add("SaleDuration");
            listMeasures.Add("CostDuration");
            listMeasures.Add("TotalSaleNet");
            listMeasures.Add("TotalCostNet");
            listMeasures.Add("PercentageLoss");


            Vanrise.Entities.DataRetrievalInput<AnalyticQuery> analyticQuery = new DataRetrievalInput<AnalyticQuery>()
            {
                Query = new AnalyticQuery()
                {
                    DimensionFields = listGrouping,
                    MeasureFields = listMeasures,
                    TableId = Guid.Parse("4C1AAA1B-675B-420F-8E60-26B0747CA79B"),
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

            if (!String.IsNullOrEmpty(parameters.ZonesId))
            {
                DimensionFilter dimensionFilter = new DimensionFilter()
                {
                    Dimension = "SaleZone",
                    FilterValues = parameters.ZonesId.Split(',').ToList().Cast<object>().ToList()
                };
                analyticQuery.Query.Filters.Add(dimensionFilter);
            }


            List<LossesByZonesFormatted> listRateLossFromatted = new List<LossesByZonesFormatted>();

            var result = analyticManager.GetFilteredRecords(analyticQuery) as AnalyticSummaryBigResult<AnalyticRecord>;
            if (result != null)
                foreach (var analyticRecord in result.Data)
                {
                    LossesByZonesFormatted rateLossFromatted = new LossesByZonesFormatted();



                    MeasureValue saleRate;
                    analyticRecord.MeasureValues.TryGetValue("SaleRate_DurAvg", out saleRate);
                    rateLossFromatted.SaleRate = Convert.ToDecimal(saleRate.Value ?? 0.0);
                    rateLossFromatted.SaleRateFormatted = ReportHelpers.FormatLongNumberDigit(rateLossFromatted.SaleRate);

                    MeasureValue costRate;
                    analyticRecord.MeasureValues.TryGetValue("CostRate_DurAvg", out costRate);
                    rateLossFromatted.CostRate = Convert.ToDecimal(costRate.Value ?? 0.0);
                    rateLossFromatted.CostRateFormatted = ReportHelpers.FormatLongNumberDigit(rateLossFromatted.CostRate);

                    MeasureValue saleDuration;
                    analyticRecord.MeasureValues.TryGetValue("SaleDuration", out saleDuration);
                    rateLossFromatted.SaleDuration = Convert.ToDecimal(saleDuration.Value ?? 0.0);
                    rateLossFromatted.SaleDurationFormatted = ReportHelpers.FormatNormalNumberDigit(rateLossFromatted.SaleDuration);

                    MeasureValue costDuration;
                    analyticRecord.MeasureValues.TryGetValue("CostDuration", out costDuration);
                    rateLossFromatted.CostDuration = Convert.ToDecimal(costDuration.Value ?? 0.0);
                    rateLossFromatted.CostDurationFormatted = ReportHelpers.FormatNormalNumberDigit(rateLossFromatted.CostDuration);

                    MeasureValue saleNet;
                    analyticRecord.MeasureValues.TryGetValue("TotalSaleNet", out saleNet);
                    rateLossFromatted.SaleNet = Convert.ToDecimal(saleNet.Value ?? 0.0);
                    rateLossFromatted.SaleNetFormatted = ReportHelpers.FormatNormalNumberDigit(rateLossFromatted.SaleNet);

                    MeasureValue costNet;
                    analyticRecord.MeasureValues.TryGetValue("TotalCostNet", out costNet);
                    rateLossFromatted.CostNet = Convert.ToDecimal(costNet.Value ?? 0.0);
                    rateLossFromatted.CostNetFormatted = ReportHelpers.FormatNormalNumberDigit(rateLossFromatted.CostNet);

                    var loss = rateLossFromatted.CostNet - rateLossFromatted.SaleNet;
                    if (loss <= 0) continue;

                    var supplierZoneValue = analyticRecord.DimensionValues[0];
                    if (supplierZoneValue != null)
                        rateLossFromatted.CostZone = supplierZoneValue.Name;

                    var customerZoneValue = analyticRecord.DimensionValues[1];
                    if (customerZoneValue != null)
                        rateLossFromatted.SaleZone = customerZoneValue.Name;


                    var supplierValue = analyticRecord.DimensionValues[2];
                    if (supplierValue != null)
                        rateLossFromatted.Supplier = supplierValue.Name;

                    var customerValue = analyticRecord.DimensionValues[3];
                    if (customerValue != null)
                        rateLossFromatted.Customer = customerValue.Name;



                    rateLossFromatted.Loss = (decimal)(rateLossFromatted.CostNet - rateLossFromatted.SaleNet);

                    rateLossFromatted.LossFormatted = ReportHelpers.FormatLongNumberDigit(System.Math.Abs((double)(rateLossFromatted.CostNet - rateLossFromatted.SaleNet)));

                    MeasureValue percentageLoss;
                    analyticRecord.MeasureValues.TryGetValue("PercentageLoss", out percentageLoss);

                    rateLossFromatted.LossPerFormatted = ReportHelpers.FormatNumberPercentage(System.Math.Abs((decimal)(rateLossFromatted.CostNet - rateLossFromatted.SaleNet)) / rateLossFromatted.CostNet);

                    listRateLossFromatted.Add(rateLossFromatted);
                }

            Dictionary<string, System.Collections.IEnumerable> dataSources =
                new Dictionary<string, System.Collections.IEnumerable> { { "RateLoss", listRateLossFromatted } };
            return dataSources;
        }

        public Dictionary<string, RdlcParameter> GetRdlcReportParameters(ReportParameters parameters)
        {
            Dictionary<string, RdlcParameter> list = new Dictionary<string, RdlcParameter>
            {
                {"FromDate", new RdlcParameter {Value = parameters.FromTime.ToString(), IsVisible = true}},
                {
                    "ToDate",
                    new RdlcParameter
                    {
                        Value = (parameters.ToTime.HasValue) ? parameters.ToTime.ToString() : null,
                        IsVisible = true
                    }
                },
                {
                    "Customer",
                    new RdlcParameter
                    {
                        Value = ReportHelpers.GetCarrierName(parameters.CustomersId, "Customers"),
                        IsVisible = true
                    }
                },
                {
                    "Supplier",
                    new RdlcParameter
                    {
                        Value = ReportHelpers.GetCarrierName(parameters.SuppliersId, "Suppliers"),
                        IsVisible = true
                    }
                },
                {"Title", new RdlcParameter {Value = "Losses by Zones", IsVisible = true}},
                {"Currency", new RdlcParameter {Value = parameters.CurrencyDescription, IsVisible = true}},
                {"LogoPath", new RdlcParameter {Value = "logo", IsVisible = true}},
                {"DigitRate", new RdlcParameter {Value = ReportHelpers.GetLongNumberDigit(), IsVisible = true}},
                {"Digit", new RdlcParameter {Value = ReportHelpers.GetNormalNumberDigit(), IsVisible = true}}
            };



            return list;
        }
    }
}
