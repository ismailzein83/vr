﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Analytics.Entities.BillingReport;
using Vanrise.Analytic.Business;
using Vanrise.Analytic.Entities;
using Vanrise.Entities;

namespace TOne.WhS.Analytics.Business.BillingReports
{
    public class DetailedBillingByZoneReportGenerator : IReportGenerator
    {
        public Dictionary<string, System.Collections.IEnumerable> GenerateDataSources(ReportParameters parameters)
        {
            AnalyticManager analyticManager = new AnalyticManager();
            List<string> listDimensions = new List<string>();
            List<string> listMeasures = new List<string> { "NumberOfCalls", "DurationNet" };
            double service = 0;

            if (parameters.GroupBySupplier && parameters.IsCost)
                listDimensions.Add("SupplierZone");
            else
                listDimensions.Add("SaleZone");

            if (parameters.IsCost)
            {
                if (parameters.GroupBySupplier)
                {
                    if (parameters.GroupByProfile) listDimensions.Add("SupplierProfile"); else listDimensions.Add("Supplier");
                }
            }
            else
            {
                if (parameters.GroupBySupplier)
                {
                    if (parameters.GroupByProfile) listDimensions.Add("SupplierProfile"); else listDimensions.Add("Supplier");
                    if (parameters.GroupByProfile) listDimensions.Add("CustomerProfile"); else listDimensions.Add("Customer");
                }
                else
                    if (parameters.GroupByProfile) listDimensions.Add("CustomerProfile"); else listDimensions.Add("Customer");
            }

            if (parameters.IsCost)
            {
                listMeasures.Add("CostRate");
                listMeasures.Add("CostDurationDetails");
                listMeasures.Add("CostNet");
                //listMeasures.Add("CostCommissions");
                listMeasures.Add("CostExtraCharges");
                listMeasures.Add("CostOffPeakDuration");
                listMeasures.Add("CostOffPeakRate");
                listMeasures.Add("CostOffPeakNet");
                listMeasures.Add("CostWeekEndRate");
                listMeasures.Add("CostWeekEndDuration");
                listMeasures.Add("CostWeekEndNet");
            }
            else
            {
                listMeasures.Add("SaleRate");
                listMeasures.Add("SaleDurationDetails");
                listMeasures.Add("SaleNet");
                //listMeasures.Add("SaleCommissions");
                listMeasures.Add("SaleExtraCharges");
                listMeasures.Add("SaleOffPeakDuration");
                listMeasures.Add("SaleOffPeakRate");
                listMeasures.Add("SaleOffPeakNet");
                listMeasures.Add("SaleWeekEndRate");
                listMeasures.Add("SaleWeekEndDuration");
                listMeasures.Add("SaleWeekEndNet");
            }

            Vanrise.Entities.DataRetrievalInput<AnalyticQuery> analyticQuery = new DataRetrievalInput<AnalyticQuery>()
            {
                Query = new AnalyticQuery()
                {
                    DimensionFields = listDimensions,
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

            List<DetailedBillingByZone> listDetailedBillingByZone = new List<DetailedBillingByZone>();

            var result = analyticManager.GetFilteredRecords(analyticQuery) as Vanrise.Analytic.Entities.AnalyticSummaryBigResult<AnalyticRecord>;
            if (result != null)
                foreach (var analyticRecord in result.Data)
                {
                    DetailedBillingByZone detailedBillingByZone = new DetailedBillingByZone();

                    var zoneValue = analyticRecord.DimensionValues[0];
                    if (zoneValue != null)
                        detailedBillingByZone.Zone = zoneValue.Name;



                    if (parameters.IsCost)
                    {
                        if (parameters.GroupBySupplier)
                        {
                            var supplierValue = analyticRecord.DimensionValues[1];
                            if (supplierValue != null)
                                detailedBillingByZone.SupplierID = supplierValue.Name;
                        }
                    }
                    else
                    {
                        var supplierValue = analyticRecord.DimensionValues[1];
                        if (supplierValue != null)
                            detailedBillingByZone.SupplierID = supplierValue.Name;
                    }

                    MeasureValue durationNet;
                    analyticRecord.MeasureValues.TryGetValue("DurationNet", out durationNet);
                    detailedBillingByZone.DurationNet = Convert.ToDecimal(durationNet.Value ?? 0.0);
                    detailedBillingByZone.DurationNetFormatted = ReportHelpers.FormatNormalNumberDigitRate(detailedBillingByZone.DurationNet);

                    MeasureValue calls;
                    analyticRecord.MeasureValues.TryGetValue("NumberOfCalls", out calls);
                    detailedBillingByZone.Calls = (calls == null) ? 0 : Convert.ToInt32(calls.Value ?? 0);

                    MeasureValue rate;
                    if (parameters.IsCost)
                        analyticRecord.MeasureValues.TryGetValue("CostRate", out rate);
                    else
                        analyticRecord.MeasureValues.TryGetValue("SaleRate", out rate);
                    detailedBillingByZone.Rate = (rate == null) ? 0.0 : Convert.ToInt32(rate.Value ?? 0.0);
                    detailedBillingByZone.RateFormatted = ReportHelpers.FormatLongNumberDigitRate(detailedBillingByZone.Rate);

                    MeasureValue net;
                    if (parameters.IsCost)
                        analyticRecord.MeasureValues.TryGetValue("CostNet", out net);
                    else
                        analyticRecord.MeasureValues.TryGetValue("SaleNet", out net);
                    detailedBillingByZone.Net = (net == null) ? 0 : Convert.ToDouble(net.Value ?? 0.0);
                    detailedBillingByZone.NetFormatted = ReportHelpers.FormatNormalNumberDigitRate(detailedBillingByZone.Net);

                    MeasureValue extraChargeValue;
                    if (parameters.IsCost)
                        analyticRecord.MeasureValues.TryGetValue("CostExtraCharges", out extraChargeValue);
                    else
                        analyticRecord.MeasureValues.TryGetValue("SaleExtraCharges", out extraChargeValue);
                    detailedBillingByZone.ExtraChargeValue = (extraChargeValue == null) ? 0.0 : Convert.ToDouble(extraChargeValue.Value ?? 0.0);
                    detailedBillingByZone.CommissionValueFormatted = ReportHelpers.FormatNumber(detailedBillingByZone.ExtraChargeValue);

                    MeasureValue durationDetails;
                    if (parameters.IsCost)
                        analyticRecord.MeasureValues.TryGetValue("CostDurationDetails", out durationDetails);
                    else
                        analyticRecord.MeasureValues.TryGetValue("SaleDurationDetails", out durationDetails);
                    detailedBillingByZone.DurationInSeconds = (durationDetails == null) ? 0 : Convert.ToDecimal(durationDetails.Value ?? 0.0);
                    detailedBillingByZone.DurationInSecondsFormatted = ReportHelpers.FormatNormalNumberDigitRate(detailedBillingByZone.DurationInSeconds);

                    MeasureValue offPeakDuration;
                    if (parameters.IsCost)
                        analyticRecord.MeasureValues.TryGetValue("CostOffPeakDuration", out offPeakDuration);
                    else
                        analyticRecord.MeasureValues.TryGetValue("SaleOffPeakDuration", out offPeakDuration);
                    detailedBillingByZone.OffPeakDurationInSeconds = (offPeakDuration == null) ? 0 : Convert.ToDecimal(offPeakDuration.Value ?? 0.0);
                    detailedBillingByZone.OffPeakDurationInSecondsFormatted = ReportHelpers.FormatNormalNumberDigitRate(detailedBillingByZone.OffPeakDurationInSeconds);

                    MeasureValue offPeakRate;
                    if (parameters.IsCost)
                        analyticRecord.MeasureValues.TryGetValue("CostOffPeakRate", out offPeakRate);
                    else
                        analyticRecord.MeasureValues.TryGetValue("SaleOffPeakRate", out offPeakRate);
                    detailedBillingByZone.OffPeakRate = (offPeakRate == null) ? 0 : Convert.ToDouble(offPeakRate.Value ?? 0.0);
                    detailedBillingByZone.OffPeakRateFormatted = ReportHelpers.FormatLongNumberDigitRate(detailedBillingByZone.OffPeakRate);

                    MeasureValue offPeakNet;
                    if (parameters.IsCost)
                        analyticRecord.MeasureValues.TryGetValue("CostOffPeakNet", out offPeakNet);
                    else
                        analyticRecord.MeasureValues.TryGetValue("SaleOffPeakNet", out offPeakNet);
                    detailedBillingByZone.OffPeakNet = (offPeakNet == null) ? 0 : Convert.ToDouble(offPeakNet.Value ?? 0.0);
                    detailedBillingByZone.OffPeakNetFormatted = ReportHelpers.FormatNormalNumberDigitRate(detailedBillingByZone.OffPeakNet);

                    MeasureValue weekEndRate;
                    if (parameters.IsCost)
                        analyticRecord.MeasureValues.TryGetValue("CostWeekEndRate", out weekEndRate);
                    else
                        analyticRecord.MeasureValues.TryGetValue("SaleWeekEndRate", out weekEndRate);
                    detailedBillingByZone.WeekEndRate = (weekEndRate == null) ? 0 : Convert.ToDouble(weekEndRate.Value ?? 0.0);
                    detailedBillingByZone.WeekEndRateFormatted = ReportHelpers.FormatLongNumberDigitRate(detailedBillingByZone.WeekEndRate);

                    MeasureValue weekEndDuration;
                    if (parameters.IsCost)
                        analyticRecord.MeasureValues.TryGetValue("CostWeekEndDuration", out weekEndDuration);
                    else
                        analyticRecord.MeasureValues.TryGetValue("SaleWeekEndDuration", out weekEndDuration);
                    detailedBillingByZone.WeekEndDurationInSeconds = (weekEndDuration == null) ? 0 : Convert.ToDecimal(weekEndDuration.Value ?? 0.0);
                    detailedBillingByZone.WeekEndDurationInSecondsFormatted = ReportHelpers.FormatNormalNumberDigitRate(detailedBillingByZone.WeekEndDurationInSeconds);

                    MeasureValue weekEndNet;
                    if (parameters.IsCost)
                        analyticRecord.MeasureValues.TryGetValue("CostWeekEndNet", out weekEndNet);
                    else
                        analyticRecord.MeasureValues.TryGetValue("SaleWeekEndNet", out weekEndNet);
                    detailedBillingByZone.WeekEndNet = (weekEndNet == null) ? 0 : Convert.ToDouble(weekEndNet.Value ?? 0.0);
                    detailedBillingByZone.WeekEndNetFormatted = ReportHelpers.FormatNormalNumberDigitRate(detailedBillingByZone.WeekEndNet);

                    detailedBillingByZone.TotalAmountFormatted =
                        ReportHelpers.FormatNormalNumberDigitRate(detailedBillingByZone.Net + detailedBillingByZone.OffPeakNet +
                                                   detailedBillingByZone.WeekEndNet);

                    detailedBillingByZone.TotalDurationFormatted =
                        ReportHelpers.FormatNormalNumberDigitRate(detailedBillingByZone.DurationInSeconds +
                                                   detailedBillingByZone.OffPeakDurationInSeconds +
                                                   detailedBillingByZone.WeekEndDurationInSeconds);

                    listDetailedBillingByZone.Add(detailedBillingByZone);
                }

            decimal services = 0;
            if (parameters.IsCost)
                if (listDetailedBillingByZone.Count != 0)
                    services = (decimal)service;

            parameters.ServicesForCustomer = services;

            Dictionary<string, System.Collections.IEnumerable> dataSources = new Dictionary<string, System.Collections.IEnumerable>();
            dataSources.Add("ZoneSummaryDetailed", listDetailedBillingByZone);
            return dataSources;
        }

        public Dictionary<string, RdlcParameter> GetRdlcReportParameters(ReportParameters parameters)
        {
            Dictionary<string, RdlcParameter> list = new Dictionary<string, RdlcParameter>();

            list.Add("FromDate", new RdlcParameter { Value = parameters.FromTime.ToString(), IsVisible = true });
            list.Add("ToDate", new RdlcParameter { Value = parameters.ToTime.HasValue?  parameters.ToTime.ToString():null, IsVisible = true });
            list.Add("Title", new RdlcParameter { Value = String.Format("Detailed Billing By Zone ({0})", parameters.IsCost == true ? "Purchase" : "Sale"), IsVisible = true });
            list.Add("Currency", new RdlcParameter { Value = parameters.CurrencyDescription, IsVisible = true });
            list.Add("LogoPath", new RdlcParameter { Value = "logo", IsVisible = true });
            list.Add("Customer", new RdlcParameter { Value = ReportHelpers.GetCarrierName(parameters.CustomersId, "Customers"), IsVisible = true });
            list.Add("Supplier", new RdlcParameter { Value = ReportHelpers.GetCarrierName(parameters.SuppliersId, "Suppliers"), IsVisible = true });
            list.Add("DigitRate", new RdlcParameter { Value = "4", IsVisible = true });
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
