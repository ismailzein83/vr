using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Vanrise.BusinessProcess;
using Vanrise.Reprocess.Entities;
using Vanrise.Queueing;

namespace Vanrise.Reprocess.BP.Activities
{
    #region Arguments Classes

    public class LoadDataToReprocessInput
    {
        public int RecordStorageId { get; set; }

        public DateTime FromTime { get; set; }

        public DateTime ToTime { get; set; }

        public StageManager StageManager { get; set; }

        public List<string> OutputStageNames { get; set; }
    }

    public class LoadDataToReprocessOutput
    {
    }

    #endregion

    public sealed class LoadDataToReprocess : BaseAsyncActivity<LoadDataToReprocessInput, LoadDataToReprocessOutput>
    {
        [RequiredArgument]
        public InArgument<int> RecordStorageId { get; set; }

        [RequiredArgument]
        public InArgument<DateTime> FromTime { get; set; }

        [RequiredArgument]
        public InArgument<DateTime> ToTime { get; set; }

        [RequiredArgument]
        public InArgument<StageManager> StageManager { get; set; }

        [RequiredArgument]
        public InArgument<List<string>> OutputStageNames { get; set; }

        protected override LoadDataToReprocessInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new LoadDataToReprocessInput
            {
                RecordStorageId = this.RecordStorageId.Get(context),
                FromTime = this.FromTime.Get(context),
                ToTime = this.ToTime.Get(context),
                StageManager = this.StageManager.Get(context),
                OutputStageNames = this.OutputStageNames.Get(context)
            };
        }

        protected override LoadDataToReprocessOutput DoWorkWithResult(LoadDataToReprocessInput inputArgument, AsyncActivityHandle handle)
        {
            if (inputArgument.OutputStageNames ==  null || inputArgument.OutputStageNames.Count == 0)
                throw new Exception("No output stages!");
            
           
            throw new NotImplementedException();
        }

        protected override void OnWorkComplete(AsyncCodeActivityContext context, LoadDataToReprocessOutput result)
        {
            throw new NotImplementedException();
        }
    }
}
