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
    public class RetailReconciliationMemoryAnalyticDataManager : MemoryAnalyticDataManager
    {
        #region MemoryAnalyticDataManager
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


            AnalyticQuery prepaidCDRAnalyticQuery = new AnalyticQuery
            {
                TableId = Guid.Parse("7a865558-0ac3-408e-85dc-3dd134a94588"),
                MeasureFields = new List<string> { "TotalDurationInMinutes", "TotalRevenue","NumberOfCDRs" },
                DimensionFields = new List<string> { "Operator", "Period" ,"Subscriber", "Scope"},
                FromTime = Utilities.Min(minDate, query.FromTime),
                ToTime = Utilities.Max(maxDate, toTime),
                Filters = new List<DimensionFilter>()
            };

            AnalyticQuery postpaidCDRAnalyticQuery = new AnalyticQuery
            {
                TableId = Guid.Parse("080b5588-685d-4b98-84f9-ebca6af81887"),
                MeasureFields = new List<string> { "TotalDurationInMinutes", "TotalRevenue","NumberOfCDRs" },
                DimensionFields = new List<string> { "Operator", "Period" ,"Subscriber" ,"Scope" },
                FromTime = Utilities.Min(minDate, query.FromTime),
                ToTime = Utilities.Max(maxDate, toTime),
                Filters = new List<DimensionFilter>()
            };


            AnalyticQuery prepaidSMSAnalyticQuery = new AnalyticQuery
            {
                TableId = Guid.Parse("bc7bfe32-b1af-4f64-b6a1-f55e0af7c962"),
                MeasureFields = new List<string> { "TotalRevenue", "NumberOfSMS"},
                DimensionFields = new List<string> { "Operator", "Period", "Subscriber", "Scope" },
                FromTime = Utilities.Min(minDate, query.FromTime),
                ToTime = Utilities.Max(maxDate, toTime),
                Filters = new List<DimensionFilter>()
            };


            AnalyticQuery postpaidSMSAnalyticQuery = new AnalyticQuery
            {
                TableId = Guid.Parse("1b23f3e5-aa4e-4d79-9838-0460738167e2"),
                MeasureFields = new List<string> { "TotalRevenue" ,"NumberOfSMS"},
                DimensionFields = new List<string> { "Operator", "Period", "Subscriber", "Scope" },
                FromTime = Utilities.Min(minDate, query.FromTime),
                ToTime = Utilities.Max(maxDate, toTime),
                Filters = new List<DimensionFilter>()
            };

            AnalyticQuery prepaidTransactionAnalyticQuery = new AnalyticQuery
            {
                TableId = Guid.Parse("d31fdfeb-056f-4a47-8df6-c4956912f989"),
                MeasureFields = new List<string> { "TotalAmount" },
                DimensionFields = new List<string> { "Operator", "Period", "Subscriber", "TransactionType"},
                FromTime = Utilities.Min(minDate, query.FromTime),
                ToTime = Utilities.Max(maxDate, toTime),
                Filters = new List<DimensionFilter>()
            };

            AnalyticQuery postpaidTransactionAnalyticQuery = new AnalyticQuery
            {
                TableId = Guid.Parse("659d5871-ae42-446e-b755-89cd2b3f2652"),
                MeasureFields = new List<string> { "TotalAmount" },
                DimensionFields = new List<string> { "Operator", "Period", "Subscriber", "TransactionType"},
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

            List<AnalyticRecord> prepaidCDRAnalyticRecords = analyticManager.GetAllFilteredRecords(prepaidCDRAnalyticQuery, out _);
            Dictionary<RetailDimensionFields, RetailBillingRecord> prepaidCDRBillingRecords = GetCDRBillingRecordsByDimensionFields(prepaidCDRAnalyticRecords, true);

            List<AnalyticRecord> postpaidCDRAnalyticRecords = analyticManager.GetAllFilteredRecords(postpaidCDRAnalyticQuery, out _);
            Dictionary<RetailDimensionFields, RetailBillingRecord> postpaidCDRBillingRecords = GetCDRBillingRecordsByDimensionFields(postpaidCDRAnalyticRecords, false);

            List<AnalyticRecord> prepaidSMSAnalyticRecords = analyticManager.GetAllFilteredRecords(prepaidSMSAnalyticQuery, out _);
            Dictionary<RetailDimensionFields, RetailBillingRecord> prepaidSMSBillingRecords = GetSMSBillingRecordsByDimensionFields(prepaidSMSAnalyticRecords, true);

            List<AnalyticRecord> postpaidSMSAnalyticRecords = analyticManager.GetAllFilteredRecords(postpaidSMSAnalyticQuery, out _);
            Dictionary<RetailDimensionFields, RetailBillingRecord> postpaidSMSBillingRecords = GetSMSBillingRecordsByDimensionFields(postpaidSMSAnalyticRecords, false);

            List<AnalyticRecord> prepaidTransactionAnalyticRecords = analyticManager.GetAllFilteredRecords(prepaidTransactionAnalyticQuery, out _);
            Dictionary<RetailDimensionFields, RetailBillingRecord> prepaidTransactionBillingRecords = GetTransactionBillingRecordsByDimensionFields(prepaidTransactionAnalyticRecords, true);

            List<AnalyticRecord> postpaidTransactionAnalyticRecords = analyticManager.GetAllFilteredRecords(postpaidTransactionAnalyticQuery, out _);
            Dictionary<RetailDimensionFields, RetailBillingRecord> postpaidTransactionBillingRecords = GetTransactionBillingRecordsByDimensionFields(postpaidTransactionAnalyticRecords, false);

            var allOperatorDeclarations = operatorDeclarationManager.GetDeclarationServices(periodDefinitions, filteredOperatorIds);
            var records = new List<RetailReconciliationObj>();
            if (allOperatorDeclarations != null)
            {
                foreach (var operatorDeclaration in allOperatorDeclarations)
                {
                    var reconcilationObj = new RetailReconciliationObj
                    {
                        OperatorID = operatorDeclaration.OperatorId,
                        PeriodID = operatorDeclaration.PeriodDefinition.PeriodDefinitionId
                    };
                    if (operatorDeclaration.Settings != null)
                    {
                        if (operatorDeclaration.Settings is OnNetPrepaidVoiceOperationDeclarationService)
                        {
                            var onNetPrepaidVoiceOperationDeclarationService = operatorDeclaration.Settings.CastWithValidate<OnNetPrepaidVoiceOperationDeclarationService>("onNetPrepaidVoiceOperationDeclarationService");
                            reconcilationObj.SubscriberType = SubscriberType.Prepaid;
                            reconcilationObj.SourceType = SourceType.Usage;
                            reconcilationObj.ServiceType = ServiceType.Voice;

                            reconcilationObj.DeclaredNumberOfCalls = onNetPrepaidVoiceOperationDeclarationService.Calls;
                            reconcilationObj.DeclaredRevenue = onNetPrepaidVoiceOperationDeclarationService.Revenue;
                            reconcilationObj.DeclaredDurationInMinutes = onNetPrepaidVoiceOperationDeclarationService.Duration;

                            reconcilationObj.Scope = onNetPrepaidVoiceOperationDeclarationService.Scope;
                        }
                        else if (operatorDeclaration.Settings is OnNetPostpaidVoiceOperationDeclarationService)
                        {
                            var onNetPostpaidVoiceOperationDeclarationService = operatorDeclaration.Settings.CastWithValidate<OnNetPostpaidVoiceOperationDeclarationService>("onNetPostpaidVoiceOperationDeclarationService");
                            reconcilationObj.SubscriberType = SubscriberType.Postpaid;
                            reconcilationObj.SourceType = SourceType.Usage;
                            reconcilationObj.ServiceType = ServiceType.Voice;

                            reconcilationObj.DeclaredNumberOfCalls = onNetPostpaidVoiceOperationDeclarationService.Calls;
                            reconcilationObj.DeclaredRevenue = onNetPostpaidVoiceOperationDeclarationService.Revenue;
                            reconcilationObj.DeclaredDurationInMinutes = onNetPostpaidVoiceOperationDeclarationService.Duration;

                            reconcilationObj.Scope = onNetPostpaidVoiceOperationDeclarationService.Scope;
                     
                        }
                        else if (operatorDeclaration.Settings is OnNetPrepaidSMSOperationDeclarationService)
                        {
                            var onNetPrepaidSMSOperationDeclarationService = operatorDeclaration.Settings.CastWithValidate<OnNetPrepaidSMSOperationDeclarationService>("OnNetPrepaidSMSOperationDeclarationService");
                            reconcilationObj.SubscriberType = SubscriberType.Prepaid;
                            reconcilationObj.SourceType = SourceType.Usage;
                            reconcilationObj.ServiceType = ServiceType.SMS;

                            reconcilationObj.DeclaredNumberOfSMS = onNetPrepaidSMSOperationDeclarationService.SMS;
                            reconcilationObj.DeclaredRevenue = onNetPrepaidSMSOperationDeclarationService.Revenue;

                            reconcilationObj.Scope = onNetPrepaidSMSOperationDeclarationService.Scope;
                        }
                        else if (operatorDeclaration.Settings is OnNetPostpaidSMSOperationDeclarationService)
                        {
                            var onNetPostpaidSMSOperationDeclarationService = operatorDeclaration.Settings.CastWithValidate<OnNetPostpaidSMSOperationDeclarationService>("onNetPostpaidSMSOperationDeclarationService");
                            reconcilationObj.SubscriberType = SubscriberType.Postpaid;
                            reconcilationObj.SourceType = SourceType.Usage;
                            reconcilationObj.ServiceType = ServiceType.SMS;

                            reconcilationObj.DeclaredNumberOfSMS = onNetPostpaidSMSOperationDeclarationService.SMS;
                            reconcilationObj.DeclaredRevenue = onNetPostpaidSMSOperationDeclarationService.Revenue;

                            reconcilationObj.Scope = onNetPostpaidSMSOperationDeclarationService.Scope;
                        }
                        else if (operatorDeclaration.Settings is OnNetPrepaidTotalOperationDeclarationService)
                        {
                            var onNetPrepaidTotalOperationDeclarationService = operatorDeclaration.Settings.CastWithValidate<OnNetPrepaidTotalOperationDeclarationService>("onNetPrepaidTotalOperationDeclarationService");
                            reconcilationObj.SubscriberType = SubscriberType.Prepaid;

                            reconcilationObj.DeclaredTransactionAmount = onNetPrepaidTotalOperationDeclarationService.AmountFromTopups;
                        }
                        else if (operatorDeclaration.Settings is OnNetPostpaidTotalOperationDeclarationService)
                        {
                            var onNetPostpaidTotalOperationDeclarationService = operatorDeclaration.Settings.CastWithValidate<OnNetPostpaidTotalOperationDeclarationService>("onNetPostpaidTotalOperationDeclarationService");
                            reconcilationObj.SubscriberType = SubscriberType.Postpaid;

                            reconcilationObj.DeclaredRevenue = onNetPostpaidTotalOperationDeclarationService.Revenue;
                        }
                    }
                    records.Add(reconcilationObj);
                }
            }

            Dictionary<RetailDimensionFields, RetailReconciliationObj> billingRecordWithoutOperatorDeclaration = new Dictionary<RetailDimensionFields, RetailReconciliationObj>();
            if (prepaidCDRBillingRecords != null && prepaidCDRBillingRecords.Count > 0)
            {
                foreach (var billingRecord in prepaidCDRBillingRecords)
                {
                    var recordObj = billingRecordWithoutOperatorDeclaration.GetOrCreateItem(billingRecord.Key);
                    recordObj.OperatorID = billingRecord.Value.OperatorID;
                    recordObj.PeriodID = billingRecord.Value.PeriodID;
                    recordObj.SubscriberType = billingRecord.Value.SubscriberType;
                    recordObj.SourceType = billingRecord.Value.SourceType;
                    recordObj.ServiceType = billingRecord.Value.ServiceType;
                    recordObj.Scope = billingRecord.Value.Scope;
                    recordObj.SubscriberID = billingRecord.Value.SubscriberID;

                    if (!recordObj.CalculatedDurationInMinutes.HasValue)
                        recordObj.CalculatedDurationInMinutes = 0;
                    recordObj.CalculatedDurationInMinutes += billingRecord.Value.DurationInMinutes;

                    if (!recordObj.CalculatedNumberOfCalls.HasValue)
                        recordObj.CalculatedNumberOfCalls = 0;
                    recordObj.CalculatedNumberOfCalls += billingRecord.Value.NumberOfCDRs;

                    if (!recordObj.CalculatedRevenue.HasValue)
                        recordObj.CalculatedRevenue = 0;
                    recordObj.CalculatedRevenue += billingRecord.Value.Revenue;
                }
            }


            if (postpaidCDRBillingRecords != null && postpaidCDRBillingRecords.Count > 0)
            {
                foreach (var billingRecord in postpaidCDRBillingRecords)
                {
                    var recordObj = billingRecordWithoutOperatorDeclaration.GetOrCreateItem(billingRecord.Key);
                    recordObj.OperatorID = billingRecord.Value.OperatorID;
                    recordObj.PeriodID = billingRecord.Value.PeriodID;
                    recordObj.SubscriberType = billingRecord.Value.SubscriberType;
                    recordObj.SourceType = billingRecord.Value.SourceType;
                    recordObj.ServiceType = billingRecord.Value.ServiceType;
                    recordObj.Scope = billingRecord.Value.Scope;
                    recordObj.SubscriberID = billingRecord.Value.SubscriberID;

                    if (!recordObj.CalculatedDurationInMinutes.HasValue)
                        recordObj.CalculatedDurationInMinutes = 0;
                    recordObj.CalculatedDurationInMinutes += billingRecord.Value.DurationInMinutes;

                    if (!recordObj.CalculatedNumberOfCalls.HasValue)
                        recordObj.CalculatedNumberOfCalls = 0;
                    recordObj.CalculatedNumberOfCalls += billingRecord.Value.NumberOfCDRs;

                    if (!recordObj.CalculatedRevenue.HasValue)
                        recordObj.CalculatedRevenue = 0;
                    recordObj.CalculatedRevenue += billingRecord.Value.Revenue;
                }
            }


            if (prepaidSMSBillingRecords != null && prepaidSMSBillingRecords.Count > 0)
            {
                foreach (var billingRecord in prepaidSMSBillingRecords)
                {
                    var recordObj = billingRecordWithoutOperatorDeclaration.GetOrCreateItem(billingRecord.Key);
                    recordObj.OperatorID = billingRecord.Value.OperatorID;
                    recordObj.PeriodID = billingRecord.Value.PeriodID;
                    recordObj.SubscriberType = billingRecord.Value.SubscriberType;
                    recordObj.SourceType = billingRecord.Value.SourceType;
                    recordObj.ServiceType = billingRecord.Value.ServiceType;
                    recordObj.Scope = billingRecord.Value.Scope;
                    recordObj.SubscriberID = billingRecord.Value.SubscriberID;

                    if (!recordObj.CalculatedNumberOfSMS.HasValue)
                        recordObj.CalculatedNumberOfSMS = 0;
                    recordObj.CalculatedNumberOfSMS += billingRecord.Value.NumberOfSMS;

                    if (!recordObj.CalculatedRevenue.HasValue)
                        recordObj.CalculatedRevenue = 0;
                    recordObj.CalculatedRevenue += billingRecord.Value.Revenue;
                }
            }

            if (postpaidSMSBillingRecords != null && postpaidSMSBillingRecords.Count > 0)
            {
                foreach (var billingRecord in postpaidSMSBillingRecords)
                {
                    var recordObj = billingRecordWithoutOperatorDeclaration.GetOrCreateItem(billingRecord.Key);
                    recordObj.OperatorID = billingRecord.Value.OperatorID;
                    recordObj.PeriodID = billingRecord.Value.PeriodID;
                    recordObj.SubscriberType = billingRecord.Value.SubscriberType;
                    recordObj.SourceType = billingRecord.Value.SourceType;
                    recordObj.ServiceType = billingRecord.Value.ServiceType;
                    recordObj.Scope = billingRecord.Value.Scope;
                    recordObj.SubscriberID = billingRecord.Value.SubscriberID;

                    if (!recordObj.CalculatedNumberOfSMS.HasValue)
                        recordObj.CalculatedNumberOfSMS = 0;
                    recordObj.CalculatedNumberOfSMS += billingRecord.Value.NumberOfSMS;

                    if (!recordObj.CalculatedRevenue.HasValue)
                        recordObj.CalculatedRevenue = 0;
                    recordObj.CalculatedRevenue += billingRecord.Value.Revenue;
                }
            }


            if (prepaidTransactionBillingRecords != null && prepaidTransactionBillingRecords.Count > 0)
            {
                foreach (var billingRecord in prepaidTransactionBillingRecords)
                {
                    var recordObj = billingRecordWithoutOperatorDeclaration.GetOrCreateItem(billingRecord.Key);
                    recordObj.OperatorID = billingRecord.Value.OperatorID;
                    recordObj.PeriodID = billingRecord.Value.PeriodID;
                    recordObj.SubscriberType = billingRecord.Value.SubscriberType;
                    recordObj.SourceType = billingRecord.Value.SourceType;
                    recordObj.ServiceType = billingRecord.Value.ServiceType;
                    recordObj.Scope = billingRecord.Value.Scope;
                    recordObj.SubscriberID = billingRecord.Value.SubscriberID;
                    recordObj.TransactionTypeID = billingRecord.Value.TransactionTypeID;

                    if (!recordObj.CalculatedRevenue.HasValue)
                        recordObj.CalculatedRevenue = 0;
                    recordObj.CalculatedRevenue += billingRecord.Value.Revenue;

                    if (!recordObj.CalculatedTransactionAmount.HasValue)
                        recordObj.CalculatedTransactionAmount = 0;
                    recordObj.CalculatedTransactionAmount += billingRecord.Value.TransactionAmount;
                }
            }

            if (postpaidTransactionBillingRecords != null && postpaidTransactionBillingRecords.Count > 0)
            {
                foreach (var billingRecord in postpaidTransactionBillingRecords)
                {
                    var recordObj = billingRecordWithoutOperatorDeclaration.GetOrCreateItem(billingRecord.Key);
                    recordObj.OperatorID = billingRecord.Value.OperatorID;
                    recordObj.PeriodID = billingRecord.Value.PeriodID;
                    recordObj.SubscriberType = billingRecord.Value.SubscriberType;
                    recordObj.SourceType = billingRecord.Value.SourceType;
                    recordObj.ServiceType = billingRecord.Value.ServiceType;
                    recordObj.Scope = billingRecord.Value.Scope;
                    recordObj.SubscriberID = billingRecord.Value.SubscriberID;
                    recordObj.TransactionTypeID = billingRecord.Value.TransactionTypeID;

                    if (!recordObj.CalculatedRevenue.HasValue)
                        recordObj.CalculatedRevenue = 0;
                    recordObj.CalculatedRevenue += billingRecord.Value.Revenue;

                    if (!recordObj.CalculatedTransactionAmount.HasValue)
                        recordObj.CalculatedTransactionAmount = 0;
                    recordObj.CalculatedTransactionAmount += billingRecord.Value.TransactionAmount;
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
        #endregion

        #region Private Methods
        private RawMemoryRecord GetRawMemoryRecord(RetailReconciliationObj reconciliationObj)
        {
            RawMemoryRecord rawMemoryRecord = new RawMemoryRecord { FieldValues = new Dictionary<string, dynamic>() };
            rawMemoryRecord.FieldValues.Add("OperatorID", reconciliationObj.OperatorID);
            rawMemoryRecord.FieldValues.Add("PeriodID", reconciliationObj.PeriodID);
            rawMemoryRecord.FieldValues.Add("SubscriberID", reconciliationObj.SubscriberID);
            rawMemoryRecord.FieldValues.Add("SubscriberType", reconciliationObj.SubscriberType);
            rawMemoryRecord.FieldValues.Add("Scope", reconciliationObj.Scope);
            rawMemoryRecord.FieldValues.Add("ServiceType", reconciliationObj.ServiceType);
            rawMemoryRecord.FieldValues.Add("SourceType", reconciliationObj.SourceType);
            rawMemoryRecord.FieldValues.Add("TransactionTypeID", reconciliationObj.TransactionTypeID);

            rawMemoryRecord.FieldValues.Add("CalculatedDurationInMinutes", reconciliationObj.CalculatedDurationInMinutes);
            rawMemoryRecord.FieldValues.Add("DeclaredDurationInMinutes", reconciliationObj.DeclaredDurationInMinutes);

            rawMemoryRecord.FieldValues.Add("CalculatedRevenue", reconciliationObj.CalculatedRevenue);
            rawMemoryRecord.FieldValues.Add("DeclaredRevenue", reconciliationObj.DeclaredRevenue);

            rawMemoryRecord.FieldValues.Add("CalculatedNumberOfCalls", reconciliationObj.CalculatedNumberOfCalls);
            rawMemoryRecord.FieldValues.Add("DeclaredNumberOfCalls", reconciliationObj.DeclaredNumberOfCalls);

            rawMemoryRecord.FieldValues.Add("CalculatedNumberOfSMS", reconciliationObj.CalculatedNumberOfSMS);
            rawMemoryRecord.FieldValues.Add("DeclaredNumberOfSMS", reconciliationObj.DeclaredNumberOfSMS);

            rawMemoryRecord.FieldValues.Add("CalculatedTransactionAmount", reconciliationObj.CalculatedTransactionAmount);
            rawMemoryRecord.FieldValues.Add("DeclaredTransactionAmount", reconciliationObj.DeclaredTransactionAmount);

            return rawMemoryRecord;
        }
        private Dictionary<RetailDimensionFields, RetailBillingRecord> GetCDRBillingRecordsByDimensionFields(List<AnalyticRecord> analyticRecords, bool isPrepaid)
        {
            Dictionary<RetailDimensionFields, RetailBillingRecord> reconcilationRecords = new Dictionary<RetailDimensionFields, RetailBillingRecord>();
            if(analyticRecords!=null && analyticRecords.Count > 0)
            {
                foreach (var analyticRecord in analyticRecords)
                {
                    var operatorIdDimension = analyticRecord.DimensionValues[0];
                    var periodDimension = analyticRecord.DimensionValues[1];
                    var subscriberIdDimension = analyticRecord.DimensionValues[2];
                    var scopeDimension = analyticRecord.DimensionValues[3];

                    if (periodDimension == null || periodDimension.Value == null)
                        continue;

                    RetailDimensionFields dimensionFields = new RetailDimensionFields()
                    {
                        OperatorID = (long)operatorIdDimension.Value,
                        PeriodID = (int)periodDimension.Value,
                        Scope = (Scope)scopeDimension.Value,
                        SubscriberType = isPrepaid ? SubscriberType.Prepaid : SubscriberType.Postpaid,
                        ServiceType = ServiceType.Voice,
                        SourceType = SourceType.Usage
                    };

                    RetailBillingRecord billingRecord = new RetailBillingRecord
                    {
                        OperatorID = (long)operatorIdDimension.Value,
                        PeriodID = (int)periodDimension.Value,
                        Scope = (Scope)scopeDimension.Value,
                        SubscriberType = isPrepaid ? SubscriberType.Prepaid : SubscriberType.Postpaid,
                        ServiceType = ServiceType.Voice,
                        SourceType = SourceType.Usage,
                        SubscriberID = (long?)subscriberIdDimension.Value
                    };

                    MeasureValue calculatedDurationInMin;
                    analyticRecord.MeasureValues.TryGetValue("TotalDurationInMinutes", out calculatedDurationInMin);
                    if (calculatedDurationInMin?.Value != null)
                        billingRecord.DurationInMinutes = Convert.ToDecimal(calculatedDurationInMin.Value ?? 0.0);

                    MeasureValue calculatedRevenue;
                    analyticRecord.MeasureValues.TryGetValue("TotalRevenue", out calculatedRevenue);
                    if (calculatedRevenue?.Value != null)
                        billingRecord.Revenue = Convert.ToDecimal(calculatedRevenue.Value ?? 0.0);

                    MeasureValue countCDR;
                    analyticRecord.MeasureValues.TryGetValue("NumberOfCDRs", out countCDR);
                    if (countCDR?.Value != null)
                        billingRecord.NumberOfCDRs = Convert.ToInt64(countCDR.Value ?? 0.0);

                    if (reconcilationRecords.ContainsKey(dimensionFields))
                    {
                        reconcilationRecords[dimensionFields].DurationInMinutes += billingRecord.DurationInMinutes;
                        reconcilationRecords[dimensionFields].NumberOfCDRs += billingRecord.NumberOfCDRs;
                        reconcilationRecords[dimensionFields].Revenue += billingRecord.Revenue;
                    }
                    else
                    {
                        reconcilationRecords.Add(dimensionFields, billingRecord);
                    }

                }
            }
            return reconcilationRecords;
        }
        private Dictionary<RetailDimensionFields, RetailBillingRecord> GetSMSBillingRecordsByDimensionFields(List<AnalyticRecord> analyticRecords, bool isPrepaid)
        {
            Dictionary<RetailDimensionFields, RetailBillingRecord> reconcilationRecords = new Dictionary<RetailDimensionFields, RetailBillingRecord>();
            foreach (var analyticRecord in analyticRecords)
            {
                var operatorIdDimension = analyticRecord.DimensionValues[0];
                var periodDimension = analyticRecord.DimensionValues[1];
                var subscriberIdDimension = analyticRecord.DimensionValues[2];
                var scopeDimension = analyticRecord.DimensionValues[3];

                if (periodDimension == null || periodDimension.Value == null)
                    continue;

                RetailDimensionFields dimensionFields = new RetailDimensionFields()
                {
                    OperatorID = (long)operatorIdDimension.Value,
                    PeriodID = (int)periodDimension.Value,
                    Scope = (Scope)scopeDimension.Value,
                    SubscriberType = isPrepaid ? SubscriberType.Prepaid : SubscriberType.Postpaid,
                    ServiceType = ServiceType.SMS,
                    SourceType = SourceType.Usage
                };

                RetailBillingRecord billingRecord = new RetailBillingRecord
                {
                    OperatorID = (long)operatorIdDimension.Value,
                    PeriodID = (int)periodDimension.Value,
                    Scope = (Scope)scopeDimension.Value,
                    SubscriberType = isPrepaid ? SubscriberType.Prepaid : SubscriberType.Postpaid,
                    ServiceType = ServiceType.SMS,
                    SourceType = SourceType.Usage,
                    SubscriberID = (long?)subscriberIdDimension.Value,
                };

                MeasureValue calculatedRevenue;
                analyticRecord.MeasureValues.TryGetValue("TotalRevenue", out calculatedRevenue);
                if (calculatedRevenue?.Value != null)
                    billingRecord.Revenue = Convert.ToDecimal(calculatedRevenue.Value ?? 0.0);

                MeasureValue countSMS;
                analyticRecord.MeasureValues.TryGetValue("NumberOfSMS", out countSMS);
                if (countSMS?.Value != null)
                    billingRecord.NumberOfSMS = Convert.ToInt64(countSMS.Value ?? 0.0);

                if (reconcilationRecords.ContainsKey(dimensionFields))
                {
                    reconcilationRecords[dimensionFields].NumberOfSMS += billingRecord.NumberOfSMS;
                    reconcilationRecords[dimensionFields].Revenue += billingRecord.Revenue;
                }
                else
                {
                    reconcilationRecords.Add(dimensionFields, billingRecord);
                }

            }
            return reconcilationRecords;
        }
        private Dictionary<RetailDimensionFields, RetailBillingRecord> GetTransactionBillingRecordsByDimensionFields(List<AnalyticRecord> analyticRecords, bool isPrepaid)
        {
            Dictionary<RetailDimensionFields, RetailBillingRecord> reconcilationRecords = new Dictionary<RetailDimensionFields, RetailBillingRecord>();
            foreach (var analyticRecord in analyticRecords)
            {
                var operatorIdDimension = analyticRecord.DimensionValues[0]; 
                var periodDimension = analyticRecord.DimensionValues[1];
                var subscriberIdDimension = analyticRecord.DimensionValues[2];
                var transactionTypeIdDimension = analyticRecord.DimensionValues[3];

                var topUpTransactionTypeId = new Guid("3a26d47e-d2f4-42f3-817b-d1638d8e4290");

                if (isPrepaid && (Guid)transactionTypeIdDimension.Value != topUpTransactionTypeId)
                    continue;

                if (periodDimension == null || periodDimension.Value == null)
                    continue;

                RetailDimensionFields dimensionFields = new RetailDimensionFields()
                {
                    OperatorID = (long)operatorIdDimension.Value,
                    PeriodID = (int)periodDimension.Value,
                    SubscriberType = isPrepaid ? SubscriberType.Prepaid : SubscriberType.Postpaid,
                    SourceType = SourceType.NonUsage
                };

                RetailBillingRecord billingRecord = new RetailBillingRecord
                {
                    OperatorID = (long)operatorIdDimension.Value,
                    PeriodID = (int)periodDimension.Value,
                    SubscriberType = isPrepaid ? SubscriberType.Prepaid : SubscriberType.Postpaid,
                    SourceType = SourceType.NonUsage,
                    TransactionTypeID = (Guid?)transactionTypeIdDimension.Value,
                    SubscriberID = (long?)subscriberIdDimension.Value
                };

                MeasureValue calculatedAmount;
                analyticRecord.MeasureValues.TryGetValue("TotalAmount", out calculatedAmount);
                if (calculatedAmount?.Value != null)
                    billingRecord.TransactionAmount = Convert.ToDecimal(calculatedAmount.Value ?? 0.0);

                if (reconcilationRecords.ContainsKey(dimensionFields))
                {
                    reconcilationRecords[dimensionFields].TransactionAmount += billingRecord.TransactionAmount;
                }
                else
                {
                    reconcilationRecords.Add(dimensionFields, billingRecord);
                }

            }
            return reconcilationRecords;
        }
        #endregion
    }

    public class RetailBillingRecord
    {
        public long OperatorID { get; set; }
        public int PeriodID { get; set; }
        public SubscriberType SubscriberType { get; set; }
        public Scope? Scope { get; set; }
        public SourceType SourceType { get; set; }
        public ServiceType? ServiceType { get; set; }

        public long? SubscriberID { get; set; }
        public Guid? TransactionTypeID { get; set; }

        public decimal? Revenue { get; set; }
        public decimal? DurationInMinutes { get; set; }
        public long? NumberOfCDRs { get; set; }
        public long? NumberOfSMS { get; set; }
        public decimal? TransactionAmount { get; set; }
    }

    public class RetailReconciliationObj
    {
        public long OperatorID { get; set; }
        public int PeriodID { get; set; }
        public SubscriberType SubscriberType { get; set; }
        public Scope? Scope { get; set; }
        public SourceType SourceType { get; set; }
        public ServiceType? ServiceType { get; set; }

        public long? SubscriberID { get; set; }
        public Guid? TransactionTypeID { get; set; }

        public decimal? CalculatedRevenue { get; set; }
        public decimal? DeclaredRevenue { get; set; }

        public decimal? CalculatedDurationInMinutes { get; set; }
        public decimal? DeclaredDurationInMinutes { get; set; }

        public long? CalculatedNumberOfCalls { get; set; }
        public long? DeclaredNumberOfCalls { get; set; }

        public long? CalculatedNumberOfSMS { get; set; }
        public long? DeclaredNumberOfSMS { get; set; }

        public decimal? CalculatedTransactionAmount { get; set; }
        public decimal? DeclaredTransactionAmount { get; set; }

    }
    public struct RetailDimensionFields
    {
        public long OperatorID { get; set; }
        public int PeriodID { get; set; }
        public SubscriberType SubscriberType { get; set; }
        public Scope? Scope { get; set; }
        public SourceType SourceType { get; set; }
        public ServiceType? ServiceType { get; set; }
    }

}
