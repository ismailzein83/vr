using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Queueing.Entities;
using Vanrise.Reprocess.Entities;
using Vanrise.Queueing;
using Vanrise.GenericData.QueueActivators;
using Vanrise.Common;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;
using Vanrise.Data.SQL;
using System.Data.SqlClient;

namespace Retail.Data.MainExtensions.QueueActivators
{
    public class AccumulateDataActivator : QueueActivator, IReprocessStageActivator
    {
        Guid _accumulatedDataRecordTypeId = new Guid("8edcf809-1819-4072-a957-628ff325f760");
        Guid _accumulatedDataRecordStorageId = new Guid("ccda79a0-6863-4018-b78a-011c76b11ea0");
        decimal _sessionOctetsLimit = 4617089843.2M;

        #region QueueActivator

        public override void OnDisposed()
        {
        }

        public override void ProcessItem(PersistentQueueItem item, ItemsToEnqueue outputItems)
        {
        }

        public override void ProcessItem(IQueueActivatorExecutionContext context)
        {
            DataRecordBatch dataRecordBatch = context.ItemToProcess as DataRecordBatch;
            var queueItemType = context.CurrentStage.QueueItemType as DataRecordBatchQueueItemType;
            if (queueItemType == null)
                throw new Exception("current stage QueueItemType is not of type DataRecordBatchQueueItemType");
            var recordTypeId = queueItemType.DataRecordTypeId;
            var batchRecords = dataRecordBatch.GetBatchRecords(recordTypeId);

            DataRecordTypeManager dataRecordTypeManager = new DataRecordTypeManager();
            Type accumulatedDataRecordRuntimeType = dataRecordTypeManager.GetDataRecordRuntimeType(_accumulatedDataRecordTypeId);

            DataRecordStorageManager dataRecordStorageManager = new DataRecordStorageManager();
            int maxParameterNumber = 51000; // dataRecordStorageManager.GetDBQueryMaxParameterNumber(_accumulatedDataRecordStorageId);

            Dictionary<string, dynamic> accumulatedDataDict = new Dictionary<string, dynamic>();
            Dictionary<string, dynamic> accumulatedDataToAddDict = new Dictionary<string, dynamic>();
            Dictionary<string, dynamic> accumulatedDataToUpdateDict = new Dictionary<string, dynamic>();

            Dictionary<string, List<dynamic>> batchRecordsByUserSession = new Dictionary<string, List<dynamic>>();
            foreach (var record in batchRecords)
            {
                List<dynamic> dataRecords = batchRecordsByUserSession.GetOrCreateItem(record.UserSession as string, () => { return new List<dynamic>(); });
                dataRecords.Add(record);
            }

            List<string> currentBatchUserSessions = new List<string>();
            List<dynamic> currentBatchDataRecords = new List<dynamic>();
            foreach (var kvp in batchRecordsByUserSession)
            {
                currentBatchUserSessions.Add(kvp.Key);
                currentBatchDataRecords.AddRange(kvp.Value);

                if (currentBatchUserSessions.Count >= maxParameterNumber)
                {
                    AccumulateData(currentBatchDataRecords, currentBatchUserSessions, dataRecordStorageManager, accumulatedDataRecordRuntimeType, accumulatedDataDict, accumulatedDataToAddDict, accumulatedDataToUpdateDict);
                    currentBatchUserSessions = new List<string>();
                    currentBatchDataRecords = new List<dynamic>();
                }
            }

            if (currentBatchUserSessions.Count > 0)
                AccumulateData(currentBatchDataRecords, currentBatchUserSessions, dataRecordStorageManager, accumulatedDataRecordRuntimeType, accumulatedDataDict, accumulatedDataToAddDict, accumulatedDataToUpdateDict);

            if (accumulatedDataToAddDict != null && accumulatedDataToAddDict.Count > 0)
            {
                dataRecordStorageManager.AddDataRecords(_accumulatedDataRecordStorageId, accumulatedDataToAddDict.Values);
            }

            if (accumulatedDataToUpdateDict != null && accumulatedDataToUpdateDict.Count > 0)
            {
                List<string> fieldsToJoin = BuildFieldsToJoin();
                List<string> fieldsToUpdate = BuildFieldsToUpdate();
                dataRecordStorageManager.UpdateDataRecords(_accumulatedDataRecordStorageId, accumulatedDataToUpdateDict.Values, fieldsToJoin, fieldsToUpdate);
            }
        }

