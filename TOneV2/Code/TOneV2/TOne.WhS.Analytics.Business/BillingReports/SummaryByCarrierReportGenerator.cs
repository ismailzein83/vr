using System;
using System.Collections.Generic;
using System.Linq;
using TOne.WhS.Analytics.Entities.BillingReport;
using Vanrise.Analytic.Business;
using Vanrise.Analytic.Entities;
using Vanrise.Entities;

namespace TOne.WhS.Analytics.Business.BillingReports
{
    public class SummaryByCarrierReportGenerator : IReportGenerator
    {
        public Dictionary<string, System.Collections.IEnumerable> GenerateDataSources(ReportParameters parameters)
        {
            AnalyticManager analyticManager = new AnalyticManager();
            List<string> listGrouping = new List<string>();
            List<string> listMeasures = new List<string>();
            listMeasures.Add("NumberOfCalls");
            if (parameters.IsCost)
            {

                listGrouping.Add("Supplier");
                listMeasures.Add("CostDuration");
                listMeasures.Add("CostNetNotNULL");

            }

            else
            {
                listGrouping.Add("Customer");
                listMeasures.Add("SaleDuration");
                listMeasures.Add("SaleNetNotNULL");

            }

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

            List<SummaryByCarrier> listCarrierSummary = new List<SummaryByCarrier>();

            var result = analyticManager.GetFilteredRecords(analyticQuery) as AnalyticSummaryBigResult<AnalyticRecord>;
            if (result != null)
                foreach (var analyticRecord in result.Data)
                {
                    SummaryByCarrier carrierSummary = new SummaryByCarrier();

                    var carrierValue = analyticRecord.DimensionValues[0];
                    if (carrierValue != null)
                        carrierSummary.Carrier = carrierValue.Name;

                    MeasureValue calls;

                    analyticRecord.MeasureValues.TryGetValue("NumberOfCalls", out calls);
                    carrierSummary.Attempts = Convert.ToInt32(calls.Value ?? 0.0);

                    carrierSummary.AttemptsFormatted = ReportHelpers.FormatNumber(carrierSummary.Attempts);


                    MeasureValue duration;
                    analyticRecord.MeasureValues.TryGetValue(parameters.IsCost ? "CostDuration" : "SaleDuration", out duration);
                    carrierSummary.Duration = Convert.ToDecimal(duration.Value ?? 0.0);
                    carrierSummary.DurationFormatted = ReportHelpers.FormatNormalNumberDigit(carrierSummary.Duration);

                    MeasureValue net;
                    analyticRecord.MeasureValues.TryGetValue(parameters.IsCost ? "CostNetNotNULL" : "SaleNetNotNULL", out net);
                    carrierSummary.Net = Convert.ToDouble(net.Value ?? 0.0);
                    carrierSummary.NetFormatted = ReportHelpers.FormatNormalNumberDigit(carrierSummary.Net);




                    listCarrierSummary.Add(carrierSummary);
                }

            Dictionary<string, System.Collections.IEnumerable> dataSources = new Dictionary<string, System.Collections.IEnumerable>();
            dataSources.Add("CarrierSummary", listCarrierSummary.OrderBy(x => x.Carrier));
            return dataSources;
        }

        public Dictionary<string, RdlcParameter> GetRdlcReportParameters(ReportParameters parameters)
        {
            Dictionary<string, RdlcParameter> list = new Dictionary<string, RdlcParameter>();
            list.Add("FromDate", new RdlcParameter { Value = parameters.FromTime.ToString(), IsVisible = true });
            list.Add("ToDate", new RdlcParameter { Value = parameters.ToTime.HasValue ? parameters.ToTime.ToString() : null, IsVisible = true });
            list.Add("Title", new RdlcParameter { Value = "Summary By Carrier", IsVisible = true });
            list.Add("Customer", new RdlcParameter { Value = ReportHelpers.GetCarrierName(parameters.CustomersId, "Customers"), IsVisible = true });
            list.Add("Supplier", new RdlcParameter { Value = ReportHelpers.GetCarrierName(parameters.SuppliersId, "Suppliers"), IsVisible = true });
            list.Add("Currency", new RdlcParameter { Value = parameters.CurrencyDescription, IsVisible = true });
            list.Add("LogoPath", new RdlcParameter { Value = "logo", IsVisible = true });
            list.Add("DigitRate", new RdlcParameter { Value = ReportHelpers.GetLongNumberDigit(), IsVisible = true });
            list.Add("Digit", new RdlcParameter { Value = ReportHelpers.GetNormalNumberDigit(), IsVisible = true });
            list.Add("IsCost", new RdlcParameter { Value = parameters.IsCost.ToString(), IsVisible = true });

            return list;
        }
    }
}
