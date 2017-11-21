using System;
using System.Collections.Generic;
using System.Linq;
using TOne.WhS.Analytics.Entities;
using TOne.WhS.Analytics.Entities.BillingReport;
using Vanrise.Analytic.Business;
using Vanrise.Analytic.Entities;
using Vanrise.Entities;
using AnalyticRecord = Vanrise.Analytic.Entities.AnalyticRecord;
using DimensionFilter = Vanrise.Analytic.Entities.DimensionFilter;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.BusinessEntity.Business;
using Vanrise.Common.Business;

namespace TOne.WhS.Analytics.Business.BillingReports
{
    public class SummaryByZoneReportGenerator : IReportGenerator
    {
        public Dictionary<string, System.Collections.IEnumerable> GenerateDataSources(ReportParameters parameters)
        {
            AnalyticManager analyticManager = new AnalyticManager();
            RateTypeManager rateTypeManager = new RateTypeManager();
            TOne.WhS.BusinessEntity.Business.ConfigManager configManager = new BusinessEntity.Business.ConfigManager();
            int offPeakRateTypeId = configManager.GetOffPeakRateTypeId();
            List<string> listDimensions = new List<string>();
            List<string> listMeasures = new List<string> { "NumberOfCalls", "DurationNet" };

            if (parameters.GroupBySupplier && parameters.IsCost)
                listDimensions.Add("SupplierZone");
            else
                listDimensions.Add("SaleZone");

            if (parameters.IsCost)
            {
                listDimensions.Add("CostRateType");
                listMeasures.Add("CostRate_DurAvg");
            }
            else
            {
                listDimensions.Add("SaleRateType");
                listMeasures.Add("SaleRate_DurAvg");
            }

            if (parameters.GroupBySupplier)
            {
                if (parameters.IsCost)
                {


                    if (parameters.GroupByProfile)
                        listDimensions.Add("SupplierProfile");
                    else
                        listDimensions.Add("Supplier");

                }

            }



            if (parameters.IsCost)
            {
                listMeasures.Add("CostDuration");
                listMeasures.Add("CostNetNotNULL");
                listMeasures.Add("CostExtraCharges");
            }
            else
            {
                listMeasures.Add("SaleDuration");
                listMeasures.Add("SaleNetNotNULL");
                listMeasures.Add("SaleExtraCharges");
            }

            Vanrise.Entities.DataRetrievalInput<AnalyticQuery> analyticQuery = new DataRetrievalInput<AnalyticQuery>()
            {
                Query = new AnalyticQuery()
                {
                    DimensionFields = listDimensions,
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

            List<SummaryByZone> listSummaryByZone = new List<SummaryByZone>();

            var result = analyticManager.GetFilteredRecords(analyticQuery) as Vanrise.Analytic.Entities.AnalyticSummaryBigResult<AnalyticRecord>;
            if (result != null)
                foreach (var analyticRecord in result.Data)
                {
                    SummaryByZone summaryByZone = new SummaryByZone();

                    var zoneValue = analyticRecord.DimensionValues[0];
                    if (zoneValue != null)
                        summaryByZone.Zone = zoneValue.Name;

                    var rateTypeValue = analyticRecord.DimensionValues[1];
                    if (rateTypeValue != null)
                        if (rateTypeValue.Value != null)
                        {
                            summaryByZone.RateType = (int)rateTypeValue.Value;
                            summaryByZone.RateTypeFormatted = rateTypeManager.GetRateTypeName(summaryByZone.RateType.Value);
                        }
                        else
                            summaryByZone.RateTypeFormatted = "Normal";

                    MeasureValue rate;
                    analyticRecord.MeasureValues.TryGetValue(parameters.IsCost ? "CostRate_DurAvg" : "SaleRate_DurAvg", out rate);
                    summaryByZone.Rate = (rate == null) ? 0 : Convert.ToDecimal(rate.Value ?? 0.0);
                    summaryByZone.RateFormatted = ReportHelpers.FormatLongNumberDigit(summaryByZone.Rate);


                    if (parameters.IsCost && parameters.GroupBySupplier)
                    {
                        var supplierValue = analyticRecord.DimensionValues[2];
                        if (supplierValue != null)
                            summaryByZone.SupplierID = supplierValue.Name;
                    }

                    MeasureValue calls;
                    analyticRecord.MeasureValues.TryGetValue("NumberOfCalls", out calls);
                    summaryByZone.Calls = (calls == null) ? 0 : Convert.ToInt32(calls.Value ?? 0);

                    MeasureValue durationNet;
                    analyticRecord.MeasureValues.TryGetValue("DurationNet", out durationNet);
                    summaryByZone.DurationNet = (durationNet == null) ? 0 : Convert.ToDecimal(durationNet.Value ?? 0.0);
                    summaryByZone.DurationNetFormatted = ReportHelpers.FormatNormalNumberDigit(summaryByZone.DurationNet);


                    MeasureValue durationInMinutes;
                    if (parameters.IsCost)
                        analyticRecord.MeasureValues.TryGetValue("CostDuration", out durationInMinutes);
                    else
                        analyticRecord.MeasureValues.TryGetValue("SaleDuration", out durationInMinutes);
                    summaryByZone.DurationInSeconds = (durationInMinutes == null) ? 0 : Convert.ToDecimal(durationInMinutes.Value ?? 0.0);
                    summaryByZone.DurationInSecondsFormatted = ReportHelpers.FormatNormalNumberDigit(summaryByZone.DurationInSeconds);

                    MeasureValue net;
                    if (parameters.IsCost)
                        analyticRecord.MeasureValues.TryGetValue("CostNetNotNULL", out net);
                    else
                        analyticRecord.MeasureValues.TryGetValue("SaleNetNotNULL", out net);
                    summaryByZone.Net = (net == null) ? 0 : Convert.ToDouble(net.Value ?? 0.0);
                    summaryByZone.NetFormatted = ReportHelpers.FormatNormalNumberDigit(summaryByZone.Net);

                    MeasureValue extraChargeValue;
                    if (parameters.IsCost)
                        analyticRecord.MeasureValues.TryGetValue("CostExtraCharges", out extraChargeValue);
                    else
                        analyticRecord.MeasureValues.TryGetValue("SaleExtraCharges", out extraChargeValue);
                    summaryByZone.ExtraChargeValue = (extraChargeValue == null) ? (decimal)0.0 : Convert.ToDecimal(extraChargeValue.Value ?? 0.0);
                    summaryByZone.CommissionValueFormatted = ReportHelpers.FormatLongNumberDigit(summaryByZone.ExtraChargeValue);

                    listSummaryByZone.Add(summaryByZone);
                }
            //parameters.ServicesForCustomer = services;
            parameters.NormalDuration = listSummaryByZone.Where(y => !y.RateType.HasValue).Sum(x => Math.Round(x.DurationInSeconds, 2));

            parameters.OffPeakDuration = Math.Round(listSummaryByZone.Where(y => y.RateType == offPeakRateTypeId).Sum(x => Math.Round(x.DurationInSeconds, 2)),2);

            parameters.NormalNet = Math.Round(listSummaryByZone.Where(y => !y.RateType.HasValue).Sum(x => x.Net).Value , 2);

            parameters.OffPeakNet = Math.Round(listSummaryByZone.Where(y => y.RateType == offPeakRateTypeId).Sum(x => x.Net).Value,2);

            parameters.TotalAmount = parameters.OffPeakNet + parameters.NormalNet;

            Dictionary<string, System.Collections.IEnumerable> dataSources = new Dictionary<string, System.Collections.IEnumerable>();
            dataSources.Add("ZoneSummaries", listSummaryByZone);
            return dataSources;
        }

        public Dictionary<string, RdlcParameter> GetRdlcReportParameters(ReportParameters parameters)
        {
            Dictionary<string, RdlcParameter> list = new Dictionary<string, RdlcParameter>();

            list.Add("FromDate", new RdlcParameter { Value = parameters.FromTime.ToString(), IsVisible = true });
            list.Add("ToDate", new RdlcParameter { Value = parameters.ToTime.HasValue ? parameters.ToTime.ToString() : null, IsVisible = true });
            list.Add("Title", new RdlcParameter { Value = String.Format("Summary By Zone -{0}", parameters.IsCost ? "Purchase" : "Sale"), IsVisible = true });
            list.Add("Currency", new RdlcParameter { Value = parameters.CurrencyDescription, IsVisible = true });
            list.Add("LogoPath", new RdlcParameter { Value = "logo", IsVisible = true });
            list.Add("Customer", new RdlcParameter { Value = ReportHelpers.GetCarrierName(parameters.CustomersId, "Customers"), IsVisible = true });
            list.Add("Supplier", new RdlcParameter { Value = ReportHelpers.GetCarrierName(parameters.SuppliersId, "Suppliers"), IsVisible = true });
            list.Add("DigitRate", new RdlcParameter { Value = ReportHelpers.GetLongNumberDigit(), IsVisible = true });
            list.Add("Digit", new RdlcParameter { Value = ReportHelpers.GetNormalNumberDigit(), IsVisible = true });
            list.Add("NormalNet", new RdlcParameter { Value = parameters.NormalNet.ToString(), IsVisible = true });
            list.Add("NormalDuration", new RdlcParameter { Value = parameters.NormalDuration.ToString(), IsVisible = true });
            list.Add("OffPeakNet", new RdlcParameter { Value = parameters.OffPeakNet.ToString(), IsVisible = true });
            list.Add("OffPeakDuration", new RdlcParameter { Value = parameters.OffPeakDuration.ToString(), IsVisible = true });
            list.Add("IsService", new RdlcParameter { Value = parameters.IsService.ToString(), IsVisible = true });
            list.Add("IsCommission", new RdlcParameter { Value = parameters.IsCommission.ToString(), IsVisible = true });
            list.Add("TotalAmount", new RdlcParameter { Value = parameters.TotalAmount.ToString(), IsVisible = true });
            list.Add("ServicesForCustomer", new RdlcParameter { Value = parameters.ServicesForCustomer.ToString(), IsVisible = true });
            list.Add("IsCost", new RdlcParameter { Value = parameters.IsCost.ToString(), IsVisible = true });
            list.Add("GroupeBySupplier", new RdlcParameter { Value = parameters.GroupBySupplier.ToString(), IsVisible = true });

            return list;
        }
    }
}
