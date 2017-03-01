using NP.IVSwitch.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Analytic.Business;
using Vanrise.Analytic.Entities;
using Vanrise.Entities;
using Vanrise.GenericData.Entities;

namespace NP.IVSwitch.Business
{
    public class SwitchDashboardManager
    {
        public LiveDashboardResult GetSwitchDashboardManagerResult()
        {
         
            LiveDashboardResult liveDashboardResult = new LiveDashboardResult();
            BuildTopCustomersResult(liveDashboardResult);
            BuildTopSuppliersResult(liveDashboardResult);
            BuildTopZonesResult(liveDashboardResult);
            BuildLastDistributionResult(liveDashboardResult);
             return liveDashboardResult;
        }

        private void BuildLastDistributionResult(LiveDashboardResult liveDashboardResult)
        {
            List<string> listMeasures = new List<string> { "CountConnected", "Attempts", "PercConnected", "ACD", "PDDInSec", "TotalDuration" };
            List<string> listDimensions = new List<string> { "DurationRange" };
            var fromDate = DateTime.Parse("09/23/1990");
            var toDate = DateTime.Now.AddYears(2);

            var analyticResult = GetFilteredRecords(listDimensions, listMeasures, fromDate, toDate,false);
            if (analyticResult.Data != null)
            {
                ConvertAnalyticDataToLastDitributionResult(liveDashboardResult, analyticResult.Data);
            }
        }
        private void ConvertAnalyticDataToLastDitributionResult(LiveDashboardResult liveDashboardResult, IEnumerable<AnalyticRecord> analyticRecords)
        {
            if (analyticRecords != null)
            {
                liveDashboardResult.LastDistributionResult = new LastDistributionResult();
                liveDashboardResult.LastDistributionResult.DistributionResults = new List<DistributionResult>();
                foreach (var analyticRecord in analyticRecords)
                {

                    #region ReadDataFromAnalyticResult
                    DimensionValue durationRange = analyticRecord.DimensionValues.ElementAtOrDefault(0);
                    MeasureValue attempts = GetMeasureValue(analyticRecord, "Attempts");
                    MeasureValue countConnected = GetMeasureValue(analyticRecord, "CountConnected");
                    MeasureValue percConnected = GetMeasureValue(analyticRecord, "PercConnected");
                    MeasureValue acd = GetMeasureValue(analyticRecord, "ACD");
                    MeasureValue pDDInSec = GetMeasureValue(analyticRecord, "PDDInSec");
                    MeasureValue totalDuration = GetMeasureValue(analyticRecord, "TotalDuration");
                    if (durationRange.Name != "Not Connected")
                    {

                        liveDashboardResult.LastDistributionResult.DistributionResults.Add(new DistributionResult
                        {
                            CountConnected = Convert.ToInt32(countConnected.Value),
                            DurationRange = durationRange.Name,
                            Attempts = Convert.ToInt32(attempts.Value),
                            PercConnected = Convert.ToDecimal(percConnected.Value),
                            ACD = Convert.ToDecimal(acd.Value),
                            PDDInSec = Convert.ToDecimal(pDDInSec.Value),
                            TotalDuration = Convert.ToDecimal(totalDuration.Value),

                        });
                    }

                    #endregion
                }
            }
        }

