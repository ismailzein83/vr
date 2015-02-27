using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.LCR.Entities;

namespace TOne.LCR.Business
{
    public class BuildLCRRouteAction : BaseRouteAction
    {
        public override RouteActionResult Execute(IRouteBuildContext context, object actionData)
        {
            context.BuildLCR();
            return null;
        }
    }
}
