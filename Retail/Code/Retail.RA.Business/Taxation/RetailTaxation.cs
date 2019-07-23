using Retail.RA.Entities;
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
    public class RetailTaxationMemoryAnalyticDataManager : MemoryAnalyticDataManager
    {
        public override List<RawMemoryRecord> GetRawRecords(AnalyticQuery query, List<string> neededFieldNames)
        {
            List<RawMemoryRecord> rawMemoryRecords = new List<RawMemoryRecord>();
            var operatorDeclarationManager = new OnNetOperatorDeclarationManager();
            var periodDefinitionManager = new PeriodDefinitionManager();
            var analyticManager = new AnalyticManager();

            List<long> filteredOperatorIds = null;
            List<object> filteredOperatorIdsObject = null;
            if (query.Filters != null && query.Filters.Count > 0)
            {
                var operatorDimensionFilter = query.Filters.FirstOrDefault(itm => itm.Dimension == "Operator");
                if (operatorDimensionFilter != null)
                    filteredOperatorIdsObject = operatorDimensionFilter.FilterValues;

                if (filteredOperatorIdsObject != null && filteredOperatorIdsObject.Count > 0)
                    filteredOperatorIds = filteredOperatorIdsObject.Select(itm => Convert.ToInt64(itm)).ToList();
            }

            var toTime = DateTime.Now;
            if (query.ToTime.HasValue)
                toTime = query.ToTime.Value;

            var periodDefinitions = periodDefinitionManager.GetPeriodDefinitionsBetweenDate(query.FromTime, toTime, out var minDate, out var maxDate);
            if (periodDefinitions == null || periodDefinitions.Count == 0)
                return rawMemoryRecords;

            var prepaidMeasureFields = new List<string> {"TotalRevenue" };
            var postpaidMeasureFields = new List<string> { "TotalIncome", "TotalRevenue" };
            var transactionMeasureFields = new List<string> { "TotalIncome", "TotalRevenue" };
            var dimensionFields = new List<string> { "Operator", "Period" };
            var transactionDimensionFields = new List<string> { "Operator", "Period", "TransactionType" };

            AnalyticQuery prepaidCDRAnalyticQuery = new AnalyticQuery
            {
                TableId = Guid.Parse("7a865558-0ac3-408e-85dc-3dd134a94588"),
                MeasureFields = prepaidMeasureFields,
                DimensionFields = dimensionFields,
                FromTime = Utilities.Min(minDate, query.FromTime),
                ToTime = Utilities.Max(maxDate, toTime),
                Filters = new List<DimensionFilter>()
            };

            AnalyticQuery postpaidCDRAnalyticQuery = new AnalyticQuery
            {
                TableId =  Guid.Parse("c9253fb2-1d0f-4776-9511-d7b5fbcb8f6b"),
                MeasureFields = postpaidMeasureFields,
                DimensionFields = dimensionFields,
                FromTime = Utilities.Min(minDate, query.FromTime),
                ToTime = Utilities.Max(maxDate, toTime),
                Filters = new List<DimensionFilter>()
            };


            AnalyticQuery prepaidSMSAnalyticQuery = new AnalyticQuery
            {
                TableId = Guid.Parse("bc7bfe32-b1af-4f64-b6a1-f55e0af7c962"),
                MeasureFields = prepaidMeasureFields,
                DimensionFields = dimensionFields,
                FromTime = Utilities.Min(minDate, query.FromTime),
                ToTime = Utilities.Max(maxDate, toTime),
                Filters = new List<DimensionFilter>()
            };


            AnalyticQuery postpaidSMSAnalyticQuery = new AnalyticQuery
            {
                TableId =  Guid.Parse("e427655f-5be6-4bf2-8f93-a64a31a9e06d"),
                MeasureFields = postpaidMeasureFields,
                DimensionFields = dimensionFields,
                FromTime = Utilities.Min(minDate, query.FromTime),
                ToTime = Utilities.Max(maxDate, toTime),
                Filters = new List<DimensionFilter>()
            };

            AnalyticQuery prepaidTransactionAnalyticQuery = new AnalyticQuery
            {
                TableId = Guid.Parse("d31fdfeb-056f-4a47-8df6-c4956912f989"),
                MeasureFields = transactionMeasureFields,
                DimensionFields = transactionDimensionFields,
                FromTime = Utilities.Min(minDate, query.FromTime),
                ToTime = Utilities.Max(maxDate, toTime),
                Filters = new List<DimensionFilter>()
            };

            AnalyticQuery postpaidTransactionAnalyticQuery = new AnalyticQuery
            {
                TableId = Guid.Parse("659d5871-ae42-446e-b755-89cd2b3f2652"),
                MeasureFields = transactionMeasureFields,
                DimensionFields = transactionDimensionFields,
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
                prepaidCDRAnalyticQuery.Filters.Add(dimensionFilter);
                postpaidCDRAnalyticQuery.Filters.Add(dimensionFilter);
                prepaidSMSAnalyticQuery.Filters.Add(dimensionFilter);
                postpaidSMSAnalyticQuery.Filters.Add(dimensionFilter);
                prepaidTransactionAnalyticQuery.Filters.Add(dimensionFilter);
                postpaidTransactionAnalyticQuery.Filters.Add(dimensionFilter);
            }

            Dictionary<TaxationDimensionFields, TaxationBillingRecord> allRecords = new Dictionary<TaxationDimensionFields, TaxationBillingRecord>();

            List<AnalyticRecord> prepaidCDRAnalyticRecords = analyticManager.GetAllFilteredRecords(prepaidCDRAnalyticQuery, out _);
            GetBillingRecordsByDimensionFields(prepaidCDRAnalyticRecords, allRecords, true, SourceType.Voice);

            List<AnalyticRecord> postpaidCDRAnalyticRecords = analyticManager.GetAllFilteredRecords(postpaidCDRAnalyticQuery, out _);
            GetBillingRecordsByDimensionFields(postpaidCDRAnalyticRecords, allRecords, false, SourceType.Voice);

            List<AnalyticRecord> prepaidSMSAnalyticRecords = analyticManager.GetAllFilteredRecords(prepaidSMSAnalyticQuery, out _);
            GetBillingRecordsByDimensionFields(prepaidSMSAnalyticRecords, allRecords,  true, SourceType.SMS);

            List<AnalyticRecord> postpaidSMSAnalyticRecords = analyticManager.GetAllFilteredRecords(postpaidSMSAnalyticQuery, out _);
            GetBillingRecordsByDimensionFields(postpaidSMSAnalyticRecords, allRecords, false, SourceType.SMS);

            List<AnalyticRecord> prepaidTransactionAnalyticRecords = analyticManager.GetAllFilteredRecords(prepaidTransactionAnalyticQuery, out _);
            GetBillingRecordsByDimensionFields(prepaidTransactionAnalyticRecords, allRecords, true, SourceType.TopUp);

            List<AnalyticRecord> postpaidTransactionAnalyticRecords = analyticManager.GetAllFilteredRecords(postpaidTransactionAnalyticQuery, out _);
            GetBillingRecordsByDimensionFields(postpaidTransactionAnalyticRecords, allRecords,  false, SourceType.TopUp);

            foreach (var record in allRecords)
            {
                rawMemoryRecords.Add(GetRawMemoryRecord(record.Value));
            }
            return rawMemoryRecords;
        }

        #region Private Methods
        private RawMemoryRecord GetRawMemoryRecord(TaxationBillingRecord billingRecord)
        {
            RawMemoryRecord rawMemoryRecord = new RawMemoryRecord { FieldValues = new Dictionary<string, dynamic>() };
            rawMemoryRecord.FieldValues.Add("OperatorID", billingRecord.OperatorID);
            rawMemoryRecord.FieldValues.Add("PeriodID", billingRecord.PeriodID);
            rawMemoryRecord.FieldValues.Add("SubscriberType", billingRecord.SubscriberType);
            rawMemoryRecord.FieldValues.Add("Income", billingRecord.Income);
            rawMemoryRecord.FieldValues.Add("Revenue", billingRecord.Revenue);
            rawMemoryRecord.FieldValues.Add("SourceType", billingRecord.SourceType);
            return rawMemoryRecord;
        }
        private void GetBillingRecordsByDimensionFields(List<AnalyticRecord> analyticRecords, Dictionary<TaxationDimensionFields, TaxationBillingRecord> recordsDictionary,  bool isPrepaid, SourceType sourceType)
        {
            if (analyticRecords != null && analyticRecords.Count > 0)
            {
                foreach (var analyticRecord in analyticRecords)
                {
                    var operatorIdDimension = analyticRecord.DimensionValues[0];
                    var periodDimension = analyticRecord.DimensionValues[1];

                    if(sourceType == SourceType.TopUp)
                    {
                        var transactionTypeIdDimension = analyticRecord.DimensionValues[2];

                        var topUpTransactionTypeId = new Guid("3a26d47e-d2f4-42f3-817b-d1638d8e4290");

                        if (isPrepaid && (Guid)transactionTypeIdDimension.Value != topUpTransactionTypeId)
                            continue;
                    }
                  

                    if (periodDimension == null || periodDimension.Value == null)
                        continue;

                    TaxationDimensionFields dimensionFields = new TaxationDimensionFields()
                    {
                        OperatorID = (long)operatorIdDimension.Value,
                        PeriodID = (int)periodDimension.Value,
                        SubscriberType = isPrepaid ? SubscriberType.Prepaid : SubscriberType.Postpaid,
                        SourceType = sourceType
                    };

                    TaxationBillingRecord billingRecord = new TaxationBillingRecord
                    {
                        OperatorID = (long)operatorIdDimension.Value,
                        PeriodID = (int)periodDimension.Value,
                        SubscriberType = isPrepaid ? SubscriberType.Prepaid : SubscriberType.Postpaid,
                        SourceType = sourceType
                    };
                    
                    MeasureValue revenue;
                    analyticRecord.MeasureValues.TryGetValue("TotalRevenue", out revenue);
                    if (revenue?.Value != null)
                        billingRecord.Revenue = Convert.ToDecimal(revenue.Value ?? 0.0);

                    if (!isPrepaid || (sourceType == SourceType.TopUp || sourceType==SourceType.TopUp))
                    {
                        MeasureValue income;
                        analyticRecord.MeasureValues.TryGetValue("TotalIncome", out income);
                        if (income?.Value != null)
                            billingRecord.Income = Convert.ToDecimal(income.Value ?? 0.0);
                    }


                    if (recordsDictionary.ContainsKey(dimensionFields))
                    {
                        recordsDictionary[dimensionFields].Revenue += billingRecord.Revenue;
                        recordsDictionary[dimensionFields].Income += billingRecord.Income;
                    }
                    else
                    {
                        recordsDictionary.Add(dimensionFields, billingRecord);
                    }
                }
            }
        }
        #endregion
    }

    public class TaxationBillingRecord
    {
        public long OperatorID { get; set; }
        public int PeriodID { get; set; }
        public SubscriberType SubscriberType { get; set; }
        public SourceType SourceType { get; set; }

        public decimal? Revenue { get; set; }
        public decimal? Income { get; set; }
    }

    public struct TaxationDimensionFields
    {
        public long OperatorID { get; set; }
        public int PeriodID { get; set; }
        public SubscriberType SubscriberType { get; set; }
        public SourceType SourceType { get; set; }
    }
}