        private void BuildTopCustomersResult(LiveDashboardResult liveDashboardResult)
        {
            List<string> listMeasures = new List<string> { "CountConnected", "Attempts", "PercConnected", "ACD", "PDDInSec", "TotalDuration" };
            List<string> listDimensions = new List<string> { "Customer" };
            var fromDate = DateTime.Parse("09/23/1990");
            var toDate = DateTime.Now.AddYears(2);

            var analyticResult = GetFilteredRecords(listDimensions, listMeasures, fromDate, toDate,true);
            if (analyticResult.Data != null)
            {
                ConvertAnalyticDataToTopCustomersResult(liveDashboardResult, analyticResult.Data);

                MeasureValue attempts = GetMeasureValue(analyticResult.Summary, "Attempts");
                MeasureValue countConnected = GetMeasureValue(analyticResult.Summary, "CountConnected");
                MeasureValue percConnected = GetMeasureValue(analyticResult.Summary, "PercConnected");
                MeasureValue acd = GetMeasureValue(analyticResult.Summary, "ACD");
                MeasureValue pDDInSec = GetMeasureValue(analyticResult.Summary, "PDDInSec");
                MeasureValue totalDuration = GetMeasureValue(analyticResult.Summary, "TotalDuration");

                liveDashboardResult.LiveSummaryResult = new LiveSummaryResult
                {
                    CountConnected = Convert.ToInt32(countConnected.Value),
                    Attempts = Convert.ToInt32(attempts.Value),
                    PercConnected = Convert.ToDecimal(percConnected.Value),
                    ACD = Convert.ToDecimal(acd.Value),
                    PDDInSec = Convert.ToDecimal(pDDInSec.Value),
                    TotalDuration = Convert.ToDecimal(totalDuration.Value),
                };

            }
        }
        private void ConvertAnalyticDataToTopCustomersResult(LiveDashboardResult liveDashboardResult, IEnumerable<AnalyticRecord> analyticRecords)
        {
            if (analyticRecords != null)
            {
                liveDashboardResult.TopCustomersResult = new TopCustomersResult();
                liveDashboardResult.TopCustomersResult.CustomerResults = new List<CustomerResult>();
                foreach (var analyticRecord in analyticRecords)
                {

                    #region ReadDataFromAnalyticResult
                    DimensionValue customer = analyticRecord.DimensionValues.ElementAtOrDefault(0);
                    MeasureValue attempts = GetMeasureValue(analyticRecord, "Attempts");
                    MeasureValue countConnected = GetMeasureValue(analyticRecord, "CountConnected");
                    MeasureValue percConnected = GetMeasureValue(analyticRecord, "PercConnected");
                    MeasureValue acd = GetMeasureValue(analyticRecord, "ACD");
                    MeasureValue pDDInSec = GetMeasureValue(analyticRecord, "PDDInSec");
                    MeasureValue totalDuration = GetMeasureValue(analyticRecord, "TotalDuration");


                    liveDashboardResult.TopCustomersResult.CustomerResults.Add(new CustomerResult
                    {
                        CountConnected = Convert.ToInt32(countConnected.Value),
                        CustomerId = Convert.ToInt32(customer.Value),
                        CustomerName = customer.Name,
                        Attempts = Convert.ToInt32(attempts.Value),
                        PercConnected = Convert.ToDecimal(percConnected.Value),
                        ACD = Convert.ToDecimal(acd.Value),
                        PDDInSec = Convert.ToDecimal(pDDInSec.Value),
                        TotalDuration = Convert.ToDecimal(totalDuration.Value),
                    });

                    #endregion
                }
            }
        }
        private void BuildTopSuppliersResult(LiveDashboardResult liveDashboardResult)
        {
            List<string> listMeasures = new List<string> { "CountConnected", "Attempts", "PercConnected", "ACD", "PDDInSec", "TotalDuration" };
            List<string> listDimensions = new List<string> { "Supplier" };
            var fromDate = DateTime.Parse("09/23/1990");
            var toDate = DateTime.Now.AddYears(2);

            var analyticResult = GetFilteredRecords(listDimensions, listMeasures, fromDate, toDate,false);
            if (analyticResult.Data != null)
            {
                ConvertAnalyticDataToTopSuppliersResult(liveDashboardResult, analyticResult.Data);
            }
        }
        private void ConvertAnalyticDataToTopSuppliersResult(LiveDashboardResult liveDashboardResult, IEnumerable<AnalyticRecord> analyticRecords)
        {
            if (analyticRecords != null)
            {
                liveDashboardResult.TopSuppliersResult = new TopSuppliersResult();
                liveDashboardResult.TopSuppliersResult.SupplierResults = new List<SupplierResult>();
                foreach (var analyticRecord in analyticRecords)
                {

                    #region ReadDataFromAnalyticResult
                    DimensionValue supplier = analyticRecord.DimensionValues.ElementAtOrDefault(0);
                    MeasureValue attempts = GetMeasureValue(analyticRecord, "Attempts");
                    MeasureValue countConnected = GetMeasureValue(analyticRecord, "CountConnected");
                    MeasureValue percConnected = GetMeasureValue(analyticRecord, "PercConnected");
                    MeasureValue acd = GetMeasureValue(analyticRecord, "ACD");
                    MeasureValue pDDInSec = GetMeasureValue(analyticRecord, "PDDInSec");
                    MeasureValue totalDuration = GetMeasureValue(analyticRecord, "TotalDuration");

                    liveDashboardResult.TopSuppliersResult.SupplierResults.Add(new SupplierResult
                    {
                        CountConnected = Convert.ToInt32(countConnected.Value),
                        SupplierId = Convert.ToInt32(supplier.Value),
                        SupplierName = supplier.Name,
                        Attempts = Convert.ToInt32(attempts.Value),
                        PercConnected = Convert.ToDecimal(percConnected.Value),
                        ACD = Convert.ToDecimal(acd.Value),
                        PDDInSec = Convert.ToDecimal(pDDInSec.Value),
                        TotalDuration = Convert.ToDecimal(totalDuration.Value),
                    });

                    #endregion
                }
            }
        }

