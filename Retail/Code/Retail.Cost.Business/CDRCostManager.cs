using Retail.Cost.Data;
using Retail.Cost.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;

namespace Retail.Cost.Business
{
    public enum ProfitStatus { NoProfit = 0, Profitable = 1, Lossy = 2 }

    public class CDRCostManager
    {
        #region Public Methods

        /// <summary>
        /// used in data transformation
        /// </summary>
        public void AssignCostToCDR(List<dynamic> cdrs, string attemptDateTimeFieldName, string cgpnFieldName, string cdpnFieldName, string durationInSecondsFieldName,
            CDRCostFieldNames cdrCostFieldNames, CDRProfitFieldNames cdrProfitFieldNames)
        {
            if (cdrCostFieldNames == null)
                return;

            CDRCostTechnicalSettingData cdrCostTechnicalSettingData = new ConfigManager().GetCDRCostTechnicalSettingData();
            if (!cdrCostTechnicalSettingData.IsCostIncluded)
                return;

            RecordFilterManager recordFilterManager = new RecordFilterManager();
            RecordFilterGroup recordFilterGroup = cdrCostTechnicalSettingData.FilterGroup;

            if (!cdrCostTechnicalSettingData.DataRecordTypeId.HasValue)
                throw new NullReferenceException("cdrCostTechnicalSettingData.DataRecordTypeId");

            Guid dataRecordTypeId = cdrCostTechnicalSettingData.DataRecordTypeId.Value;

            List<CDRCostRequest> cdrCostRequests = new List<CDRCostRequest>();
            var configManager = new Vanrise.Common.Business.ConfigManager();
            int systemCurrencyId = configManager.GetSystemCurrencyId();

            foreach (var cdr in cdrs)
            {
                RemoveCDRCostData(cdr, cdrCostFieldNames, cdrProfitFieldNames);

                DataRecordFilterGenericFieldMatchContext filterContext = new DataRecordFilterGenericFieldMatchContext(cdr, dataRecordTypeId);
                if (!recordFilterManager.IsFilterGroupMatch(recordFilterGroup, filterContext))
                {
                    if (!string.IsNullOrEmpty(cdrCostFieldNames.CostAmount))
                        cdr.SetFieldValue(cdrCostFieldNames.CostAmount, 0);

                    if (!string.IsNullOrEmpty(cdrCostFieldNames.CostRate))
                        cdr.SetFieldValue(cdrCostFieldNames.CostRate, 0);

                    if (!string.IsNullOrEmpty(cdrCostFieldNames.CostCurrency))
                        cdr.SetFieldValue(cdrCostFieldNames.CostCurrency, systemCurrencyId);

                    FillCDRWithoutCostProfitData(cdr, cdrProfitFieldNames);

                    continue;
                }

                CDRCostRequest cdrCostRequest = new CDRCostRequest()
                {
                    OriginalCDR = cdr,
                    AttemptDateTime = cdr.GetFieldValue(attemptDateTimeFieldName),
                    Duration = cdr.GetFieldValue(durationInSecondsFieldName),
                    CGPN = cdr.GetFieldValue(cgpnFieldName),
                    CDPN = cdr.GetFieldValue(cdpnFieldName)
                };

                cdrCostRequests.Add(cdrCostRequest);
            }

            FillCost(cdrCostRequests, cdrCostFieldNames, cdrProfitFieldNames);
        }

        public void UpadeOverridenCostCDRAfterDate(DateTime? fromTime)
        {
            ICDRCostDataManager cdrCostDataManager = CostDataManagerFactory.GetDataManager<ICDRCostDataManager>();
            cdrCostDataManager.UpadeOverridenCostCDRAfterDate(fromTime);
        }

        public HashSet<DateTime> GetDistinctDatesAfterId(long? cdrCostId)
        {
            ICDRCostDataManager cdrCostDataManager = CostDataManagerFactory.GetDataManager<ICDRCostDataManager>();
            return cdrCostDataManager.GetDistinctDatesAfterId(cdrCostId);
        }

        public long? GetMaxCDRCostId()
        {
            ICDRCostDataManager cdrCostDataManager = CostDataManagerFactory.GetDataManager<ICDRCostDataManager>();
            return cdrCostDataManager.GetMaxCDRCostId();
        }

