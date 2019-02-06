using System;
using System.Collections.Generic;
using System.Linq;
using Vanrise.Analytic.Business;
using Vanrise.Analytic.Entities;
using Vanrise.Common;

namespace Retail.RA.Business
{
    public class SMSINTLReconciliationMemoryAnalyticDataManager : MemoryAnalyticDataManager
    {
        public override List<RawMemoryRecord> GetRawRecords(Vanrise.Analytic.Entities.AnalyticQuery query, List<string> neededFieldNames)
        {
            List<RawMemoryRecord> rawMemoryRecords = new List<RawMemoryRecord>();
            var operatorDeclarationManager = new IntlOperatorDeclarationManager();
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

            var periodDefinitions = periodDefinitionManager.GetPeriodDefinitionsBetweenDate(query.FromTime, toTime, out var minDate, out var maxDate);
            if (periodDefinitions == null || periodDefinitions.Count == 0)
                return rawMemoryRecords;

            var allOperatorDeclarations = operatorDeclarationManager.GetSMSDeclarationServices(periodDefinitions, filteredOperatorIds);

            var analyticManager = new AnalyticManager();
            AnalyticQuery analyticQuery = new AnalyticQuery
            {
                TableId = Guid.Parse("9ce5c4be-13f3-4830-948f-c7b18753b66d"),
                MeasureFields = new List<string> { "Revenue", "NumberOfSMSs" },
                DimensionFields = new List<string> { "Operator", "Period", "TrafficDirection" },
                FromTime = Utilities.Min(minDate, query.FromTime),
                ToTime = Utilities.Max(maxDate, toTime),
                Filters = new List<DimensionFilter>()
            };

            if (filteredOperatorIdsObject != null && filteredOperatorIdsObject.Count > 0)
            {
                DimensionFilter dimensionFilter = new DimensionFilter()
                {
                    Dimension = "Operator",
                    FilterValues = filteredOperatorIdsObject
                };
                analyticQuery.Filters.Add(dimensionFilter);
            }

            List<AnalyticRecord> analyticRecords = analyticManager.GetAllFilteredRecords(analyticQuery, out _);
            Dictionary<long, List<SMSBillingRecord>> smsBillingRecordsByOperator = GetSMSBillingRecordsByOperator(analyticRecords);

            var records = new List<SMSReconcilationObj>();
            int startingIndex = 0;
            long lastOperatorId = 0;

            if (allOperatorDeclarations != null && allOperatorDeclarations.Any())
            {
                foreach (var operatorDeclaration in allOperatorDeclarations)
                {
                    var operatorId = operatorDeclaration.OperatorId;
                    if (lastOperatorId != operatorId)
                    {
                        startingIndex = 0;
                        lastOperatorId = operatorId;
                    }
                    var smsReconcilationObj = new SMSReconcilationObj
                    {
                        PeriodId = operatorDeclaration.PeriodDefinition.PeriodDefinitionId,
                        OperatorId = operatorId,
                        TrafficDirection = operatorDeclaration.SMSSettings.TrafficDirection,
                        DeclaredRevenue = operatorDeclaration.SMSSettings.Revenue,
                        DeclaredNbrOfSMS = operatorDeclaration.SMSSettings.NumberOfSMSs
                    };

                    if (smsBillingRecordsByOperator.TryGetValue(operatorId, out var operatorSMSBillingRecords))
                    {
                        List<SMSBillingRecord> orderedBillingRecords = operatorSMSBillingRecords.OrderBy(item => item.PeriodDefinitionId).ToList();
                        for (int i = startingIndex; i < orderedBillingRecords.Count; i++)
                        {
                            var smsbillingRecord = orderedBillingRecords[i];
                            if (smsbillingRecord.PeriodDefinitionId == operatorDeclaration.PeriodDefinition.PeriodDefinitionId && smsbillingRecord.TrafficDirection == operatorDeclaration.SMSSettings.TrafficDirection)
                            {
                                smsReconcilationObj.CalculatedRevenue += smsbillingRecord.Revenue;
                                smsReconcilationObj.CalculatedNbrOfSMS += smsbillingRecord.NbrOfSMS;
                                operatorSMSBillingRecords.Remove(smsbillingRecord);
                            }
                            else
                            {
                                startingIndex = i;
                            }
                        }
                    }
                    records.Add(smsReconcilationObj);
                }
            }

            Dictionary<string, SMSReconcilationObj> smsBillingRecordWithoutOperatorDeclaration = new Dictionary<string, SMSReconcilationObj>();
            if (smsBillingRecordsByOperator != null)
            {
                foreach (var smsBillingRecords in smsBillingRecordsByOperator.Values)
                {
                    if (smsBillingRecords != null && smsBillingRecords.Count > 0)
                    {
                        foreach (var smsBillingRecord in smsBillingRecords)
                        {
                            string key = $"{smsBillingRecord.OperatorId}_{smsBillingRecord.PeriodDefinitionId}_{(int)smsBillingRecord.TrafficDirection}";
                            var recordObj = smsBillingRecordWithoutOperatorDeclaration.GetOrCreateItem(key);
                            recordObj.OperatorId = smsBillingRecord.OperatorId;
                            recordObj.PeriodId = smsBillingRecord.PeriodDefinitionId;
                            recordObj.CalculatedNbrOfSMS += smsBillingRecord.NbrOfSMS;
                            recordObj.CalculatedRevenue += smsBillingRecord.Revenue;
                            recordObj.TrafficDirection = smsBillingRecord.TrafficDirection;
                        }
                    }
                }
            }
            if (smsBillingRecordWithoutOperatorDeclaration.Count > 0)
                records.AddRange(smsBillingRecordWithoutOperatorDeclaration.Values);

            foreach (var smsReconcilationObj in records)
            {
                rawMemoryRecords.Add(GetRawMemoryRecord(smsReconcilationObj));
            }
            return rawMemoryRecords;
        }

