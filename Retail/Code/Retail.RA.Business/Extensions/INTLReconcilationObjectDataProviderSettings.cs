using System;
using System.Linq;
using Vanrise.Common;
using Retail.RA.Entities;
//using Retail.RA.MainExtensions;
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
            //if (!context.FromTime.HasValue)
            //    throw new NullReferenceException("context.FromTime");

            //if (!context.ToTime.HasValue)
            //    throw new NullReferenceException("context.ToTime");

            //OperatorDeclarationManager operatorDeclarationManager = new OperatorDeclarationManager();
            //var cachedDeclarations = operatorDeclarationManager.GetCachedInVoiceDeclaration();

            //PeriodDefinitionManager periodDefinitionManager = new PeriodDefinitionManager();
            //DateTime minDate, maxDate;
            //var periods = periodDefinitionManager.GetPeriodDefinitionsBetweenDate(context.FromTime.Value, context.ToTime.Value, out minDate, out maxDate);


            //AnalyticManager analyticManager = new AnalyticManager();
            //AnalyticRecord analyticRecordSummary;
            //AnalyticQuery saleAnalyticQuery = new AnalyticQuery
            //{
            //    TableId = Guid.Parse("efe9d65f-0b0e-4466-918f-4d8982a368f6"),
            //    MeasureFields = new List<string> { "CalculatedDurationInMin", "CalculatedRevenue", "CountCDRs", "Revenue" },
            //    DimensionFields = new List<string> { "Operator", "Period" },
            //    FromTime = minDate,
            //    ToTime = maxDate
            //};
            //List<AnalyticRecord> analyticRecords = analyticManager.GetAllFilteredRecords(saleAnalyticQuery, out analyticRecordSummary);
            //Dictionary<long, List<BillingRecord>> billingRecordsByOperator = GetBillingRecordsByOperator(analyticRecords);


            //IEnumerable<OperatorDeclaration> operatorDeclarations = operatorDeclarationManager.GetOperatorDeclarations(periods);

            //List<ReconcilationObj> records = new List<ReconcilationObj>();
            //foreach (var operatorDeclaration in operatorDeclarations)
            //{
            //    var settings = operatorDeclaration.OperatorDeclarationService.Settings as Voice;
            //    var operatorId = operatorDeclaration.OperatorId;
            //    int startingIndex = 0;
            //    if (billingRecordsByOperator.TryGetValue(operatorId, out var operatorBillingRecords))
            //    {
            //        List<BillingRecord> orderedBillingRecords = operatorBillingRecords.OrderBy(item => item.PeriodDefinitionId).ToList();
            //        for (int i = startingIndex; i < orderedBillingRecords.Count(); i++)
            //        {
            //            var billingRecord = orderedBillingRecords[i];
            //            if (billingRecord.PeriodDefinitionId == operatorDeclaration.PeriodId)
            //            {
            //                ReconcilationObj reconcilationObj = new ReconcilationObj
            //                {
            //                    PeriodId = billingRecord.PeriodDefinitionId,
            //                    OperatorId = billingRecord.OperatorId,
            //                    DeclaredDurationInMin = settings.Duration,
            //                    DeclaredRevenue = settings.Amount,
            //                    DeclaredNbrOfCDR = settings.NumberOfCalls,
            //                };

            //                reconcilationObj.CalculatedDurationInMin += billingRecord.DurationInMin;
            //                reconcilationObj.CalculatedRevenue += billingRecord.Revenue;
            //                reconcilationObj.CalculatedNbrOfCDR += billingRecord.NbrOfCDR;
            //                records.Add(reconcilationObj);
            //            }
            //        }
            //    }
            //}

            //foreach (var reconcilationDataRecord in records)
            //{
            //    var dataRecord = GetDataRecordObject(reconcilationDataRecord);
            //    context.OnRecordLoaded(dataRecord, DateTime.Now);
            //}
        }

        //public DataRecordObject GetDataRecordObject(ReconcilationObj reconcilationObj)
        //{
        //    var reconcilationDataRecordObject = new Dictionary<string, object>
        //    {
        //        {"Operator", reconcilationObj.OperatorId},
        //        {"PeriodDefinition", reconcilationObj.PeriodId},
        //        {"CalculatedDurationInMin", reconcilationObj.CalculatedDurationInMin},
        //        {"CalculatedRevenue", reconcilationObj.CalculatedRevenue},
        //        {"CalculatedNbrOfCDR", reconcilationObj.CalculatedNbrOfCDR},
        //        {"DeclaredNbrOfCDR", reconcilationObj.DeclaredNbrOfCDR},
        //        {"DeclaredRevenue", reconcilationObj.DeclaredRevenue},
        //        {"DeclaredDurationInMin", reconcilationObj.DeclaredDurationInMin}
        //    };
        //    return new DataRecordObject(new Guid("9f9639f2-6934-4b74-a52a-46cd9d663a9a"), reconcilationDataRecordObject);
        //}
        //public Dictionary<long, List<BillingRecord>> GetBillingRecordsByOperator(List<AnalyticRecord> analyticRecords)
        //{
        //    Dictionary<long, List<BillingRecord>> reconcilationRecordsByOperator = new Dictionary<long, List<BillingRecord>>();
        //    foreach (var analyticRecord in analyticRecords)
        //    {
        //        var operatorIdDimension = analyticRecord.DimensionValues[0];
        //        var periodDimension = analyticRecord.DimensionValues[1];

        //        long operatorId = (long)operatorIdDimension.Value;
        //        int periodId = (int)periodDimension.Value;

        //        BillingRecord reconcilationObj = new BillingRecord
        //        {
        //            OperatorId = operatorId,
        //            PeriodDefinitionId = periodId
        //        };

        //        decimal calculatedDurationInMinValue = 0;
        //        MeasureValue calculatedDurationInMin;
        //        analyticRecord.MeasureValues.TryGetValue("", out calculatedDurationInMin);
        //        if (calculatedDurationInMin?.Value != null)
        //            reconcilationObj.DurationInMin = Convert.ToDecimal(calculatedDurationInMin.Value ?? 0.0);

        //        decimal calculatedRevenueValue = 0;
        //        MeasureValue calculatedRevenue;
        //        analyticRecord.MeasureValues.TryGetValue("", out calculatedRevenue);
        //        if (calculatedRevenue?.Value != null)
        //            reconcilationObj.Revenue = Convert.ToDecimal(calculatedRevenue.Value ?? 0.0);

        //        decimal countCDRValue = 0;
        //        MeasureValue countCDR;
        //        analyticRecord.MeasureValues.TryGetValue("", out countCDR);
        //        if (countCDR?.Value != null)
        //            reconcilationObj.NbrOfCDR = Convert.ToDecimal(countCDR.Value ?? 0.0);

        //        List<BillingRecord> billingRecords = reconcilationRecordsByOperator.GetOrCreateItem(operatorId);
        //        billingRecords.Add(reconcilationObj);
        //    }
        //    return reconcilationRecordsByOperator;
        //}
    }

    public class BillingRecord
    {
        public long OperatorId { get; set; }
        public int PeriodDefinitionId { get; set; }
        public decimal DurationInMin { get; set; }
        public decimal Revenue { get; set; }
        public decimal NbrOfCDR { get; set; }
    }

    public class ReconcilationObj
    {
        public long OperatorId { get; set; }
        public int PeriodId { get; set; }
        public decimal CalculatedDurationInMin { get; set; }
        public decimal CalculatedRevenue { get; set; }
        public decimal CalculatedNbrOfCDR { get; set; }

        public decimal DeclaredDurationInMin { get; set; }
        public decimal DeclaredRevenue { get; set; }
        public decimal DeclaredNbrOfCDR { get; set; }
    }
}