        private void BuildTopZonesResult(LiveDashboardResult liveDashboardResult)
        {
            List<string> listMeasures = new List<string> { "CountConnected", "Attempts", "PercConnected", "ACD", "PDDInSec", "TotalDuration" };
            List<string> listDimensions = new List<string> { "Zone" };
            var fromDate = DateTime.Parse("09/23/1990");
            var toDate = DateTime.Now.AddYears(2);

            var analyticResult = GetFilteredRecords(listDimensions, listMeasures, fromDate, toDate,false);
            if (analyticResult.Data != null)
            {
                ConvertAnalyticDataToTopZonesResult(liveDashboardResult, analyticResult.Data);
            }
        }
        private void ConvertAnalyticDataToTopZonesResult(LiveDashboardResult liveDashboardResult, IEnumerable<AnalyticRecord> analyticRecords)
        {
            if (analyticRecords != null)
            {
                liveDashboardResult.TopZonesResult = new TopZonesResult();
                liveDashboardResult.TopZonesResult.ZoneResults = new List<ZoneResult>();
                foreach (var analyticRecord in analyticRecords)
                {

                    #region ReadDataFromAnalyticResult
                    DimensionValue zone = analyticRecord.DimensionValues.ElementAtOrDefault(0);
                    MeasureValue attempts = GetMeasureValue(analyticRecord, "Attempts");
                    MeasureValue countConnected = GetMeasureValue(analyticRecord, "CountConnected");
                    MeasureValue percConnected = GetMeasureValue(analyticRecord, "PercConnected");
                    MeasureValue acd = GetMeasureValue(analyticRecord, "ACD");
                    MeasureValue pDDInSec = GetMeasureValue(analyticRecord, "PDDInSec");
                    MeasureValue totalDuration = GetMeasureValue(analyticRecord, "TotalDuration");


                    liveDashboardResult.TopZonesResult.ZoneResults.Add(new ZoneResult
                    {
                        CountConnected = Convert.ToInt32(countConnected.Value),
                        ZoneName = zone.Name,
                        Attempts = Convert.ToInt32(attempts.Value),
                        PercConnected = Convert.ToDecimal(percConnected.Value),
                        ACD = Convert.ToDecimal(acd.Value),
                        PDDInSec = Convert.ToDecimal(pDDInSec.Value),
                        TotalDuration = Convert.ToDecimal(totalDuration.Value),
                    });

                    #endregion
                }
            }
        }

        private AnalyticSummaryBigResult<AnalyticRecord> GetFilteredRecords(List<string> listDimensions, List<string> listMeasures,DateTime fromDate, DateTime? toDate,bool withSummary)
        {
            AnalyticManager analyticManager = new AnalyticManager();
            Vanrise.Entities.DataRetrievalInput<AnalyticQuery> analyticQuery = new DataRetrievalInput<AnalyticQuery>()
            {
                Query = new AnalyticQuery()
                {
                    DimensionFields = listDimensions,
                    MeasureFields = listMeasures,
                    TableId = 10,
                    FromTime = fromDate,
                    ToTime = toDate,
                    ParentDimensions = new List<string>(),
                    Filters = new List<DimensionFilter>(),
                    OrderType = AnalyticQueryOrderType.ByAllMeasures,
                    WithSummary = withSummary,
                    TopRecords = 10
                },
                SortByColumnName = "DimensionValues[0].Name"
            };
            return analyticManager.GetFilteredRecords(analyticQuery) as Vanrise.Analytic.Entities.AnalyticSummaryBigResult<AnalyticRecord>;
        }

        private MeasureValue GetMeasureValue(AnalyticRecord analyticRecord, string measureName)
        {
            MeasureValue measureValue;
            analyticRecord.MeasureValues.TryGetValue(measureName, out measureValue);
            return measureValue;
        }

    }
}
