using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using TOne.WhS.Routing.Entities;

namespace TOne.WhS.Routing.BP.Activities
{
     public sealed class LoadActiveCustomerInfos : CodeActivity
    {
         [RequiredArgument]
         public OutArgument<List<RoutingCustomerInfo>> ActiveRoutingCustomerInfos{ get; set; }
        protected override void Execute(CodeActivityContext context)
        {
            throw new NotImplementedException();
        }
    }
}
