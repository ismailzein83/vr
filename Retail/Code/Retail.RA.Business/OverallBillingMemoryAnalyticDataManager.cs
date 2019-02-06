using System;
using System.Collections.Generic;
using System.Linq;
using Vanrise.Analytic.Business;
using Vanrise.Analytic.Entities;
using Vanrise.Common;

namespace Retail.RA.Business
{
    public class INTLOverallBillingMemoryAnalyticDataManager : MemoryAnalyticDataManager
    {
        public override List<RawMemoryRecord> GetRawRecords(Vanrise.Analytic.Entities.AnalyticQuery query, List<string> neededFieldNames)
        {
            List<RawMemoryRecord> rawMemoryRecords = new List<RawMemoryRecord>();
            var periodDefinitionManager = new PeriodDefinitionManager();

            List<long> filteredOperatorIds = null;
            List<object> filteredOperatorIdsObject = null;
            if (query.Filters != null)
            {
                var operatorDimensionFilter = query.Filters.FirstOrDefault(itm => itm.Dimension == "Operator");
                if (operatorDimensionFilter != null)
                    filteredOperatorIdsObject = operatorDimensionFilter.FilterValues;

                if (filteredOperatorIdsObject != null && filteredOperatorIdsObject.Count > 0)
                    filteredOperatorIds = operatorDimensionFilter.FilterValues.Select(itm => Convert.ToInt64(itm)).ToList();
            }

            var toTime = DateTime.Now;
            if (query.ToTime.HasValue)
                toTime = query.ToTime.Value;

            var analyticManager = new AnalyticManager();

            #region voice reconciliation
            AnalyticQuery voiceReconciliationAnalyticQuery = new AnalyticQuery
            {
                TableId = Guid.Parse("1da15e1b-3497-4b71-9bd9-a165b693f7af"),
                MeasureFields = new List<string> { "CalculatedDurationInMin", "CalculatedNumberOfCDR", "CalculatedRevenue", "DeclaredDurationInMin", "DeclaredNumberOfCDR", "DeclaredRevenue" },
                DimensionFields = new List<string> { "Operator", "Period", "TrafficDirection" },
                FromTime = query.FromTime,
                ToTime = toTime,
                Filters = new List<DimensionFilter>()
            };

            if (filteredOperatorIdsObject != null && filteredOperatorIdsObject.Count > 0)
            {
                DimensionFilter dimensionFilter = new DimensionFilter()
                {
                    Dimension = "Operator",
                    FilterValues = filteredOperatorIdsObject
                };
                voiceReconciliationAnalyticQuery.Filters.Add(dimensionFilter);
            }

            List<AnalyticRecord> voiceReconciliationAnalyticRecords = analyticManager.GetAllFilteredRecords(voiceReconciliationAnalyticQuery, out _);
            List<VoiceReconciliationRecord> voiceReconciliationRecords = GetVoiceReconciliationRecords(voiceReconciliationAnalyticRecords);
            #endregion

            #region sms reconciliation
            AnalyticQuery smsRecociliationAnalyticQuery = new AnalyticQuery
            {
                TableId = Guid.Parse("ef0ac053-731b-4d5a-93bf-52b4b4dbbb49"),
                MeasureFields = new List<string> { "CalculatedNumberOfSMS", "CalculatedRevenue", "DeclaredNumberOfSMS", "DeclaredRevenue" },
                DimensionFields = new List<string> { "Operator", "Period", "TrafficDirection" },
                FromTime = query.FromTime,
                ToTime = toTime,
                Filters = new List<DimensionFilter>()
            };

            if (filteredOperatorIdsObject != null && filteredOperatorIdsObject.Count > 0)
            {
                DimensionFilter dimensionFilter = new DimensionFilter()
                {
                    Dimension = "Operator",
                    FilterValues = filteredOperatorIdsObject
                };
                smsRecociliationAnalyticQuery.Filters.Add(dimensionFilter);
            }

            List<AnalyticRecord> smsReconciliationAnalyticRecords = analyticManager.GetAllFilteredRecords(smsRecociliationAnalyticQuery, out _);
            List<SMSReconciliationRecord> smsReconciliationRecords = GetSMSReconciliationRecords(smsReconciliationAnalyticRecords);
            #endregion

            #region voice billing stats
            AnalyticQuery voiceBillingStatsAnalyticQuery = new AnalyticQuery
            {
                TableId = Guid.Parse("efe9d65f-0b0e-4466-918f-4d8982a368f6"),
                MeasureFields = new List<string> { "Income" },
                DimensionFields = new List<string> { "Operator", "Period", "TrafficDirection" },
                FromTime = query.FromTime,
                ToTime = toTime,
                Filters = new List<DimensionFilter>()
            };

            if (filteredOperatorIdsObject != null && filteredOperatorIdsObject.Count > 0)
            {
                DimensionFilter dimensionFilter = new DimensionFilter()
                {
                    Dimension = "Operator",
                    FilterValues = filteredOperatorIdsObject
                };
                voiceBillingStatsAnalyticQuery.Filters.Add(dimensionFilter);
            }

            List<AnalyticRecord> voiceBillingAnalyticRecords = analyticManager.GetAllFilteredRecords(voiceBillingStatsAnalyticQuery, out _);
            List<VoiceBillingStatsRecord> voiceBillingStatsRecords = GetVoiceBillingStatsRecords(voiceBillingAnalyticRecords);
            #endregion

            #region sms billing stats
            AnalyticQuery smsBillingStatsAnalyticQuery = new AnalyticQuery
            {
                TableId = Guid.Parse("9ce5c4be-13f3-4830-948f-c7b18753b66d"),
                MeasureFields = new List<string> { "Income" },
                DimensionFields = new List<string> { "Operator", "Period", "TrafficDirection" },
                FromTime = query.FromTime,
                ToTime = toTime,
                Filters = new List<DimensionFilter>()
            };

            if (filteredOperatorIdsObject != null && filteredOperatorIdsObject.Count > 0)
            {
                DimensionFilter dimensionFilter = new DimensionFilter()
                {
                    Dimension = "Operator",
                    FilterValues = filteredOperatorIdsObject
                };
                smsBillingStatsAnalyticQuery.Filters.Add(dimensionFilter);
            }

            List<AnalyticRecord> smsBillingStatsAnalyticRecords = analyticManager.GetAllFilteredRecords(smsBillingStatsAnalyticQuery, out _);
            List<SMSBillingStatsRecord> smsBillingStatsRecords = GetSMSBillingStatsRecords(smsBillingStatsAnalyticRecords);
            #endregion

            var recordsDictionary = new Dictionary<OverallBillingRecordIdentifier, OverallBillingRecord>();

            foreach (var record in voiceReconciliationRecords)
            {
                var overallBillingRecord = recordsDictionary.GetOrCreateItem(new OverallBillingRecordIdentifier()
                { OperatorId = record.OperatorId, PeriodDefinitionId = record.PeriodDefinitionId, TrafficDirection = record.TrafficDirection },
                () => { return new OverallBillingRecord { OperatorId = record.OperatorId, PeriodDefinitionId = record.PeriodDefinitionId, TrafficDirection = record.TrafficDirection, CalculatedRevenue = 0, DeclaredRevenue = 0, Income = 0 }; });
                overallBillingRecord.CalculatedDurationInMin = record.CalculatedDurationInMin;
                overallBillingRecord.CalculatedNumberOfCDR = record.CalculatedNumberOfCDR;
                overallBillingRecord.CalculatedRevenue += record.CalculatedRevenue;
                overallBillingRecord.DeclaredDurationInMin = record.DeclaredDurationInMin;
                overallBillingRecord.DeclaredNumberOfCDR = record.DeclaredNumberOfCDR;
                overallBillingRecord.DeclaredRevenue += record.DeclaredRevenue;
            }

            foreach (var record in smsReconciliationRecords)
            {
                var overallBillingRecord = recordsDictionary.GetOrCreateItem(new OverallBillingRecordIdentifier()
                { OperatorId = record.OperatorId, PeriodDefinitionId = record.PeriodDefinitionId, TrafficDirection = record.TrafficDirection },
                () => { return new OverallBillingRecord { OperatorId = record.OperatorId, PeriodDefinitionId = record.PeriodDefinitionId, TrafficDirection = record.TrafficDirection, CalculatedRevenue = 0, DeclaredRevenue = 0, Income = 0 }; });
                overallBillingRecord.CalculatedNumberOfSMS = record.CalculatedNumberOfSMS;
                overallBillingRecord.CalculatedRevenue += record.CalculatedRevenue;
                overallBillingRecord.DeclaredNumberOfSMS = record.DeclaredNumberOfSMS;
                overallBillingRecord.DeclaredRevenue += record.DeclaredRevenue;
            }

            foreach (var record in voiceBillingStatsRecords)
            {
                var overallBillingRecord = recordsDictionary.GetOrCreateItem(new OverallBillingRecordIdentifier()
                { OperatorId = record.OperatorId, PeriodDefinitionId = record.PeriodDefinitionId, TrafficDirection = record.TrafficDirection },
                () => { return new OverallBillingRecord { OperatorId = record.OperatorId, PeriodDefinitionId = record.PeriodDefinitionId, TrafficDirection = record.TrafficDirection, CalculatedRevenue = 0, DeclaredRevenue = 0, Income = 0 }; });
                overallBillingRecord.Income += record.Income;
            }

            foreach (var record in smsBillingStatsRecords)
            {
                var overallBillingRecord = recordsDictionary.GetOrCreateItem(new OverallBillingRecordIdentifier()
                { OperatorId = record.OperatorId, PeriodDefinitionId = record.PeriodDefinitionId, TrafficDirection = record.TrafficDirection },
                () => { return new OverallBillingRecord { OperatorId = record.OperatorId, PeriodDefinitionId = record.PeriodDefinitionId, TrafficDirection = record.TrafficDirection, CalculatedRevenue = 0, DeclaredRevenue = 0, Income = 0 }; });
                overallBillingRecord.Income += record.Income;
            }

            var records = recordsDictionary.Values;

            foreach (var overallBillingRecord in records)
            {
                rawMemoryRecords.Add(GetRawMemoryRecord(overallBillingRecord));
            }

            return rawMemoryRecords;
        }

