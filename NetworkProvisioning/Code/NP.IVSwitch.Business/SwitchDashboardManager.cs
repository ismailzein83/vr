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
using Vanrise.Common;

namespace NP.IVSwitch.Business
{
    public class SwitchDashboardManager
    {
        public LiveDashboardResult GetSwitchDashboardManagerResult()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetSwitchDashboardManagerResult", 
                new Vanrise.Caching.TimeExpirableCacheExpirationChecker(TimeSpan.FromSeconds(2)),
                () =>
            {
                List<Action> actions = new List<Action>();

                AnalyticSummaryBigResult<AnalyticRecord> lastDistributionRecords = null;
                AnalyticSummaryBigResult<AnalyticRecord> topCustomersRecords = null;
                AnalyticSummaryBigResult<AnalyticRecord> topSuppliersRecords = null;
                AnalyticSummaryBigResult<AnalyticRecord> topZonesRecords = null;
                actions.Add(() => lastDistributionRecords = QueryLastDistribution());
                actions.Add(() => topCustomersRecords = QueryTopCustomers());
                actions.Add(() => topSuppliersRecords = QueryTopSuppliers());
                actions.Add(() => topZonesRecords = QueryTopZones());
                Parallel.ForEach(actions, (a) => a());
                LiveDashboardResult liveDashboardResult = new LiveDashboardResult();
                BuildTopCustomersResult(liveDashboardResult, topCustomersRecords);
                BuildTopSuppliersResult(liveDashboardResult, topSuppliersRecords);
                BuildTopZonesResult(liveDashboardResult, topZonesRecords);
                BuildLastDistributionResult(liveDashboardResult, lastDistributionRecords);
                return liveDashboardResult;
                });
            
        }

        private void BuildLastDistributionResult(LiveDashboardResult liveDashboardResult, AnalyticSummaryBigResult<AnalyticRecord> lastDistributionRecords)
        {
            if (lastDistributionRecords.Data != null)
            {
                ConvertAnalyticDataToLastDitributionResult(liveDashboardResult, lastDistributionRecords.Data);
            }
        }

        private AnalyticSummaryBigResult<AnalyticRecord> QueryLastDistribution()
        {
            List<string> listMeasures = new List<string> { "Attempts", "CountConnected", "PercConnected", "ACD", "PDDInSec", "TotalDuration" };
            List<string> listDimensions = new List<string> { "DurationRange" };
            var fromDate = DateTime.Today.AddYears(-1);
            var toDate = DateTime.Now.AddYears(1);

            var analyticResult = GetFilteredRecords(listDimensions, listMeasures, fromDate, toDate, false);
            return analyticResult;
        }

        private void ConvertAnalyticDataToLastDitributionResult(LiveDashboardResult liveDashboardResult, IEnumerable<AnalyticRecord> analyticRecords)
        {
            if (analyticRecords != null)
            {
                liveDashboardResult.LastDistributionResult = new LastDistributionResult();
                liveDashboardResult.LastDistributionResult.DistributionResults = new List<DistributionResult>();
                var durationChoices = GetDurationRangeChoices();
                Dictionary<Object, DistributionResult> distributionResults = new Dictionary<Object, DistributionResult>();
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
                    distributionResults.Add(durationRange.Value,
                        new DistributionResult
                    {
                        DurationRange = durationRange.Name,
                        CountConnected = Convert.ToInt32(countConnected.Value),
                        Attempts = Convert.ToInt32(attempts.Value),
                        PercConnected = Convert.ToDecimal(percConnected == null ? 0.0 : percConnected.Value ?? 0.0),
                        ACD = Convert.ToDecimal(acd == null ? 0.0 : acd.Value ?? 0.0),
                        PDDInSec = (decimal?)pDDInSec.Value,
                        TotalDuration = Convert.ToDecimal(totalDuration == null ? 0.0 : totalDuration.Value ?? 0.0),
                        ResponseDate = DateTime.Now
                    });
                    #endregion
                }
                foreach (var durationChoice in durationChoices.OrderByDescending(itm => itm.Value))
                {
                    if (durationChoice.Value == 0)//Not Connected calls
                        continue;
                    DistributionResult matchItem;
                    if (distributionResults.TryGetValue(durationChoice.Value, out matchItem))
                        liveDashboardResult.LastDistributionResult.DistributionResults.Add(matchItem);
                    else
                        liveDashboardResult.LastDistributionResult.DistributionResults.Add(new DistributionResult
                            {
                                 DurationRange = durationChoice.Text                                  
                            });
                }
            }
        }

        private void BuildTopCustomersResult(LiveDashboardResult liveDashboardResult, AnalyticSummaryBigResult<AnalyticRecord> topCustomersRecords)
        {
            
            if (topCustomersRecords.Data != null)
            {
                ConvertAnalyticDataToTopCustomersResult(liveDashboardResult, topCustomersRecords.Data);

                MeasureValue attempts = GetMeasureValue(topCustomersRecords.Summary, "Attempts");
                MeasureValue countConnected = GetMeasureValue(topCustomersRecords.Summary, "CountConnected");
                MeasureValue percConnected = GetMeasureValue(topCustomersRecords.Summary, "PercConnected");
                MeasureValue acd = GetMeasureValue(topCustomersRecords.Summary, "ACD");
                MeasureValue pDDInSec = GetMeasureValue(topCustomersRecords.Summary, "PDDInSec");
                MeasureValue totalDuration = GetMeasureValue(topCustomersRecords.Summary, "TotalDuration");

                liveDashboardResult.LiveSummaryResult = new LiveSummaryResult
                {
                    CountConnected = Convert.ToInt32(countConnected.Value),
                    Attempts = Convert.ToInt32(attempts.Value),
                    PercConnected = Convert.ToDecimal(percConnected == null ? 0.0 : percConnected.Value ?? 0.0),
                    ACD = Decimal.Round(Convert.ToDecimal(acd == null ? 0.0 : acd.Value), 2),
                    PDDInSec = (decimal?)pDDInSec.Value,
                    TotalDuration = Decimal.Round(Convert.ToDecimal(totalDuration == null ? 0.0 : totalDuration.Value ?? 0.0), 2),
                    ResponseDate = DateTime.Now
                };

            }
        }

        private AnalyticSummaryBigResult<AnalyticRecord> QueryTopCustomers()
        {
            List<string> listMeasures = new List<string> { "Attempts", "CountConnected", "PercConnected", "ACD", "PDDInSec", "TotalDuration" };
            List<string> listDimensions = new List<string> { "Customer" };
            var fromDate = DateTime.Today.AddYears(-1);
            var toDate = DateTime.Now.AddYears(1);

            var analyticResult = GetFilteredRecords(listDimensions, listMeasures, fromDate, toDate, true);
            return analyticResult;
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
                        CustomerName = customer.Name !=null?customer.Name:"Null",
                        CountConnected = Convert.ToInt32(countConnected.Value),
                        Attempts = Convert.ToInt32(attempts.Value),
                        PercConnected = Convert.ToDecimal(percConnected == null ? 0.0 : percConnected.Value ?? 0.0),
                        ACD = Convert.ToDecimal(acd == null ? 0.0 : acd.Value ?? 0.0),
                        PDDInSec = (decimal?)pDDInSec.Value,
                        TotalDuration = Convert.ToDecimal(totalDuration == null ? 0.0 : totalDuration.Value ?? 0.0),
                        ResponseDate = DateTime.Now
                    });

                    #endregion
                }
            }
        }
        private void BuildTopSuppliersResult(LiveDashboardResult liveDashboardResult, AnalyticSummaryBigResult<AnalyticRecord> topSuppliersRecords)
        {
            if (topSuppliersRecords.Data != null)
            {
                ConvertAnalyticDataToTopSuppliersResult(liveDashboardResult, topSuppliersRecords.Data);
            }
        }

        private AnalyticSummaryBigResult<AnalyticRecord> QueryTopSuppliers()
        {
            List<string> listMeasures = new List<string> { "Attempts", "CountConnected", "PercConnected", "ACD", "PDDInSec", "TotalDuration" };
            List<string> listDimensions = new List<string> { "Supplier" };
            var fromDate = DateTime.Today.AddYears(-1);
            var toDate = DateTime.Now.AddYears(1);

            var analyticResult = GetFilteredRecords(listDimensions, listMeasures, fromDate, toDate, false);
            return analyticResult;
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
                        SupplierName = supplier.Name != null ? supplier.Name : "Null",
                        CountConnected = Convert.ToInt32(countConnected.Value),
                        Attempts = Convert.ToInt32(attempts.Value),
                        PercConnected = Convert.ToDecimal(percConnected == null ? 0.0 : percConnected.Value ?? 0.0),
                        ACD = Convert.ToDecimal(acd == null ? 0.0 : acd.Value ?? 0.0),
                        PDDInSec = (decimal?)pDDInSec.Value,
                        TotalDuration = Convert.ToDecimal(totalDuration == null ? 0.0 : totalDuration.Value ?? 0.0),
                        ResponseDate = DateTime.Now
                    });

                    #endregion
                }
            }
        }

        private void BuildTopZonesResult(LiveDashboardResult liveDashboardResult, AnalyticSummaryBigResult<AnalyticRecord> topZonesRecords)
        {
            if (topZonesRecords.Data != null)
            {
                ConvertAnalyticDataToTopZonesResult(liveDashboardResult, topZonesRecords.Data);
            }
        }

        private AnalyticSummaryBigResult<AnalyticRecord> QueryTopZones()
        {
            List<string> listMeasures = new List<string> { "Attempts", "CountConnected", "PercConnected", "ACD", "PDDInSec", "TotalDuration" };
            List<string> listDimensions = new List<string> { "Zone" };
            var fromDate = DateTime.Today.AddYears(-1);
            var toDate = DateTime.Now.AddYears(1);

            var analyticResult = GetFilteredRecords(listDimensions, listMeasures, fromDate, toDate, false);
            return analyticResult;
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
                        ZoneName = zone.Name != null ? zone.Name : "Null",
                        CountConnected = Convert.ToInt32(countConnected.Value),
                        Attempts = Convert.ToInt32(attempts.Value),
                        PercConnected = Convert.ToDecimal(percConnected == null ? 0.0 : percConnected.Value ?? 0.0),
                        ACD = Convert.ToDecimal(acd == null ? 0.0 : acd.Value ?? 0.0),
                        PDDInSec = (decimal?)pDDInSec.Value,
                        TotalDuration = Convert.ToDecimal(totalDuration == null ? 0.0 : totalDuration.Value ?? 0.0),
                        ResponseDate = DateTime.Now
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
                    TableId = GetAnalyticTableId(),
                    FromTime = fromDate,
                    ToTime = toDate,
                    ParentDimensions = new List<string>(),
                    Filters = new List<DimensionFilter>(),
                    OrderType = AnalyticQueryOrderType.ByAllMeasures,
                    WithSummary = withSummary,
                    TopRecords = 7
                },
                SortByColumnName = "DimensionValues[0].Name"
            };
            return analyticManager.GetFilteredRecords(analyticQuery) as Vanrise.Analytic.Entities.AnalyticSummaryBigResult<AnalyticRecord>;
        }

        private static int GetAnalyticTableId()
        {
            return 10;
        }
        
        public List<Vanrise.GenericData.MainExtensions.DataRecordFields.Choice> GetDurationRangeChoices()
        {
            Vanrise.Analytic.Business.AnalyticItemConfigManager itemConfig = new AnalyticItemConfigManager();
            AnalyticDimension durationRecordDimension = itemConfig.GetDimensions(GetAnalyticTableId()).GetRecord("DurationRange");
            durationRecordDimension.ThrowIfNull("durationRecordDimension", "DurationRange");
            durationRecordDimension.Config.ThrowIfNull("durationRecordDimension.Config", "DurationRange");
            var fldChoiceType = durationRecordDimension.Config.FieldType.CastWithValidate<Vanrise.GenericData.MainExtensions.DataRecordFields.FieldChoicesType>("durationRecordDimension.Config.FieldType", "DurationRange");
            return fldChoiceType.Choices;
        }

        private MeasureValue GetMeasureValue(AnalyticRecord analyticRecord, string measureName)
        {
            MeasureValue measureValue;
            analyticRecord.MeasureValues.TryGetValue(measureName, out measureValue);
            return measureValue;
        }

        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {

        }

    }
}
