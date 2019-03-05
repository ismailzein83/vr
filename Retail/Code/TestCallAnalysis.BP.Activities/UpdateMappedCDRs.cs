using System;
using System.Activities;
using System.Collections.Generic;
using TestCallAnalysis.Entities;
using Vanrise.BusinessProcess;
using Vanrise.Entities;
using Vanrise.GenericData.Business;
using Vanrise.Queueing;

namespace TestCallAnalysis.BP.Activities
{
    public class UpdatedMappedCDRs
    {
        public List<long> UpdatedIds { get; set; }

        public List<TCAnalMappedCDR> MappedCDRsToUpdate { get; set; }

        public UpdatedMappedCDRs()
        {
            UpdatedIds = new List<long>();
            MappedCDRsToUpdate = new List<TCAnalMappedCDR>();
        }
    }

    #region Arguments
    public class UpdateMappedCDRsInput
    {
        public MemoryQueue<UpdatedMappedCDRs> InputQueue { get; set; }
    }

    public class UpdateMappedCDRsOutput
    {

    }
    #endregion

    public class UpdateMappedCDRs : DependentAsyncActivity<UpdateMappedCDRsInput, UpdateMappedCDRsOutput>
    {
        [RequiredArgument]
        public InArgument<MemoryQueue<UpdatedMappedCDRs>> InputQueueIds { get; set; }

        protected override UpdateMappedCDRsInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new UpdateMappedCDRsInput
            {
                InputQueue = this.InputQueueIds.Get(context),
            };
        }

        protected override UpdateMappedCDRsOutput DoWorkWithResult(UpdateMappedCDRsInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            Guid dataRecordStorage = new Guid("58FCA073-8F5C-4A56-A4AF-025EB3B8BB60");
            var recordStorageDataManager = new DataRecordStorageManager().GetStorageDataManager(dataRecordStorage);

            DoWhilePreviousRunning(previousActivityStatus, handle, () =>
            {
                bool hasItems = false;
                do
                {
                    if (inputArgument.InputQueue != null && inputArgument.InputQueue.Count > 0)
                    {
                        hasItems = inputArgument.InputQueue.TryDequeue((updatedMappedCDRs) =>
                        {
                            DateTime batchStartTime = DateTime.Now;
                            if (updatedMappedCDRs.UpdatedIds != null && updatedMappedCDRs.UpdatedIds.Count > 0)
                            {
                                List<string> fieldsToJoin = new List<string>();
                                List<string> fieldsToUpdate = new List<string>();
                                fieldsToJoin.Add("ID");
                                fieldsToUpdate.Add("IsCorrelated");

                                Entities.CDRCorrelationBatch correlationBatch = new Entities.CDRCorrelationBatch();
                                var dataRecordTypeManager = new Vanrise.GenericData.Business.DataRecordTypeManager();
                                Type cdrRuntimeType = dataRecordTypeManager.GetDataRecordRuntimeType("TCAnal_MappedCDR");

                                foreach (var mappedCDR in updatedMappedCDRs.MappedCDRsToUpdate)
                                {
                                    if(updatedMappedCDRs.UpdatedIds.IndexOf(mappedCDR.ID) != -1)
                                    {
                                        dynamic runtimeCDR = Activator.CreateInstance(cdrRuntimeType) as dynamic;
                                        runtimeCDR.ID = mappedCDR.ID;
                                        runtimeCDR.AttemptDateTime = mappedCDR.AttemptDateTime;
                                        runtimeCDR.DurationInSeconds = mappedCDR.DurationInSeconds;
                                        runtimeCDR.CalledNumber = mappedCDR.CalledNumber;
                                        runtimeCDR.OperatorID = mappedCDR.OperatorID;
                                        runtimeCDR.OrigCallingNumber = mappedCDR.OrigCallingNumber;
                                        runtimeCDR.OrigCalledNumber = mappedCDR.OrigCalledNumber;
                                        runtimeCDR.DataSourceId = mappedCDR.DataSourceId;
                                        runtimeCDR.CallingNumber = mappedCDR.CallingNumber;
                                        runtimeCDR.CDRType = (int)mappedCDR.CDRType;
                                        runtimeCDR.CreatedDate = mappedCDR.CreatedDate;
                                        runtimeCDR.IsCorrelated = mappedCDR.IsCorrelated;
                                        runtimeCDR.CallingNumberType = mappedCDR.CallingNumberType;
                                        runtimeCDR.CalledNumberType = mappedCDR.CalledNumberType;
                                        correlationBatch.OutputRecordsToInsert.Add(runtimeCDR);
                                    }
                                }
                                recordStorageDataManager.UpdateRecords(correlationBatch.OutputRecordsToInsert, fieldsToJoin, fieldsToUpdate);
                              
                                double elapsedTime = Math.Round((DateTime.Now - batchStartTime).TotalSeconds);
                                handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Information, "Update 'IsCorrolated' field in MappedCDRs Table is done. Events Count: {0}.  ElapsedTime: {1} (s)",
                                    updatedMappedCDRs.UpdatedIds.Count, elapsedTime.ToString());
                            }
                        });
                    };
                } while (!ShouldStop(handle) && hasItems);
            });
            handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Information, "Update 'IsCorrolated' field in MappedCDRs Table is done.");
            return new UpdateMappedCDRsOutput();
        }

        protected override void OnWorkComplete(AsyncCodeActivityContext context, UpdateMappedCDRsOutput result)
        {
            throw new NotImplementedException();
        }
    }
}
