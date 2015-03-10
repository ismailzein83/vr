using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Vanrise.Queueing;
using TOne.LCR.Entities;
using Vanrise.BusinessProcess;

namespace TOne.LCRProcess.Activities
{
    #region Argument Classes
    public class PrepareRoutesForDBApplyInput
    {
        public int RoutingDatabaseId { get; set; }

        public BaseQueue<RouteDetailBatch> InputQueue { get; set; }

        public BaseQueue<Object> OutputQueue { get; set; }
    }

    #endregion

    public sealed class PrepareRoutesForDBApply : DependentAsyncActivity<PrepareRoutesForDBApplyInput>
    {
        [RequiredArgument]
        public InArgument<int> RoutingDatabaseId { get; set; }

        [RequiredArgument]
        public InArgument<BaseQueue<RouteDetailBatch>> InputQueue { get; set; }

        [RequiredArgument]
        public InArgument<BaseQueue<Object>> OutputQueue { get; set; }

        protected override PrepareRoutesForDBApplyInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new PrepareRoutesForDBApplyInput
            {
                RoutingDatabaseId = this.RoutingDatabaseId.Get(context),
                InputQueue = this.InputQueue.Get(context),
                OutputQueue = this.OutputQueue.Get(context)
            };
        }


        protected override void DoWork(PrepareRoutesForDBApplyInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            throw new NotImplementedException();
        }
    }
}