        #region Private Method
        public RawMemoryRecord GetRawMemoryRecord(OverallBillingRecord overallBillingRecord)
        {
            RawMemoryRecord rawMemoryRecord = new RawMemoryRecord { FieldValues = new Dictionary<string, dynamic>() };
            rawMemoryRecord.FieldValues.Add("OperatorID", overallBillingRecord.OperatorId);
            rawMemoryRecord.FieldValues.Add("PeriodID", overallBillingRecord.PeriodDefinitionId);
            rawMemoryRecord.FieldValues.Add("TrafficDirection", overallBillingRecord.TrafficDirection);
            rawMemoryRecord.FieldValues.Add("Income", overallBillingRecord.Income);

            rawMemoryRecord.FieldValues.Add("CalculatedNumberOfSMS", overallBillingRecord.CalculatedDurationInMin);
            rawMemoryRecord.FieldValues.Add("CalculatedNumberOfCDR", overallBillingRecord.CalculatedNumberOfCDR);
            rawMemoryRecord.FieldValues.Add("CalculatedNumberOfSMS", overallBillingRecord.CalculatedNumberOfSMS);
            rawMemoryRecord.FieldValues.Add("CalculatedRevenue", overallBillingRecord.CalculatedRevenue);

            rawMemoryRecord.FieldValues.Add("DeclaredDurationInMin", overallBillingRecord.DeclaredDurationInMin);
            rawMemoryRecord.FieldValues.Add("DeclaredNumberOfCDR", overallBillingRecord.DeclaredNumberOfCDR);
            rawMemoryRecord.FieldValues.Add("DeclaredNumberOfSMS", overallBillingRecord.DeclaredNumberOfSMS);
            rawMemoryRecord.FieldValues.Add("DeclaredRevenue", overallBillingRecord.DeclaredRevenue);

            return rawMemoryRecord;
        }

