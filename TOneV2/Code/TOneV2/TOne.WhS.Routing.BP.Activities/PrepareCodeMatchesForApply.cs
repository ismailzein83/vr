using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Vanrise.Queueing;
using TOne.WhS.Routing.Entities;
using Vanrise.BusinessProcess;
using TOne.WhS.Routing.Data;

namespace TOne.WhS.Routing.BP.Activities
{
    public class PrepareCodeMatchesForApplyInput
    {
        public BaseQueue<CodeMatchesBatch> InputQueue_1 { get; set; }

        public BaseQueue<CodeMatchesBatch> InputQueue_2 { get; set; }

        public BaseQueue<Object> OutputQueue_1 { get; set; }

        public BaseQueue<Object> OutputQueue_2 { get; set; }
    }


    public sealed class PrepareCodeMatchesForApply : DependentAsyncActivity<PrepareCodeMatchesForApplyInput>
    {
        [RequiredArgument]
        public InArgument<BaseQueue<CodeMatchesBatch>> InputQueue_1 { get; set; }

        [RequiredArgument]
        public InArgument<BaseQueue<CodeMatchesBatch>> InputQueue_2 { get; set; }
        
        [RequiredArgument]
        public InOutArgument<BaseQueue<Object>> OutputQueue_1 { get; set; }

        [RequiredArgument]
        public InOutArgument<BaseQueue<Object>> OutputQueue_2 { get; set; }

        protected override void DoWork(PrepareCodeMatchesForApplyInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            ICodeMatchesDataManager codeMatchesDataManager = RoutingDataManagerFactory.GetDataManager<ICodeMatchesDataManager>();
            ICodeSaleZoneDataManager CodeSaleZoneDataManager = RoutingDataManagerFactory.GetDataManager<ICodeSaleZoneDataManager>();
            PrepareDataForDBApply(previousActivityStatus, handle, codeMatchesDataManager, inputArgument.InputQueue_1, inputArgument.OutputQueue_1, CodeMatchesBatch => CodeMatchesBatch.CodeMatches);
            PrepareDataForDBApply(previousActivityStatus, handle, CodeSaleZoneDataManager, inputArgument.InputQueue_2, inputArgument.OutputQueue_2, CodeMatchesBatch => CodeMatchesBatch.CodeMatches);
        }

        protected override PrepareCodeMatchesForApplyInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new PrepareCodeMatchesForApplyInput
            {
                InputQueue_1 = this.InputQueue_1.Get(context),
                InputQueue_2 = this.InputQueue_2.Get(context),
                OutputQueue_1 = this.OutputQueue_1.Get(context),
                OutputQueue_2 = this.OutputQueue_2.Get(context)
            };
        }

        protected override void OnBeforeExecute(AsyncCodeActivityContext context, AsyncActivityHandle handle)
        {
            if (this.OutputQueue_1.Get(context) == null)
                this.OutputQueue_1.Set(context, new MemoryQueue<Object>());

            if (this.OutputQueue_2.Get(context) == null)
                this.OutputQueue_2.Set(context, new MemoryQueue<Object>());

            base.OnBeforeExecute(context, handle);
        }
    }
}
