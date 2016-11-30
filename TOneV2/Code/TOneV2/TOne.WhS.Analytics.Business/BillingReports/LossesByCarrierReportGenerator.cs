using System;
using System.Collections.Generic;
using System.Linq;
using TOne.WhS.Analytics.Entities.BillingReport;
using Vanrise.Analytic.Business;
using Vanrise.Analytic.Entities;
using Vanrise.Entities;

namespace TOne.WhS.Analytics.Business.BillingReports
{
    public class LossesByCarrierReportGenerator : IReportGenerator
    {
        public Dictionary<string, System.Collections.IEnumerable> GenerateDataSources(ReportParameters parameters)
        {
            AnalyticManager analyticManager = new AnalyticManager();

            Vanrise.Entities.DataRetrievalInput<AnalyticQuery> analyticQuery = new DataRetrievalInput<AnalyticQuery>()
            {
                Query = new AnalyticQuery()
                {
                    DimensionFields = new List<string> { "Supplier", "SaleZone", "SupplierZone", "Customer" },
                    MeasureFields = new List<string>() { "SaleNetNotNULL", "CostNetNotNULL", "DurationNet" },
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

            List<LossesByCarrier> listLossesByCarrier = new List<LossesByCarrier>();

            var result = analyticManager.GetFilteredRecords(analyticQuery) as AnalyticSummaryBigResult<AnalyticRecord>;

            if (result != null)
                foreach (var analyticRecord in result.Data)
                {

                    LossesByCarrier lossesByCarrier = new LossesByCarrier();

                    var supplierValue = analyticRecord.DimensionValues[0];
                    if (supplierValue != null)
                        lossesByCarrier.SupplierName = supplierValue.Name;

                    var saleZoneValue = analyticRecord.DimensionValues[1];
                    if (saleZoneValue != null)
                        lossesByCarrier.SaleZoneName = saleZoneValue.Name;

                    var zoneProfitValue = analyticRecord.DimensionValues[2];
                    if (zoneProfitValue != null)
                        lossesByCarrier.CostZoneName = zoneProfitValue.Name;

                    var customerValue = analyticRecord.DimensionValues[3];
                    if (customerValue != null)
                        lossesByCarrier.CustomerName = customerValue.Name;

                    MeasureValue saleNet;
                    analyticRecord.MeasureValues.TryGetValue("SaleNetNotNULL", out saleNet);

                    lossesByCarrier.SaleNet = Convert.ToDouble(saleNet == null ? 0.0 : saleNet.Value ?? 0.0);
                    lossesByCarrier.SaleNetFormatted = lossesByCarrier.SaleNet == 0 ? "" :
                        ReportHelpers.FormatNormalNumberDigit(lossesByCarrier.SaleNet);

                    MeasureValue costNet;
                    analyticRecord.MeasureValues.TryGetValue("CostNetNotNULL", out costNet);
                    lossesByCarrier.CostNet = Convert.ToDouble(costNet == null ? 0.0 : costNet.Value ?? 0.0);
                    lossesByCarrier.CostNetFormatted = lossesByCarrier.CostNet == 0 ? "" :
                        ReportHelpers.FormatNormalNumberDigit(lossesByCarrier.CostNet);

                    MeasureValue DurationNet;
                    analyticRecord.MeasureValues.TryGetValue("DurationNet", out DurationNet);
                    lossesByCarrier.Duration = Convert.ToDecimal(DurationNet.Value ?? 0.0);
                    lossesByCarrier.DurationFormatted = lossesByCarrier.Duration == 0 ? "" :
                        ReportHelpers.FormatNormalNumberDigit(lossesByCarrier.Duration);

                    lossesByCarrier.Margin =
                        ReportHelpers.FormatNormalNumberDigit(lossesByCarrier.SaleNet - lossesByCarrier.CostNet);

                    lossesByCarrier.Percentage = (lossesByCarrier.SaleNet != 0.0) ? ReportHelpers.FormatNumberPercentage(1 - lossesByCarrier.CostNet / lossesByCarrier.SaleNet) : "0%";
                    if ((lossesByCarrier.SaleNet - lossesByCarrier.CostNet) / lossesByCarrier.SaleNet * 100 < parameters.Margin)
                        listLossesByCarrier.Add(lossesByCarrier);
                }

            Dictionary<string, System.Collections.IEnumerable> dataSources = new Dictionary<string, System.Collections.IEnumerable>();
            dataSources.Add("CarrierLost", listLossesByCarrier);
            return dataSources;
        }

        public Dictionary<string, RdlcParameter> GetRdlcReportParameters(ReportParameters parameters)
        {
            Dictionary<string, RdlcParameter> list = new Dictionary<string, RdlcParameter>();

            list.Add("FromDate", new RdlcParameter { Value = parameters.FromTime.ToString(), IsVisible = true });
            list.Add("ToDate", new RdlcParameter { Value = (parameters.ToTime.HasValue) ? parameters.ToTime.ToString() : null, IsVisible = true });
            list.Add("Customer", new RdlcParameter { Value = ReportHelpers.GetCarrierName(parameters.CustomersId, "Customers"), IsVisible = true });
            list.Add("Supplier", new RdlcParameter { Value = ReportHelpers.GetCarrierName(parameters.SuppliersId, "Suppliers"), IsVisible = true });
            list.Add("Title", new RdlcParameter { Value = "Losses by Carrier", IsVisible = true });
            list.Add("Currency", new RdlcParameter { Value = parameters.CurrencyDescription, IsVisible = true });
            list.Add("LogoPath", new RdlcParameter { Value = "logo", IsVisible = true });
            list.Add("DigitRate", new RdlcParameter { Value = ReportHelpers.GetNormalNumberDigit(), IsVisible = true });
            list.Add("Digit", new RdlcParameter { Value = ReportHelpers.GetNormalNumberDigit(), IsVisible = true });

            list.Add("PageBreak", new RdlcParameter { Value = parameters.PageBreak.ToString(), IsVisible = true });

            return list;
        }
    }
}
