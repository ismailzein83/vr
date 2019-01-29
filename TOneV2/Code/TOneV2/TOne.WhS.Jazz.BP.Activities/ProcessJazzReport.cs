using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using TOne.WhS.Jazz.Entities;
using TOne.WhS.Jazz.Business;
using Vanrise.Analytic.Business;
using Vanrise.Entities;
using Vanrise.Analytic.Entities;
using Vanrise.GenericData.Entities;
using TOne.BusinessEntity.Business;

namespace TOne.WhS.Jazz.BP.Activities
{

    public sealed class ProcessJazzReport : CodeActivity
    {
        [RequiredArgument]
        public InArgument<JazzReportDefinition> ReportDefinition { get; set; }

        [RequiredArgument]
        public InArgument<DateTime> FromDate { get; set; }

        [RequiredArgument]
        public InArgument<DateTime> ToDate { get; set; }

        [RequiredArgument]
        public OutArgument<JazzTransactionsReport> JazzTransactionsReport { get; set; }

        [RequiredArgument]
        public OutArgument<JazzReport> JazzReport { get; set; }

        public AnalyticSummaryBigResult<AnalyticRecord> GetFilteredRecords(List<string> listDimensions, List<string> listMeasures, DateTime fromDate, DateTime toDate, RecordFilter recordFilter)
        {

            AnalyticManager analyticManager = new AnalyticManager();
            Vanrise.Entities.DataRetrievalInput<AnalyticQuery> analyticQuery = new DataRetrievalInput<AnalyticQuery>()
            {
                Query = new AnalyticQuery()
                {
                    DimensionFields = listDimensions,
                    MeasureFields = listMeasures,
                    TableId = Guid.Parse("4c1aaa1b-675b-420f-8e60-26b0747ca79b"),
                    FromTime = fromDate,
                    ToTime = toDate,

                },
                SortByColumnName = "DimensionValues[0].Name"
            };
            RecordFilterGroup recordFilterGroup = new RecordFilterGroup();
            recordFilterGroup.Filters = new List<RecordFilter>();
            recordFilterGroup.Filters.Add(recordFilter);
            analyticQuery.Query.FilterGroup = recordFilterGroup;

            return analyticManager.GetFilteredRecords(analyticQuery) as Vanrise.Analytic.Entities.AnalyticSummaryBigResult<AnalyticRecord>;
        }
        private MeasureValue GetMeasureValue(AnalyticRecord analyticRecord, string measureName)
        {
            MeasureValue measureValue;
            analyticRecord.MeasureValues.TryGetValue(measureName, out measureValue);
            return measureValue;
        }
        private List<JazzReportData> BuildReportItemSetNameFromAnalytic(IEnumerable<AnalyticRecord> analyticRecords, JazzReportDefinition reportDefinition)
        {
            List<JazzReportData> jazzReportsData = null;

            if (analyticRecords != null)
            {
                jazzReportsData = new List<JazzReportData>();
                foreach (var analyticRecord in analyticRecords)
                {
                    #region ReadDataFromAnalyticResult
                    DimensionValue carrierAccountId = analyticRecord.DimensionValues.ElementAtOrDefault(0);
                    MeasureValue netValue = null;
                    MeasureValue durationValue = null;

                    if (reportDefinition.Direction.Equals(ReportDefinitionDirectionEnum.In))
                    {
                        netValue = GetMeasureValue(analyticRecord, "SaleNet");
                        durationValue = GetMeasureValue(analyticRecord, "SaleDuration");
                    }
                    else
                    {
                        netValue = GetMeasureValue(analyticRecord, "CostNet");
                        durationValue = GetMeasureValue(analyticRecord, "CostDuration");
                    }
                    #endregion

                   
                        CarrierAccountManager carrierAccountManager = new CarrierAccountManager();
                           var jazzReportData = new JazzReportData
                        {
                            CarrierAccountId = Convert.ToInt32(carrierAccountId.Value),
                            CarrierAccountName = carrierAccountManager.GetCarrierAccount(carrierAccountId.Name).ProfileName,
                            Duration = Convert.ToDecimal(durationValue.Value ?? 0.0),
                            Amount = Convert.ToDecimal(netValue.Value ?? 0.0),
                        };
                        if (reportDefinition.Settings != null)
                        {
                            if (reportDefinition.Settings.DivideByMarket)
                            {
                                if (reportDefinition.Settings.MarketSettings != null && reportDefinition.Settings.MarketSettings.MarketOptions != null && reportDefinition.Settings.MarketSettings.MarketOptions.Count > 0)
                                {
                                    jazzReportData.Markets = new List<JazzReportMarket>();
                                    MarketManager _marketManager = new MarketManager();
                                    foreach (var market in reportDefinition.Settings.MarketSettings.MarketOptions)
                                    {
                                        JazzReportMarket reportMarket = new JazzReportMarket
                                        {
                                            MarketId = market.MarketCodeId,
                                            MarketName = _marketManager.GetMarketById(market.MarketCodeId).Name,
                                            MarketValue = market.Percentage * jazzReportData.Amount
                                        };
                                        if (reportDefinition.Settings.DivideByRegion && reportDefinition.Settings.RegionSettings != null && reportDefinition.Settings.RegionSettings.RegionOptions != null && reportDefinition.Settings.RegionSettings.RegionOptions.Count > 0)
                                        {
                                            reportMarket.Regions = new List<JazzReportRegion>();
                                            RegionManager _regionManager = new RegionManager();
                                            foreach (var region in reportDefinition.Settings.RegionSettings.RegionOptions)
                                                reportMarket.Regions.Add(new JazzReportRegion
                                                {
                                                    RegionId = region.RegionCodeId,
                                                    RegionName = _regionManager.GetRegionById(region.RegionCodeId).Name,
                                                    RegionValue = region.Percentage * reportMarket.MarketValue
                                                });
                                        }
                                        jazzReportData.Markets.Add(reportMarket);
                                    }
                                }
                            }
                            else
                            {
                                if (reportDefinition.Settings.RegionSettings != null && reportDefinition.Settings.RegionSettings.RegionOptions != null && reportDefinition.Settings.RegionSettings.RegionOptions.Count > 0)
                                {
                                    jazzReportData.Regions = new List<JazzReportRegion>();
                                    RegionManager _regionManager = new RegionManager();
                                    foreach (var region in reportDefinition.Settings.RegionSettings.RegionOptions)
                                        jazzReportData.Regions.Add(new JazzReportRegion
                                        {
                                            RegionId = region.RegionCodeId,
                                            RegionName = _regionManager.GetRegionById(region.RegionCodeId).Name,
                                            RegionValue = region.Percentage * jazzReportData.Amount
                                        });
                                }
                            }

                        }
                        jazzReportsData.Add(jazzReportData);
                    }
                }
            
            return jazzReportsData;
        }

