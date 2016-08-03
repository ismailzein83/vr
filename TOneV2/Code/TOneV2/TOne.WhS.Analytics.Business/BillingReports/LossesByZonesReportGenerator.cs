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



            listMeasures.Add("SaleRate");
            listMeasures.Add("CostRate");
            listMeasures.Add("SaleDuration");
            listMeasures.Add("CostDuration");
            listMeasures.Add("SaleNet");
            listMeasures.Add("CostNet");
            listMeasures.Add("PercentageLoss");


            Vanrise.Entities.DataRetrievalInput<AnalyticQuery> analyticQuery = new DataRetrievalInput<AnalyticQuery>()
            {
                Query = new AnalyticQuery()
                {
                    DimensionFields = listGrouping,
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





                    MeasureValue saleRate;
                    analyticRecord.MeasureValues.TryGetValue("SaleRate", out saleRate);
                    rateLossFromatted.SaleRate = Convert.ToDouble(saleRate.Value ?? 0.0);
                    rateLossFromatted.SaleRateFormatted = ReportHelpers.FormatLongNumberDigitRate(rateLossFromatted.SaleRate);


                    MeasureValue costRate;
                    analyticRecord.MeasureValues.TryGetValue("CostRate", out costRate);
                    rateLossFromatted.CostRate = Convert.ToDouble(costRate.Value ?? 0.0);
                    rateLossFromatted.CostRateFormatted = ReportHelpers.FormatLongNumberDigitRate(rateLossFromatted.CostRate);


                    MeasureValue saleDuration;
                    analyticRecord.MeasureValues.TryGetValue("SaleDuration", out saleDuration);
                    rateLossFromatted.SaleDuration = Convert.ToDecimal(saleDuration.Value ?? 0.0);
                    rateLossFromatted.SaleDurationFormatted = ReportHelpers.FormatNormalNumberDigitRate(rateLossFromatted.SaleDuration);


                    MeasureValue costDuration;
                    analyticRecord.MeasureValues.TryGetValue("CostDuration", out costDuration);
                    rateLossFromatted.CostDuration = Convert.ToDecimal(costDuration.Value ?? 0.0);
                    rateLossFromatted.CostDurationFormatted = ReportHelpers.FormatNormalNumberDigitRate(rateLossFromatted.CostDuration);



                    MeasureValue saleNet;
                    analyticRecord.MeasureValues.TryGetValue("SaleNet", out saleNet);
                    rateLossFromatted.SaleNet = Convert.ToDouble(saleNet.Value ?? 0.0);
                    rateLossFromatted.SaleNetFormatted = ReportHelpers.FormatNormalNumberDigitRate(rateLossFromatted.SaleNet);


                    MeasureValue costNet;
                    analyticRecord.MeasureValues.TryGetValue("CostNet", out costNet);
                    rateLossFromatted.CostNet = Convert.ToDouble(costNet.Value ?? 0.0);
                    rateLossFromatted.CostNetFormatted = ReportHelpers.FormatNormalNumberDigitRate(rateLossFromatted.CostNet);

                    MeasureValue percentageLoss;
                    analyticRecord.MeasureValues.TryGetValue("PercentageLoss", out percentageLoss);
                    rateLossFromatted.Loss = Convert.ToDouble(percentageLoss.Value ?? 0.0);
                    rateLossFromatted.LossFormatted = ReportHelpers.FormatNormalNumberDigitRate(rateLossFromatted.Loss);
                    rateLossFromatted.LossPerFormatted = ReportHelpers.FormatNumberPercentage(rateLossFromatted.Loss);

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
                {"DigitRate", new RdlcParameter {Value = "2", IsVisible = true}}
            };



            return list;
        }
    }
}
