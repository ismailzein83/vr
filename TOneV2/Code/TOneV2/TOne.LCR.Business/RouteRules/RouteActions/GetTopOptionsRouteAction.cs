using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.LCR.Entities;

namespace TOne.LCR.Business
{
    public class GetTopOptionsRouteAction : BaseRouteAction
    {
        public override Type GetActionDataType()
        {
            return null;
        }

        public override RouteActionResult Execute(IRouteBuildContext context, object actionData)
        {
            context.ExecuteOptionsActions(5, false);
            return null;
        }
    }
}