        private List<VoiceReconciliationRecord> GetVoiceReconciliationRecords(List<AnalyticRecord> analyticRecords)
        {
            List<VoiceReconciliationRecord> voiceReconciliationStatsRecords = new List<VoiceReconciliationRecord>();
            foreach (var analyticRecord in analyticRecords)
            {
                var operatorIdDimension = analyticRecord.DimensionValues[0];
                var periodDimension = analyticRecord.DimensionValues[1];
                var trafficDirectionDimension = analyticRecord.DimensionValues[2];

                if (periodDimension == null || periodDimension.Value == null)
                    continue;

                long operatorId = (long)operatorIdDimension.Value;
                int periodId = (int)periodDimension.Value;
                TrafficDirection trafficDirection = (TrafficDirection)((int)trafficDirectionDimension.Value);

                VoiceReconciliationRecord voiceReconciliationRecord = new VoiceReconciliationRecord
                {
                    OperatorId = operatorId,
                    PeriodDefinitionId = periodId,
                    TrafficDirection = trafficDirection
                };

                MeasureValue calculatedDurationInMin;
                analyticRecord.MeasureValues.TryGetValue("CalculatedDurationInMin", out calculatedDurationInMin);
                if (calculatedDurationInMin?.Value != null)
                    voiceReconciliationRecord.CalculatedDurationInMin = Convert.ToDecimal(calculatedDurationInMin.Value ?? 0);

                MeasureValue calculatedNumberOfCDR;
                analyticRecord.MeasureValues.TryGetValue("CalculatedNumberOfCDR", out calculatedNumberOfCDR);
                if (calculatedNumberOfCDR?.Value != null)
                    voiceReconciliationRecord.CalculatedNumberOfCDR = Convert.ToInt32(calculatedNumberOfCDR.Value ?? 0);

                MeasureValue calculatedRevenue;
                analyticRecord.MeasureValues.TryGetValue("CalculatedRevenue", out calculatedRevenue);
                if (calculatedRevenue?.Value != null)
                    voiceReconciliationRecord.CalculatedRevenue = Convert.ToDecimal(calculatedRevenue.Value ?? 0);

                MeasureValue declaredDurationInMin;
                analyticRecord.MeasureValues.TryGetValue("DeclaredDurationInMin", out declaredDurationInMin);
                if (declaredDurationInMin?.Value != null)
                    voiceReconciliationRecord.DeclaredDurationInMin = Convert.ToDecimal(declaredDurationInMin.Value ?? 0);

                MeasureValue declaredNumberOfCDR;
                analyticRecord.MeasureValues.TryGetValue("DeclaredNumberOfCDR", out declaredNumberOfCDR);
                if (declaredNumberOfCDR?.Value != null)
                    voiceReconciliationRecord.DeclaredNumberOfCDR = Convert.ToInt32(declaredNumberOfCDR.Value ?? 0);

                MeasureValue declaredRevenue;
                analyticRecord.MeasureValues.TryGetValue("DeclaredRevenue", out declaredRevenue);
                if (declaredRevenue?.Value != null)
                    voiceReconciliationRecord.DeclaredRevenue = Convert.ToDecimal(declaredRevenue.Value ?? 0);

                voiceReconciliationStatsRecords.Add(voiceReconciliationRecord);
            }

            return voiceReconciliationStatsRecords;
        }
        private List<SMSReconciliationRecord> GetSMSReconciliationRecords(List<AnalyticRecord> analyticRecords)
        {
            List<SMSReconciliationRecord> smsReconciliationStatsRecords = new List<SMSReconciliationRecord>();
            foreach (var analyticRecord in analyticRecords)
            {
                var operatorIdDimension = analyticRecord.DimensionValues[0];
                var periodDimension = analyticRecord.DimensionValues[1];
                var trafficDirectionDimension = analyticRecord.DimensionValues[2];

                if (periodDimension == null || periodDimension.Value == null)
                    continue;

                long operatorId = (long)operatorIdDimension.Value;
                int periodId = (int)periodDimension.Value;
                TrafficDirection trafficDirection = (TrafficDirection)((int)trafficDirectionDimension.Value);

                SMSReconciliationRecord smsReconciliationRecord = new SMSReconciliationRecord
                {
                    OperatorId = operatorId,
                    PeriodDefinitionId = periodId,
                    TrafficDirection = trafficDirection
                };

                MeasureValue calculatedNumberOfSMS;
                analyticRecord.MeasureValues.TryGetValue("CalculatedNumberOfSMS", out calculatedNumberOfSMS);
                if (calculatedNumberOfSMS?.Value != null)
                    smsReconciliationRecord.CalculatedNumberOfSMS = Convert.ToInt32(calculatedNumberOfSMS.Value ?? 0);

                MeasureValue calculatedRevenue;
                analyticRecord.MeasureValues.TryGetValue("CalculatedRevenue", out calculatedRevenue);
                if (calculatedRevenue?.Value != null)
                    smsReconciliationRecord.CalculatedRevenue = Convert.ToDecimal(calculatedRevenue.Value ?? 0);

                MeasureValue declaredNumberOfSMS;
                analyticRecord.MeasureValues.TryGetValue("DeclaredNumberOfSMS", out declaredNumberOfSMS);
                if (declaredNumberOfSMS?.Value != null)
                    smsReconciliationRecord.DeclaredNumberOfSMS = Convert.ToInt32(declaredNumberOfSMS.Value ?? 0);

                MeasureValue declaredRevenue;
                analyticRecord.MeasureValues.TryGetValue("DeclaredRevenue", out declaredRevenue);
                if (declaredRevenue?.Value != null)
                    smsReconciliationRecord.DeclaredRevenue = Convert.ToDecimal(declaredRevenue.Value ?? 0);

                smsReconciliationStatsRecords.Add(smsReconciliationRecord);
            }

            return smsReconciliationStatsRecords;
        }
        private List<VoiceBillingStatsRecord> GetVoiceBillingStatsRecords(List<AnalyticRecord> analyticRecords)
        {
            List<VoiceBillingStatsRecord> voiceBillingStatsRecords = new List<VoiceBillingStatsRecord>();
            foreach (var analyticRecord in analyticRecords)
            {
                var operatorIdDimension = analyticRecord.DimensionValues[0];
                var periodDimension = analyticRecord.DimensionValues[1];
                var trafficDirectionDimension = analyticRecord.DimensionValues[2];

                if (periodDimension == null || periodDimension.Value == null)
                    continue;

                long operatorId = (long)operatorIdDimension.Value;
                int periodId = (int)periodDimension.Value;
                TrafficDirection trafficDirection = (TrafficDirection)((int)trafficDirectionDimension.Value);

                VoiceBillingStatsRecord voiceBillingStatsRecord = new VoiceBillingStatsRecord
                {
                    OperatorId = operatorId,
                    PeriodDefinitionId = periodId,
                    TrafficDirection = trafficDirection
                };

                MeasureValue income;
                analyticRecord.MeasureValues.TryGetValue("Income", out income);
                if (income?.Value != null)
                    voiceBillingStatsRecord.Income = Convert.ToDecimal(income.Value ?? 0);

                voiceBillingStatsRecords.Add(voiceBillingStatsRecord);
            }

            return voiceBillingStatsRecords;
        }
        private List<SMSBillingStatsRecord> GetSMSBillingStatsRecords(List<AnalyticRecord> analyticRecords)
        {
            List<SMSBillingStatsRecord> smsBillingStatsRecords = new List<SMSBillingStatsRecord>();
            foreach (var analyticRecord in analyticRecords)
            {
                var operatorIdDimension = analyticRecord.DimensionValues[0];
                var periodDimension = analyticRecord.DimensionValues[1];
                var trafficDirectionDimension = analyticRecord.DimensionValues[2];

                if (periodDimension == null || periodDimension.Value == null)
                    continue;

                long operatorId = (long)operatorIdDimension.Value;
                int periodId = (int)periodDimension.Value;
                TrafficDirection trafficDirection = (TrafficDirection)((int)trafficDirectionDimension.Value);

                SMSBillingStatsRecord smsBillingStatsRecord = new SMSBillingStatsRecord
                {
                    OperatorId = operatorId,
                    PeriodDefinitionId = periodId,
                    TrafficDirection = trafficDirection
                };

                MeasureValue income;
                analyticRecord.MeasureValues.TryGetValue("Income", out income);
                if (income?.Value != null)
                    smsBillingStatsRecord.Income = Convert.ToDecimal(income.Value ?? 0);

                smsBillingStatsRecords.Add(smsBillingStatsRecord);
            }

            return smsBillingStatsRecords;
        }

