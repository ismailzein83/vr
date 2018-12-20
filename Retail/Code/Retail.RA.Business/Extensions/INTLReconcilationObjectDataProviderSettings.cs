using System;
using System.Linq;
using Vanrise.Common;
using Vanrise.Analytic.Entities;
using Vanrise.Analytic.Business;
using System.Collections.Generic;
using Vanrise.GenericData.Business;

namespace Retail.RA.Business
{
    public class INTLReconcilationObjectDataProviderSettings : BusinessObjectDataProviderExtendedSettings
    {
        public override Guid ConfigId { get; }
        public override bool DoesSupportFilterOnAllFields => false;

        public override void LoadRecords(IBusinessObjectDataProviderLoadRecordsContext context)
        {
            if (!context.FromTime.HasValue)
                throw new NullReferenceException("context.FromTime");

            if (!context.ToTime.HasValue)
                throw new NullReferenceException("context.ToTime");

            var dataRecordData = new DataRecordData();
            IEnumerable<DataRecordObject> records = dataRecordData.GetDataRecords(context.FromTime.Value, context.ToTime.Value, TrafficDirection.IN);
            foreach (var dataRecord in records)
            {
                context.OnRecordLoaded(dataRecord, DateTime.Now);
            }
        }
    }

    public class INTLOutVoiceDataProviderSettings : BusinessObjectDataProviderExtendedSettings
    {
        public override void LoadRecords(IBusinessObjectDataProviderLoadRecordsContext context)
        {
            if (!context.FromTime.HasValue)
                throw new NullReferenceException("context.FromTime");

            if (!context.ToTime.HasValue)
                throw new NullReferenceException("context.ToTime");

            DataRecordData dataRecordData = new DataRecordData();
            IEnumerable<DataRecordObject> records = dataRecordData.GetDataRecords(context.FromTime.Value, context.ToTime.Value, TrafficDirection.OUT);

            foreach (var dataRecord in records)
            {
                context.OnRecordLoaded(dataRecord, DateTime.Now);
            }
        }

        public override Guid ConfigId { get; }
        public override bool DoesSupportFilterOnAllFields { get; }
    }

