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
        public BaseQueue<PreparedCdrBatch> OutputQueue { get; set; }
    }
    public sealed class ProcessMediationRecordsBatch : DependentAsyncActivity<ProcessMediationRecordsBatchInput>
    {
        [RequiredArgument]
        public InArgument<MediationDefinition> MediationDefinition { get; set; }

        [RequiredArgument]
        public InArgument<BaseQueue<MediationRecordBatch>> MediationRecordsBatch { get; set; }

        [RequiredArgument]
        public InArgument<DataTransformationDefinition> DataTransformationDefinition { get; set; }

        [RequiredArgument]
        public InOutArgument<BaseQueue<PreparedCdrBatch>> OutputQueue { get; set; }
        protected override void DoWork(ProcessMediationRecordsBatchInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            PreparedCdrBatch cdrBatch = new PreparedCdrBatch();
            MediationDefinitionManager mediationDefinitionManager = new MediationDefinitionManager();
            DataTransformer dataTransformer = new DataTransformer();
                       
            var cookedRecordType = inputArgument.DataTransformationDefinition.RecordTypes.FindRecord(c => c.RecordName == inputArgument.MediationDefinition.CookedFromParsedSettings.CookedRecordName);

            DoWhilePreviousRunning(previousActivityStatus, handle, () =>
            {
                bool hasItems = false;

                do
                {
                    hasItems = inputArgument.MediationRecordsBatch.TryDequeue((mediationRecordBatch) =>
                    {
                        var output = dataTransformer.ExecuteDataTransformation(inputArgument.MediationDefinition.CookedFromParsedSettings.TransformationDefinitionId, (context) =>
                        {
                            var details = mediationRecordBatch.MediationRecords.Select(m => m.EventDetails).ToList();
                            context.SetRecordValue("parsedCDRs", details);
                        });
                        if (cookedRecordType.IsArray)
                            cdrBatch.Cdrs.AddRange(output.GetRecordValue(cookedRecordType.RecordName) as List<dynamic>);
                        else
                            cdrBatch.Cdrs.Add(output.GetRecordValue(cookedRecordType.RecordName));

                        if (cdrBatch.Cdrs.Count > 100)
                        {
                            inputArgument.OutputQueue.Enqueue(cdrBatch);
                            cdrBatch = new PreparedCdrBatch();
                        }
                    });
                } while (!ShouldStop(handle) && hasItems);

                if (cdrBatch.Cdrs.Count > 0)
                    inputArgument.OutputQueue.Enqueue(cdrBatch);
            });

        }

        protected override ProcessMediationRecordsBatchInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new ProcessMediationRecordsBatchInput()
            {
                MediationDefinition = this.MediationDefinition.Get(context),
                MediationRecordsBatch = this.MediationRecordsBatch.Get(context),
                DataTransformationDefinition = this.DataTransformationDefinition.Get(context),
                OutputQueue = this.OutputQueue.Get(context)
            };
        }

        protected override void OnBeforeExecute(AsyncCodeActivityContext context, AsyncActivityHandle handle)
        {
            if (this.MediationRecordsBatch.Get(context) == null)
                this.MediationRecordsBatch.Set(context, new MemoryQueue<MediationRecord>());
            if (this.DataTransformationDefinition.Get(context) == null)
                this.DataTransformationDefinition.Set(context, new DataTransformationDefinition());
            if (this.OutputQueue.Get(context) == null)
                this.OutputQueue.Set(context, new MemoryQueue<PreparedCdrBatch>());
            base.OnBeforeExecute(context, handle);
        }
    }
}
