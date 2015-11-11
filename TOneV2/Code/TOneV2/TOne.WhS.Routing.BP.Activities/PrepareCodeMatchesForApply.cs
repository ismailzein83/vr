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
        public BaseQueue<CodeMatchesBatch> InputQueue { get; set; }
        public BaseQueue<Object> OutputQueue { get; set; }
    }


    public sealed class PrepareCodeMatchesForApply : DependentAsyncActivity<PrepareCodeMatchesForApplyInput>
    {
        [RequiredArgument]
        public InArgument<BaseQueue<CodeMatchesBatch>> InputQueue { get; set; }
        
        [RequiredArgument]
        public InOutArgument<BaseQueue<Object>> OutputQueue { get; set; }

        protected override void DoWork(PrepareCodeMatchesForApplyInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            ICodeMatchesDataManager dataManager = RoutingDataManagerFactory.GetDataManager<ICodeMatchesDataManager>();
            PrepareDataForDBApply(previousActivityStatus, handle, dataManager, inputArgument.InputQueue, inputArgument.OutputQueue, CodeMatchesBatch => CodeMatchesBatch.CodeMatches);
        }

        protected override PrepareCodeMatchesForApplyInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            throw new NotImplementedException();
        }
    }
}
