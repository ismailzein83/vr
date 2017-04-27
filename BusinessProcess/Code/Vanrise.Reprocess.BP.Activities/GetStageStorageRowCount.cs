using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Vanrise.BusinessProcess;
using Vanrise.Reprocess.Entities;
using Vanrise.Common;

namespace Vanrise.Reprocess.BP.Activities
{

    public sealed class GetStageStorageRowCount : BaseCodeActivity
    {
        [RequiredArgument]
        public InArgument<IReprocessStageActivator> StageActivator { get; set; }

        [RequiredArgument]
        public InArgument<string> StageName { get; set; }

        [RequiredArgument]
        public InArgument<Dictionary<string, object>> InitializationOutputByStage { get; set; }

        [RequiredArgument]
        public OutArgument<int?> StorageRowCount { get; set; }

        protected override void VRExecute(IBaseCodeActivityContext context)
        {
            IReprocessStageActivator stageActivator = this.StageActivator.Get(context.ActivityContext);
            string stageName = this.StageName.Get(context.ActivityContext);
            Dictionary<string, object> initializationOutputByStage = this.InitializationOutputByStage.Get(context.ActivityContext);

            object initializationStageOutput = initializationOutputByStage.GetRecord(stageName);

            var initializatingContext = new ReprocessStageActivatorGetStorageRowCountContext(initializationStageOutput);
            int? rowCount = stageActivator.GetStorageRowCount(initializatingContext);

            this.StorageRowCount.Set(context.ActivityContext, rowCount);
        }

        private class ReprocessStageActivatorGetStorageRowCountContext : IReprocessStageActivatorGetStorageRowCountContext
        {
            object _initializationStageOutput;

            public ReprocessStageActivatorGetStorageRowCountContext(object initializationStageOutput)
            {
                _initializationStageOutput = initializationStageOutput;
            }

            public object InitializationStageOutput { get { return _initializationStageOutput; } }
        }
    }
}
