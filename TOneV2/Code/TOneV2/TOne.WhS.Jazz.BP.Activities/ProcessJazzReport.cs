using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using TOne.WhS.Jazz.Entities;
using TOne.WhS.Jazz.Business;
using Vanrise.Analytic.Business;
using Vanrise.Entities;
using Vanrise.Common;
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
        public OutArgument<List<JazzTransactionsReport>> JazzTransactionsReport { get; set; }

        [RequiredArgument]
        public OutArgument<JazzReport> JazzReport { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            JazzReport report = null;
            List<JazzTransactionsReport> transactionsReports = null;
            var reportDefinition = ReportDefinition.Get(context);
            var fromDate = FromDate.Get(context);
            var toDate = ToDate.Get(context);
            RecordFilter recordFilter = null;
            if (reportDefinition.Settings != null && reportDefinition.Settings.ReportFilter != null)
                recordFilter = reportDefinition.Settings.ReportFilter;
            List<string> dimensions = null;
            List<string> measures = null;
            if (reportDefinition.Direction.Equals(ReportDefinitionDirectionEnum.In))
            {
                dimensions = new List<string> { "Customer" };
                measures = new List<string> { "SaleNet", "SaleDuration" };

            }
            else
            {
                dimensions = new List<string> { "Supplier" };
                measures = new List<string> { "CostNet", "CostDuration" };

            }
            var analyticResult = GetFilteredRecords(dimensions, measures, fromDate, toDate, recordFilter);
            if (analyticResult != null && analyticResult.Data != null && analyticResult.Data.Count() != 0)
            {
                var reportData = BuildReportItemSetNameFromAnalytic(analyticResult.Data, reportDefinition);

                report = new JazzReport
                {
                    ReportName = reportDefinition.Name,
                    Direction = reportDefinition.Direction,
                    ReportData = reportData
                };

                transactionsReports = GetTransactionReportsData(reportDefinition, reportData);

            }
            JazzReport.Set(context, report);
            JazzTransactionsReport.Set(context, transactionsReports);
        }

        private AnalyticSummaryBigResult<AnalyticRecord> GetFilteredRecords(List<string> listDimensions, List<string> listMeasures, DateTime fromDate, DateTime toDate, RecordFilter recordFilter)
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
            if (recordFilter != null)
            {
                RecordFilterGroup recordFilterGroup = new RecordFilterGroup();
                recordFilterGroup.Filters = new List<RecordFilter>();
                recordFilterGroup.Filters.Add(recordFilter);
                analyticQuery.Query.FilterGroup = recordFilterGroup;
            }
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
                    DimensionValue carrierAccount = analyticRecord.DimensionValues.ElementAtOrDefault(0);
                    MeasureValue netValue = null;
                    MeasureValue durationValue = null;

                    if (reportDefinition.Direction == ReportDefinitionDirectionEnum.In)
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

                   
                    var jazzReportData = new JazzReportData

                    {
                        CarrierAccountId = Convert.ToInt32(carrierAccount.Value),
                        CarrierAccountName = carrierAccount.Name,
                        Duration = Convert.ToDecimal(durationValue.Value ?? 0.0),
                        Amount = Convert.ToDecimal(netValue.Value ?? 0.0),
                    };

                    if (reportDefinition.Settings != null && reportDefinition.Settings.MarketSettings != null && reportDefinition.Settings.MarketSettings.MarketOptions != null && reportDefinition.Settings.MarketSettings.MarketOptions.Count > 0)
                    {
                        jazzReportData.Markets = new List<JazzReportMarket>();
                        MarketManager _marketManager = new MarketManager();
                        CustomerTypeManager _customerTypeManager = new CustomerTypeManager();
                        foreach (var market in reportDefinition.Settings.MarketSettings.MarketOptions)
                        {
                            JazzReportMarket reportMarket = new JazzReportMarket
                            {
                                MarketId = market.MarketCodeId,
                                MarketName = _marketManager.GetMarketName(market.MarketCodeId),
                                CustomerTypeId = market.CustomerTypeCodeId,
                                CustomerTypeName=_customerTypeManager.GetCustomerTypeName(market.CustomerTypeCodeId),
                                MarketValue = market.Percentage * jazzReportData.Amount/100,
                                Percentage =market.Percentage,
                            };
                            if (reportDefinition.Settings.RegionSettings != null && reportDefinition.Settings.RegionSettings.RegionOptions != null && reportDefinition.Settings.RegionSettings.RegionOptions.Count > 0)
                            {
                                reportMarket.Regions = new List<JazzReportRegion>();
                                RegionManager _regionManager = new RegionManager();
                                foreach (var region in reportDefinition.Settings.RegionSettings.RegionOptions)
                                {
                                    reportMarket.Regions.Add(new JazzReportRegion
                                    {
                                        RegionId = region.RegionCodeId,
                                        RegionName = _regionManager.GetRegionById(region.RegionCodeId).Name,
                                        RegionValue = region.Percentage * reportMarket.MarketValue/100,
                                        Percentage = region.Percentage,
                                    });
                                }
                            }
                            jazzReportData.Markets.Add(reportMarket);
                        }
                    }
                    jazzReportsData.Add(jazzReportData);
                }
            }
            return jazzReportsData;
        }
        private List<JazzTransactionsReport> GetTransactionReportsData(JazzReportDefinition reportDefinition, List<JazzReportData> reportsData)
        {

            TransactionTypeManager transactionTypeManager = new TransactionTypeManager();
            List<JazzTransactionsReport> transactionsReports = null;
            var transactionTypes = transactionTypeManager.GetMatchedTransactionTypes(reportDefinition.Direction);

            if (transactionTypes != null && transactionTypes.Count > 0)
            {
                var reportsDataList = reportsData.ToDictionary(x => x.CarrierAccountId, x => x);
                transactionsReports = new List<JazzTransactionsReport>();
                foreach (var transactionType in transactionTypes)
                {

                        List<JazzTransactionsReportData> transactionsReportsData = null;

                        AccountCodeManager accountCodeManager = new AccountCodeManager();
                        var accountCodes = accountCodeManager.GetAccountCodes(transactionType.ID, reportDefinition.SwitchId).ToList();

                        if (accountCodes != null && accountCodes.Count > 0)
                        {
                            SwitchCodeManager switchCodeManager = new SwitchCodeManager();
                            transactionsReportsData = new List<JazzTransactionsReportData>();
                            foreach (var accountCode in accountCodes)
                            {
                                if (accountCode.Carriers != null && accountCode.Carriers.Carriers != null && accountCode.Carriers.Carriers.Count > 0)
                                {
                                    if (transactionType.TransactionScope==TransactionScopeEnum.Account)
                                    {
                                        decimal amount = 0;
                                        foreach (var carrier in accountCode.Carriers.Carriers)
                                        {
                                            if (reportsData != null)
                                            {
                                                JazzReportData reportData = null;
                                                if (reportsDataList.TryGetValue(carrier.CarrierAccountId, out reportData))
                                                {
                                                    amount += reportData.Amount;
                                                }
                                            }
                                        }
                                        var accountCodeSwitchCode = switchCodeManager.GetSwitchCodeBySwitchId(accountCode.SwitchId);
                                        transactionsReportsData.Add(new JazzTransactionsReportData
                                        {
                                            TransationDescription = accountCode.Name + (accountCodeSwitchCode != null ? accountCodeSwitchCode.Name : null),
                                            TransactionCode = accountCode.Code + "_" + (accountCodeSwitchCode != null ? accountCodeSwitchCode.Code : null),
                                            Credit = transactionType.IsCredit ? amount : 0,
                                            Debit = transactionType.IsCredit ? 0 : amount,
                                        });
                                    }
                                    if (transactionType.TransactionScope==TransactionScopeEnum.Region)
                                    {
                                        var transactionsReportsDataDictionary = new Dictionary<string, JazzTransactionsReportData>();
                                        foreach (var carrier in accountCode.Carriers.Carriers)
                                        {
                                            if (reportsData != null)
                                            {
                                                JazzReportData reportData = null;
                                                if (reportsDataList.TryGetValue(carrier.CarrierAccountId, out reportData) && reportData.Markets != null && reportData.Markets.Count > 0)
                                                {
                                                    MarketManager marketManager = new MarketManager();
                                                    CustomerTypeManager customerTypeManager = new CustomerTypeManager();
                                                    RegionManager regionManager = new RegionManager();
                                                    foreach (var market in reportData.Markets)
                                                    {
                                                        if (market != null && market.Regions != null && market.Regions.Count > 0)
                                                        {
                                                            foreach (var region in market.Regions)
                                                            {
                                                                JazzTransactionsReportData transactionData = null;
                                                                var regionId = string.Concat(market.MarketId, region.RegionId);

                                                                if (!transactionsReportsDataDictionary.TryGetValue(regionId, out transactionData))
                                                                {
                                                                    var transactionSwitch = switchCodeManager.GetSwitchCodeBySwitchId(accountCode.SwitchId);
                                                                    var transactionMarket = marketManager.GetMarketById(market.MarketId);
                                                                    var transactionCustomerType = customerTypeManager.GetCustomerTypeById(market.CustomerTypeId);
                                                                    var transactionRegion = regionManager.GetRegionById(region.RegionId);

                                                                    var newTransactionReportData = new JazzTransactionsReportData
                                                                    {
                                                                        TransationDescription = string.Concat(accountCode.Name," ", transactionSwitch.Name, " ", transactionMarket.Name, " ", transactionCustomerType.Name, " ", transactionRegion.Name),
                                                                        TransactionCode = string.Concat(accountCode.Code,"-", transactionSwitch.Code, "-", transactionMarket.Code, "-", transactionCustomerType.Code, "-", transactionRegion.Code),
                                                                        Credit = transactionType.IsCredit ? region.RegionValue : 0,
                                                                        Debit = transactionType.IsCredit ? 0 : region.RegionValue
                                                                    };
                                                                    transactionsReportsData.Add(newTransactionReportData);
                                                                    transactionsReportsDataDictionary.Add(regionId, newTransactionReportData);
                                                                }
                                                                else
                                                                {
                                                                    if (transactionType.IsCredit)
                                                                        transactionData.Credit += region.RegionValue;
                                                                    else
                                                                        transactionData.Debit += region.RegionValue;
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        var jazzTransactionsReport = new JazzTransactionsReport
                        {
                            ReportDefinitionId = reportDefinition.JazzReportDefinitionId,
                            SheetName = string.Concat(reportDefinition.Name," ", transactionType.Name),
                            TransactionTypeId = transactionType.ID,
                        };
                        if (transactionsReportsData != null && transactionsReportsData.Count > 0)
                        {
                            jazzTransactionsReport.ReportData = transactionsReportsData;
                        }
                        transactionsReports.Add(jazzTransactionsReport);
                }

            }
            return transactionsReports;
        }
    }
}
