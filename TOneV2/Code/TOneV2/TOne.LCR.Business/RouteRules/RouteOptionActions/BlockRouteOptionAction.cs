using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.LCR.Entities;

namespace TOne.LCR.Business
{
    public class BlockRouteOptionAction : BaseRouteOptionAction
    {
        public override Type GetActionDataType()
        {
            return typeof(BlockRouteOptionActionData);
        }
    
        public override RouteOptionActionResult Execute(IRouteOptionBuildContext context, object actionData)
        {
            BlockRouteOptionActionData blockOptionActionData = actionData as BlockRouteOptionActionData;

            if (blockOptionActionData == null)
                return InvalidActionData("actionData is null or it is not of type BlockRouteOptionActionData");

            if (blockOptionActionData.Customers != null && blockOptionActionData.Customers.Contains(context.Route.CustomerID))
            {
                return new RouteOptionActionResult
                {
                    BlockOption = true
                };
            }
            else
                return new RouteOptionActionResult
                {
                    DontMatchRoute = true
                };
        }
    }
}
