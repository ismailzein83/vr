using System;
using System.Collections.Generic;
using System.Linq;
using TOne.WhS.Analytics.Entities.BillingReport;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Analytic.Business;
using Vanrise.Analytic.Entities;
using Vanrise.Entities;

namespace TOne.WhS.Analytics.Business.BillingReports
{
    public class ProfitByZoneReportGenerator : IReportGenerator
    {
        public Dictionary<string, System.Collections.IEnumerable> GenerateDataSources(ReportParameters parameters)
        {
            AnalyticManager analyticManager = new AnalyticManager();
            CarrierAccountManager carrierAccountManager = new CarrierAccountManager();

            Vanrise.Entities.DataRetrievalInput<AnalyticQuery> analyticQuery = new DataRetrievalInput<AnalyticQuery>()
            {
                Query = new AnalyticQuery()
                {
                    DimensionFields = new List<string> { "Supplier", "SaleZone", "SupplierZone" },
                    MeasureFields = new List<string>() { "SaleNet", "CostNet", "SaleDuration", "CostDuration", "DurationNet", "NumberOfCalls" },
                    TableId = 8,
                    FromTime = parameters.FromTime,
                    ToTime = parameters.ToTime,
                    CurrencyId = parameters.CurrencyId,
                    ParentDimensions = new List<string>(),
                    Filters = new List<DimensionFilter>()
                },
                SortByColumnName = "DimensionValues[1].Name"
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

            List<ProfitByZone> listProfitByZone = new List<ProfitByZone>();

            var result = analyticManager.GetFilteredRecords(analyticQuery) as AnalyticSummaryBigResult<AnalyticRecord>;

            if (result != null)
                foreach (var analyticRecord in result.Data)
                {
                    CarrierAccount customer = new CarrierAccount();
                    CarrierAccount supplier = new CarrierAccount();
                    ProfitByZone profitByZone = new ProfitByZone();

                    var supplierValue = analyticRecord.DimensionValues[0];
                    if (supplierValue != null)
                    {
                        supplier = carrierAccountManager.GetCarrierAccount((int)supplierValue.Value);
                        profitByZone.SupplierID = supplierValue.Name;
                    }

                    var saleZoneValue = analyticRecord.DimensionValues[1];
                    if (saleZoneValue != null)
                        profitByZone.SaleZone = saleZoneValue.Name;

                    var zoneProfitValue = analyticRecord.DimensionValues[2];
                    if (zoneProfitValue != null)
                        profitByZone.CostZone = zoneProfitValue.Name;

                    if (parameters.GroupByCustomer)
                    {
                        var customerValue = analyticRecord.DimensionValues[3];
                        if (customerValue != null)
                        {
                            customer = carrierAccountManager.GetCarrierAccount((int)customerValue.Value);
                            profitByZone.CustomerID = customerValue.Name;
                        }
                    }

                    if (supplier != null && customer != null && supplier.CarrierAccountSettings.ActivationStatus != ActivationStatus.Inactive &&
                        customer.CarrierAccountSettings.ActivationStatus != ActivationStatus.Inactive &&
                        !supplier.IsDeleted && !customer.IsDeleted)
                    {
                        MeasureValue saleNet;
                        analyticRecord.MeasureValues.TryGetValue("SaleNet", out saleNet);

                        profitByZone.SaleNet = Convert.ToDouble(saleNet == null ? 0.0 : saleNet.Value ?? 0.0);
                        profitByZone.SaleNetFormated = profitByZone.SaleNet == 0 ? "" : (profitByZone.SaleNet.HasValue) ?
                            ReportHelpers.FormatNormalNumberDigit(profitByZone.SaleNet) : "0.00";

                        MeasureValue costNet;
                        analyticRecord.MeasureValues.TryGetValue("CostNet", out costNet);
                        profitByZone.CostNet = Convert.ToDouble(costNet == null ? 0.0 : costNet.Value ?? 0.0);
                        profitByZone.CostNetFormated = (profitByZone.CostNet.HasValue)
                            ? ReportHelpers.FormatNormalNumberDigit(profitByZone.CostNet)
                            : "0.00";

                        MeasureValue saleDuration;
                        analyticRecord.MeasureValues.TryGetValue("SaleDuration", out saleDuration);
                        profitByZone.SaleDuration = Convert.ToDecimal(saleDuration.Value ?? 0.0);
                        profitByZone.SaleDurationFormated = profitByZone.SaleNet == 0 ? "" : (profitByZone.SaleDuration.HasValue) ?
                            ReportHelpers.FormatNormalNumberDigit(profitByZone.SaleDuration) : "0.00";

                        MeasureValue costDuration;
                        analyticRecord.MeasureValues.TryGetValue("CostDuration", out costDuration);
                        profitByZone.CostDuration = Convert.ToDecimal(costDuration.Value ?? 0.0);
                        profitByZone.CostDurationFormated = ReportHelpers.FormatNormalNumberDigit(profitByZone.CostDuration);

                        MeasureValue durationInMinutes;
                        analyticRecord.MeasureValues.TryGetValue("DurationNet", out durationInMinutes);
                        profitByZone.DurationNet = Convert.ToDecimal(durationInMinutes.Value ?? 0.0);
                        profitByZone.DurationNetFormated = ReportHelpers.FormatNormalNumberDigit(profitByZone.DurationNet);

                        MeasureValue calls;
                        analyticRecord.MeasureValues.TryGetValue("NumberOfCalls", out calls);
                        profitByZone.Calls = Convert.ToInt32(calls.Value ?? 0.0);

                        profitByZone.Profit = profitByZone.SaleNet == 0
                            ? ""
                            : ReportHelpers.FormatNormalNumberDigit((!profitByZone.SaleNet.HasValue) ? 0 : profitByZone.SaleNet - profitByZone.CostNet);
                        profitByZone.ProfitSum = (!profitByZone.SaleNet.HasValue || profitByZone.SaleNet == 0)
                            ? 0
                            : profitByZone.SaleNet - profitByZone.CostNet;
                        profitByZone.ProfitPercentage = profitByZone.SaleNet == 0
                            ? ""
                            : (profitByZone.SaleNet.HasValue)
                                ? ReportHelpers.FormatNumberPercentage((1 - profitByZone.CostNet / profitByZone.SaleNet))
                                : "-100%";

                        listProfitByZone.Add(profitByZone);
                    }
                }

            Dictionary<string, System.Collections.IEnumerable> dataSources = new Dictionary<string, System.Collections.IEnumerable>();
            dataSources.Add("ZoneProfit", listProfitByZone);
            return dataSources;
        }

        public Dictionary<string, RdlcParameter> GetRdlcReportParameters(ReportParameters parameters)
        {
            Dictionary<string, RdlcParameter> list = new Dictionary<string, RdlcParameter>();
            list.Add("GroupByCustomer", new RdlcParameter() { Value = parameters.GroupByCustomer.ToString(), IsVisible = false });
            list.Add("FromDate", new RdlcParameter { Value = parameters.FromTime.ToString(), IsVisible = true });
            list.Add("ToDate", new RdlcParameter { Value = parameters.ToTime.HasValue ? parameters.ToTime.ToString():null, IsVisible = true });
            list.Add("Title", new RdlcParameter { Value = "Profit By Zone", IsVisible = true });
            list.Add("Currency", new RdlcParameter { Value = parameters.CurrencyDescription, IsVisible = true });
            list.Add("LogoPath", new RdlcParameter { Value = "logo", IsVisible = true });
            list.Add("Customer", new RdlcParameter { Value = ReportHelpers.GetCarrierName(parameters.CustomersId, "Customers"), IsVisible = true });
            list.Add("Supplier", new RdlcParameter { Value = ReportHelpers.GetCarrierName(parameters.SuppliersId, "Suppliers"), IsVisible = true });
            list.Add("DigitRate", new RdlcParameter { Value = ReportHelpers.GetLongNumberDigit(), IsVisible = true });
            list.Add("Digit", new RdlcParameter { Value = ReportHelpers.GetNormalNumberDigit(), IsVisible = true });

            return list;
        }
    }
}
