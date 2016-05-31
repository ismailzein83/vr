using System;
using System.Collections.Generic;
using System.Linq;
using TOne.WhS.Analytics.Entities.BillingReport;
using Vanrise.Analytic.Business;
using Vanrise.Analytic.Entities;
using Vanrise.Entities;

namespace TOne.WhS.Analytics.Business.BillingReports
{
    public class ZoneProfitReportGenerator : IReportGenerator
    {
        public Dictionary<string, System.Collections.IEnumerable> GenerateDataSources(ReportParameters parameters)
        {
            AnalyticManager analyticManager = new AnalyticManager();
            BillingStatisticManager manager = new BillingStatisticManager();

            Vanrise.Entities.DataRetrievalInput<AnalyticQuery> analyticQuery = new DataRetrievalInput<AnalyticQuery>()
            {
                Query = new AnalyticQuery()
                {
                    DimensionFields = new List<string> { "Supplier", "SaleZone", "SupplierZone" },
                    MeasureFields = new List<string>() { "SaleNet", "CostNet", "SaleDuration", "CostDuration", "DurationInMinutes", "Attempts" },
                    TableId = 8,
                    FromTime = parameters.FromTime,
                    ToTime = parameters.ToTime,
                    CurrencyId = parameters.CurrencyId,
                    ParentDimensions = new List<string>(),
                    Filters = new List<DimensionFilter>()
                },
                SortByColumnName = "DimensionValues[0].Name"
            };

            if (parameters.GroupByCustomer)
                analyticQuery.Query.DimensionFields.Add("Customer");

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
            
            List<ZoneProfitFormatted> listZoneProfit = new List<ZoneProfitFormatted>();
            
            var result = analyticManager.GetFilteredRecords(analyticQuery) as AnalyticSummaryBigResult<AnalyticRecord>;
            foreach (var analyticRecord in result.Data)
            {
                ZoneProfitFormatted zoneProfit = new ZoneProfitFormatted();

                var supplierValue = analyticRecord.DimensionValues[0];
                if (supplierValue != null)
                    zoneProfit.SupplierID = supplierValue.Name;

                var saleZoneValue = analyticRecord.DimensionValues[1];
                if (saleZoneValue != null)
                    zoneProfit.SaleZone = saleZoneValue.Name;

                var zoneProfitValue = analyticRecord.DimensionValues[2];
                if (zoneProfitValue != null)
                    zoneProfit.CostZone = zoneProfitValue.Name;
                
                if (parameters.GroupByCustomer)
                {
                    var customerValue = analyticRecord.DimensionValues[3];
                    if (customerValue != null)
                        zoneProfit.CustomerID = customerValue.Name;
                }

                MeasureValue saleNet;
                analyticRecord.MeasureValues.TryGetValue("SaleNet", out saleNet);

                zoneProfit.SaleNet = Convert.ToDouble(saleNet.Value ?? 0.0);
                zoneProfit.SaleNetFormated = zoneProfit.SaleNet == 0 ? "" : (zoneProfit.SaleNet.HasValue) ? 
                    manager.FormatNumberDigitRate(zoneProfit.SaleNet) : "0.00";

                MeasureValue costNet;
                analyticRecord.MeasureValues.TryGetValue("CostNet", out costNet);
                zoneProfit.CostNet = Convert.ToDouble(costNet.Value ?? 0.0);
                zoneProfit.CostNetFormated = (zoneProfit.CostNet.HasValue)
                    ? manager.FormatNumberDigitRate(zoneProfit.CostNet)
                    : "0.00";

                MeasureValue saleDuration;
                analyticRecord.MeasureValues.TryGetValue("SaleDuration", out saleDuration);
                zoneProfit.SaleDuration = Convert.ToDecimal(saleDuration.Value ?? 0.0);
                zoneProfit.SaleDurationFormated = zoneProfit.SaleNet == 0 ? "" : (zoneProfit.SaleDuration.HasValue) ? 
                    manager.FormatNumber(zoneProfit.SaleDuration) : "0.00";

                MeasureValue costDuration;
                analyticRecord.MeasureValues.TryGetValue("CostDuration", out costDuration);
                zoneProfit.CostDuration = Convert.ToDecimal(costDuration.Value ?? 0.0);
                zoneProfit.CostDurationFormated = manager.FormatNumberDigitRate(zoneProfit.CostDuration);

                MeasureValue durationInMinutes;
                analyticRecord.MeasureValues.TryGetValue("DurationInMinutes", out durationInMinutes);
                zoneProfit.DurationNet = Convert.ToDecimal(durationInMinutes.Value ?? 0.0);
                zoneProfit.DurationNetFormated = manager.FormatNumber(zoneProfit.DurationNet);

                MeasureValue calls;
                analyticRecord.MeasureValues.TryGetValue("Attempts", out calls);
                zoneProfit.Calls = Convert.ToInt32(calls.Value ?? 0.0);

                zoneProfit.Profit = zoneProfit.SaleNet == 0
                    ? ""
                    : manager.FormatNumber((!zoneProfit.SaleNet.HasValue) ? 0 : zoneProfit.SaleNet - zoneProfit.CostNet);
                zoneProfit.ProfitSum = (!zoneProfit.SaleNet.HasValue || zoneProfit.SaleNet == 0)
                    ? 0
                    : zoneProfit.SaleNet - zoneProfit.CostNet;
                zoneProfit.ProfitPercentage = zoneProfit.SaleNet == 0
                    ? ""
                    : (zoneProfit.SaleNet.HasValue)
                        ? manager.FormatNumber(((1 - zoneProfit.CostNet / zoneProfit.SaleNet)) * 100)
                        : "-100%";

                listZoneProfit.Add(zoneProfit);
            }
            
            Dictionary<string, System.Collections.IEnumerable> dataSources = new Dictionary<string, System.Collections.IEnumerable>();
            dataSources.Add("ZoneProfit", listZoneProfit);
            return dataSources;
        }

        public Dictionary<string, RdlcParameter> GetRdlcReportParameters(ReportParameters parameters)
        {
            Dictionary<string, RdlcParameter> list = new Dictionary<string, RdlcParameter>();
            list.Add("GroupByCustomer", new RdlcParameter() { Value = parameters.GroupByCustomer.ToString(), IsVisible = false });
            list.Add("FromDate", new RdlcParameter { Value = parameters.FromTime.ToString(), IsVisible = true });
            list.Add("ToDate", new RdlcParameter { Value = parameters.ToTime.ToString(), IsVisible = true });
            list.Add("Title", new RdlcParameter { Value = "Zone Profit", IsVisible = true });
            list.Add("Currency", new RdlcParameter { Value = parameters.CurrencyDescription, IsVisible = true });
            list.Add("LogoPath", new RdlcParameter { Value = "logo", IsVisible = true });
            list.Add("Customer", new RdlcParameter { Value = ReportHelpers.GetCarrierName(parameters.CustomersId, "Customers"), IsVisible = true });
            list.Add("Supplier", new RdlcParameter { Value = ReportHelpers.GetCarrierName(parameters.SuppliersId, "Suppliers"), IsVisible = true });
            list.Add("DigitRate", new RdlcParameter { Value = "4", IsVisible = true });

            return list;
        }
    }
}