        //private Dictionary<long, List<SMSBillingStatsRecord>> GetSMSBillingStatsRecordsByOperator(List<AnalyticRecord> analyticRecords)
        //{
        //    Dictionary<long, List<SMSBillingStatsRecord>> smsBillingStatsRecordsByOperator = new Dictionary<long, List<SMSBillingStatsRecord>>();
        //    foreach (var analyticRecord in analyticRecords)
        //    {
        //        var operatorIdDimension = analyticRecord.DimensionValues[0];
        //        var periodDimension = analyticRecord.DimensionValues[1];
        //        var trafficDirectionDimension = analyticRecord.DimensionValues[2];

        //        if (periodDimension == null || periodDimension.Value == null)
        //            continue;

        //        long operatorId = (long)operatorIdDimension.Value;
        //        int periodId = (int)periodDimension.Value;
        //        TrafficDirection trafficDirection = (TrafficDirection)((int)trafficDirectionDimension.Value);

        //        SMSBillingStatsRecord smsBillingStatsRecord = new SMSBillingStatsRecord
        //        {
        //            OperatorId = operatorId,
        //            PeriodDefinitionId = periodId,
        //            TrafficDirection = trafficDirection
        //        };

        //        MeasureValue calculatedRevenue;
        //        analyticRecord.MeasureValues.TryGetValue("Income", out calculatedRevenue);
        //        if (calculatedRevenue?.Value != null)
        //            smsBillingStatsRecord.Income = Convert.ToDecimal(calculatedRevenue.Value ?? 0);

        //        List<SMSBillingStatsRecord> smsBillingStatsRecords = smsBillingStatsRecordsByOperator.GetOrCreateItem(operatorId);
        //        smsBillingStatsRecords.Add(smsBillingStatsRecord);
        //    }

        //    return smsBillingStatsRecordsByOperator;
        //}
        #endregion
    }