    public class DataRecordData
    {
        public List<DataRecordObject> GetDataRecords(DateTime fromTime, DateTime toTime, TrafficDirection trafficDirection)
        {
            var operatorDeclarationManager = new IntlOperatorDeclarationManager();
            var periodDefinitionManager = new PeriodDefinitionManager();

            var periodDefinitions = periodDefinitionManager.GetPeriodDefinitionsBetweenDate(fromTime, toTime, out var minDate, out var maxDate);
            var operatorDeclarations = operatorDeclarationManager.GetVoiceDeclarationServices(periodDefinitions, TrafficDirection.IN);

            var analyticManager = new AnalyticManager();
            AnalyticQuery analyticQuery = new AnalyticQuery
            {
                TableId = Guid.Parse("efe9d65f-0b0e-4466-918f-4d8982a368f6"),
                MeasureFields = new List<string> { "CalculatedDurationInMin", "CalculatedRevenue", "CountCDRs" },
                DimensionFields = new List<string> { "Operator", "Period" },
                FromTime = Utilities.Min(minDate, fromTime),
                ToTime = Utilities.Max(maxDate, toTime)
            };
            List<AnalyticRecord> analyticRecords = analyticManager.GetAllFilteredRecords(analyticQuery, out _);
            Dictionary<long, List<BillingRecord>> billingRecordsByOperator = GetBillingRecordsByOperator(analyticRecords);

            var records = new List<ReconcilationObj>();
            int startingIndex = 0;
            long lastOperatorId = 0;

            foreach (var operatorDeclaration in operatorDeclarations)
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
                        if (billingRecord.PeriodDefinitionId == operatorDeclaration.PeriodDefinition.PeriodDefinitionId)
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

            Dictionary<string, ReconcilationObj> billingRecordWithoutOperatorDeclaration = new Dictionary<string, ReconcilationObj>();
            foreach (var billingRecords in billingRecordsByOperator.Values)
            {
                if (billingRecords != null && billingRecords.Count > 0)
                {
                    foreach (var billingRecord in billingRecords)
                    {
                        string key = $"{billingRecord.OperatorId}_{billingRecord.PeriodDefinitionId}";
                        var recordObj = billingRecordWithoutOperatorDeclaration.GetOrCreateItem(key);
                        recordObj.OperatorId = billingRecord.OperatorId;
                        recordObj.PeriodId = billingRecord.PeriodDefinitionId;
                        recordObj.CalculatedDurationInMin += billingRecord.DurationInMin;
                        recordObj.CalculatedNbrOfCDR += billingRecord.NbrOfCDR;
                        recordObj.CalculatedRevenue += billingRecord.Revenue;
                    }
                }
            }
            if (billingRecordWithoutOperatorDeclaration.Count > 0)
                records.AddRange(billingRecordWithoutOperatorDeclaration.Values);

            List<DataRecordObject> recordObjects = new List<DataRecordObject>();
            foreach (var reconcilationObj in records)
            {
                recordObjects.Add(GetDataRecordObject(reconcilationObj));
            }
            return recordObjects;
        }
        private Dictionary<long, List<BillingRecord>> GetBillingRecordsByOperator(List<AnalyticRecord> analyticRecords)
        {
            Dictionary<long, List<BillingRecord>> reconcilationRecordsByOperator = new Dictionary<long, List<BillingRecord>>();
            foreach (var analyticRecord in analyticRecords)
            {
                var operatorIdDimension = analyticRecord.DimensionValues[0];
                var periodDimension = analyticRecord.DimensionValues[1];

                long operatorId = (long)operatorIdDimension.Value;
                int periodId = (int)periodDimension.Value;

                BillingRecord reconcilationObj = new BillingRecord
                {
                    OperatorId = operatorId,
                    PeriodDefinitionId = periodId
                };

                decimal calculatedDurationInMinValue = 0;
                MeasureValue calculatedDurationInMin;
                analyticRecord.MeasureValues.TryGetValue("CalculatedDurationInMin", out calculatedDurationInMin);
                if (calculatedDurationInMin?.Value != null)
                    reconcilationObj.DurationInMin = Convert.ToDecimal(calculatedDurationInMin.Value ?? 0.0);

                decimal calculatedRevenueValue = 0;
                MeasureValue calculatedRevenue;
                analyticRecord.MeasureValues.TryGetValue("CalculatedRevenue", out calculatedRevenue);
                if (calculatedRevenue?.Value != null)
                    reconcilationObj.Revenue = Convert.ToDecimal(calculatedRevenue.Value ?? 0.0);

                decimal countCDRValue = 0;
                MeasureValue countCDR;
                analyticRecord.MeasureValues.TryGetValue("CountCDRs", out countCDR);
                if (countCDR?.Value != null)
                    reconcilationObj.NbrOfCDR = Convert.ToInt64(countCDR.Value ?? 0);

                List<BillingRecord> billingRecords = reconcilationRecordsByOperator.GetOrCreateItem(operatorId);
                billingRecords.Add(reconcilationObj);
            }
            return reconcilationRecordsByOperator;
        }
        public DataRecordObject GetDataRecordObject(ReconcilationObj reconcilationObj)
        {
            var reconcilationDataRecordObject = new Dictionary<string, object>
            {
                {"Operator", reconcilationObj.OperatorId},
                {"PeriodDefinition", reconcilationObj.PeriodId},
                {"CalculatedDurationInMin", reconcilationObj.CalculatedDurationInMin},
                {"CalculatedRevenue", reconcilationObj.CalculatedRevenue},
                {"CalculatedNbrOfCDR", reconcilationObj.CalculatedNbrOfCDR},
                {"DeclaredNbrOfCDR", reconcilationObj.DeclaredNbrOfCDR},
                {"DeclaredRevenue", reconcilationObj.DeclaredRevenue},
                {"DeclaredDurationInMin", reconcilationObj.DeclaredDurationInMin},
                {"Difference", reconcilationObj.DifferencePerc}
            };
            return new DataRecordObject(new Guid("9f9639f2-6934-4b74-a52a-46cd9d663a9a"), reconcilationDataRecordObject);
        }
   
        #region Private Classes
        public class BillingRecord
        {
            public long OperatorId { get; set; }
            public int PeriodDefinitionId { get; set; }
            public decimal DurationInMin { get; set; }
            public decimal Revenue { get; set; }
            public long NbrOfCDR { get; set; }
        }
        public class ReconcilationObj
        {
            public long OperatorId { get; set; }
            public int PeriodId { get; set; }
            public decimal CalculatedDurationInMin { get; set; }
            public decimal CalculatedRevenue { get; set; }
            public long CalculatedNbrOfCDR { get; set; }

            public decimal DeclaredDurationInMin { get; set; }
            public int DifferencePerc { get; set; }
            public decimal DeclaredRevenue { get; set; }
            public long DeclaredNbrOfCDR { get; set; }
        }
        #endregion
    }
}
