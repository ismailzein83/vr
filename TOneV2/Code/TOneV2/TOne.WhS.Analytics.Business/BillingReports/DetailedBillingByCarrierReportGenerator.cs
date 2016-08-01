﻿using System;
using System.Collections.Generic;
using System.Linq;
using TOne.WhS.Analytics.Entities.BillingReport;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Analytic.Business;
using Vanrise.Analytic.Entities;
using Vanrise.Entities;

namespace TOne.WhS.Analytics.Business.BillingReports
{
    public class DetailedBillingByCarrierReportGenerator : IReportGenerator
    {
        public Dictionary<string, System.Collections.IEnumerable> GenerateDataSources(ReportParameters parameters)
        {
            AnalyticManager analyticManager = new AnalyticManager();

            DataRetrievalInput<AnalyticQuery> analyticQuery = new DataRetrievalInput<AnalyticQuery>
            {
                Query = new AnalyticQuery
                {
                    DimensionFields = new List<string> { "Customer", "Supplier", "SaleZone", "SupplierZone", "CostRate", "SaleRate", "SaleRateId", "SaleRateChange", "SaleRateEffectiveDate", "CostRateId", "CostRateChange", "CostRateEffectiveDate" },
                    MeasureFields = new List<string> { "SaleDuration", "SaleNet", "CostDuration", "CostNet", "Profit" },
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

            List<CarrierSummaryFormatted> listCarrierSummaryDetailed = new List<CarrierSummaryFormatted>();

            var result = analyticManager.GetFilteredRecords(analyticQuery) as AnalyticSummaryBigResult<AnalyticRecord>;
            if (result != null)
                foreach (var analyticRecord in result.Data)
                {
                    CarrierSummaryFormatted carrierSummary = new CarrierSummaryFormatted();

                    var customerValue = analyticRecord.DimensionValues[0];
                    if (customerValue != null)
                        carrierSummary.Customer = customerValue.Name;
                    var supplierValue = analyticRecord.DimensionValues[1];
                    if (supplierValue != null)
                        carrierSummary.Supplier = supplierValue.Name;

                    var saleZoneValue = analyticRecord.DimensionValues[2];
                    if (saleZoneValue != null)
                        carrierSummary.SaleZoneName = saleZoneValue.Name;

                    var costZoneValue = analyticRecord.DimensionValues[3];
                    if (costZoneValue != null)
                        carrierSummary.CostZoneName = costZoneValue.Name;

                    var costRateValue = analyticRecord.DimensionValues[4];
                    if (costRateValue != null)
                    {
                        carrierSummary.CostRate = Convert.ToDouble(costRateValue.Value ?? 0.0);
                        carrierSummary.CostRateFormatted = ReportHelpers.FormatLongNumberDigitRate(carrierSummary.CostRate);
                    }
                    var saleRateValue = analyticRecord.DimensionValues[5];
                    if (saleRateValue != null)
                    {
                        carrierSummary.SaleRate = Convert.ToDouble(saleRateValue.Value ?? 0.0);
                        carrierSummary.SaleRateFormatted = ReportHelpers.FormatLongNumberDigitRate(carrierSummary.SaleRate);
                    }
                    var saleRateChange = analyticRecord.DimensionValues[7];
                    if (saleRateChange != null)
                    {
                        carrierSummary.SaleRateChange = saleRateChange.Value != null ? saleRateChange.Value.ToString() : "";
                        carrierSummary.SaleRateChangeFormatted = carrierSummary.SaleRateChange;
                    }
                    var saleRateEffectiveDate = analyticRecord.DimensionValues[8];
                    if (saleRateEffectiveDate != null)
                    {
                        carrierSummary.SaleRateEffectiveDate = Convert.ToDateTime(saleRateEffectiveDate.Value);
                    }
                    var costRateChange = analyticRecord.DimensionValues[10];
                    if (costRateChange != null)
                    {
                        carrierSummary.CostRateChange = costRateChange.Value != null ? costRateChange.Value.ToString() : "";
                        carrierSummary.CostRateChangeFormatted = carrierSummary.CostRateChange;
                    }
                    var costRateEffectiveDate = analyticRecord.DimensionValues[11];
                    if (costRateEffectiveDate != null)
                    {
                        carrierSummary.CostRateEffectiveDate = Convert.ToDateTime(costRateEffectiveDate.Value);
                    }

                    MeasureValue saleDuration;
                    analyticRecord.MeasureValues.TryGetValue("SaleDuration", out saleDuration);
                    carrierSummary.SaleDuration = Convert.ToDecimal(saleDuration.Value ?? 0.0);
                    carrierSummary.SaleDurationFormatted = ReportHelpers.FormatNormalNumberDigitRate(carrierSummary.SaleDuration);

                    MeasureValue costDuration;
                    analyticRecord.MeasureValues.TryGetValue("CostDuration", out costDuration);
                    carrierSummary.CostDuration = Convert.ToDecimal(costDuration.Value ?? 0.0);
                    carrierSummary.CostDurationFormatted = ReportHelpers.FormatNormalNumberDigitRate(carrierSummary.CostDuration);

                    MeasureValue costNet;
                    analyticRecord.MeasureValues.TryGetValue("CostNet", out costNet);
                    carrierSummary.CostAmount = Convert.ToDouble(costNet.Value ?? 0.0);
                    carrierSummary.CostAmountFormatted = ReportHelpers.FormatNormalNumberDigitRate(carrierSummary.CostAmount);

                    MeasureValue saleNet;
                    analyticRecord.MeasureValues.TryGetValue("SaleNet", out saleNet);

                    carrierSummary.SaleAmount = Convert.ToDouble(saleNet == null ? 0.0 : saleNet.Value ?? 0.0);
                    carrierSummary.SaleAmountFormatted = ReportHelpers.FormatNormalNumberDigitRate(carrierSummary.SaleAmount);

                    MeasureValue profit;
                    analyticRecord.MeasureValues.TryGetValue("Profit", out profit);
                    carrierSummary.Profit = Convert.ToDouble(profit.Value ?? 0.0);
                    carrierSummary.ProfitFormatted = ReportHelpers.FormatNormalNumberDigitRate(carrierSummary.Profit);

                    carrierSummary.ProfitPercentage = carrierSummary.SaleAmount == 0 ? "" : (carrierSummary.SaleAmount != 0) ? ReportHelpers.FormatNumberPercentage(((1 - carrierSummary.CostAmount / carrierSummary.SaleAmount))) : "-100%";


                    listCarrierSummaryDetailed.Add(carrierSummary);
                }

            Dictionary<string, System.Collections.IEnumerable> dataSources =
                new Dictionary<string, System.Collections.IEnumerable> { { "DetailedCarrier", listCarrierSummaryDetailed } };
            return dataSources;

        }

        public Dictionary<string, RdlcParameter> GetRdlcReportParameters(ReportParameters parameters)
        {
            Dictionary<string, RdlcParameter> list = new Dictionary<string, RdlcParameter>
            {
                {"FromDate", new RdlcParameter {Value = parameters.FromTime.ToString(), IsVisible = true}},
                {"ToDate", new RdlcParameter {Value =(parameters.ToTime.HasValue)? parameters.ToTime.ToString():null, IsVisible = true}},
                {"Customer", new RdlcParameter {Value = ReportHelpers.GetCarrierName(parameters.CustomersId,"Customers"), IsVisible = true}},
                {"Supplier", new RdlcParameter { Value = ReportHelpers.GetCarrierName(parameters.SuppliersId, "Suppliers"), IsVisible = true }},
                {"Title", new RdlcParameter {Value = "Detailed Billing by Carrier", IsVisible = true}},
                {"Currency", new RdlcParameter {Value = parameters.CurrencyDescription, IsVisible = true}},
                {"LogoPath", new RdlcParameter {Value = "logo", IsVisible = true}},
                {"DigitRate", new RdlcParameter {Value = "2", IsVisible = true}}
            };

            return list;
        }
    }
}
