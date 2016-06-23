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
    public class ProcessStagingRecordsBatchInput
    {
        public MediationDefinition MediationDefinition { get; set; }
        public BaseQueue<StoreStagingRecordBatch> StoreStagingRecords { get; set; }
    }
    public sealed class ProcessStagingRecordsBatch : DependentAsyncActivity<ProcessStagingRecordsBatchInput>
    {
        [RequiredArgument]
        public InArgument<MediationDefinition> MediationDefinition { get; set; }
        [RequiredArgument]
        public InArgument<BaseQueue<StoreStagingRecordBatch>> StoreStagingRecords { get; set; }


        protected override void DoWork(ProcessStagingRecordsBatchInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            MediationDefinitionManager mediationDefinitionManager = new MediationDefinitionManager();
            DataTransformationDefinitionManager dataTransformationManager = new DataTransformationDefinitionManager();
            DataTransformer dataTransformer = new DataTransformer();

            DataTransformationDefinition transformationDefinition = dataTransformationManager.GetDataTransformationDefinition(inputArgument.MediationDefinition.CookedFromParsedSettings.TransformationDefinitionId);

            var cookedRecordType = transformationDefinition.RecordTypes.FindRecord(c => c.RecordName == inputArgument.MediationDefinition.CookedFromParsedSettings.CookedRecordName);



            DoWhilePreviousRunning(previousActivityStatus, handle, () =>
            {
                bool hasItems = false;

                do
                {
                    hasItems = inputArgument.StoreStagingRecords.TryDequeue((storeStagingRecordBatch) =>
                    {


                        var output = dataTransformer.ExecuteDataTransformation(inputArgument.MediationDefinition.CookedFromParsedSettings.TransformationDefinitionId, (context) =>
                        {
                            context.SetRecordValue(inputArgument.MediationDefinition.CookedFromParsedSettings.ParsedRecordName, null);
                        });
                    });
                } while (!ShouldStop(handle) && hasItems);
            });

        }

        protected override ProcessStagingRecordsBatchInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new ProcessStagingRecordsBatchInput()
            {
                MediationDefinition = this.MediationDefinition.Get(context),
                StoreStagingRecords = this.StoreStagingRecords.Get(context)
            };
        }

        protected override void OnBeforeExecute(AsyncCodeActivityContext context, AsyncActivityHandle handle)
        {
            if (this.StoreStagingRecords.Get(context) == null)
                this.StoreStagingRecords.Set(context, new MemoryQueue<StoreStagingRecord>());
            base.OnBeforeExecute(context, handle);
        }
    }
}