    public class ICXOverallBillingMemoryAnalyticDataManager : MemoryAnalyticDataManager
    {
        public override List<RawMemoryRecord> GetRawRecords(Vanrise.Analytic.Entities.AnalyticQuery query, List<string> neededFieldNames)
        {
            List<RawMemoryRecord> rawMemoryRecords = new List<RawMemoryRecord>();
            var periodDefinitionManager = new PeriodDefinitionManager();

            List<long> filteredOperatorIds = null;
            List<object> filteredOperatorIdsObject = null;
            if (query.Filters != null)
            {
                var operatorDimensionFilter = query.Filters.FirstOrDefault(itm => itm.Dimension == "Operator");
                if (operatorDimensionFilter != null)
                    filteredOperatorIdsObject = operatorDimensionFilter.FilterValues;

                if (filteredOperatorIdsObject != null && filteredOperatorIdsObject.Count > 0)
                    filteredOperatorIds = operatorDimensionFilter.FilterValues.Select(itm => Convert.ToInt64(itm)).ToList();
            }

            var toTime = DateTime.Now;
            if (query.ToTime.HasValue)
                toTime = query.ToTime.Value;

            var analyticManager = new AnalyticManager();

            #region voice reconciliation
            AnalyticQuery voiceReconciliationAnalyticQuery = new AnalyticQuery
            {
                TableId = Guid.Parse("1c2800db-17aa-4697-a04a-d81bbcd1452a"),
                MeasureFields = new List<string> { "CalculatedDurationInMin", "CalculatedNumberOfCDR", "CalculatedRevenue", "DeclaredDurationInMin", "DeclaredNumberOfCDR", "DeclaredRevenue" },
                DimensionFields = new List<string> { "Operator", "Period", "TrafficDirection" },
                FromTime = query.FromTime,
                ToTime = toTime,
                Filters = new List<DimensionFilter>()
            };

            if (filteredOperatorIdsObject != null && filteredOperatorIdsObject.Count > 0)
            {
                DimensionFilter dimensionFilter = new DimensionFilter()
                {
                    Dimension = "Operator",
                    FilterValues = filteredOperatorIdsObject
                };
                voiceReconciliationAnalyticQuery.Filters.Add(dimensionFilter);
            }

            List<AnalyticRecord> voiceReconciliationAnalyticRecords = analyticManager.GetAllFilteredRecords(voiceReconciliationAnalyticQuery, out _);
            List<VoiceReconciliationRecord> voiceReconciliationRecords = GetVoiceReconciliationRecords(voiceReconciliationAnalyticRecords);
            #endregion

            #region sms reconciliation
            AnalyticQuery smsRecociliationAnalyticQuery = new AnalyticQuery
            {
                TableId = Guid.Parse("9b0ed423-78ec-4047-b02c-549e10c74de0"),
                MeasureFields = new List<string> { "CalculatedNumberOfSMS", "CalculatedRevenue", "DeclaredNumberOfSMS", "DeclaredRevenue" },
                DimensionFields = new List<string> { "Operator", "Period", "TrafficDirection" },
                FromTime = query.FromTime,
                ToTime = toTime,
                Filters = new List<DimensionFilter>()
            };

            if (filteredOperatorIdsObject != null && filteredOperatorIdsObject.Count > 0)
            {
                DimensionFilter dimensionFilter = new DimensionFilter()
                {
                    Dimension = "Operator",
                    FilterValues = filteredOperatorIdsObject
                };
                smsRecociliationAnalyticQuery.Filters.Add(dimensionFilter);
            }

            List<AnalyticRecord> smsReconciliationAnalyticRecords = analyticManager.GetAllFilteredRecords(smsRecociliationAnalyticQuery, out _);
            List<SMSReconciliationRecord> smsReconciliationRecords = GetSMSReconciliationRecords(smsReconciliationAnalyticRecords);
            #endregion

            #region voice billing stats
            AnalyticQuery voiceBillingStatsAnalyticQuery = new AnalyticQuery
            {
                TableId = Guid.Parse("8e3baf48-a2d3-4b09-8198-cbb4ad6f76f5"),
                MeasureFields = new List<string> { "Income" },
                DimensionFields = new List<string> { "Operator", "Period", "TrafficDirection" },
                FromTime = query.FromTime,
                ToTime = toTime,
                Filters = new List<DimensionFilter>()
            };

            if (filteredOperatorIdsObject != null && filteredOperatorIdsObject.Count > 0)
            {
                DimensionFilter dimensionFilter = new DimensionFilter()
                {
                    Dimension = "Operator",
                    FilterValues = filteredOperatorIdsObject
                };
                voiceBillingStatsAnalyticQuery.Filters.Add(dimensionFilter);
            }

            List<AnalyticRecord> voiceBillingAnalyticRecords = analyticManager.GetAllFilteredRecords(voiceBillingStatsAnalyticQuery, out _);
            List<VoiceBillingStatsRecord> voiceBillingStatsRecords = GetVoiceBillingStatsRecords(voiceBillingAnalyticRecords);
            #endregion

            #region sms billing stats
            AnalyticQuery smsBillingStatsAnalyticQuery = new AnalyticQuery
            {
                TableId = Guid.Parse("193cb09a-1817-42fb-b0a1-9317f05391c0"),
                MeasureFields = new List<string> { "Income" },
                DimensionFields = new List<string> { "Operator", "Period", "TrafficDirection" },
                FromTime = query.FromTime,
                ToTime = toTime,
                Filters = new List<DimensionFilter>()
            };

            if (filteredOperatorIdsObject != null && filteredOperatorIdsObject.Count > 0)
            {
                DimensionFilter dimensionFilter = new DimensionFilter()
                {
                    Dimension = "Operator",
                    FilterValues = filteredOperatorIdsObject
                };
                smsBillingStatsAnalyticQuery.Filters.Add(dimensionFilter);
            }

            List<AnalyticRecord> smsBillingStatsAnalyticRecords = analyticManager.GetAllFilteredRecords(smsBillingStatsAnalyticQuery, out _);
            List<SMSBillingStatsRecord> smsBillingStatsRecords = GetSMSBillingStatsRecords(smsBillingStatsAnalyticRecords);
            #endregion

            var recordsDictionary = new Dictionary<OverallBillingRecordIdentifier, OverallBillingRecord>();

            foreach (var record in voiceReconciliationRecords)
            {
                var overallBillingRecord = recordsDictionary.GetOrCreateItem(new OverallBillingRecordIdentifier()
                { OperatorId = record.OperatorId, PeriodDefinitionId = record.PeriodDefinitionId, TrafficDirection = record.TrafficDirection },
                () => { return new OverallBillingRecord { OperatorId = record.OperatorId, PeriodDefinitionId = record.PeriodDefinitionId, TrafficDirection = record.TrafficDirection, CalculatedRevenue = 0, DeclaredRevenue = 0, Income = 0 }; });
                overallBillingRecord.CalculatedDurationInMin = record.CalculatedDurationInMin;
                overallBillingRecord.CalculatedNumberOfCDR = record.CalculatedNumberOfCDR;
                overallBillingRecord.CalculatedRevenue += record.CalculatedRevenue;
                overallBillingRecord.DeclaredDurationInMin = record.DeclaredDurationInMin;
                overallBillingRecord.DeclaredNumberOfCDR = record.DeclaredNumberOfCDR;
                overallBillingRecord.DeclaredRevenue += record.DeclaredRevenue;
            }

            foreach (var record in smsReconciliationRecords)
            {
                var overallBillingRecord = recordsDictionary.GetOrCreateItem(new OverallBillingRecordIdentifier()
                { OperatorId = record.OperatorId, PeriodDefinitionId = record.PeriodDefinitionId, TrafficDirection = record.TrafficDirection },
                () => { return new OverallBillingRecord { OperatorId = record.OperatorId, PeriodDefinitionId = record.PeriodDefinitionId, TrafficDirection = record.TrafficDirection, CalculatedRevenue = 0, DeclaredRevenue = 0, Income = 0 }; });
                overallBillingRecord.CalculatedNumberOfSMS = record.CalculatedNumberOfSMS;
                overallBillingRecord.CalculatedRevenue += record.CalculatedRevenue;
                overallBillingRecord.DeclaredNumberOfSMS = record.DeclaredNumberOfSMS;
                overallBillingRecord.DeclaredRevenue += record.DeclaredRevenue;
            }

            foreach (var record in voiceBillingStatsRecords)
            {
                var overallBillingRecord = recordsDictionary.GetOrCreateItem(new OverallBillingRecordIdentifier()
                { OperatorId = record.OperatorId, PeriodDefinitionId = record.PeriodDefinitionId, TrafficDirection = record.TrafficDirection },
                () => { return new OverallBillingRecord { OperatorId = record.OperatorId, PeriodDefinitionId = record.PeriodDefinitionId, TrafficDirection = record.TrafficDirection, CalculatedRevenue = 0, DeclaredRevenue = 0, Income = 0 }; });
                overallBillingRecord.Income += record.Income;
            }

            foreach (var record in smsBillingStatsRecords)
            {
                var overallBillingRecord = recordsDictionary.GetOrCreateItem(new OverallBillingRecordIdentifier()
                { OperatorId = record.OperatorId, PeriodDefinitionId = record.PeriodDefinitionId, TrafficDirection = record.TrafficDirection },
                () => { return new OverallBillingRecord { OperatorId = record.OperatorId, PeriodDefinitionId = record.PeriodDefinitionId, TrafficDirection = record.TrafficDirection, CalculatedRevenue = 0, DeclaredRevenue = 0, Income = 0 }; });
                overallBillingRecord.Income += record.Income;
            }

            var records = recordsDictionary.Values;

            foreach (var overallBillingRecord in records)
            {
                rawMemoryRecords.Add(GetRawMemoryRecord(overallBillingRecord));
            }

            return rawMemoryRecords;
        }