        protected override void Execute(CodeActivityContext context)
        {
            JazzReport report = null;
            var reportDefinition = ReportDefinition.Get(context);
            var fromDate = FromDate.Get(context);
            var toDate = ToDate.Get(context);
            RecordFilter recordFilter = null;
            if (reportDefinition != null && reportDefinition.Settings != null)
                recordFilter = reportDefinition.Settings.ReportFilter;
            List<string> dimensions =null;
            List<string> measures = null;
            if (reportDefinition.Direction.Equals(ReportDefinitionDirectionEnum.In))
            {
                dimensions = new List<string>{ "Customer" };
                measures = new List<string> { "SaleNet", "SaleDuration" };

            }
            else
            {
                dimensions= new List<string> { "Supplier" };
                measures = new List<string> { "CostNet", "CostDuration" };

            }
            var analyticResult=GetFilteredRecords(dimensions, measures, fromDate, toDate, recordFilter);
            if (analyticResult != null && analyticResult.Data != null && analyticResult.Data.Count() != 0)
            {
                 report = new JazzReport();
                report.ReportName = reportDefinition.Name;
                report.ReportData = BuildReportItemSetNameFromAnalytic(analyticResult.Data,reportDefinition);
            }
            JazzReport.Set(context, report);
        }
    }
}
