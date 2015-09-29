using System;
using System.Activities;
using Vanrise.BusinessProcess;
using Vanrise.Fzero.CDRImport.Data;
using Vanrise.Queueing;

namespace Vanrise.Fzero.CDRImport.BP.Activities
{
    public class ApplyUnifiedCDRsToDBInput
    {
        public BaseQueue<Object> InputQueue { get; set; }
    }

    public sealed class ApplyUnifiedCDRsToDB : DependentAsyncActivity<ApplyUnifiedCDRsToDBInput>
    {
        //[RequiredArgument]
        //public InArgument<BaseQueue<Object>> InputQueue { get; set; }

        //protected override void DoWork(ApplyUnifiedCDRsToDBInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        //{
        //    INumberProfileDataManager dataManager = FraudDataManagerFactory.GetDataManager<INumberProfileDataManager>();

        //    DoWhilePreviousRunning(previousActivityStatus, handle, () =>
        //    {
        //        bool hasItems = false;
        //        do
        //        {
        //            hasItems = inputArgument.InputQueue.TryDequeue(
        //                (preparedNumberProfiles) =>
        //                {
        //                    dataManager.ApplyUnifiedCDRsToDB(preparedNumberProfiles);
        //                });
        //        } while (!ShouldStop(handle) && hasItems);
        //    }
        //        );
        //}

        //protected override ApplyUnifiedCDRsToDBInput GetInputArgument2(AsyncCodeActivityContext context)
        //{
        //    return new ApplyUnifiedCDRsToDBInput()
        //    {
        //        InputQueue = this.InputQueue.Get(context)
        //    };
        //}
        protected override void DoWork(ApplyUnifiedCDRsToDBInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            throw new NotImplementedException();
        }

        protected override ApplyUnifiedCDRsToDBInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            throw new NotImplementedException();
        }
    }
}