        public RawMemoryRecord GetRawMemoryRecord(SMSReconcilationObj smsReconcilationObj)
        {
            RawMemoryRecord rawMemoryRecord = new RawMemoryRecord { FieldValues = new Dictionary<string, dynamic>() };
            rawMemoryRecord.FieldValues.Add("OperatorID", smsReconcilationObj.OperatorId);
            rawMemoryRecord.FieldValues.Add("PeriodID", smsReconcilationObj.PeriodId);
            rawMemoryRecord.FieldValues.Add("TrafficDirection", smsReconcilationObj.TrafficDirection);
            rawMemoryRecord.FieldValues.Add("CalculatedRevenue", smsReconcilationObj.CalculatedRevenue);
            rawMemoryRecord.FieldValues.Add("CalculatedNumberOfSMS", smsReconcilationObj.CalculatedNbrOfSMS);
            rawMemoryRecord.FieldValues.Add("DeclaredNumberOfSMS", smsReconcilationObj.DeclaredNbrOfSMS);
            rawMemoryRecord.FieldValues.Add("DeclaredRevenue", smsReconcilationObj.DeclaredRevenue);

            return rawMemoryRecord;
        }

        private Dictionary<long, List<SMSBillingRecord>> GetSMSBillingRecordsByOperator(List<AnalyticRecord> analyticRecords)
        {
            Dictionary<long, List<SMSBillingRecord>> reconcilationRecordsByOperator = new Dictionary<long, List<SMSBillingRecord>>();
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

                SMSBillingRecord smsBillingRecord = new SMSBillingRecord
                {
                    OperatorId = operatorId,
                    PeriodDefinitionId = periodId,
                    TrafficDirection = trafficDirection
                };

                MeasureValue calculatedRevenue;
                analyticRecord.MeasureValues.TryGetValue("Revenue", out calculatedRevenue);
                if (calculatedRevenue?.Value != null)
                    smsBillingRecord.Revenue = Convert.ToDecimal(calculatedRevenue.Value ?? 0.0);

                MeasureValue countSMS;
                analyticRecord.MeasureValues.TryGetValue("NumberOfSMSs", out countSMS);
                if (countSMS?.Value != null)
                    smsBillingRecord.NbrOfSMS = Convert.ToInt64(countSMS.Value ?? 0.0);

                List<SMSBillingRecord> smsBillingRecords = reconcilationRecordsByOperator.GetOrCreateItem(operatorId);
                smsBillingRecords.Add(smsBillingRecord);
            }
            return reconcilationRecordsByOperator;
        }
    }

    public class SMSICXReconciliationMemoryAnalyticDataManager : MemoryAnalyticDataManager
    {
        public override List<RawMemoryRecord> GetRawRecords(Vanrise.Analytic.Entities.AnalyticQuery query, List<string> neededFieldNames)
        {
            List<RawMemoryRecord> rawMemoryRecords = new List<RawMemoryRecord>();
            var operatorDeclarationManager = new IcxOperatorDeclarationManager();
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

            var periodDefinitions = periodDefinitionManager.GetPeriodDefinitionsBetweenDate(query.FromTime, toTime, out var minDate, out var maxDate);
            if (periodDefinitions == null || periodDefinitions.Count == 0)
                return rawMemoryRecords;

            var allOperatorDeclarations = operatorDeclarationManager.GetSMSDeclarationServices(periodDefinitions, filteredOperatorIds);

            var analyticManager = new AnalyticManager();
            AnalyticQuery analyticQuery = new AnalyticQuery
            {
                TableId = Guid.Parse("9ce5c4be-13f3-4830-948f-c7b18753b66d"),
                MeasureFields = new List<string> { "Revenue", "NumberOfSMSs" },
                DimensionFields = new List<string> { "Operator", "Period", "TrafficDirection" },
                FromTime = Utilities.Min(minDate, query.FromTime),
                ToTime = Utilities.Max(maxDate, toTime),
                Filters = new List<DimensionFilter>()
            };

            if (filteredOperatorIdsObject != null && filteredOperatorIdsObject.Count > 0)
            {
                DimensionFilter dimensionFilter = new DimensionFilter()
                {
                    Dimension = "Operator",
                    FilterValues = filteredOperatorIdsObject
                };
                analyticQuery.Filters.Add(dimensionFilter);
            }

            List<AnalyticRecord> analyticRecords = analyticManager.GetAllFilteredRecords(analyticQuery, out _);
            Dictionary<long, List<SMSBillingRecord>> smsBillingRecordsByOperator = GetSMSBillingRecordsByOperator(analyticRecords);

            var records = new List<SMSReconcilationObj>();
            int startingIndex = 0;
            long lastOperatorId = 0;

            if (allOperatorDeclarations != null && allOperatorDeclarations.Any())
            {
                foreach (var operatorDeclaration in allOperatorDeclarations)
                {
                    var operatorId = operatorDeclaration.OperatorId;
                    if (lastOperatorId != operatorId)
                    {
                        startingIndex = 0;
                        lastOperatorId = operatorId;
                    }
                    var smsReconcilationObj = new SMSReconcilationObj
                    {
                        PeriodId = operatorDeclaration.PeriodDefinition.PeriodDefinitionId,
                        OperatorId = operatorId,
                        TrafficDirection = operatorDeclaration.SMSSettings.TrafficDirection,
                        DeclaredRevenue = operatorDeclaration.SMSSettings.Revenue,
                        DeclaredNbrOfSMS = operatorDeclaration.SMSSettings.NumberOfSMSs
                    };

                    if (smsBillingRecordsByOperator.TryGetValue(operatorId, out var operatorSMSBillingRecords))
                    {
                        List<SMSBillingRecord> orderedBillingRecords = operatorSMSBillingRecords.OrderBy(item => item.PeriodDefinitionId).ToList();
                        for (int i = startingIndex; i < orderedBillingRecords.Count; i++)
                        {
                            var smsbillingRecord = orderedBillingRecords[i];
                            if (smsbillingRecord.PeriodDefinitionId == operatorDeclaration.PeriodDefinition.PeriodDefinitionId && smsbillingRecord.TrafficDirection == operatorDeclaration.SMSSettings.TrafficDirection)
                            {
                                smsReconcilationObj.CalculatedRevenue += smsbillingRecord.Revenue;
                                smsReconcilationObj.CalculatedNbrOfSMS += smsbillingRecord.NbrOfSMS;
                                operatorSMSBillingRecords.Remove(smsbillingRecord);
                            }
                            else
                            {
                                startingIndex = i;
                            }
                        }
                    }
                    records.Add(smsReconcilationObj);
                }
            }

            Dictionary<string, SMSReconcilationObj> smsBillingRecordWithoutOperatorDeclaration = new Dictionary<string, SMSReconcilationObj>();
            if (smsBillingRecordsByOperator != null)
            {
                foreach (var smsBillingRecords in smsBillingRecordsByOperator.Values)
                {
                    if (smsBillingRecords != null && smsBillingRecords.Count > 0)
                    {
                        foreach (var smsBillingRecord in smsBillingRecords)
                        {
                            string key = $"{smsBillingRecord.OperatorId}_{smsBillingRecord.PeriodDefinitionId}_{(int)smsBillingRecord.TrafficDirection}";
                            var recordObj = smsBillingRecordWithoutOperatorDeclaration.GetOrCreateItem(key);
                            recordObj.OperatorId = smsBillingRecord.OperatorId;
                            recordObj.PeriodId = smsBillingRecord.PeriodDefinitionId;
                            recordObj.CalculatedNbrOfSMS += smsBillingRecord.NbrOfSMS;
                            recordObj.CalculatedRevenue += smsBillingRecord.Revenue;
                            recordObj.TrafficDirection = smsBillingRecord.TrafficDirection;
                        }
                    }
                }
            }
            if (smsBillingRecordWithoutOperatorDeclaration.Count > 0)
                records.AddRange(smsBillingRecordWithoutOperatorDeclaration.Values);

            foreach (var smsReconcilationObj in records)
            {
                rawMemoryRecords.Add(GetRawMemoryRecord(smsReconcilationObj));
            }
            return rawMemoryRecords;
        }

        public RawMemoryRecord GetRawMemoryRecord(SMSReconcilationObj smsReconcilationObj)
        {
            RawMemoryRecord rawMemoryRecord = new RawMemoryRecord { FieldValues = new Dictionary<string, dynamic>() };
            rawMemoryRecord.FieldValues.Add("OperatorID", smsReconcilationObj.OperatorId);
            rawMemoryRecord.FieldValues.Add("PeriodID", smsReconcilationObj.PeriodId);
            rawMemoryRecord.FieldValues.Add("TrafficDirection", smsReconcilationObj.TrafficDirection);
            rawMemoryRecord.FieldValues.Add("CalculatedRevenue", smsReconcilationObj.CalculatedRevenue);
            rawMemoryRecord.FieldValues.Add("CalculatedNumberOfSMS", smsReconcilationObj.CalculatedNbrOfSMS);
            rawMemoryRecord.FieldValues.Add("DeclaredNumberOfSMS", smsReconcilationObj.DeclaredNbrOfSMS);
            rawMemoryRecord.FieldValues.Add("DeclaredRevenue", smsReconcilationObj.DeclaredRevenue);

            return rawMemoryRecord;
        }

        private Dictionary<long, List<SMSBillingRecord>> GetSMSBillingRecordsByOperator(List<AnalyticRecord> analyticRecords)
        {
            Dictionary<long, List<SMSBillingRecord>> reconcilationRecordsByOperator = new Dictionary<long, List<SMSBillingRecord>>();
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

                SMSBillingRecord smsBillingRecord = new SMSBillingRecord
                {
                    OperatorId = operatorId,
                    PeriodDefinitionId = periodId,
                    TrafficDirection = trafficDirection
                };

                MeasureValue calculatedRevenue;
                analyticRecord.MeasureValues.TryGetValue("Revenue", out calculatedRevenue);
                if (calculatedRevenue?.Value != null)
                    smsBillingRecord.Revenue = Convert.ToDecimal(calculatedRevenue.Value ?? 0.0);

                MeasureValue countSMS;
                analyticRecord.MeasureValues.TryGetValue("NumberOfSMSs", out countSMS);
                if (countSMS?.Value != null)
                    smsBillingRecord.NbrOfSMS = Convert.ToInt64(countSMS.Value ?? 0.0);

                List<SMSBillingRecord> smsBillingRecords = reconcilationRecordsByOperator.GetOrCreateItem(operatorId);
                smsBillingRecords.Add(smsBillingRecord);
            }
            return reconcilationRecordsByOperator;
        }
    }

    public class SMSBillingRecord
    {
        public long OperatorId { get; set; }
        public int PeriodDefinitionId { get; set; }
        public decimal Revenue { get; set; }
        public long NbrOfSMS { get; set; }
        public TrafficDirection TrafficDirection { get; set; }
    }

    public class SMSReconcilationObj
    {
        public long OperatorId { get; set; }
        public int PeriodId { get; set; }
        public decimal CalculatedRevenue { get; set; }
        public long CalculatedNbrOfSMS { get; set; }
        public TrafficDirection TrafficDirection { get; set; }
        public decimal DeclaredRevenue { get; set; }
        public long DeclaredNbrOfSMS { get; set; }
    }
}
