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

            var amountMeasure = (reportDefinition.TaxOption == TaxOptionEnum.TaxMeasure && !reportDefinition.SplitRateValue.HasValue) ? "AMT" : "SaleNet";

            if (reportDefinition.Direction.Equals(ReportDefinitionDirectionEnum.In))
            {
                dimensions = new List<string> { "Customer" };
                measures = new List<string> { amountMeasure, "SaleDuration" };
                if (reportDefinition.TaxOption == TaxOptionEnum.TaxMeasure)
                    measures.Add("STAX");
            }
            else
            {
                if (reportDefinition.TaxOption == TaxOptionEnum.TaxMeasure)
                    throw new Exception("Suppliers Cannot Be Assigned Taxes!");

                dimensions = new List<string> { "Supplier" };
                measures = new List<string> { "CostNet", "CostDuration" };
            }
            var analyticResult = GetFilteredRecords(dimensions, measures, fromDate, toDate, recordFilter);
            if (analyticResult != null && analyticResult.Data != null && analyticResult.Data.Count() != 0)
            {
                var reportData = BuildReportItemSetNameFromAnalytic(analyticResult.Data, reportDefinition, amountMeasure);

                report = new JazzReport
                {
                    ReportName = reportDefinition.Name,
                    Direction = reportDefinition.Direction,
                    SplitRateValue=reportDefinition.SplitRateValue,
                    TaxOption= reportDefinition.TaxOption,
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
                    TableId = Guid.Parse("795440c9-69e4-442e-a067-896bc969c73f "),
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
            if (!analyticRecord.MeasureValues.TryGetValue(measureName, out measureValue))
                throw new NullReferenceException($"measureValue '{measureName}'");
            return measureValue;
        }
        private List<JazzReportData> BuildReportItemSetNameFromAnalytic(IEnumerable<AnalyticRecord> analyticRecords, JazzReportDefinition reportDefinition,string amountMeasure)
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
                    MeasureValue taxValue = null;

                    if (reportDefinition.Direction == ReportDefinitionDirectionEnum.In)
                    {
                        netValue = GetMeasureValue(analyticRecord,amountMeasure) ;
                        durationValue = GetMeasureValue(analyticRecord, "SaleDuration");
                        if (reportDefinition.TaxOption == TaxOptionEnum.TaxMeasure)
                            taxValue = GetMeasureValue(analyticRecord, "STAX");
                    }
                    else
                    {
                        netValue = GetMeasureValue(analyticRecord, "CostNet");
                        durationValue = GetMeasureValue(analyticRecord, "CostDuration");
                    }
                    #endregion

                    var amount = Convert.ToDecimal(netValue.Value ?? 0.0);
                    var duration= Convert.ToDecimal(durationValue.Value ?? 0.0);
                    var jazzReportData = new JazzReportData
                    {
                        CarrierAccountId = Convert.ToInt32(carrierAccount.Value),
                        CarrierAccountName = carrierAccount.Name,
                        Duration = Convert.ToDecimal(durationValue.Value ?? 0.0),
                        Amount1 = !reportDefinition.SplitRateValue.HasValue ? amount : (amount - duration * reportDefinition.SplitRateValue.Value),
                        Amount2 = reportDefinition.SplitRateValue.HasValue ? duration * reportDefinition.SplitRateValue.Value : 0,
                        Tax = taxValue != null ? Convert.ToDecimal(taxValue.Value ?? 0.0) : 0
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
                                MarketId = market.MarketId,
                                MarketName = _marketManager.GetMarketName(market.MarketId),
                                CustomerTypeId = market.CustomerTypeId,
                                CustomerTypeName = _customerTypeManager.GetCustomerTypeName(market.CustomerTypeId),
                                MarketValue1 = market.Percentage * jazzReportData.Amount1 / 100,
                                MarketValue2 = market.Percentage * jazzReportData.Amount2 / 100,
                                Percentage = market.Percentage,
                            };
                            if (reportDefinition.Settings.RegionSettings != null && reportDefinition.Settings.RegionSettings.RegionOptions != null && reportDefinition.Settings.RegionSettings.RegionOptions.Count > 0)
                            {
                                reportMarket.Regions = new List<JazzReportRegion>();
                                RegionManager _regionManager = new RegionManager();
                                foreach (var region in reportDefinition.Settings.RegionSettings.RegionOptions)
                                {
                                    reportMarket.Regions.Add(new JazzReportRegion
                                    {
                                        RegionId = region.RegionId,
                                        RegionName = _regionManager.GetRegionById(region.RegionId).Name,
                                        RegionValue1 = region.Percentage * reportMarket.MarketValue1 / 100,
                                        RegionValue2 = region.Percentage * reportMarket.MarketValue2 / 100,
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
                var reportsDataDictionary = reportsData.ToDictionary(x => x.CarrierAccountId, x => x);

                transactionsReports = GetTransactionsReports(reportDefinition, reportsDataDictionary, transactionTypes, false, false);

                if (reportDefinition.TaxOption.HasValue)
                {
                    if(reportDefinition.TaxOption.Value==TaxOptionEnum.TaxMeasure)
                        transactionsReports = transactionsReports.Concat(GetTransactionsReports(reportDefinition, reportsDataDictionary, transactionTypes, false, true)).ToList();

                    else
                        transactionsReports.Add(GetZeroTaxTransactionReport(reportDefinition));
                }

                if (reportDefinition.SplitRateValue.HasValue)
                    transactionsReports = transactionsReports.Concat(GetTransactionsReports(reportDefinition, reportsDataDictionary, transactionTypes, true, false)).ToList();
            }
            return transactionsReports;
        }

        private JazzTransactionsReport GetZeroTaxTransactionReport(JazzReportDefinition reportDefinition)
        {
            SwitchCodeManager switchCodeManager = new SwitchCodeManager();
            TaxCodeManager taxCodeManger = new TaxCodeManager();

            var switchCode = switchCodeManager.GetSwitchCodeBySwitchId(reportDefinition.SwitchId);
            switchCode.ThrowIfNull("switchCode", switchCode.ID);
            var taxCode = taxCodeManger.GetTaxCode(reportDefinition.SwitchId, reportDefinition.Direction);
            taxCode.ThrowIfNull("taxCode", taxCode.ID);

            return new JazzTransactionsReport
            {
                ReportDefinitionId = reportDefinition.JazzReportDefinitionId,
                SheetName = string.Concat(reportDefinition.Name, " Tax"),
                IsTaxTransaction = true,
                ReportData = new List<JazzTransactionsReportData>
                {
                    new JazzTransactionsReportData
                    {
                        TransationDescription = string.Format("{0} {1}",switchCode.Name,taxCode.Name ),
                        TransactionCode =string.Format("{0}-0000-0000-0000-0000-0000-0000-000000-{1}-0000-0000-0000",switchCode.Code, taxCode.Code),
                        Credit = 0,
                        Debit = 0
                    }
                }
            };
        }
        private List<JazzTransactionsReport> GetTransactionsReports(JazzReportDefinition reportDefinition, Dictionary<int,JazzReportData> reportsDataDictionary, List<TransactionType> transactionTypes,bool splitRate,bool applyTax)
        {
            var transactionsReports = new List<JazzTransactionsReport>();

            foreach (var transactionType in transactionTypes)
            {
                List<JazzTransactionsReportData> transactionsReportsData = null;
                AccountCodeManager accountCodeManager = new AccountCodeManager();
                var accountCodes = accountCodeManager.GetAccountCodes(transactionType.ID, reportDefinition.SwitchId).ToList();

                if (accountCodes != null && accountCodes.Count > 0)
                {
                    SwitchCodeManager switchCodeManager = new SwitchCodeManager();
                    TaxCodeManager taxCodeManger = new TaxCodeManager();
                    transactionsReportsData = new List<JazzTransactionsReportData>();

                   
                    var switchCode = switchCodeManager.GetSwitchCodeBySwitchId(reportDefinition.SwitchId);
                    switchCode.ThrowIfNull("switchCode", switchCode.ID);
                    var taxCode = taxCodeManger.GetTaxCode(reportDefinition.SwitchId, reportDefinition.Direction);
                    taxCode.ThrowIfNull("taxCode", taxCode.ID);
                    foreach (var accountCode in accountCodes)
                    {
                        if (accountCode.Carriers != null && accountCode.Carriers.Carriers != null && accountCode.Carriers.Carriers.Count > 0)
                        {
                            if (transactionType.TransactionScope == TransactionScopeEnum.Account)
                            {
                                decimal amount = 0;
                                decimal tax = 0;
                                foreach (var carrier in accountCode.Carriers.Carriers)
                                {
                                    if (reportsDataDictionary != null)
                                    {
                                        JazzReportData reportData = null;

                                        if (reportsDataDictionary.TryGetValue(carrier.CarrierAccountId, out reportData))
                                        {
                                            amount += splitRate ? reportData.Amount2 : reportData.Amount1;
                                            tax += reportData.Tax;
                                        }
                                    }
                                }


                                if (!applyTax)
                                {

                                    transactionsReportsData.Add(new JazzTransactionsReportData
                                    {
                                        TransationDescription = string.Format("{0} {1}", switchCode.Name, accountCode.Name),
                                        TransactionCode = string.Format("{0}-0000-0000-0000-0000-0000-0000-000000-{1}-0000-0000-0000", switchCode.Code, accountCode.Code),
                                        Credit = transactionType.IsCredit ? amount : 0,
                                        Debit = transactionType.IsCredit ? 0 : amount,
                                    });
                                }
                                else
                                {

                                    if (!(reportDefinition.TaxOption == TaxOptionEnum.ZeroTax && transactionsReportsData.Count > 0))
                                        transactionsReportsData.Add(new JazzTransactionsReportData
                                        {
                                            TransationDescription = string.Format("{0} {1}", switchCode.Name, taxCode.Name),
                                            TransactionCode = string.Format("{0}-0000-0000-0000-0000-0000-0000-000000-{1}-0000-0000-0000", switchCode.Code, taxCode.Code),
                                            Credit = transactionType.IsCredit ? 0 : tax,
                                            Debit = transactionType.IsCredit ? tax : 0,
                                        });
                                }
                            }
                            else if (transactionType.TransactionScope == TransactionScopeEnum.Region && !applyTax)
                            {
                                var transactionsReportsDataDictionary = new Dictionary<string, JazzTransactionsReportData>();
                                MarketManager marketManager = new MarketManager();
                                CustomerTypeManager customerTypeManager = new CustomerTypeManager();
                                RegionManager regionManager = new RegionManager();
                                ProductServiceManager productServiceManager = new ProductServiceManager();
                                foreach (var carrier in accountCode.Carriers.Carriers)
                                {
                                    if (reportsDataDictionary != null)
                                    {
                                        JazzReportData reportData = null;
                                        if (reportsDataDictionary.TryGetValue(carrier.CarrierAccountId, out reportData) && reportData.Markets != null && reportData.Markets.Count > 0)
                                        {
                                            foreach (var market in reportData.Markets)
                                            {
                                                if (market != null && market.Regions != null && market.Regions.Count > 0)
                                                {
                                                    foreach (var region in market.Regions)
                                                    {
                                                        JazzTransactionsReportData transactionData = null;
                                                        var regionUniqueKey = string.Concat(market.MarketId, "_", region.RegionId);

                                                        if (!transactionsReportsDataDictionary.TryGetValue(regionUniqueKey, out transactionData))
                                                        {
                                                            var transactionMarket = marketManager.GetMarketById(market.MarketId);
                                                            var transactionCustomerType = customerTypeManager.GetCustomerTypeById(market.CustomerTypeId);
                                                            var transactionRegion = regionManager.GetRegionById(region.RegionId);
                                                            var transactionProductService = productServiceManager.GetProductServiceById(transactionMarket.ProductServiceId);
                                                            var departement = "4020";
                                                            var lobs = "5010";
                                                            var productsPricing = "400010";
                                                            var relatedParties = "0000";
                                                            var future1 = "0000";
                                                            var future2 = "0000";
                                                            var newTransactionReportData = new JazzTransactionsReportData
                                                            {
                                                                TransationDescription = string.Format("{0} {1} {2} {3} {4} {5}", switchCode.Name, transactionRegion.Name, transactionMarket.Name, transactionCustomerType.Name, transactionProductService.Name, accountCode.Name),
                                                                TransactionCode = string.Format("{0}-{1}-{2}-{3}-{4}-{5}-{6}-{7}-{8}-{9}-{10}-{11}", switchCode.Code, transactionRegion.Code, departement, lobs, transactionMarket.Code, transactionCustomerType.Code, transactionProductService.Code, productsPricing, accountCode.Code, relatedParties, future1, future2),
                                                                Credit = transactionType.IsCredit ? (splitRate ? region.RegionValue2 : region.RegionValue1) : 0,
                                                                Debit = transactionType.IsCredit ? 0 : (splitRate ? region.RegionValue2 : region.RegionValue1)
                                                            };
                                                            transactionsReportsData.Add(newTransactionReportData);
                                                            transactionsReportsDataDictionary.Add(regionUniqueKey, newTransactionReportData);
                                                        }
                                                        else
                                                        {
                                                            if (transactionType.IsCredit)
                                                                transactionData.Credit += splitRate ? region.RegionValue2 : region.RegionValue1;
                                                            else
                                                                transactionData.Debit += splitRate ? region.RegionValue2 : region.RegionValue1;
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
                var rateTitlePart = reportDefinition.SplitRateValue.HasValue ? string.Concat( splitRate ? Decimal.Round(reportDefinition.SplitRateValue.Value, 4).ToString() : "Rate-" + Decimal.Round(reportDefinition.SplitRateValue.Value, 4)) : null;
                var jazzTransactionsReport = new JazzTransactionsReport
                {
                    ReportDefinitionId = reportDefinition.JazzReportDefinitionId,
                    SheetName = string.Concat(reportDefinition.Name, " ", applyTax ? "Tax" : transactionType.Name," ", rateTitlePart),
                    TransactionTypeId = transactionType.ID,
                    IsTaxTransaction = false
                };

                if (transactionsReportsData != null && transactionsReportsData.Count > 0)
                {
                    jazzTransactionsReport.ReportData = transactionsReportsData;
                }
                transactionsReports.Add(jazzTransactionsReport);
            }
            return transactionsReports;
        }

    }
}