        #endregion

        #region Private Methods

        private void RemoveCDRCostData(dynamic cdr, CDRCostFieldNames cdrCostFieldNames, CDRProfitFieldNames cdrProfitFieldNames)
        {
            //Cost Fields
            if (!string.IsNullOrEmpty(cdrCostFieldNames.CDRCostId))
                cdr.SetFieldValue(cdrCostFieldNames.CDRCostId, null);

            if (!string.IsNullOrEmpty(cdrCostFieldNames.CostRate))
                cdr.SetFieldValue(cdrCostFieldNames.CostRate, null);

            if (!string.IsNullOrEmpty(cdrCostFieldNames.CostAmount))
                cdr.SetFieldValue(cdrCostFieldNames.CostAmount, null);

            if (!string.IsNullOrEmpty(cdrCostFieldNames.CostCurrency))
                cdr.SetFieldValue(cdrCostFieldNames.CostCurrency, null);

            if (!string.IsNullOrEmpty(cdrCostFieldNames.SupplierName))
                cdr.SetFieldValue(cdrCostFieldNames.SupplierName, null);

            //Profit Fields
            if (cdrProfitFieldNames != null)
            {
                if (!string.IsNullOrEmpty(cdrProfitFieldNames.Profit))
                    cdr.SetFieldValue(cdrProfitFieldNames.Profit, null);

                if (!string.IsNullOrEmpty(cdrProfitFieldNames.ProfitStatus))
                    cdr.SetFieldValue(cdrProfitFieldNames.ProfitStatus, null);
            }
        }

        private void FillCost(List<CDRCostRequest> cdrCostRequests, CDRCostFieldNames cdrCostFieldNames, CDRProfitFieldNames cdrProfitFieldNames)
        {
            if (cdrCostRequests == null || cdrCostRequests.Count == 0)
                return;

            ConfigManager configManager = new ConfigManager();
            int maxBatchDurationInMinutes = configManager.GetMaxBatchDurationInMinutes();
            TimeSpan attemptDateTimeMargin = configManager.GetAttemptDateTimeMargin();
            TimeSpan attemptDateTimeOffset = configManager.GetAttemptDateTimeOffset();
            TimeSpan durationMargin = configManager.GetDurationMargin();
            decimal durationMarginInSeconds = Convert.ToDecimal(durationMargin.TotalSeconds);
            int? profitPrecision = configManager.GetProfitPrecision();

            IOrderedEnumerable<CDRCostRequest> orderedCDRCostRequests = cdrCostRequests.Select(itm => BuildCDRCostRequest(itm, attemptDateTimeOffset)).OrderBy(itm => itm.AttemptDateTime);

            List<CDRCostBatchRequest> cdrCostBatchRequests = BuildCDRCostBatchRequests(orderedCDRCostRequests, attemptDateTimeMargin, maxBatchDurationInMinutes);

            ICDRCostDataManager cdrCostDataManager = CostDataManagerFactory.GetDataManager<ICDRCostDataManager>();

            foreach (var cdrCostBatchRequest in cdrCostBatchRequests)
            {
                List<CDRCost> cdrCostList = cdrCostDataManager.GetCDRCostByCDPNs(cdrCostBatchRequest);
                if (cdrCostList == null || cdrCostList.Count == 0)
                {
                    foreach (CDRCostRequest cdrCostRequest in cdrCostBatchRequest.CDRCostRequests)
                        FillCDRWithoutCostProfitData(cdrCostRequest.OriginalCDR, cdrProfitFieldNames);

                    continue;
                }

                Dictionary<string, CDRCostData> cdrCostDataByCDPN = this.BuildCDRCostDataByCDPN(cdrCostList);
                foreach (CDRCostRequest cdrCostRequest in cdrCostBatchRequest.CDRCostRequests)
                {
                    CDRCostData tempCDRCostData;
                    if (!cdrCostDataByCDPN.TryGetValue(cdrCostRequest.CDPN, out tempCDRCostData))
                    {
                        FillCDRWithoutCostProfitData(cdrCostRequest.OriginalCDR, cdrProfitFieldNames);
                        continue;
                    }

                    List<CDRCost> cdrCostsByCGPN = null;
                    if (!string.IsNullOrEmpty(cdrCostRequest.CGPN))
                        tempCDRCostData.CDRCostsByCGPN.TryGetValue(cdrCostRequest.CGPN, out cdrCostsByCGPN);

                    CDRCost matchingCDRCost = this.GetMatchingCDRCost(cdrCostsByCGPN, cdrCostRequest, durationMarginInSeconds, attemptDateTimeMargin, null);
                    if (matchingCDRCost == null)
                        matchingCDRCost = this.GetMatchingCDRCost(tempCDRCostData.CDRCosts, cdrCostRequest, durationMarginInSeconds, attemptDateTimeMargin, cdrCostRequest.CGPN);

                    if (matchingCDRCost != null) //matching cost is found
                    {
                        FillCDRCostData(cdrCostRequest.OriginalCDR, matchingCDRCost, cdrCostFieldNames);
                        FillCDRProfitData(cdrCostRequest.OriginalCDR, matchingCDRCost, cdrCostFieldNames, cdrProfitFieldNames, cdrCostRequest.AttemptDateTime, profitPrecision);
                    }
                    else
                    {
                        FillCDRWithoutCostProfitData(cdrCostRequest.OriginalCDR, cdrProfitFieldNames);
                    }
                }
            }
        }

