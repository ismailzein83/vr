using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Mediation.Generic.Entities;
using Vanrise.Queueing;
using Vanrise.BusinessProcess;
using Mediation.Generic.Business;
using Vanrise.GenericData.Transformation;
using Vanrise.GenericData.Transformation.Entities;
using Vanrise.Common;
using Mediation.Generic.Data;
using Vanrise.Entities;

namespace Mediation.Generic.BP.Activities
{
    public class ProcessMediationRecordsBatchInput
    {
        public MediationDefinition MediationDefinition { get; set; }
        public BaseQueue<MediationRecordBatch> MediationRecordsBatch { get; set; }
        public DataTransformationDefinition DataTransformationDefinition { get; set; }
        public List<OutputHandlerExecutionEntity> OutputHandlerExecutionEntities { get; set; }
    }
    public sealed class ProcessMediationRecordsBatch : DependentAsyncActivity<ProcessMediationRecordsBatchInput>
    {
        [RequiredArgument]
        public InArgument<MediationDefinition> MediationDefinition { get; set; }

        [RequiredArgument]
        public InArgument<BaseQueue<MediationRecordBatch>> MediationRecordsBatch { get; set; }

        [RequiredArgument]
        public InArgument<DataTransformationDefinition> DataTransformationDefinition { get; set; }

        public InArgument<List<OutputHandlerExecutionEntity>> OutputHandlerExecutionEntities { get; set; }

        protected override void DoWork(ProcessMediationRecordsBatchInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Information, "Start Processing Mediation Records");
            MediationProcessWorkflowMainContext workflowMainContext = new MediationProcessWorkflowMainContext(inputArgument.MediationDefinition.MediationDefinitionId);
            List<ProcessHandlerItem> processHandlers = new List<ProcessHandlerItem>();
            var batchProxy = new PreparedRecordsBatchProxy { EventIdsToDelete = new List<long>() };
            foreach (var outputHandler in inputArgument.OutputHandlerExecutionEntities)
            {
                ProcessHandlerItem item = new ProcessHandlerItem
                {
                    RecordName = outputHandler.OutputHandler.OutputRecordName,
                    InputQueue = outputHandler.InputQueue,
                    CdrBatch = new PreparedRecordsBatch { Proxy = batchProxy }
                };
                processHandlers.Add(item);
            }

            MediationDefinitionManager mediationDefinitionManager = new MediationDefinitionManager();
            DataTransformer dataTransformer = new DataTransformer();
            bool needsContextRecord = inputArgument.DataTransformationDefinition.RecordTypes.Any(c => c.RecordName == "context");
            bool needssessionIdRecord = inputArgument.DataTransformationDefinition.RecordTypes.Any(c => c.RecordName == "sessionId");

            DoWhilePreviousRunning(previousActivityStatus, handle, () =>
            {
                bool hasItems = false;
                do
                {
                    hasItems = inputArgument.MediationRecordsBatch.TryDequeue((sessionMediationRecordBatch) =>
                    {
                        batchProxy.LastCommittedId = sessionMediationRecordBatch.LastCommittedId;
                        var processingContext = new MediationProcessContext(workflowMainContext);
                        var transformationOutput = dataTransformer.ExecuteDataTransformation(inputArgument.MediationDefinition.ParsedTransformationSettings.TransformationDefinitionId, (context) =>
                        {
                            if (needsContextRecord)
                                context.SetRecordValue("context", processingContext);

                            var details = sessionMediationRecordBatch.MediationRecords.Select(m => m.EventDetails).ToList();

                            context.SetRecordValue(inputArgument.MediationDefinition.ParsedTransformationSettings.ParsedRecordName, details);

                            if (needssessionIdRecord)
                                context.SetRecordValue("sessionId", details.Select(itm => itm.MultiLegSessionId).First());

                        });

                        if (!processingContext.NeedsMoreMediationRecords)
                        {
                            //batchProxy.SessionIdToDelete.Add(sessionMediationRecordBatch.SessionId);
                            batchProxy.EventIdsToDelete.AddRange(sessionMediationRecordBatch.MediationRecords.Select(m => m.EventId));
                            foreach (var processHandler in processHandlers)
                            {
                                UpdateProcessHandlers(inputArgument, transformationOutput, processHandler);
                            }
                            //if (batchProxy.SessionIdToDelete.Count > 10000)
                            if (batchProxy.EventIdsToDelete.Count > 20000)
                            {
                                SetNbOfHandlersToExecute(inputArgument.MediationDefinition.MediationDefinitionId, processHandlers, batchProxy, handle);
                                batchProxy = new PreparedRecordsBatchProxy { EventIdsToDelete = new List<long>(), LastCommittedId = sessionMediationRecordBatch.LastCommittedId };
                                foreach (var processHandler in processHandlers)
                                {
                                    if (processHandler.CdrBatch.BatchRecords.Count > 0)
                                        processHandler.InputQueue.Enqueue(processHandler.CdrBatch);
                                    processHandler.CdrBatch = new PreparedRecordsBatch { Proxy = batchProxy };
                                }
                            }
                        }
                    });
                } while (!ShouldStop(handle) && hasItems);
            });

