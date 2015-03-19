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
        public override bool IsImportant
        {
            get
            {
                return false;
            }
        }
        public override RouteOptionActionResult Execute(IRouteOptionBuildContext context, object actionData)
        {
            if (context.Route.Rate < context.RouteOption.Rate)
                return new RouteOptionActionResult { RemoveOption = true };
            else
                return null;
        }
    }
}