        private CDRCostRequest BuildCDRCostRequest(CDRCostRequest itm, TimeSpan attemptDateTimeOffset)
        {
            return new CDRCostRequest()
            {
                OriginalCDR = itm.OriginalCDR,
                AttemptDateTime = itm.AttemptDateTime.Add(attemptDateTimeOffset),
                CGPN = itm.CGPN,
                CDPN = itm.CDPN,
                Duration = itm.Duration
            };
        }

        private List<CDRCostBatchRequest> BuildCDRCostBatchRequests(IOrderedEnumerable<CDRCostRequest> orderedCDRCostRequests, TimeSpan attemptDateTimeMargin, int maxBatchDurationInMinutes)
        {
            CDRCostBatchRequest batchRequest = null;
            DateTime nextAttemptDateTimeThreshold = default(DateTime);
            List<CDRCostBatchRequest> cdrCostBatchRequests = new List<CDRCostBatchRequest>();

            foreach (var cdrCostRequest in orderedCDRCostRequests)
            {
                if (cdrCostRequest.AttemptDateTime < nextAttemptDateTimeThreshold)
                {
                    batchRequest.CDPNs.Add(cdrCostRequest.CDPN);
                    batchRequest.CDRCostRequests.Add(cdrCostRequest);
                    batchRequest.BatchEnd = cdrCostRequest.AttemptDateTime.Add(attemptDateTimeMargin);
                }
                else
                {
                    batchRequest = new CDRCostBatchRequest()
                    {
                        CDPNs = new HashSet<string>() { cdrCostRequest.CDPN },
                        BatchStart = cdrCostRequest.AttemptDateTime.Subtract(attemptDateTimeMargin),
                        BatchEnd = cdrCostRequest.AttemptDateTime.Add(attemptDateTimeMargin),
                        CDRCostRequests = new List<CDRCostRequest>() { cdrCostRequest }
                    };
                    nextAttemptDateTimeThreshold = cdrCostRequest.AttemptDateTime.AddMinutes(maxBatchDurationInMinutes);
                    cdrCostBatchRequests.Add(batchRequest);
                }
            }

            return cdrCostBatchRequests;
        }

        private Dictionary<string, CDRCostData> BuildCDRCostDataByCDPN(List<CDRCost> cdrCostList)
        {
            if (cdrCostList == null || cdrCostList.Count == 0)
                return null;

            Dictionary<string, CDRCostData> results = new Dictionary<string, CDRCostData>();

            foreach (var cdrCost in cdrCostList)
            {
                CDRCostData cdrCostData = results.GetOrCreateItem(cdrCost.CDPN);
                cdrCostData.CDRCosts.Add(cdrCost);

                if (!string.IsNullOrEmpty(cdrCost.CGPN))
                {
                    List<CDRCost> cdrCosts = cdrCostData.CDRCostsByCGPN.GetOrCreateItem(cdrCost.CGPN);
                    cdrCosts.Add(cdrCost);
                }
            }

            return results;
        }

