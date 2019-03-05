using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using TestCallAnalysis.Entities;
using Vanrise.BusinessProcess;
using Vanrise.Entities;
using Vanrise.GenericData.Business;
using Vanrise.Queueing;

namespace TestCallAnalysis.BP.Activities
{
    public class UpdateCorrelatedCDRsBatch
    {
        public Dictionary<long, string> ExcitingCaseCDRs { get; set; }

        public UpdateCorrelatedCDRsBatch()
        {
            ExcitingCaseCDRs = new Dictionary<long, string>();
        }
    }

    #region Arguments
    public class UpdateCorrelatedCDRsInput
    {
        public MemoryQueue<UpdateCorrelatedCDRsBatch> InputCaseCDRsQueue { get; set; }
        public MemoryQueue<CDRCorrelationBatch> InsertedCorrelatedCDRs { get; set; }
    }
    public class UpdateCorrelatedCDRsOutput
    {

    }
    #endregion

    public class UpdateCorrelatedCDRs : DependentAsyncActivity<UpdateCorrelatedCDRsInput, UpdateCorrelatedCDRsOutput>
    {
        [RequiredArgument]
        public InOutArgument<MemoryQueue<UpdateCorrelatedCDRsBatch>> UpdateCorrelatedCDRsInput { get; set; }

        [RequiredArgument]
        public InOutArgument<MemoryQueue<CDRCorrelationBatch>> InsertedCorrelatedCDRs { get; set; }

        protected override UpdateCorrelatedCDRsInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new UpdateCorrelatedCDRsInput
            {
                InputCaseCDRsQueue = this.UpdateCorrelatedCDRsInput.Get(context),
                InsertedCorrelatedCDRs = this.InsertedCorrelatedCDRs.Get(context),
            };
        }

        protected override UpdateCorrelatedCDRsOutput DoWorkWithResult(UpdateCorrelatedCDRsInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            Guid dataRecordStorage = new Guid("F5E8B48B-70E0-46B8-BA69-9A4C37E6A520");
            var recordStorageDataManager = new DataRecordStorageManager().GetStorageDataManager(dataRecordStorage);

            DoWhilePreviousRunning(previousActivityStatus, handle, () =>
            {
                bool hasItems = false;
                do
                {
                    if (inputArgument.InputCaseCDRsQueue != null && inputArgument.InputCaseCDRsQueue.Count > 0 && inputArgument.InsertedCorrelatedCDRs != null && inputArgument.InsertedCorrelatedCDRs.Count > 0)
                    {
                        hasItems = inputArgument.InputCaseCDRsQueue.TryDequeue((listOfCaseCDRs) =>
                        {
                            DateTime batchStartTime = DateTime.Now;
                            if (listOfCaseCDRs.ExcitingCaseCDRs != null && listOfCaseCDRs.ExcitingCaseCDRs.Count > 0)
                            {
                                bool hasItems2 = false;
                                hasItems2 = inputArgument.InsertedCorrelatedCDRs.TryDequeue((listOfUpdatedCorrolatedCDRs) =>
                                {
                                    if (listOfUpdatedCorrolatedCDRs.OutputRecordsToInsert != null && listOfUpdatedCorrolatedCDRs.OutputRecordsToInsert.Count > 0)
                                    {
                                        Entities.CDRCorrelationBatch correlationBatch = new Entities.CDRCorrelationBatch();
                                        foreach (var correlatedCDR in listOfUpdatedCorrolatedCDRs.OutputRecordsToInsert)
                                        {
                                            var index = listOfCaseCDRs.ExcitingCaseCDRs.FirstOrDefault(x => x.Value == correlatedCDR.ReceivedCallingNumber).Key;
                                            if (index != 0)
                                                correlatedCDR.CaseId = index;
                                            
                                            var dataRecordTypeManager = new Vanrise.GenericData.Business.DataRecordTypeManager();
                                            Type cdrRuntimeType = dataRecordTypeManager.GetDataRecordRuntimeType("TCAnal_CorrelatedCDR");

                                            dynamic runtimeCDR = Activator.CreateInstance(cdrRuntimeType) as dynamic;
                                            runtimeCDR.ID = correlatedCDR.ID;
                                            runtimeCDR.AttemptDateTime = correlatedCDR.AttemptDateTime;
                                            runtimeCDR.DurationInSeconds = correlatedCDR.DurationInSeconds;
                                            runtimeCDR.CalledNumber = correlatedCDR.CalledNumber;
                                            runtimeCDR.GeneratedCallingNumber = correlatedCDR.GeneratedCallingNumber;
                                            runtimeCDR.ReceivedCallingNumber = correlatedCDR.ReceivedCallingNumber;
                                            runtimeCDR.OperatorID = correlatedCDR.OperatorID;
                                            runtimeCDR.CaseId = correlatedCDR.CaseId;
                                            runtimeCDR.OrigCallingNumber = correlatedCDR.OrigCallingNumber;
                                            runtimeCDR.OrigCalledNumber = correlatedCDR.OrigCalledNumber;
                                            runtimeCDR.ReceivedCallingNumberType = (int?)correlatedCDR.ReceivedCallingNumberType;
                                            runtimeCDR.ReceivedCallingNumberOperatorID = correlatedCDR.ReceivedCallingNumberOperatorID;
                                            correlationBatch.OutputRecordsToInsert.Add(runtimeCDR);
                                        }

                                        List<string> fieldsToJoin = new List<string>();
                                        List<string> fieldsToUpdate = new List<string>();
                                        fieldsToJoin.Add("ID");
                                        fieldsToUpdate.Add("CaseId");

                                        recordStorageDataManager.UpdateRecords(correlationBatch.OutputRecordsToInsert, fieldsToJoin, fieldsToUpdate);

                                        double elapsedTime = Math.Round((DateTime.Now - batchStartTime).TotalSeconds);
                                        handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Information, "Update 'CaseId' field in CorrelatedCDRs Table is done. Events Count: {0}.  ElapsedTime: {1} (s)",
                                            correlationBatch.OutputRecordsToInsert.Count, elapsedTime.ToString());
                                    }
                                });
                            }
                        });
                    }
                } while (!ShouldStop(handle) && hasItems);
            });
            handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Information, "Update 'CaseId' field in CorrelatedCDRs Table is done.");
            return new UpdateCorrelatedCDRsOutput();
        }

        protected override void OnWorkComplete(AsyncCodeActivityContext context, UpdateCorrelatedCDRsOutput result)
        {
        }
    }
}