        private void AccumulateData(List<dynamic> batchRecords, List<string> batchUserSessions, DataRecordStorageManager dataRecordStorageManager, Type accumulatedDataRecordRuntimeType,
            Dictionary<string, dynamic> accumulatedDataDict, Dictionary<string, dynamic> accumulatedDataToAddDict, Dictionary<string, dynamic> accumulatedDataToUpdateDict)
        {
            RecordFilterGroup recordFilterGroup = this.BuildRecordFilterGroup(batchUserSessions);

            dataRecordStorageManager.GetDataRecords(_accumulatedDataRecordStorageId, null, null, recordFilterGroup, null, (itm) =>
            {
                string userSession = itm.GetFieldValue(ProcessedDataPropertyName.UserSession);
                accumulatedDataDict.Add(userSession, itm);
                accumulatedDataToUpdateDict.Add(userSession, itm);
            });

            foreach (var currentRecord in batchRecords)
            {
                DateTime recordDateTime = currentRecord.GetFieldValue(ProcessedDataPropertyName.RecordDateTime);
                string userSession = currentRecord.GetFieldValue(ProcessedDataPropertyName.UserSession);
                decimal? inputOctets = currentRecord.GetFieldValue(ProcessedDataPropertyName.InputOctets);
                decimal? outputOctets = currentRecord.GetFieldValue(ProcessedDataPropertyName.OutputOctets);

                dynamic dataRecord;
                if (accumulatedDataDict.TryGetValue(userSession, out dataRecord))
                {
                    dataRecord.SetFieldValue(AccumulatedDataPropertyName.LastInTime, recordDateTime);
                    dataRecord.SetFieldValue(AccumulatedDataPropertyName.LastOutTime, recordDateTime);

                    if (inputOctets.HasValue)
                    {
                        decimal? lastReceivedIn = dataRecord.GetFieldValue(AccumulatedDataPropertyName.LastReceivedIn);
                        decimal? totalIn = dataRecord.GetFieldValue(AccumulatedDataPropertyName.TotalIn);

                        if (!lastReceivedIn.HasValue || (inputOctets.Value - lastReceivedIn.Value) >= 0)
                        {
                            decimal newInputOctets = inputOctets.Value - (lastReceivedIn.HasValue ? lastReceivedIn.Value : 0);
                            decimal newTotalIn = (totalIn.HasValue ? totalIn.Value : 0) + newInputOctets;
                            dataRecord.SetFieldValue(AccumulatedDataPropertyName.LastReceivedIn, inputOctets.Value);
                            dataRecord.SetFieldValue(AccumulatedDataPropertyName.TotalIn, newTotalIn);
                        }
                        else
                        {
                            decimal newInputOctets = (_sessionOctetsLimit - lastReceivedIn.Value) + inputOctets.Value;
                            decimal newTotalIn = (totalIn.HasValue ? totalIn.Value : 0) + newInputOctets;
                            int numberOfInReset = dataRecord.GetFieldValue(AccumulatedDataPropertyName.NumberOfInReset);

                            dataRecord.SetFieldValue(AccumulatedDataPropertyName.LastReceivedIn, inputOctets.Value);
                            dataRecord.SetFieldValue(AccumulatedDataPropertyName.TotalIn, newTotalIn);
                            dataRecord.SetFieldValue(AccumulatedDataPropertyName.NumberOfInReset, numberOfInReset + 1);
                            dataRecord.SetFieldValue(AccumulatedDataPropertyName.LastInResetTime, recordDateTime);
                        }
                    }

                    if (outputOctets.HasValue)
                    {
                        decimal? lastReceivedOut = dataRecord.GetFieldValue(AccumulatedDataPropertyName.LastReceivedOut);
                        decimal? totalOut = dataRecord.GetFieldValue(AccumulatedDataPropertyName.TotalOut);

                        if (!lastReceivedOut.HasValue || (outputOctets.Value - lastReceivedOut.Value) >= 0)
                        {
                            decimal newOutputOctets = outputOctets.Value - (lastReceivedOut.HasValue ? lastReceivedOut.Value : 0);
                            decimal newTotalOut = (totalOut.HasValue ? totalOut.Value : 0) + newOutputOctets;

                            dataRecord.SetFieldValue(AccumulatedDataPropertyName.LastReceivedOut, outputOctets.Value);
                            dataRecord.SetFieldValue(AccumulatedDataPropertyName.TotalOut, newTotalOut);
                        }
                        else
                        {
                            decimal newOutputOctets = (_sessionOctetsLimit - lastReceivedOut.Value) + outputOctets.Value;
                            decimal newTotalOut = (totalOut.HasValue ? totalOut.Value : 0) + newOutputOctets;
                            int numberOfOutReset = dataRecord.GetFieldValue(AccumulatedDataPropertyName.NumberOfOutReset);

                            dataRecord.SetFieldValue(AccumulatedDataPropertyName.LastReceivedOut, outputOctets.Value);
                            dataRecord.SetFieldValue(AccumulatedDataPropertyName.TotalOut, newTotalOut);
                            dataRecord.SetFieldValue(AccumulatedDataPropertyName.NumberOfOutReset, numberOfOutReset + 1);
                            dataRecord.SetFieldValue(AccumulatedDataPropertyName.LastOutResetTime, recordDateTime);
                        }
                    }
                }
                else
                {
                    long? ispId = currentRecord.GetFieldValue(ProcessedDataPropertyName.ISP);
                    string userName = currentRecord.GetFieldValue(ProcessedDataPropertyName.UserName);
                    string sessionId = currentRecord.GetFieldValue(ProcessedDataPropertyName.SessionId);

                    dataRecord = Activator.CreateInstance(accumulatedDataRecordRuntimeType) as dynamic;
                    dataRecord.SetFieldValue(AccumulatedDataPropertyName.ISP, ispId);
                    dataRecord.SetFieldValue(AccumulatedDataPropertyName.UserName, userName);
                    dataRecord.SetFieldValue(AccumulatedDataPropertyName.SessionId, sessionId);
                    dataRecord.SetFieldValue(AccumulatedDataPropertyName.UserSession, userSession);
                    dataRecord.SetFieldValue(AccumulatedDataPropertyName.RecordDateTime, recordDateTime);
                    dataRecord.SetFieldValue(AccumulatedDataPropertyName.LastInTime, recordDateTime);
                    dataRecord.SetFieldValue(AccumulatedDataPropertyName.LastOutTime, recordDateTime);

                    if (inputOctets.HasValue)
                    {
                        dataRecord.SetFieldValue(AccumulatedDataPropertyName.LastReceivedIn, inputOctets.Value);
                        dataRecord.SetFieldValue(AccumulatedDataPropertyName.TotalIn, inputOctets.Value);
                    }

                    if (outputOctets.HasValue)
                    {
                        dataRecord.SetFieldValue(AccumulatedDataPropertyName.LastReceivedOut, outputOctets.Value);
                        dataRecord.SetFieldValue(AccumulatedDataPropertyName.TotalOut, outputOctets.Value);
                    }

                    accumulatedDataDict.Add(userSession, dataRecord);
                    accumulatedDataToAddDict.Add(userSession, dataRecord);
                }
            }
        }

