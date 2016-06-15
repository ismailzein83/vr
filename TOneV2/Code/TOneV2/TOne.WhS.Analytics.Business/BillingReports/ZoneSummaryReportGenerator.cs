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

namespace TOne.WhS.Analytics.Business.BillingReports
{
    public class ZoneSummaryReportGenerator : IReportGenerator
    {
        public Dictionary<string, System.Collections.IEnumerable> GenerateDataSources(ReportParameters parameters)
        {
            AnalyticManager analyticManager = new AnalyticManager();
            BillingStatisticManager manager = new BillingStatisticManager();
            List<string> listDimensions = new List<string>();
            List<string> listMeasures = new List<string> { "NumberOfCalls", "DurationNet" };

            if (parameters.GroupBySupplier)
                listDimensions.Add("Supplier");

            if (parameters.IsCost)
                listDimensions.Add("CostRateType");
            else
                listDimensions.Add("SaleRateType");
            
            if (parameters.IsCost)
            {
                listMeasures.Add("CostRate");
                listMeasures.Add("CostDuration");
                listMeasures.Add("CostNet");
                listMeasures.Add("CostCommissions");
            }
            else
            {                
                listMeasures.Add("SaleRate");
                listMeasures.Add("SaleDurations");
                listMeasures.Add("SaleNet");
                listMeasures.Add("SaleCommissions");
            }
            Vanrise.Entities.DataRetrievalInput<AnalyticQuery> analyticQuery = new DataRetrievalInput<AnalyticQuery>()
            {
                Query = new AnalyticQuery()
                {
                    DimensionFields = new List<string> { "SaleZone" },
                    MeasureFields = new List<string>() {  },
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

            List<ZoneSummaryFormatted> listZoneSummary = new List<ZoneSummaryFormatted>();

            var result = analyticManager.GetFilteredRecords(analyticQuery) as Vanrise.Analytic.Entities.AnalyticSummaryBigResult<AnalyticRecord>;
            if(result != null)
            foreach (var analyticRecord in result.Data)
            {
                ZoneSummaryFormatted zoneSummary = new ZoneSummaryFormatted();

                var supplierValue = analyticRecord.DimensionValues[0];
                if (supplierValue != null)
                    zoneSummary.SupplierID = supplierValue.Name;

                                
                var saleZoneValue = analyticRecord.DimensionValues[1];
                if (saleZoneValue != null)
                    zoneSummary.Zone = saleZoneValue.Name;

                if (parameters.IsCost)
                {
                    var costRateTypeValue = analyticRecord.DimensionValues[2];
                    if (costRateTypeValue != null)
                        zoneSummary.RateType = (int)costRateTypeValue.Value;
                }
                else
                {
                    var saleRateTypeValue = analyticRecord.DimensionValues[2];
                    if (saleRateTypeValue != null)
                        zoneSummary.RateType = (int)saleRateTypeValue.Value;
                }

                zoneSummary.RateTypeFormatted = ((RateTypeEnum) zoneSummary.RateType).ToString();

                //MeasureValue calls;
                //analyticRecord.MeasureValues.TryGetValue("NumberOfCalls", out calls);
                //zoneSummary.Calls = (calls == null ) ? 0 : Convert.ToInt32(calls.Value ?? 0);

                //MeasureValue rate;
                //if (parameters.IsCost)
                //    analyticRecord.MeasureValues.TryGetValue("CostRate", out rate);
                //else
                //    analyticRecord.MeasureValues.TryGetValue("SaleRate", out rate);
                //zoneSummary.Rate = (rate == null) ? 0.0 : Convert.ToInt32(rate.Value ?? 0.0);


                //MeasureValue durationNet;
                //analyticRecord.MeasureValues.TryGetValue("DurationNet", out durationNet);
                //zoneSummary.DurationNet = Convert.ToDecimal(durationNet.Value ?? 0.0);
                //zoneSummary.DurationNetFormatted = manager.FormatNumber(zoneSummary.DurationNet);

                //MeasureValue durationInMinutes;
                //analyticRecord.MeasureValues.TryGetValue("DurationInMinutes", out durationInMinutes);
                //zoneSummary.DurationInSeconds = Convert.ToDecimal(durationInMinutes.Value ?? 0.0);
                //zoneSummary.DurationInSecondsFormatted = manager.FormatNumber(zoneSummary.DurationInSeconds);

                //zoneSummary.RateFormatted = manager.FormatNumberDigitRate(zoneSummary.Rate);

                //MeasureValue commissionValue;
                //if (parameters.IsCost)
                //    analyticRecord.MeasureValues.TryGetValue("CostCommissions", out commissionValue);
                //else
                //    analyticRecord.MeasureValues.TryGetValue("SaleCommissions", out commissionValue);

                //zoneSummary.CommissionValue = Convert.ToDouble(commissionValue.Value ?? 0.0);
                //zoneSummary.CommissionValueFormatted = manager.FormatNumber(zoneSummary.CommissionValue);

                //MeasureValue extraChargeValue;
                //if (parameters.IsCost)
                //    analyticRecord.MeasureValues.TryGetValue("CostExtraCharges", out extraChargeValue);
                //else
                //    analyticRecord.MeasureValues.TryGetValue("SaleExtraCharges", out extraChargeValue);

                //zoneSummary.ExtraChargeValue = Convert.ToDouble(extraChargeValue.Value ?? 0.0);

                //if (parameters.IsCost)
                //{
                //    MeasureValue net;
                //    analyticRecord.MeasureValues.TryGetValue("CostNet", out net);
                //    zoneSummary.Net = Convert.ToDouble(net.Value ?? 0.0);
                //    zoneSummary.NetFormatted = manager.FormatNumberDigitRate(zoneSummary.Net);

                //    MeasureValue commisionValue;
                //    analyticRecord.MeasureValues.TryGetValue("CostCommissions", out commisionValue);
                //    zoneSummary.CommissionValue = Convert.ToDouble(commisionValue.Value ?? 0.0);
                //    zoneSummary.CommissionValueFormatted = manager.FormatNumberDigitRate(zoneSummary.CommissionValue);

                //    MeasureValue extraChargesValue;
                //    analyticRecord.MeasureValues.TryGetValue("CostExtraCharges", out extraChargesValue);
                //    zoneSummary.ExtraChargeValue = Convert.ToDouble(extraChargesValue.Value ?? 0.0);
                //}
                //else
                //{
                //    MeasureValue net;
                //    analyticRecord.MeasureValues.TryGetValue("SaleNet", out net);
                //    zoneSummary.Net = Convert.ToDouble(net.Value ?? 0.0);
                //    zoneSummary.NetFormatted = manager.FormatNumberDigitRate(zoneSummary.Net);

                //    MeasureValue commisionValue;
                //    analyticRecord.MeasureValues.TryGetValue("SaleCommissions", out commisionValue);
                //    zoneSummary.CommissionValue = Convert.ToDouble(commisionValue.Value ?? 0.0);
                //    zoneSummary.CommissionValueFormatted = manager.FormatNumberDigitRate(zoneSummary.CommissionValue);

                //    MeasureValue extraChargesValue;
                //    analyticRecord.MeasureValues.TryGetValue("SaleExtraCharges", out extraChargesValue);
                //    zoneSummary.ExtraChargeValue = Convert.ToDouble(extraChargesValue.Value ?? 0.0);
                //}

                listZoneSummary.Add(zoneSummary);
            }

            //parameters.ServicesForCustomer = services;
            parameters.NormalDuration = listZoneSummary.Where(y => y.RateTypeFormatted == "Normal").Sum(x => Math.Round(x.DurationInSeconds, 2));

            parameters.OffPeakDuration = Math.Ceiling(listZoneSummary.Where(y => y.RateTypeFormatted == "OffPeak").Sum(x => Math.Round(x.DurationInSeconds, 2)));

            parameters.NormalNet = listZoneSummary.Where(y => y.RateTypeFormatted == "Normal").Sum(x => x.Net).Value;

            parameters.OffPeakNet = Math.Round(listZoneSummary.Where(y => y.RateTypeFormatted == "OffPeak").Sum(x => x.Net).Value, 0);

            parameters.TotalAmount = parameters.OffPeakNet + parameters.NormalNet;

            Dictionary<string, System.Collections.IEnumerable> dataSources = new Dictionary<string, System.Collections.IEnumerable>();
            dataSources.Add("ZoneSummaries", listZoneSummary);
            return dataSources;
        }

        public Dictionary<string, RdlcParameter> GetRdlcReportParameters(ReportParameters parameters)
        {
            Dictionary<string, RdlcParameter> list = new Dictionary<string, RdlcParameter>();

            list.Add("FromDate", new RdlcParameter { Value = parameters.FromTime.ToString(), IsVisible = true });
            list.Add("ToDate", new RdlcParameter { Value = parameters.ToTime.ToString(), IsVisible = true });
            list.Add("Title", new RdlcParameter { Value = String.Format("Zone Summary ({0})", parameters.IsCost == true ? "Buy" : "Sale"), IsVisible = true });
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