        #region Private Method
        public RawMemoryRecord GetRawMemoryRecord(OverallBillingRecord overallBillingRecord)
        {
            RawMemoryRecord rawMemoryRecord = new RawMemoryRecord { FieldValues = new Dictionary<string, dynamic>() };
            rawMemoryRecord.FieldValues.Add("OperatorID", overallBillingRecord.OperatorId);
            rawMemoryRecord.FieldValues.Add("PeriodID", overallBillingRecord.PeriodDefinitionId);
            rawMemoryRecord.FieldValues.Add("TrafficDirection", overallBillingRecord.TrafficDirection);
            rawMemoryRecord.FieldValues.Add("Income", overallBillingRecord.Income);

            rawMemoryRecord.FieldValues.Add("CalculatedNumberOfSMS", overallBillingRecord.CalculatedDurationInMin);
            rawMemoryRecord.FieldValues.Add("CalculatedNumberOfCDR", overallBillingRecord.CalculatedNumberOfCDR);
            rawMemoryRecord.FieldValues.Add("CalculatedNumberOfSMS", overallBillingRecord.CalculatedNumberOfSMS);
            rawMemoryRecord.FieldValues.Add("CalculatedRevenue", overallBillingRecord.CalculatedRevenue);

            rawMemoryRecord.FieldValues.Add("DeclaredDurationInMin", overallBillingRecord.DeclaredDurationInMin);
            rawMemoryRecord.FieldValues.Add("DeclaredNumberOfCDR", overallBillingRecord.DeclaredNumberOfCDR);
            rawMemoryRecord.FieldValues.Add("DeclaredNumberOfSMS", overallBillingRecord.DeclaredNumberOfSMS);
            rawMemoryRecord.FieldValues.Add("DeclaredRevenue", overallBillingRecord.DeclaredRevenue);

            return rawMemoryRecord;
        }