            SetNbOfHandlersToExecute(inputArgument.MediationDefinition.MediationDefinitionId, processHandlers, batchProxy, handle);
            foreach (var processHandler in processHandlers)
            {
                if (processHandler.CdrBatch.BatchRecords.Count > 0)
                    processHandler.InputQueue.Enqueue(processHandler.CdrBatch);
            }
            handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Information, "End Processing Mediation Records");
        }

        void SetNbOfHandlersToExecute(Guid mediationDefinitionId, List<ProcessHandlerItem> processHandlers, PreparedRecordsBatchProxy batchProxy, AsyncActivityHandle handle)
        {
            batchProxy.NbOfHandlersToExecute = processHandlers.Count(itm => itm.CdrBatch.BatchRecords.Count > 0);
            if (batchProxy.NbOfHandlersToExecute == 0)
            {

                IMediationRecordsDataManager dataManager = MediationGenericDataManagerFactory.GetDataManager<IMediationRecordsDataManager>();
                //dataManager.DeleteMediationRecordsBySessionIds(mediationDefinitionId, batchProxy.SessionIdToDelete, batchProxy.LastCommittedId);
                dataManager.DeleteMediationRecordsByEventIds(batchProxy.EventIdsToDelete);
                handle.SharedInstanceData.WriteBusinessTrackingMsg(LogEntryType.Information, "{0} Session Ids are deleted", batchProxy.EventIdsToDelete.Count);
            }
        }

        void UpdateProcessHandlers(ProcessMediationRecordsBatchInput inputArgument, DataTransformationExecutionOutput output, ProcessHandlerItem processHandler)
        {
            var recordType = inputArgument.DataTransformationDefinition.RecordTypes.FindRecord(c => c.RecordName == processHandler.RecordName);
            recordType.ThrowIfNull("recordType", "");
            if (recordType.IsArray)
            {
                var records = output.GetRecordValue(processHandler.RecordName) as List<dynamic>;
                if (records != null)
                    processHandler.CdrBatch.BatchRecords.AddRange(records);
            }
            else
            {
                dynamic record = output.GetRecordValue(processHandler.RecordName);
                if (record != null)
                    processHandler.CdrBatch.BatchRecords.Add(record);
            }
        }

        protected override ProcessMediationRecordsBatchInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new ProcessMediationRecordsBatchInput()
            {
                MediationDefinition = this.MediationDefinition.Get(context),
                MediationRecordsBatch = this.MediationRecordsBatch.Get(context),
                DataTransformationDefinition = this.DataTransformationDefinition.Get(context),
                OutputHandlerExecutionEntities = this.OutputHandlerExecutionEntities.Get(context)
            };
        }
    }

    class ProcessHandlerItem
    {
        public PreparedRecordsBatch CdrBatch { get; set; }
        public BaseQueue<PreparedRecordsBatch> InputQueue { get; set; }
        public string RecordName { get; set; }
    }


}
