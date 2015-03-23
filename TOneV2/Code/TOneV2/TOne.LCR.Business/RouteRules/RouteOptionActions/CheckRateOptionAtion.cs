using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.LCR.Entities;

namespace TOne.LCR.Business
{
    class CheckRateOptionAtion : BaseRouteOptionAction
    {
        public override RouteOptionActionResult Execute(IRouteOptionBuildContext context, object actionData)
        {
            if (Decimal.Compare(context.Route.Rate, context.RouteOption.Rate) < 0)
                return new RouteOptionActionResult { RemoveOption = true };
            else
                return null;
        }
    }
}
