using Retail.RA.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using Vanrise.Analytic.Business;
using Vanrise.Analytic.Entities;
using Vanrise.Common;

namespace Retail.RA.Business
{
    public class INTLReconciliationMemoryAnalyticDataManager : MemoryAnalyticDataManager
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

            var allOperatorDeclarations = operatorDeclarationManager.GetVoiceDeclarationServices(periodDefinitions, filteredOperatorIds);

            var analyticManager = new AnalyticManager();
            AnalyticQuery analyticQuery = new AnalyticQuery
            {
                TableId = Guid.Parse("efe9d65f-0b0e-4466-918f-4d8982a368f6"),
                MeasureFields = new List<string> { "TotalSaleDurationInMin", "Revenue", "NumberOfCDRs" },
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
            Dictionary<long, List<BillingRecord>> billingRecordsByOperator = GetBillingRecordsByOperator(analyticRecords);

            var records = new List<ReconcilationObj>();
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
                    var reconcilationObj = new ReconcilationObj
                    {
                        PeriodId = operatorDeclaration.PeriodDefinition.PeriodDefinitionId,
                        OperatorId = operatorId,
                        TrafficDirection = operatorDeclaration.VoiceSettings.TrafficDirection,
                        DeclaredDurationInMin = operatorDeclaration.VoiceSettings.DeclaredDuration,
                        DeclaredRevenue = operatorDeclaration.VoiceSettings.DeclaredRevenue,
                        DeclaredNbrOfCDR = operatorDeclaration.VoiceSettings.DeclaredNumberOfCalls
                    };

                    if (billingRecordsByOperator.TryGetValue(operatorId, out var operatorBillingRecords))
                    {
                        List<BillingRecord> orderedBillingRecords = operatorBillingRecords.OrderBy(item => item.PeriodDefinitionId).ToList();
                        for (int i = startingIndex; i < orderedBillingRecords.Count; i++)
                        {
                            var billingRecord = orderedBillingRecords[i];
                            if (billingRecord.PeriodDefinitionId == operatorDeclaration.PeriodDefinition.PeriodDefinitionId && billingRecord.TrafficDirection == operatorDeclaration.VoiceSettings.TrafficDirection)
                            {
                                reconcilationObj.CalculatedDurationInMin += billingRecord.DurationInMin;
                                reconcilationObj.CalculatedRevenue += billingRecord.Revenue;
                                reconcilationObj.CalculatedNbrOfCDR += billingRecord.NbrOfCDR;
                                reconcilationObj.DifferencePerc = reconcilationObj.CalculatedDurationInMin > 0
                                    ? (int)((reconcilationObj.CalculatedDurationInMin - reconcilationObj.DeclaredDurationInMin) / reconcilationObj.CalculatedDurationInMin)
                                    : 0;
                                operatorBillingRecords.Remove(billingRecord);
                            }
                            else
                            {
                                startingIndex = i;
                            }
                        }
                    }
                    records.Add(reconcilationObj);
                }
            }

            Dictionary<string, ReconcilationObj> billingRecordWithoutOperatorDeclaration = new Dictionary<string, ReconcilationObj>();
            if (billingRecordsByOperator != null)
            {
                foreach (var billingRecords in billingRecordsByOperator.Values)
                {
                    if (billingRecords != null && billingRecords.Count > 0)
                    {
                        foreach (var billingRecord in billingRecords)
                        {
                            string key = $"{billingRecord.OperatorId}_{billingRecord.PeriodDefinitionId}_{(int)billingRecord.TrafficDirection}";
                            var recordObj = billingRecordWithoutOperatorDeclaration.GetOrCreateItem(key);
                            recordObj.OperatorId = billingRecord.OperatorId;
                            recordObj.PeriodId = billingRecord.PeriodDefinitionId;
                            recordObj.CalculatedDurationInMin += billingRecord.DurationInMin;
                            recordObj.CalculatedNbrOfCDR += billingRecord.NbrOfCDR;
                            recordObj.CalculatedRevenue += billingRecord.Revenue;
                            recordObj.TrafficDirection = billingRecord.TrafficDirection;
                        }
                    }
                }
            }
            if (billingRecordWithoutOperatorDeclaration.Count > 0)
                records.AddRange(billingRecordWithoutOperatorDeclaration.Values);

            foreach (var reconcilationObj in records)
            {
                rawMemoryRecords.Add(GetRawMemoryRecord(reconcilationObj));
            }
            return rawMemoryRecords;
        }

        public RawMemoryRecord GetRawMemoryRecord(ReconcilationObj reconcilationObj)
        {
            RawMemoryRecord rawMemoryRecord = new RawMemoryRecord { FieldValues = new Dictionary<string, dynamic>() };
            rawMemoryRecord.FieldValues.Add("OperatorID", reconcilationObj.OperatorId);
            rawMemoryRecord.FieldValues.Add("PeriodID", reconcilationObj.PeriodId);
            rawMemoryRecord.FieldValues.Add("TrafficDirection", reconcilationObj.TrafficDirection);
            rawMemoryRecord.FieldValues.Add("CalculatedDurationInMin", reconcilationObj.CalculatedDurationInMin);
            rawMemoryRecord.FieldValues.Add("CalculatedRevenue", reconcilationObj.CalculatedRevenue);
            rawMemoryRecord.FieldValues.Add("CalculatedNumberOfCDR", reconcilationObj.CalculatedNbrOfCDR);
            rawMemoryRecord.FieldValues.Add("DeclaredNumberOfCDR", reconcilationObj.DeclaredNbrOfCDR);
            rawMemoryRecord.FieldValues.Add("DeclaredRevenue", reconcilationObj.DeclaredRevenue);
            rawMemoryRecord.FieldValues.Add("DeclaredDurationInMin", reconcilationObj.DeclaredDurationInMin);
            rawMemoryRecord.FieldValues.Add("Difference", reconcilationObj.DifferencePerc);

            return rawMemoryRecord;
        }

        private Dictionary<long, List<BillingRecord>> GetBillingRecordsByOperator(List<AnalyticRecord> analyticRecords)
        {
            Dictionary<long, List<BillingRecord>> reconcilationRecordsByOperator = new Dictionary<long, List<BillingRecord>>();
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

                BillingRecord reconcilationObj = new BillingRecord
                {
                    OperatorId = operatorId,
                    PeriodDefinitionId = periodId,
                    TrafficDirection = trafficDirection
                };

                MeasureValue calculatedDurationInMin;
                analyticRecord.MeasureValues.TryGetValue("TotalSaleDurationInMin", out calculatedDurationInMin);
                if (calculatedDurationInMin?.Value != null)
                    reconcilationObj.DurationInMin = Convert.ToDecimal(calculatedDurationInMin.Value ?? 0.0);

                MeasureValue calculatedRevenue;
                analyticRecord.MeasureValues.TryGetValue("Revenue", out calculatedRevenue);
                if (calculatedRevenue?.Value != null)
                    reconcilationObj.Revenue = Convert.ToDecimal(calculatedRevenue.Value ?? 0.0);

                MeasureValue countCDR;
                analyticRecord.MeasureValues.TryGetValue("NumberOfCDRs", out countCDR);
                if (countCDR?.Value != null)
                    reconcilationObj.NbrOfCDR = Convert.ToInt64(countCDR.Value ?? 0.0);

                List<BillingRecord> billingRecords = reconcilationRecordsByOperator.GetOrCreateItem(operatorId);
                billingRecords.Add(reconcilationObj);
            }
            return reconcilationRecordsByOperator;
        }
    }

    public class ICXReconciliationMemoryAnalyticDataManager : MemoryAnalyticDataManager
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

                if (filteredOperatorIdsObject != null)
                    filteredOperatorIds = operatorDimensionFilter.FilterValues.Select(itm => Convert.ToInt64(itm)).ToList();
            }

            var toTime = DateTime.Now;
            if (query.ToTime.HasValue)
                toTime = query.ToTime.Value;

            var periodDefinitions = periodDefinitionManager.GetPeriodDefinitionsBetweenDate(query.FromTime, toTime, out var minDate, out var maxDate);
            if (periodDefinitions == null || periodDefinitions.Count == 0)
                return rawMemoryRecords;

            var allOperatorDeclarations = operatorDeclarationManager.GetVoiceDeclarationServices(periodDefinitions, filteredOperatorIds);

            var analyticManager = new AnalyticManager();
            AnalyticQuery analyticQuery = new AnalyticQuery
            {
                TableId = Guid.Parse("8e3baf48-a2d3-4b09-8198-cbb4ad6f76f5"),
                MeasureFields = new List<string> { "TotalSaleDurationInMin", "Revenue", "NumberOfCDRs" },
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
            Dictionary<long, List<BillingRecord>> billingRecordsByOperator = GetBillingRecordsByOperator(analyticRecords);

            var records = new List<ReconcilationObj>();
            int startingIndex = 0;
            long lastOperatorId = 0;

            if (allOperatorDeclarations != null)
            {
                foreach (var operatorDeclaration in allOperatorDeclarations)
                {
                    var operatorId = operatorDeclaration.OperatorId;
                    if (lastOperatorId != operatorId)
                    {
                        startingIndex = 0;
                        lastOperatorId = operatorId;
                    }
                    var reconcilationObj = new ReconcilationObj
                    {
                        PeriodId = operatorDeclaration.PeriodDefinition.PeriodDefinitionId,
                        OperatorId = operatorId,
                        TrafficDirection = operatorDeclaration.VoiceSettings.TrafficDirection,
                        DeclaredDurationInMin = operatorDeclaration.VoiceSettings.DeclaredDuration,
                        DeclaredRevenue = operatorDeclaration.VoiceSettings.DeclaredRevenue,
                        DeclaredNbrOfCDR = operatorDeclaration.VoiceSettings.DeclaredNumberOfCalls
                    };

                    if (billingRecordsByOperator.TryGetValue(operatorId, out var operatorBillingRecords))
                    {
                        List<BillingRecord> orderedBillingRecords = operatorBillingRecords.OrderBy(item => item.PeriodDefinitionId).ToList();
                        for (int i = startingIndex; i < orderedBillingRecords.Count; i++)
                        {
                            var billingRecord = orderedBillingRecords[i];
                            if (billingRecord.PeriodDefinitionId == operatorDeclaration.PeriodDefinition.PeriodDefinitionId && billingRecord.TrafficDirection == operatorDeclaration.VoiceSettings.TrafficDirection)
                            {
                                reconcilationObj.CalculatedDurationInMin += billingRecord.DurationInMin;
                                reconcilationObj.CalculatedRevenue += billingRecord.Revenue;
                                reconcilationObj.CalculatedNbrOfCDR += billingRecord.NbrOfCDR;
                                reconcilationObj.DifferencePerc = reconcilationObj.CalculatedDurationInMin > 0
                                    ? (int)((reconcilationObj.CalculatedDurationInMin - reconcilationObj.DeclaredDurationInMin) / reconcilationObj.CalculatedDurationInMin)
                                    : 0;
                                operatorBillingRecords.Remove(billingRecord);
                            }
                            else
                            {
                                startingIndex = i;
                                break;
                            }
                        }
                    }
                    records.Add(reconcilationObj);
                }
            }

            Dictionary<string, ReconcilationObj> billingRecordWithoutOperatorDeclaration = new Dictionary<string, ReconcilationObj>();
            if (billingRecordsByOperator != null)
            {
                foreach (var billingRecords in billingRecordsByOperator.Values)
                {
                    if (billingRecords != null && billingRecords.Count > 0)
                    {
                        foreach (var billingRecord in billingRecords)
                        {
                            string key = $"{billingRecord.OperatorId}_{billingRecord.PeriodDefinitionId}_{(int)billingRecord.TrafficDirection}";
                            var recordObj = billingRecordWithoutOperatorDeclaration.GetOrCreateItem(key);
                            recordObj.OperatorId = billingRecord.OperatorId;
                            recordObj.PeriodId = billingRecord.PeriodDefinitionId;
                            recordObj.CalculatedDurationInMin += billingRecord.DurationInMin;
                            recordObj.CalculatedNbrOfCDR += billingRecord.NbrOfCDR;
                            recordObj.CalculatedRevenue += billingRecord.Revenue;
                            recordObj.TrafficDirection = billingRecord.TrafficDirection;
                        }
                    }
                }
            }

            if (billingRecordWithoutOperatorDeclaration.Count > 0)
                records.AddRange(billingRecordWithoutOperatorDeclaration.Values);

            foreach (var reconcilationObj in records)
            {
                rawMemoryRecords.Add(GetRawMemoryRecord(reconcilationObj));
            }
            return rawMemoryRecords;
        }

        public RawMemoryRecord GetRawMemoryRecord(ReconcilationObj reconcilationObj)
        {
            RawMemoryRecord rawMemoryRecord = new RawMemoryRecord { FieldValues = new Dictionary<string, dynamic>() };
            rawMemoryRecord.FieldValues.Add("OperatorID", reconcilationObj.OperatorId);
            rawMemoryRecord.FieldValues.Add("PeriodID", reconcilationObj.PeriodId);
            rawMemoryRecord.FieldValues.Add("TrafficDirection", reconcilationObj.TrafficDirection);
            rawMemoryRecord.FieldValues.Add("CalculatedDurationInMin", reconcilationObj.CalculatedDurationInMin);
            rawMemoryRecord.FieldValues.Add("CalculatedRevenue", reconcilationObj.CalculatedRevenue);
            rawMemoryRecord.FieldValues.Add("CalculatedNumberOfCDR", reconcilationObj.CalculatedNbrOfCDR);
            rawMemoryRecord.FieldValues.Add("DeclaredNumberOfCDR", reconcilationObj.DeclaredNbrOfCDR);
            rawMemoryRecord.FieldValues.Add("DeclaredRevenue", reconcilationObj.DeclaredRevenue);
            rawMemoryRecord.FieldValues.Add("DeclaredDurationInMin", reconcilationObj.DeclaredDurationInMin);
            rawMemoryRecord.FieldValues.Add("Difference", reconcilationObj.DifferencePerc);

            return rawMemoryRecord;
        }

        private Dictionary<long, List<BillingRecord>> GetBillingRecordsByOperator(List<AnalyticRecord> analyticRecords)
        {
            Dictionary<long, List<BillingRecord>> reconcilationRecordsByOperator = new Dictionary<long, List<BillingRecord>>();
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

                BillingRecord reconcilationObj = new BillingRecord
                {
                    OperatorId = operatorId,
                    PeriodDefinitionId = periodId,
                    TrafficDirection = trafficDirection
                };

                MeasureValue calculatedDurationInMin;
                analyticRecord.MeasureValues.TryGetValue("TotalSaleDurationInMin", out calculatedDurationInMin);
                if (calculatedDurationInMin?.Value != null)
                    reconcilationObj.DurationInMin = Convert.ToDecimal(calculatedDurationInMin.Value ?? 0.0);

                MeasureValue calculatedRevenue;
                analyticRecord.MeasureValues.TryGetValue("Revenue", out calculatedRevenue);
                if (calculatedRevenue?.Value != null)
                    reconcilationObj.Revenue = Convert.ToDecimal(calculatedRevenue.Value ?? 0.0);

                MeasureValue countCDR;
                analyticRecord.MeasureValues.TryGetValue("NumberOfCDRs", out countCDR);
                if (countCDR?.Value != null)
                    reconcilationObj.NbrOfCDR = Convert.ToInt64(countCDR.Value ?? 0.0);

                List<BillingRecord> billingRecords = reconcilationRecordsByOperator.GetOrCreateItem(operatorId);
                billingRecords.Add(reconcilationObj);
            }
            return reconcilationRecordsByOperator;
        }
    }

    public class BillingRecord
    {
        public long OperatorId { get; set; }
        public int PeriodDefinitionId { get; set; }
        public decimal DurationInMin { get; set; }
        public decimal Revenue { get; set; }
        public long NbrOfCDR { get; set; }
        public TrafficDirection TrafficDirection { get; set; }
    }

    public class ReconcilationObj
    {
        public long OperatorId { get; set; }
        public int PeriodId { get; set; }
        public decimal CalculatedDurationInMin { get; set; }
        public decimal CalculatedRevenue { get; set; }
        public long CalculatedNbrOfCDR { get; set; }
        public TrafficDirection TrafficDirection { get; set; }
        public decimal DeclaredDurationInMin { get; set; }
        public int DifferencePerc { get; set; }
        public decimal DeclaredRevenue { get; set; }
        public long DeclaredNbrOfCDR { get; set; }
    }
}