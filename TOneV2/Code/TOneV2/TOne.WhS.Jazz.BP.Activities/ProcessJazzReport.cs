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
using Vanrise.Common.Business;
using Vanrise.BusinessProcess;
using TOne.WhS.BusinessEntity.Business;

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

        private class JazzReportInfo
        {
            public JazzReportData JazzReportData { get; set; }
            public bool HasCarrierAccount { get; set; }
        }
        protected override void Execute(CodeActivityContext context)
        {
            JazzReport report = null;
            List<JazzTransactionsReport> transactionsReports = null;
            var reportDefinition = ReportDefinition.Get(context);
            var fromDate = FromDate.Get(context);
            var toDate = ToDate.Get(context);
            RecordFilterGroup recordFilterGroup = null;
            if (reportDefinition.Settings != null && reportDefinition.Settings.ReportFilter != null)
                recordFilterGroup = reportDefinition.Settings.ReportFilter;
            List<string> dimensions = null;
            List<string> measures = null;

            string amountMeasure; 

            if (reportDefinition.Direction==ReportDefinitionDirection.In)
            {
                amountMeasure = reportDefinition.AmountMeasureType.HasValue ? "AMT" : "SaleNet";
                dimensions = new List<string> { "Customer" };
                measures = new List<string> { amountMeasure, "SaleDuration" };
                if (reportDefinition.TaxOption == TaxOption.TaxMeasure)
                    measures.Add("STAX");
            }
            else
            {
                amountMeasure = "CostNet";
                if (reportDefinition.TaxOption == TaxOption.TaxMeasure)
                    throw new Exception("Suppliers Cannot Be Assigned Taxes!");

                dimensions = new List<string> { "Supplier" };
                measures = new List<string> { amountMeasure, "CostDuration" };
            }
            var analyticResult = GetFilteredRecords(dimensions, measures, fromDate, toDate, recordFilterGroup);
            if (analyticResult != null  && analyticResult.Count() != 0)
            {
                List<JazzReportInfo> jazzReportsInfo = null;

                var reportData = BuildReportItemSetNameFromAnalytic(analyticResult, reportDefinition, amountMeasure,out jazzReportsInfo);

                report = new JazzReport
                {
                    ReportName = reportDefinition.Name,
                    Direction = reportDefinition.Direction,
                    AmountMeasureType = reportDefinition.AmountMeasureType,
                    TaxOption= reportDefinition.TaxOption,
                    ReportData = reportData
                };

                transactionsReports = GetTransactionReportsData(reportDefinition, reportData, jazzReportsInfo,context);

            }
            JazzReport.Set(context, report);
            JazzTransactionsReport.Set(context, transactionsReports);
        }

        private List<AnalyticRecord> GetFilteredRecords(List<string> listDimensions, List<string> listMeasures, DateTime fromDate, DateTime toDate, RecordFilterGroup recordFilter)
        {
            
            AnalyticManager analyticManager = new AnalyticManager();
            var regulatedToDate = toDate.AddDays(1).AddMilliseconds(-3);

            TOne.WhS.Jazz.Business.ConfigManager configManager = new TOne.WhS.Jazz.Business.ConfigManager();

            AnalyticQuery analyticQuery = new AnalyticQuery
            {

                DimensionFields = listDimensions,
                MeasureFields = listMeasures,
                TableId = configManager.GetERPIntegrationAnalyticTableId(),
                FromTime = fromDate,
                ToTime = regulatedToDate,
                FilterGroup = recordFilter
            };
            return analyticManager.GetAllFilteredRecords(analyticQuery);
        }
        private MeasureValue GetMeasureValue(AnalyticRecord analyticRecord, string measureName)
        {
            MeasureValue measureValue;
            if (!analyticRecord.MeasureValues.TryGetValue(measureName, out measureValue))
                throw new NullReferenceException($"measureValue '{measureName}'");
            return measureValue;
        }
        private List<JazzReportData> BuildReportItemSetNameFromAnalytic(IEnumerable<AnalyticRecord> analyticRecords, JazzReportDefinition reportDefinition,string amountMeasure,out List<JazzReportInfo> jazzReportsInfo)
        {
            List<JazzReportData> jazzReportsData = null;
            jazzReportsInfo = new List<JazzReportInfo>();

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

                    if (reportDefinition.Direction == ReportDefinitionDirection.In)
                    {
                        netValue = GetMeasureValue(analyticRecord,amountMeasure) ;
                        durationValue = GetMeasureValue(analyticRecord, "SaleDuration");
                        if (reportDefinition.TaxOption == TaxOption.TaxMeasure)
                            taxValue = GetMeasureValue(analyticRecord, "STAX");
                    }
                    else
                    {
                        netValue = GetMeasureValue(analyticRecord, amountMeasure);
                        durationValue = GetMeasureValue(analyticRecord, "CostDuration");
                    }
                    #endregion

                    var amount = Convert.ToDecimal(netValue.Value ?? 0.0);
                    var duration= Convert.ToDecimal(durationValue.Value ?? 0.0);
                    var amountType = reportDefinition.AmountType;


                    if (amountType.HasValue)
                    {
                        if (!reportDefinition.SplitRateValue.HasValue)
                            throw new NullReferenceException($"splitRateValue '{reportDefinition.JazzReportDefinitionId}'");
                       
                            var splitRateValue = reportDefinition.SplitRateValue.Value;
                            CurrencyExchangeRateManager currencyExchangeRateManager = new CurrencyExchangeRateManager();

                            if (reportDefinition.CurrencyId.HasValue)
                                splitRateValue = currencyExchangeRateManager.ConvertValueToSystemCurrency(splitRateValue, reportDefinition.CurrencyId.Value, DateTime.Now);

                            if (amountType.Value == AmountType.FixedRate)
                                amount = duration * splitRateValue;
                            else
                                amount = (amount - duration * splitRateValue);
                        
                    }

                     
                    var jazzReportData = new JazzReportData
                    {
                        CarrierAccountId = Convert.ToInt32(carrierAccount.Value),
                        CarrierAccountName = carrierAccount.Name,
                        Duration = Decimal.Round(Convert.ToDecimal(durationValue.Value ?? 0.0),2),
                        Amount = Decimal.Round(amount, 3),
                        Tax = taxValue != null ? Decimal.Round(Convert.ToDecimal(taxValue.Value ?? 0.0),3) : 0
                    };

                    jazzReportsInfo.Add(new JazzReportInfo
                    {
                        JazzReportData = jazzReportData,
                        HasCarrierAccount=false
                    });
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
                                MarketValue =market.Percentage * jazzReportData.Amount / 100,
                                Percentage = market.Percentage,
                            };
                            if (reportDefinition.Settings.RegionSettings != null && reportDefinition.Settings.RegionSettings.RegionOptions != null && reportDefinition.Settings.RegionSettings.RegionOptions.Count > 0)
                            {
                                reportMarket.Regions = new List<JazzReportRegion>();
                                TOne.WhS.Jazz.Business.RegionManager _regionManager = new Business.RegionManager();
                                foreach (var region in reportDefinition.Settings.RegionSettings.RegionOptions)
                                {
                                    reportMarket.Regions.Add(new JazzReportRegion
                                    {
                                        RegionId = region.RegionId,
                                        RegionName = _regionManager.GetRegionById(region.RegionId).Name,
                                        RegionValue =region.Percentage * reportMarket.MarketValue / 100,
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
        private List<JazzTransactionsReport> GetTransactionReportsData(JazzReportDefinition reportDefinition, List<JazzReportData> reportsData, List<JazzReportInfo> jazzReportsInfo, CodeActivityContext context)
        {
            TransactionTypeManager transactionTypeManager = new TransactionTypeManager();
            List<JazzTransactionsReport> transactionsReports = null;
            var transactionTypes = transactionTypeManager.GetMatchedTransactionTypes(reportDefinition.Direction);

            if (transactionTypes != null && transactionTypes.Count > 0)
            {
                var reportsDataDictionary = reportsData.ToDictionary(x => x.CarrierAccountId, x => x);

                transactionsReports = GetTransactionsReports(reportDefinition, reportsDataDictionary, transactionTypes, false, jazzReportsInfo,context);

                if (reportDefinition.TaxOption.HasValue)
                {
                    if (reportDefinition.TaxOption.Value == TaxOption.TaxMeasure)
                        transactionsReports = transactionsReports.Concat(GetTransactionsReports(reportDefinition, reportsDataDictionary, transactionTypes, true, jazzReportsInfo,context)).ToList();

                    else
                        transactionsReports.Add(GetZeroTaxTransactionReport(reportDefinition));
                }
            }
            return transactionsReports;
        }

        private JazzTransactionsReport GetZeroTaxTransactionReport(JazzReportDefinition reportDefinition)
        {
            SwitchCodeManager switchCodeManager = new SwitchCodeManager();
            TaxCodeManager taxCodeManger = new TaxCodeManager();

            var switchCode = switchCodeManager.GetSwitchCodeBySwitchId(reportDefinition.SwitchId);
            switchCode.ThrowIfNull("switchCode", reportDefinition.JazzReportDefinitionId);
            var taxCode = taxCodeManger.GetTaxCode(reportDefinition.SwitchId, reportDefinition.Direction);
            if (taxCode == null)
            {
                SwitchManager switchManager = new SwitchManager();
                throw new VRBusinessException(string.Format("Missing Tax Code For Switch:'{0}' & Direction:'{1}'", switchManager.GetSwitchName(reportDefinition.SwitchId), reportDefinition.Direction));
            }
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
        private List<JazzTransactionsReport> GetTransactionsReports(JazzReportDefinition reportDefinition, Dictionary<int,JazzReportData> reportsDataDictionary, List<TransactionType> transactionTypes,bool applyTax, List<JazzReportInfo> jazzReportsInfo, CodeActivityContext context)
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
                    if (switchCode == null)
                    {
                        SwitchManager switchManager = new SwitchManager();
                        throw new VRBusinessException(string.Format("Missing Switch Code For Switch:'{0}'", switchManager.GetSwitchName(reportDefinition.SwitchId)));
                    }

                    foreach (var accountCode in accountCodes)
                    {
                        if (accountCode.Carriers != null && accountCode.Carriers.Carriers != null && accountCode.Carriers.Carriers.Count > 0)
                        {
                            if (transactionType.TransactionScope == TransactionScope.Account)
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
                                            amount += reportData.Amount;
                                            tax += reportData.Tax;

                                            var reportInfo = jazzReportsInfo.Find(x => x.JazzReportData.CarrierAccountId == reportData.CarrierAccountId);
                                            {
                                                if (reportInfo.HasCarrierAccount == false)
                                                {
                                                    reportInfo.HasCarrierAccount = true;
                                                }
                                                else
                                                {
                                                    SwitchManager switchManager = new SwitchManager();
                                                    throw new VRBusinessException(string.Format("Carrier Account:'{0}' Exists More Than Once In Switch:'{1}' & Transaction Type:'{2}' Combination", reportData.CarrierAccountName, switchManager.GetSwitchName(reportDefinition.SwitchId), transactionType.Name));
                                                }
                                            }
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
                                    var taxCode = taxCodeManger.GetTaxCode(reportDefinition.SwitchId, reportDefinition.Direction);
                                    if (taxCode == null)
                                    {
                                        SwitchManager switchManager = new SwitchManager();
                                        throw new VRBusinessException(string.Format("Missing Tax Code For Switch:'{0}' & Direction:'{1}'", switchManager.GetSwitchName(reportDefinition.SwitchId), reportDefinition.Direction));
                                    }

                                    transactionsReportsData.Add(new JazzTransactionsReportData
                                    {
                                        TransationDescription = string.Format("{0} {1}", switchCode.Name, taxCode.Name),
                                        TransactionCode = string.Format("{0}-0000-0000-0000-0000-0000-0000-000000-{1}-0000-0000-0000", switchCode.Code, taxCode.Code),
                                        Credit = transactionType.IsCredit ? 0 : tax,
                                        Debit = transactionType.IsCredit ? tax : 0,
                                    });
                                }
                            }
                            else if (transactionType.TransactionScope == TransactionScope.Region && !applyTax)
                            {
                                var transactionReportsByRegionKey = new Dictionary<string, JazzTransactionsReportData>();
                                MarketManager marketManager = new MarketManager();
                                CustomerTypeManager customerTypeManager = new CustomerTypeManager();
                                TOne.WhS.Jazz.Business.RegionManager regionManager = new Business.RegionManager();
                                ProductServiceManager productServiceManager = new ProductServiceManager();
                                foreach (var carrier in accountCode.Carriers.Carriers)
                                {
                                    if (reportsDataDictionary != null)
                                    {
                                        JazzReportData reportData = null;
                                        if (reportsDataDictionary.TryGetValue(carrier.CarrierAccountId, out reportData) && reportData.Markets != null && reportData.Markets.Count > 0)
                                        {
                                            var reportInfo = jazzReportsInfo.Find(x => x.JazzReportData.CarrierAccountId == reportData.CarrierAccountId);
                                            {
                                                if (reportInfo.HasCarrierAccount == false)
                                                {
                                                    reportInfo.HasCarrierAccount = true;
                                                }
                                                else
                                                {
                                                    SwitchManager switchManager = new SwitchManager();
                                                    throw new VRBusinessException(string.Format("Carrier Account:'{0}' Exists More Than Once In Switch:'{1}' & Transaction Type:'{2}' Combination", reportData.CarrierAccountName, switchManager.GetSwitchName(reportDefinition.SwitchId), transactionType.Name));
                                                }
                                            }

                                            foreach (var market in reportData.Markets)
                                            {
                                                if (market != null && market.Regions != null && market.Regions.Count > 0)
                                                {
                                                    foreach (var region in market.Regions)
                                                    {
                                                        JazzTransactionsReportData transactionData = null;
                                                        var regionUniqueKey = string.Concat(market.MarketId, "_", region.RegionId);

                                                        if (!transactionReportsByRegionKey.TryGetValue(regionUniqueKey, out transactionData))
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
                                                                Credit = transactionType.IsCredit ? region.RegionValue : 0,
                                                                Debit = transactionType.IsCredit ? 0 : region.RegionValue
                                                            };
                                                            transactionsReportsData.Add(newTransactionReportData);
                                                            transactionReportsByRegionKey.Add(regionUniqueKey, newTransactionReportData);
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
                    var jazzReportsWithNoCarrierAccounts = jazzReportsInfo.FindAll(x => !x.HasCarrierAccount);
                    if (jazzReportsWithNoCarrierAccounts != null && jazzReportsWithNoCarrierAccounts.Count > 0)
                    {
                        SwitchManager switchManager = new SwitchManager();
                        foreach (var report in jazzReportsWithNoCarrierAccounts)
                        {
                            context.WriteBusinessTrackingMsg(LogEntryType.Warning, $"Carrier '{report.JazzReportData.CarrierAccountName}' does not have an Account Code on Switch '{switchManager.GetSwitchName(reportDefinition.SwitchId)}' & Transaction Type'{transactionType.Name}'");
                        }
                    }
                    foreach (var report in jazzReportsInfo)
                    {
                        report.HasCarrierAccount = false;
                    }
                }

                var amountType = reportDefinition.AmountType;
                var jazzTransactionsReport = new JazzTransactionsReport
                {
                    ReportDefinitionId = reportDefinition.JazzReportDefinitionId,
                    SheetName = string.Concat(reportDefinition.Name.Length > 27 ? reportDefinition.Name.Substring(0, 27) : reportDefinition.Name, " ", applyTax ? "Tax" : transactionType.Name.Substring(0, 3)),
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
