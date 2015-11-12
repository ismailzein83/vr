using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Vanrise.Queueing;
using Vanrise.BusinessProcess;

namespace TOne.WhS.Routing.BP.Activities
{

    public class ApplyCodesToCodeSaleZoneTableInput
    {
        public BaseQueue<Object> InputQueue { get; set; }

        public int RoutingDatabaseId { get; set; }
    }


    public sealed class ApplyCodesToCodeSaleZoneTable : DependentAsyncActivity<ApplyCodesToCodeSaleZoneTableInput>
    {

        protected override void DoWork(ApplyCodesToCodeSaleZoneTableInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            throw new NotImplementedException();
        }

        protected override ApplyCodesToCodeSaleZoneTableInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            throw new NotImplementedException();
        }
    }
}