        #endregion

        #region IReprocessStageActivator

        public object InitializeStage(IReprocessStageActivatorInitializingContext context)
        {
            throw new NotImplementedException();
        }

        public void ExecuteStage(IReprocessStageActivatorExecutionContext context)
        {
            throw new NotImplementedException();
        }

        public void FinalizeStage(IReprocessStageActivatorFinalizingContext context)
        {
            throw new NotImplementedException();
        }

        public int? GetStorageRowCount(IReprocessStageActivatorGetStorageRowCountContext context)
        {
            throw new NotImplementedException();
        }

        public void CommitChanges(IReprocessStageActivatorCommitChangesContext context)
        {
            throw new NotImplementedException();
        }

        public void DropStorage(IReprocessStageActivatorDropStorageContext context)
        {
            throw new NotImplementedException();
        }

        public List<string> GetOutputStages(List<string> stageNames)
        {
            throw new NotImplementedException();
        }

        public BaseQueue<IReprocessBatch> GetQueue()
        {
            throw new NotImplementedException();
        }

        public List<BatchRecord> GetStageBatchRecords(IReprocessStageActivatorPreparingContext context)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Private Methods

        private RecordFilterGroup BuildRecordFilterGroup(List<string> batchUserSessions)
        {
            if (batchUserSessions == null || batchUserSessions.Count == 0)
                return null;

            StringListRecordFilter stringListRecordFilter = new StringListRecordFilter();
            stringListRecordFilter.CompareOperator = ListRecordFilterOperator.In;
            stringListRecordFilter.FieldName = "UserSession";
            stringListRecordFilter.Values = batchUserSessions;

            return new RecordFilterGroup() { Filters = new List<RecordFilter>() { stringListRecordFilter } };
        }

