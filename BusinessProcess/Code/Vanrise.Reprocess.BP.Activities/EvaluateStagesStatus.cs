using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Vanrise.BusinessProcess;

namespace Vanrise.Reprocess.BP.Activities
{

    public sealed class EvaluateStagesStatus : CodeActivity
    {
        [RequiredArgument]
        public InArgument<StageManager> StageManager { get; set; }

        [RequiredArgument]
        public InArgument<AsyncActivityStatus> LoadDataToReprocessStatus { get; set; }


        protected override void Execute(CodeActivityContext context)
        {
            var stageManager = this.StageManager.Get(context);
            if (stageManager == null)
                throw new NullReferenceException("stageManager");
            stageManager.EvaluateStagesStatus(this.LoadDataToReprocessStatus.Get(context));
        }
    }
}