        private CDRCost GetMatchingCDRCost(List<CDRCost> cdrCosts, CDRCostRequest cdrCostRequest, decimal durationMarginInSeconds, TimeSpan attemptDateTimeMargin, string excludedCGPN)
        {
            if (cdrCosts == null)
                return null;

            CDRCost matchingCDRCost = null;
            int minAttemptDateTimeDiffInMilliseconds = int.MaxValue;

            foreach (var cdrCostItem in cdrCosts)
            {
                if (!string.IsNullOrEmpty(excludedCGPN) && !string.IsNullOrEmpty(cdrCostItem.CGPN) && excludedCGPN == cdrCostItem.CGPN)
                    continue;

                if (!IsCDRCostMatch(cdrCostItem, cdrCostRequest, durationMarginInSeconds, attemptDateTimeMargin))
                    continue;

                TimeSpan attemptDateTimeDiff = cdrCostItem.AttemptDateTime.Value - cdrCostRequest.AttemptDateTime;
                int attemptDateTimeDiffInMilliseconds = (int)Math.Abs(attemptDateTimeDiff.TotalMilliseconds);

                if (matchingCDRCost != null && attemptDateTimeDiffInMilliseconds > minAttemptDateTimeDiffInMilliseconds)
                    continue;

                if (matchingCDRCost != null && (attemptDateTimeDiffInMilliseconds == minAttemptDateTimeDiffInMilliseconds && cdrCostItem.CDRCostId < matchingCDRCost.CDRCostId))
                    continue;

                //matchingCDRCost == null or attemptDateTimeDiffInMilliseconds < minAttemptDateTimeDiffInMilliseconds or 
                //(attemptDateTimeDiffInMilliseconds == minAttemptDateTimeDiffInMilliseconds and cdrCostItem.CDRCostId > matchingCDRCost.CDRCostId)
                matchingCDRCost = cdrCostItem;
                minAttemptDateTimeDiffInMilliseconds = attemptDateTimeDiffInMilliseconds;
            }

            return matchingCDRCost;
        }

        private bool IsCDRCostMatch(CDRCost cdrCost, CDRCostRequest cdrCostRequest, decimal durationMarginInSeconds, TimeSpan attemptDateTimeMargin)
        {
            if (!cdrCost.AttemptDateTime.HasValue || !cdrCost.DurationInSeconds.HasValue)
                return false;

            if (Math.Abs((cdrCost.AttemptDateTime.Value - cdrCostRequest.AttemptDateTime).TotalSeconds) > attemptDateTimeMargin.TotalSeconds
                || Math.Abs((cdrCost.DurationInSeconds.Value - cdrCostRequest.Duration)) > durationMarginInSeconds)
                return false;

            return true;
        }

        private void FillCDRCostData(dynamic cdr, CDRCost cdrCost, CDRCostFieldNames cdrCostFieldNames)
        {
            if (!string.IsNullOrEmpty(cdrCostFieldNames.CDRCostId))
                cdr.SetFieldValue(cdrCostFieldNames.CDRCostId, cdrCost.CDRCostId);

            if (!string.IsNullOrEmpty(cdrCostFieldNames.CostRate))
                cdr.SetFieldValue(cdrCostFieldNames.CostRate, cdrCost.Rate);

            if (!string.IsNullOrEmpty(cdrCostFieldNames.CostAmount))
                cdr.SetFieldValue(cdrCostFieldNames.CostAmount, cdrCost.Amount);

            if (!string.IsNullOrEmpty(cdrCostFieldNames.CostCurrency))
                cdr.SetFieldValue(cdrCostFieldNames.CostCurrency, cdrCost.CurrencyId);

            if (!string.IsNullOrEmpty(cdrCostFieldNames.SupplierName))
                cdr.SetFieldValue(cdrCostFieldNames.SupplierName, cdrCost.SupplierName);
        }

