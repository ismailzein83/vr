using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.LCR.Entities;

namespace TOne.LCR.Business
{
    public class BlockSuppliersRouteAction : BaseRouteAction
    {
        public override Type GetActionDataType()
        {
            return typeof(BlockSuppliersRouteActionData);
        }

        public override RouteActionResult Execute(IRouteBuildContext context, object actionData)
        {
            List<RouteSupplierOption> options = null;
            if (context.Route.Options != null)
                options = context.Route.Options.SupplierOptions;

            if(options != null)
            {

            }

            return base.Execute(context, actionData);
        }
    }
}
