﻿using System;
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
            public JazzReportData ReportData { get; set; }
            public bool AccountCodeFound { get; set; }
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
                var reportData = BuildReportItemSetNameFromAnalytic(analyticResult, reportDefinition, amountMeasure);

                report = new JazzReport
                {
                    ReportName = reportDefinition.Name,
                    Direction = reportDefinition.Direction,
                    AmountMeasureType = reportDefinition.AmountMeasureType,
                    TaxOption= reportDefinition.TaxOption,
                    Order=reportDefinition.Order,
                    ReportData = reportData
                };

                transactionsReports = GetTransactionReportsData(reportDefinition, reportData,context);

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
        private List<JazzTransactionsReport> GetTransactionReportsData(JazzReportDefinition reportDefinition, List<JazzReportData> reportsData, CodeActivityContext context)
        {
            TransactionTypeManager transactionTypeManager = new TransactionTypeManager();
            List<JazzTransactionsReport> transactionsReports = null;
            var transactionTypes = transactionTypeManager.GetMatchedTransactionTypes(reportDefinition.Direction);

            if (transactionTypes != null && transactionTypes.Count > 0)
            {
                var reportsInfoDictionary = reportsData.ToDictionary(x => x.CarrierAccountId, x => new JazzReportInfo {
                    ReportData =x,
                    AccountCodeFound =false
                });

                transactionsReports = GetTransactionsReports(reportDefinition, reportsInfoDictionary, transactionTypes, false,context);

                if (reportDefinition.TaxOption.HasValue)
                {
                    if (reportDefinition.TaxOption.Value == TaxOption.TaxMeasure)
                        transactionsReports = transactionsReports.Concat(GetTransactionsReports(reportDefinition, reportsInfoDictionary, transactionTypes, true,context)).ToList();

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
            SwitchManager switchManager = new SwitchManager();
            var switchName = switchManager.GetSwitchName(reportDefinition.SwitchId);

            var switchCode = switchCodeManager.GetSwitchCodeBySwitchId(reportDefinition.SwitchId);
            switchCode.ThrowIfNull("switchCode", reportDefinition.JazzReportDefinitionId);
            var taxCode = taxCodeManger.GetTaxCode(reportDefinition.SwitchId, reportDefinition.Direction);
            if (taxCode == null)
            {
                throw new VRBusinessException(string.Format("Missing Tax Code For Switch: '{0}' & Direction: '{1}'", switchName, reportDefinition.Direction));
            }
            return new JazzTransactionsReport
            {
                ReportDefinitionId = reportDefinition.JazzReportDefinitionId,
                SheetName = string.Concat(reportDefinition.Name, " Tax"),
                IsTaxTransaction = true,
                SwitchName = switchName,
                Direction = reportDefinition.Direction,
                TransactionTypeName = "Zero Tax",
                ReportData = new List<JazzTransactionsReportData>
                {
                    new JazzTransactionsReportData
                    {
                        TransationDescription = string.Format("{0} {1}",switchCode.Name,taxCode.Name ),
                        TransactionCode =string.Format("{0}-0000-0000-0000-0000-0000-0000-000000-{1}-0000-0000-0000",switchCode.Code, taxCode.Code),
                        Credit = 0,
                        Debit = 0,
                        CarriersNames = null
                    }
                }
            };
        }
        private List<JazzTransactionsReport> GetTransactionsReports(JazzReportDefinition reportDefinition, Dictionary<int,JazzReportInfo> reportsInfoDictionary, List<TransactionType> transactionTypes,bool applyTax,  CodeActivityContext context)
        {
            var transactionsReports = new List<JazzTransactionsReport>();
            SwitchManager switchManager = new SwitchManager();
            CarrierAccountManager carrierAccountManager = new CarrierAccountManager();
            SwitchCodeManager switchCodeManager = new SwitchCodeManager();
            TaxCodeManager taxCodeManger = new TaxCodeManager();
            var switchName = switchManager.GetSwitchName(reportDefinition.SwitchId);
            foreach (var transactionType in transactionTypes)
            {
                List<JazzTransactionsReportData> transactionsReportsData = null;
                AccountCodeManager accountCodeManager = new AccountCodeManager();
                var accountCodes = accountCodeManager.GetAccountCodes(transactionType.ID, reportDefinition.SwitchId).ToList();
                if (accountCodes != null && accountCodes.Count > 0)
                {
                    transactionsReportsData = new List<JazzTransactionsReportData>();


                    var switchCode = switchCodeManager.GetSwitchCodeBySwitchId(reportDefinition.SwitchId);
                    if (switchCode == null)
                    {
                        throw new VRBusinessException(string.Format("Missing Switch Code For Switch:'{0}'", switchName));
                    }
                    foreach (var accountCode in accountCodes)
                    {
                        if (accountCode.Carriers != null && accountCode.Carriers.Carriers != null && accountCode.Carriers.Carriers.Count > 0)
                        {
                            List<string> carriersNames = new List<string>();
                            if (transactionType.TransactionScope == TransactionScope.Account)
                            {
                                decimal amount = 0;
                                decimal tax = 0;
                                foreach (var carrier in accountCode.Carriers.Carriers)
                                {
                                    if (reportsInfoDictionary != null)
                                    {
                                        JazzReportInfo reportInfo = null;

                                        if (reportsInfoDictionary.TryGetValue(carrier.CarrierAccountId, out reportInfo))
                                        {
                                            carriersNames.Add(carrierAccountManager.GetCarrierAccountName(carrier.CarrierAccountId));
                                            var reportData = reportInfo.ReportData;
                                            amount += reportData.Amount;
                                            tax += reportData.Tax;

                                            if (reportInfo.AccountCodeFound == false)
                                            {
                                                reportInfo.AccountCodeFound = true;
                                            }
                                            else
                                            {
                                                throw new VRBusinessException(string.Format("Duplicate Account Code Found For The Combination: Carrier Account: '{0}' Switch: '{1}' & Transaction Type: '{2}'", reportData.CarrierAccountName, switchName, transactionType.Name));
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
                                        CarriersNames =carriersNames.Count!=0? string.Join(",", carriersNames):null
                                    });
                                }
                                else
                                {
                                    var taxCode = taxCodeManger.GetTaxCode(reportDefinition.SwitchId, reportDefinition.Direction);
                                    if (taxCode == null)
                                    {
                                        throw new VRBusinessException(string.Format("Missing Tax Code For Switch: '{0}' & Direction: '{1}'", switchName, reportDefinition.Direction));
                                    }

                                    transactionsReportsData.Add(new JazzTransactionsReportData
                                    {
                                        TransationDescription = string.Format("{0} {1}", switchCode.Name, taxCode.Name),
                                        TransactionCode = string.Format("{0}-0000-0000-0000-0000-0000-0000-000000-{1}-0000-0000-0000", switchCode.Code, taxCode.Code),
                                        Credit = transactionType.IsCredit ? 0 : tax,
                                        Debit = transactionType.IsCredit ? tax : 0,
                                        CarriersNames = carriersNames.Count != 0 ? string.Join(",", carriersNames) : null
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

                                    if (reportsInfoDictionary != null)
                                    {
                                        JazzReportInfo reportInfo = null;
                                        if (reportsInfoDictionary.TryGetValue(carrier.CarrierAccountId, out reportInfo) && reportInfo.ReportData.Markets != null && reportInfo.ReportData.Markets.Count > 0)
                                        {
                                            carriersNames.Add(carrierAccountManager.GetCarrierAccountName(carrier.CarrierAccountId));

                                            var reportData = reportInfo.ReportData;
                                            if (reportInfo.AccountCodeFound == false)
                                            {
                                                reportInfo.AccountCodeFound = true;
                                            }
                                            else
                                            {
                                                throw new VRBusinessException(string.Format("Duplicate Account Code Found For The Combination: Carrier Account: '{0}' Switch: '{1}' & Transaction Type: '{2}'", reportData.CarrierAccountName,switchName, transactionType.Name));
                                            }

                                            foreach (var market in reportData.Markets)
                                            {
                                                if (market != null && market.Regions != null && market.Regions.Count > 0)
                                                {
                                                    foreach (var region in market.Regions)
                                                    {
                                                        JazzTransactionsReportData transactionData = null;
                                                        var regionUniqueKey = string.Concat(market.MarketId, "_", market.CustomerTypeId, "_", region.RegionId);

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
                                                                Debit = transactionType.IsCredit ? 0 : region.RegionValue,
                                                                CarriersNames = carriersNames.Count != 0 ? string.Join(",", carriersNames) : null
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
                    var jazzReportsWithNoCarrierAccounts = reportsInfoDictionary.FindAllRecords(x => !x.AccountCodeFound).ToList();

                    if (jazzReportsWithNoCarrierAccounts != null && jazzReportsWithNoCarrierAccounts.Count > 0)
                    {
                        foreach (var report in jazzReportsWithNoCarrierAccounts)
                        {
                            context.WriteBusinessTrackingMsg(LogEntryType.Warning, $"Carrier '{report.ReportData.CarrierAccountName}' does not have an Account Code on Switch '{switchName}' & Transaction Type '{transactionType.Name}'");
                        }
                    }
                    foreach (var report in reportsInfoDictionary.Values)
                    {
                        report.AccountCodeFound = false;
                    }
                }

                var amountType = reportDefinition.AmountType;
                var jazzTransactionsReport = new JazzTransactionsReport
                {
                    ReportDefinitionId = reportDefinition.JazzReportDefinitionId,
                    SheetName = string.Concat(reportDefinition.Name.Length > 27 ? reportDefinition.Name.Substring(0, 27) : reportDefinition.Name, " ", applyTax ? "Tax" : transactionType.Name.Substring(0, 3)),
                    TransactionTypeId = transactionType.ID,
                    IsTaxTransaction = false,
                    SwitchName = switchName,
                    Direction = reportDefinition.Direction,
                    TransactionTypeName = applyTax ? "Tax" : transactionType.Name,
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
