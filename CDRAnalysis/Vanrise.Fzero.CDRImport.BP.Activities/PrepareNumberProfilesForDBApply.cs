using System;
using System.Activities;
using Vanrise.BusinessProcess;
using Vanrise.Fzero.CDRImport.Data;
using Vanrise.Fzero.CDRImport.Entities;
using Vanrise.Queueing;

namespace Vanrise.Fzero.CDRImport.BP.Activities
{
    #region Argument Classes
    public class PrepareNumberProfilesForDBApplyInput
    {
        //public BaseQueue<NumberProfileBatch> InputQueue { get; set; }

        public BaseQueue<Object> OutputQueue { get; set; }
    }

    #endregion
    public sealed class PrepareNumberProfilesForDBApply : DependentAsyncActivity<PrepareNumberProfilesForDBApplyInput>
    {
        //[RequiredArgument]
        //public InArgument<BaseQueue<NumberProfileBatch>> InputQueue { get; set; }

        //[RequiredArgument]
        //public InOutArgument<BaseQueue<Object>> OutputQueue { get; set; }

        protected override void DoWork(PrepareNumberProfilesForDBApplyInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            //INumberProfileDataManager dataManager = FraudDataManagerFactory.GetDataManager<INumberProfileDataManager>();
            //PrepareDataForDBApply(previousActivityStatus, handle, dataManager, inputArgument.InputQueue, inputArgument.OutputQueue, numberProfilesBatch => numberProfilesBatch.NumberProfiles);
        }

        protected override PrepareNumberProfilesForDBApplyInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new PrepareNumberProfilesForDBApplyInput
            {
                //InputQueue = this.InputQueue.Get(context),
                //OutputQueue = this.OutputQueue.Get(context)
            };
        }

        protected override void OnBeforeExecute(AsyncCodeActivityContext context, AsyncActivityHandle handle)
        {
            //if (this.OutputQueue.Get(context) == null)
            //    this.OutputQueue.Set(context, new MemoryQueue<object>());
            base.OnBeforeExecute(context, handle);
        }
    }
}