        private List<VoiceReconciliationRecord> GetVoiceReconciliationRecords(List<AnalyticRecord> analyticRecords)
        {
            List<VoiceReconciliationRecord> voiceReconciliationStatsRecords = new List<VoiceReconciliationRecord>();
            foreach (var analyticRecord in analyticRecords)
            {
                var operatorIdDimension = analyticRecord.DimensionValues[0];
                var periodDimension = analyticRecord.DimensionValues[1];
                var trafficDirectionDimension = analyticRecord.DimensionValues[2];

                if (periodDimension == null || periodDimension.Value == null)
                    continue;

                long operatorId = (long)operatorIdDimension.Value;
                int periodId = (int)periodDimension.Value;
                TrafficDirection trafficDirection = (TrafficDirection)((int)trafficDirectionDimension.Value);

                VoiceReconciliationRecord voiceReconciliationRecord = new VoiceReconciliationRecord
                {
                    OperatorId = operatorId,
                    PeriodDefinitionId = periodId,
                    TrafficDirection = trafficDirection
                };

                MeasureValue calculatedDurationInMin;
                analyticRecord.MeasureValues.TryGetValue("CalculatedDurationInMin", out calculatedDurationInMin);
                if (calculatedDurationInMin?.Value != null)
                    voiceReconciliationRecord.CalculatedDurationInMin = Convert.ToDecimal(calculatedDurationInMin.Value ?? 0);

                MeasureValue calculatedNumberOfCDR;
                analyticRecord.MeasureValues.TryGetValue("CalculatedNumberOfCDR", out calculatedNumberOfCDR);
                if (calculatedNumberOfCDR?.Value != null)
                    voiceReconciliationRecord.CalculatedNumberOfCDR = Convert.ToInt32(calculatedNumberOfCDR.Value ?? 0);

                MeasureValue calculatedRevenue;
                analyticRecord.MeasureValues.TryGetValue("CalculatedRevenue", out calculatedRevenue);
                if (calculatedRevenue?.Value != null)
                    voiceReconciliationRecord.CalculatedRevenue = Convert.ToDecimal(calculatedRevenue.Value ?? 0);

                MeasureValue declaredDurationInMin;
                analyticRecord.MeasureValues.TryGetValue("DeclaredDurationInMin", out declaredDurationInMin);
                if (declaredDurationInMin?.Value != null)
                    voiceReconciliationRecord.DeclaredDurationInMin = Convert.ToDecimal(declaredDurationInMin.Value ?? 0);

                MeasureValue declaredNumberOfCDR;
                analyticRecord.MeasureValues.TryGetValue("DeclaredNumberOfCDR", out declaredNumberOfCDR);
                if (declaredNumberOfCDR?.Value != null)
                    voiceReconciliationRecord.DeclaredNumberOfCDR = Convert.ToInt32(declaredNumberOfCDR.Value ?? 0);

                MeasureValue declaredRevenue;
                analyticRecord.MeasureValues.TryGetValue("DeclaredRevenue", out declaredRevenue);
                if (declaredRevenue?.Value != null)
                    voiceReconciliationRecord.DeclaredRevenue = Convert.ToDecimal(declaredRevenue.Value ?? 0);

                voiceReconciliationStatsRecords.Add(voiceReconciliationRecord);
            }

            return voiceReconciliationStatsRecords;
        }
        private List<SMSReconciliationRecord> GetSMSReconciliationRecords(List<AnalyticRecord> analyticRecords)
        {
            List<SMSReconciliationRecord> smsReconciliationStatsRecords = new List<SMSReconciliationRecord>();
            foreach (var analyticRecord in analyticRecords)
            {
                var operatorIdDimension = analyticRecord.DimensionValues[0];
                var periodDimension = analyticRecord.DimensionValues[1];
                var trafficDirectionDimension = analyticRecord.DimensionValues[2];

                if (periodDimension == null || periodDimension.Value == null)
                    continue;

                long operatorId = (long)operatorIdDimension.Value;
                int periodId = (int)periodDimension.Value;
                TrafficDirection trafficDirection = (TrafficDirection)((int)trafficDirectionDimension.Value);

                SMSReconciliationRecord smsReconciliationRecord = new SMSReconciliationRecord
                {
                    OperatorId = operatorId,
                    PeriodDefinitionId = periodId,
                    TrafficDirection = trafficDirection
                };

                MeasureValue calculatedNumberOfSMS;
                analyticRecord.MeasureValues.TryGetValue("CalculatedNumberOfSMS", out calculatedNumberOfSMS);
                if (calculatedNumberOfSMS?.Value != null)
                    smsReconciliationRecord.CalculatedNumberOfSMS = Convert.ToInt32(calculatedNumberOfSMS.Value ?? 0);

                MeasureValue calculatedRevenue;
                analyticRecord.MeasureValues.TryGetValue("CalculatedRevenue", out calculatedRevenue);
                if (calculatedRevenue?.Value != null)
                    smsReconciliationRecord.CalculatedRevenue = Convert.ToDecimal(calculatedRevenue.Value ?? 0);

                MeasureValue declaredNumberOfSMS;
                analyticRecord.MeasureValues.TryGetValue("DeclaredNumberOfSMS", out declaredNumberOfSMS);
                if (declaredNumberOfSMS?.Value != null)
                    smsReconciliationRecord.DeclaredNumberOfSMS = Convert.ToInt32(declaredNumberOfSMS.Value ?? 0);

                MeasureValue declaredRevenue;
                analyticRecord.MeasureValues.TryGetValue("DeclaredRevenue", out declaredRevenue);
                if (declaredRevenue?.Value != null)
                    smsReconciliationRecord.DeclaredRevenue = Convert.ToDecimal(declaredRevenue.Value ?? 0);

                smsReconciliationStatsRecords.Add(smsReconciliationRecord);
            }

            return smsReconciliationStatsRecords;
        }
        private List<VoiceBillingStatsRecord> GetVoiceBillingStatsRecords(List<AnalyticRecord> analyticRecords)
        {
            List<VoiceBillingStatsRecord> voiceBillingStatsRecords = new List<VoiceBillingStatsRecord>();
            foreach (var analyticRecord in analyticRecords)
            {
                var operatorIdDimension = analyticRecord.DimensionValues[0];
                var periodDimension = analyticRecord.DimensionValues[1];
                var trafficDirectionDimension = analyticRecord.DimensionValues[2];

                if (periodDimension == null || periodDimension.Value == null)
                    continue;

                long operatorId = (long)operatorIdDimension.Value;
                int periodId = (int)periodDimension.Value;
                TrafficDirection trafficDirection = (TrafficDirection)((int)trafficDirectionDimension.Value);

                VoiceBillingStatsRecord voiceBillingStatsRecord = new VoiceBillingStatsRecord
                {
                    OperatorId = operatorId,
                    PeriodDefinitionId = periodId,
                    TrafficDirection = trafficDirection
                };

                MeasureValue income;
                analyticRecord.MeasureValues.TryGetValue("Income", out income);
                if (income?.Value != null)
                    voiceBillingStatsRecord.Income = Convert.ToDecimal(income.Value ?? 0);

                voiceBillingStatsRecords.Add(voiceBillingStatsRecord);
            }

            return voiceBillingStatsRecords;
        }
        private List<SMSBillingStatsRecord> GetSMSBillingStatsRecords(List<AnalyticRecord> analyticRecords)
        {
            List<SMSBillingStatsRecord> smsBillingStatsRecords = new List<SMSBillingStatsRecord>();
            foreach (var analyticRecord in analyticRecords)
            {
                var operatorIdDimension = analyticRecord.DimensionValues[0];
                var periodDimension = analyticRecord.DimensionValues[1];
                var trafficDirectionDimension = analyticRecord.DimensionValues[2];

                if (periodDimension == null || periodDimension.Value == null)
                    continue;

                long operatorId = (long)operatorIdDimension.Value;
                int periodId = (int)periodDimension.Value;
                TrafficDirection trafficDirection = (TrafficDirection)((int)trafficDirectionDimension.Value);

                SMSBillingStatsRecord smsBillingStatsRecord = new SMSBillingStatsRecord
                {
                    OperatorId = operatorId,
                    PeriodDefinitionId = periodId,
                    TrafficDirection = trafficDirection
                };

                MeasureValue income;
                analyticRecord.MeasureValues.TryGetValue("Income", out income);
                if (income?.Value != null)
                    smsBillingStatsRecord.Income = Convert.ToDecimal(income.Value ?? 0);

                smsBillingStatsRecords.Add(smsBillingStatsRecord);
            }

            return smsBillingStatsRecords;
        }
        #endregion
    }

