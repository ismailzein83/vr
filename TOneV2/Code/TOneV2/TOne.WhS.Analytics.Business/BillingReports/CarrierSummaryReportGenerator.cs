using System;
using System.Collections.Generic;
using System.Linq;
using TOne.WhS.Analytics.Entities.BillingReport;
using TOne.WhS.Analytics.Entities.BillingReport.CarrierSummary;
using Vanrise.Analytic.Business;
using Vanrise.Analytic.Entities;
using Vanrise.Entities;

namespace TOne.WhS.Analytics.Business.BillingReports
{
    public class CarrierSummaryReportGenerator : IReportGenerator
    {
        public Dictionary<string, System.Collections.IEnumerable> GenerateDataSources(ReportParameters parameters)
        {
            AnalyticManager analyticManager = new AnalyticManager();
            BillingStatisticManager manager = new BillingStatisticManager();
            List<string> listGrouping = new List<string>();
            
            if(parameters.IsCost)
                listGrouping.Add("Customer");
            else
            {
                listGrouping.Add("Supplier");
            }
            Vanrise.Entities.DataRetrievalInput<AnalyticQuery> analyticQuery = new DataRetrievalInput<AnalyticQuery>()
            {
                Query = new AnalyticQuery()
                {
                    DimensionFields = listGrouping,
                    MeasureFields = new List<string>() { "SaleNet", "Calls", "SaleDuration", "CostDuration" },
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

            List<CarrierSummaryFormatted> listCarrierSummary = new List<CarrierSummaryFormatted>();

            var result = analyticManager.GetFilteredRecords(analyticQuery) as AnalyticSummaryBigResult<AnalyticRecord>;
            foreach (var analyticRecord in result.Data)
            {
                CarrierSummaryFormatted carrierSummary = new CarrierSummaryFormatted();

                var supplierValue = analyticRecord.DimensionValues[0];
                if (supplierValue != null)
                    carrierSummary.SupplierID = supplierValue.Name;


                MeasureValue saleNet;
                analyticRecord.MeasureValues.TryGetValue("SaleNet", out saleNet);

                carrierSummary.SaleNet = Convert.ToDouble(saleNet.Value ?? 0.0);
                carrierSummary.SaleNetFormatted = carrierSummary.SaleNet == 0 ? "" : (carrierSummary.SaleNet.HasValue) ?
                    manager.FormatNumberDigitRate(carrierSummary.SaleNet) : "0.00";

                MeasureValue costNet;
                analyticRecord.MeasureValues.TryGetValue("CostNet", out costNet);
                carrierSummary.CostNet = Convert.ToDouble(costNet.Value ?? 0.0);
                carrierSummary.CostNetFormatted = (carrierSummary.CostNet.HasValue)
                    ? manager.FormatNumberDigitRate(carrierSummary.CostNet)
                    : "0.00";

                MeasureValue saleDuration;
                analyticRecord.MeasureValues.TryGetValue("SaleDuration", out saleDuration);
                carrierSummary.SaleDuration = Convert.ToDecimal(saleDuration.Value ?? 0.0);
                carrierSummary.SaleDurationFormatted = carrierSummary.SaleNet == 0 ? "" : (carrierSummary.SaleDuration.HasValue) ?
                    manager.FormatNumber(carrierSummary.SaleDuration) : "0.00";

                MeasureValue costDuration;
                analyticRecord.MeasureValues.TryGetValue("CostDuration", out costDuration);
                carrierSummary.CostDuration = Convert.ToDecimal(costDuration.Value ?? 0.0);
                carrierSummary.CostDurationFormatted = manager.FormatNumberDigitRate(carrierSummary.CostDuration);

        

                listCarrierSummary.Add(carrierSummary);
            }

            Dictionary<string, System.Collections.IEnumerable> dataSources = new Dictionary<string, System.Collections.IEnumerable>();
            dataSources.Add("CarrierSummary", listCarrierSummary);
            return dataSources;
        }

        public Dictionary<string, RdlcParameter> GetRdlcReportParameters(ReportParameters parameters)
        {
            Dictionary<string, RdlcParameter> list = new Dictionary<string, RdlcParameter>();
            list.Add("FromDate", new RdlcParameter { Value = parameters.FromTime.ToString(), IsVisible = true });
            list.Add("ToDate", new RdlcParameter { Value = parameters.ToTime.ToString(), IsVisible = true });
            list.Add("Title", new RdlcParameter { Value = string.Format("{0} Summary", parameters.IsCost ? "Suppliers" : "Customers"), IsVisible = true });
            list.Add("Currency", new RdlcParameter { Value = parameters.CurrencyDescription, IsVisible = true });
            list.Add("LogoPath", new RdlcParameter { Value = "logo", IsVisible = true });
            list.Add("DigitRate", new RdlcParameter { Value = "2", IsVisible = true });

            return list;
        }
    }
}
