using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Analytic.Business;
using Vanrise.Analytic.Entities;
using Vanrise.Common;

namespace Retail.RA.Business
{
    public class INTLOverallBillingMemoryAnalyticDataManager : MemoryAnalyticDataManager
    {

        public override List<RawMemoryRecord> GetRawRecords(Vanrise.Analytic.Entities.AnalyticQuery query, List<string> neededFieldNames)
        {
            //List<RawMemoryRecord> rawMemoryRecords = new List<RawMemoryRecord>();
            //var operatorDeclarationManager = new IntlOperatorDeclarationManager();
            //var periodDefinitionManager = new PeriodDefinitionManager();

            //List<long> filteredOperatorIds = null;
            //List<object> filteredOperatorIdsObject = null;
            //if (query.Filters != null)
            //{
            //    var operatorDimensionFilter = query.Filters.FirstOrDefault(itm => itm.Dimension == "Operator");
            //    if (operatorDimensionFilter != null)
            //        filteredOperatorIdsObject = operatorDimensionFilter.FilterValues;

            //    if (filteredOperatorIdsObject != null && filteredOperatorIdsObject.Count > 0)
            //        filteredOperatorIds = operatorDimensionFilter.FilterValues.Select(itm => Convert.ToInt64(itm)).ToList();
            //}

            //var toTime = DateTime.Now;
            //if (query.ToTime.HasValue)
            //    toTime = query.ToTime.Value;

            //var periodDefinitions = periodDefinitionManager.GetPeriodDefinitionsBetweenDate(query.FromTime, toTime, out var minDate, out var maxDate);
            //if (periodDefinitions == null || periodDefinitions.Count == 0)
            //    return rawMemoryRecords;

            //var allOperatorDeclarations = operatorDeclarationManager.GetSMSDeclarationServices(periodDefinitions, filteredOperatorIds);

            //var analyticManager = new AnalyticManager();
            //AnalyticQuery analyticQuery = new AnalyticQuery
            //{
            //    TableId = Guid.Parse("1da15e1b-3497-4b71-9bd9-a165b693f7af"),
            //    MeasureFields = new List<string> { "CalculatedDurationInMin", "CalculatedNumberOfCDR", "CalculatedRevenue", "DeclaredDurationInMin", "DeclaredNumberOfCDR", "DeclaredRevenue" },
            //    DimensionFields = new List<string> { "Operator", "Period", "TrafficDirection" },
            //    FromTime = Utilities.Min(minDate, query.FromTime),
            //    ToTime = Utilities.Max(maxDate, toTime),
            //    Filters = new List<DimensionFilter>()
            //};

            //if (filteredOperatorIdsObject != null && filteredOperatorIdsObject.Count > 0)
            //{
            //    DimensionFilter dimensionFilter = new DimensionFilter()
            //    {
            //        Dimension = "Operator",
            //        FilterValues = filteredOperatorIdsObject
            //    };
            //    analyticQuery.Filters.Add(dimensionFilter);
            //}

            //List<AnalyticRecord> analyticRecords = analyticManager.GetAllFilteredRecords(analyticQuery, out _);
            //Dictionary<long, List<SMSBillingRecord>> smsBillingRecordsByOperator = GetSMSBillingRecordsByOperator(analyticRecords);

            //var records = new List<SMSReconcilationObj>();
            //int startingIndex = 0;
            //long lastOperatorId = 0;

            //if (allOperatorDeclarations != null && allOperatorDeclarations.Any())
            //{
            //    foreach (var operatorDeclaration in allOperatorDeclarations)
            //    {
            //        var operatorId = operatorDeclaration.OperatorId;
            //        if (lastOperatorId != operatorId)
            //        {
            //            startingIndex = 0;
            //            lastOperatorId = operatorId;
            //        }
            //        var smsReconcilationObj = new SMSReconcilationObj
            //        {
            //            PeriodId = operatorDeclaration.PeriodDefinition.PeriodDefinitionId,
            //            OperatorId = operatorId,
            //            TrafficDirection = operatorDeclaration.SMSSettings.TrafficDirection,
            //            DeclaredRevenue = operatorDeclaration.SMSSettings.Revenue,
            //            DeclaredNbrOfSMS = operatorDeclaration.SMSSettings.NumberOfSMSs
            //        };

            //        if (smsBillingRecordsByOperator.TryGetValue(operatorId, out var operatorSMSBillingRecords))
            //        {
            //            List<SMSBillingRecord> orderedBillingRecords = operatorSMSBillingRecords.OrderBy(item => item.PeriodDefinitionId).ToList();
            //            for (int i = startingIndex; i < orderedBillingRecords.Count; i++)
            //            {
            //                var smsbillingRecord = orderedBillingRecords[i];
            //                if (smsbillingRecord.PeriodDefinitionId == operatorDeclaration.PeriodDefinition.PeriodDefinitionId && smsbillingRecord.TrafficDirection == operatorDeclaration.SMSSettings.TrafficDirection)
            //                {
            //                    smsReconcilationObj.CalculatedRevenue += smsbillingRecord.Revenue;
            //                    smsReconcilationObj.CalculatedNbrOfSMS += smsbillingRecord.NbrOfSMS;
            //                    operatorSMSBillingRecords.Remove(smsbillingRecord);
            //                }
            //                else
            //                {
            //                    startingIndex = i;
            //                }
            //            }
            //        }
            //        records.Add(smsReconcilationObj);
            //    }
            //}

            //Dictionary<string, SMSReconcilationObj> smsBillingRecordWithoutOperatorDeclaration = new Dictionary<string, SMSReconcilationObj>();
            //if (smsBillingRecordsByOperator != null)
            //{
            //    foreach (var smsBillingRecords in smsBillingRecordsByOperator.Values)
            //    {
            //        if (smsBillingRecords != null && smsBillingRecords.Count > 0)
            //        {
            //            foreach (var smsBillingRecord in smsBillingRecords)
            //            {
            //                string key = $"{smsBillingRecord.OperatorId}_{smsBillingRecord.PeriodDefinitionId}_{(int)smsBillingRecord.TrafficDirection}";
            //                var recordObj = smsBillingRecordWithoutOperatorDeclaration.GetOrCreateItem(key);
            //                recordObj.OperatorId = smsBillingRecord.OperatorId;
            //                recordObj.PeriodId = smsBillingRecord.PeriodDefinitionId;
            //                recordObj.CalculatedNbrOfSMS += smsBillingRecord.NbrOfSMS;
            //                recordObj.CalculatedRevenue += smsBillingRecord.Revenue;
            //                recordObj.TrafficDirection = smsBillingRecord.TrafficDirection;
            //            }
            //        }
            //    }
            //}
            //if (smsBillingRecordWithoutOperatorDeclaration.Count > 0)
            //    records.AddRange(smsBillingRecordWithoutOperatorDeclaration.Values);

            //foreach (var smsReconcilationObj in records)
            //{
            //    rawMemoryRecords.Add(GetRawMemoryRecord(smsReconcilationObj));
            //}
            //return rawMemoryRecords;
            return new List<RawMemoryRecord>();
        }

    }
}