    public class OverallBillingRecord
    {
        public long OperatorId { get; set; }
        public int PeriodDefinitionId { get; set; }
        public TrafficDirection TrafficDirection { get; set; }
        public decimal Income { get; set; }
        public decimal CalculatedDurationInMin { get; set; }
        public int CalculatedNumberOfCDR { get; set; }
        public int CalculatedNumberOfSMS { get; set; }
        public decimal CalculatedRevenue { get; set; }
        public decimal DeclaredDurationInMin { get; set; }
        public int DeclaredNumberOfCDR { get; set; }
        public int DeclaredNumberOfSMS { get; set; }
        public decimal DeclaredRevenue { get; set; }
    }
    public class VoiceReconciliationRecord
    {
        public long OperatorId { get; set; }
        public int PeriodDefinitionId { get; set; }
        public TrafficDirection TrafficDirection { get; set; }
        public decimal CalculatedDurationInMin { get; set; }
        public int CalculatedNumberOfCDR { get; set; }
        public decimal CalculatedRevenue { get; set; }
        public decimal DeclaredDurationInMin { get; set; }
        public int DeclaredNumberOfCDR { get; set; }
        public decimal DeclaredRevenue { get; set; }
    }
    public class SMSReconciliationRecord
    {
        public long OperatorId { get; set; }
        public int PeriodDefinitionId { get; set; }
        public TrafficDirection TrafficDirection { get; set; }
        public int CalculatedNumberOfSMS { get; set; }
        public decimal CalculatedRevenue { get; set; }
        public int DeclaredNumberOfSMS { get; set; }
        public decimal DeclaredRevenue { get; set; }
    }
    public class VoiceBillingStatsRecord
    {
        public long OperatorId { get; set; }
        public int PeriodDefinitionId { get; set; }
        public TrafficDirection TrafficDirection { get; set; }
        public decimal Income { get; set; }
    }
    public class SMSBillingStatsRecord
    {
        public long OperatorId { get; set; }
        public int PeriodDefinitionId { get; set; }
        public TrafficDirection TrafficDirection { get; set; }
        public decimal Income { get; set; }
    }
    public struct OverallBillingRecordIdentifier
    {
        public long OperatorId { get; set; }
        public int PeriodDefinitionId { get; set; }
        public TrafficDirection TrafficDirection { get; set; }
        public override int GetHashCode()
        {
            return OperatorId.GetHashCode() + PeriodDefinitionId.GetHashCode() + TrafficDirection.GetHashCode();
        }
    }

}