        private void FillCDRProfitData(dynamic cdr, CDRCost cdrCost, CDRCostFieldNames cdrCostFieldNames, CDRProfitFieldNames cdrProfitFieldNames, DateTime attemptDateTime, int? profitPrecision)
        {
            if (!cdrCost.Amount.HasValue || !cdrCost.CurrencyId.HasValue)
                return;

            if (cdrProfitFieldNames == null || string.IsNullOrEmpty(cdrProfitFieldNames.SaleAmount) || string.IsNullOrEmpty(cdrProfitFieldNames.SaleCurrency))
                return;

            decimal? saleAmount = cdr.GetFieldValue(cdrProfitFieldNames.SaleAmount);
            if (!saleAmount.HasValue)
                throw new NullReferenceException("saleAmount");

            int? saleCurrencyId = cdr.GetFieldValue(cdrProfitFieldNames.SaleCurrency);
            if (!saleCurrencyId.HasValue)
                throw new NullReferenceException("saleCurrencyId");

            decimal convertedCostAmount = new CurrencyExchangeRateManager().ConvertValueToCurrency(cdrCost.Amount.Value, cdrCost.CurrencyId.Value, saleCurrencyId.Value, attemptDateTime);
            decimal profitValue = saleAmount.Value - convertedCostAmount;
            ProfitStatus profitStatus = this.GetProfitStatus(profitValue, profitPrecision);

            if (!string.IsNullOrEmpty(cdrProfitFieldNames.Profit))
                cdr.SetFieldValue(cdrProfitFieldNames.Profit, profitValue);

            if (!string.IsNullOrEmpty(cdrProfitFieldNames.ProfitStatus))
                cdr.SetFieldValue(cdrProfitFieldNames.ProfitStatus, profitStatus);
        }

        private void FillCDRWithoutCostProfitData(dynamic cdr, CDRProfitFieldNames cdrProfitFieldNames)
        {
            if (cdrProfitFieldNames == null || string.IsNullOrEmpty(cdrProfitFieldNames.SaleAmount))
                return;

            decimal profitValue;
            ProfitStatus profitStatus;

            decimal? saleAmount = cdr.GetFieldValue(cdrProfitFieldNames.SaleAmount);
            if (!saleAmount.HasValue || saleAmount.Value == 0)
            {
                profitValue = 0;
                profitStatus = ProfitStatus.NoProfit;
            }
            else
            {
                profitValue = saleAmount.Value;
                profitStatus = ProfitStatus.Profitable;
            }

            if (!string.IsNullOrEmpty(cdrProfitFieldNames.Profit))
                cdr.SetFieldValue(cdrProfitFieldNames.Profit, profitValue);

            if (!string.IsNullOrEmpty(cdrProfitFieldNames.ProfitStatus))
                cdr.SetFieldValue(cdrProfitFieldNames.ProfitStatus, profitStatus);
        }

        private ProfitStatus GetProfitStatus(decimal profitValue, int? profitPrecision)
        {
            if (profitPrecision.HasValue)
                profitValue = decimal.Round(profitValue, profitPrecision.Value);

            if (Math.Abs(profitValue) == 0)
                return ProfitStatus.NoProfit;

            if (profitValue < 0)
                return ProfitStatus.Lossy;

            return ProfitStatus.Profitable;
        }

        #endregion

        #region Private Classes

        private class CDRCostData
        {
            public CDRCostData()
            {
                CDRCosts = new List<CDRCost>();
                CDRCostsByCGPN = new Dictionary<string, List<CDRCost>>();
            }

            public List<CDRCost> CDRCosts { get; set; }

            public Dictionary<string, List<CDRCost>> CDRCostsByCGPN { get; set; }
        }

        #endregion
    }

    public class CDRCostFieldNames
    {
        public string CDRCostId { get; set; }
        public string CostRate { get; set; }
        public string CostAmount { get; set; }
        public string CostCurrency { get; set; }
        public string SupplierName { get; set; }
    }

    public class CDRProfitFieldNames
    {
        public string SaleAmount { get; set; }
        public string SaleCurrency { get; set; }
        public string Profit { get; set; }
        public string ProfitStatus { get; set; }
    }
}