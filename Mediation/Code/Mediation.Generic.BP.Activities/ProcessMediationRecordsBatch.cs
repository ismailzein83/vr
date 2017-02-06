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
            List<ProcessHandlerItem> processHandlers = new List<ProcessHandlerItem>();
            foreach (var outputHandler in inputArgument.OutputHandlerExecutionEntities)
            {
                ProcessHandlerItem item = new ProcessHandlerItem
                {
                    RecordName = outputHandler.OutputHandler.OutputRecordName,
                    InputQueue = outputHandler.InputQueue,
                    CdrBatch = new PreparedCdrBatch()
                };
                processHandlers.Add(item);
            }
            MediationDefinitionManager mediationDefinitionManager = new MediationDefinitionManager();
            DataTransformer dataTransformer = new DataTransformer();

            DoWhilePreviousRunning(previousActivityStatus, handle, () =>
            {
                bool hasItems = false;
                do
                {
                    hasItems = inputArgument.MediationRecordsBatch.TryDequeue((mediationRecordBatch) =>
                    {
                        var transformationOutput = dataTransformer.ExecuteDataTransformation(inputArgument.MediationDefinition.ParsedTransformationSettings.TransformationDefinitionId, (context) =>
                        {
                            var details = mediationRecordBatch.MediationRecords.Select(m => m.EventDetails).ToList();
                            context.SetRecordValue(inputArgument.MediationDefinition.ParsedTransformationSettings.ParsedRecordName, details);
                        });

                        foreach (var processHandler in processHandlers)
                        {
                            UpdateProcessHandlers(inputArgument, transformationOutput, processHandler);
                        }
                    });
                } while (!ShouldStop(handle) && hasItems);

                foreach (var processHandler in processHandlers)
                {
                    if (processHandler.CdrBatch.Cdrs.Count > 0)
                        processHandler.InputQueue.Enqueue(processHandler.CdrBatch);
                }
            });
        }

        void UpdateProcessHandlers(ProcessMediationRecordsBatchInput inputArgument, DataTransformationExecutionOutput output, ProcessHandlerItem processHandler)
        {
            var recordType = inputArgument.DataTransformationDefinition.RecordTypes.FindRecord(c => c.RecordName == processHandler.RecordName);
            recordType.ThrowIfNull("recordType", "");
            if (recordType.IsArray)
                processHandler.CdrBatch.Cdrs.AddRange(output.GetRecordValue(processHandler.RecordName) as List<dynamic>);
            else
                processHandler.CdrBatch.Cdrs.Add(output.GetRecordValue(processHandler.RecordName));

            if (processHandler.CdrBatch.Cdrs.Count > 100)
            {
                processHandler.InputQueue.Enqueue(processHandler.CdrBatch);
                processHandler.CdrBatch = new PreparedCdrBatch();
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

        protected override void OnBeforeExecute(AsyncCodeActivityContext context, AsyncActivityHandle handle)
        {
            if (this.MediationRecordsBatch.Get(context) == null)
                this.MediationRecordsBatch.Set(context, new MemoryQueue<MediationRecord>());
            if (this.DataTransformationDefinition.Get(context) == null)
                this.DataTransformationDefinition.Set(context, new DataTransformationDefinition());
            base.OnBeforeExecute(context, handle);
        }
    }

    class ProcessHandlerItem
    {
        public PreparedCdrBatch CdrBatch { get; set; }
        public BaseQueue<PreparedCdrBatch> InputQueue { get; set; }
        public string RecordName { get; set; }
    }
}