        private List<string> BuildFieldsToJoin()
        {
            return new List<string>() { AccumulatedDataPropertyName.UserSession };
        }

        private List<string> BuildFieldsToUpdate()
        {
            List<string> fieldsToUpdate = new List<string>();
            fieldsToUpdate.Add(AccumulatedDataPropertyName.LastReceivedIn);
            fieldsToUpdate.Add(AccumulatedDataPropertyName.TotalIn);
            fieldsToUpdate.Add(AccumulatedDataPropertyName.NumberOfInReset);
            fieldsToUpdate.Add(AccumulatedDataPropertyName.LastInTime);
            fieldsToUpdate.Add(AccumulatedDataPropertyName.LastInResetTime);
            fieldsToUpdate.Add(AccumulatedDataPropertyName.LastReceivedOut);
            fieldsToUpdate.Add(AccumulatedDataPropertyName.TotalOut);
            fieldsToUpdate.Add(AccumulatedDataPropertyName.NumberOfOutReset);
            fieldsToUpdate.Add(AccumulatedDataPropertyName.LastOutTime);
            fieldsToUpdate.Add(AccumulatedDataPropertyName.LastOutResetTime);
            return fieldsToUpdate;
        }

        #endregion

        #region Private Classes

        public static class ProcessedDataPropertyName
        {
            public static string RecordDateTime = "RecordDateTime";
            public static string ISP = "ISP";
            public static string UserName = "UserName";
            public static string SessionId = "SessionId";
            public static string UserSession = "UserSession";
            public static string InputOctets = "InputOctets";
            public static string OutputOctets = "OutputOctets";
        }

        public static class AccumulatedDataPropertyName
        {
            public static string RecordDateTime = "RecordDateTime";
            public static string ISP = "ISP";
            public static string UserName = "UserName";
            public static string SessionId = "SessionId";
            public static string UserSession = "UserSession";

            public static string LastReceivedIn = "LastReceivedIn";
            public static string TotalIn = "TotalIn";
            public static string NumberOfInReset = "NumberOfInReset";
            public static string LastInTime = "LastInTime";
            public static string LastInResetTime = "LastInResetTime";

            public static string LastReceivedOut = "LastReceivedOut";
            public static string TotalOut = "TotalOut";
            public static string NumberOfOutReset = "NumberOfOutReset";
            public static string LastOutTime = "LastOutTime";
            public static string LastOutResetTime = "LastOutResetTime";
        }

        #endregion
    }
}