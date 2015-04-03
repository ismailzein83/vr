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
            BlockSuppliersRouteActionData blockSupplierActionData = actionData as BlockSuppliersRouteActionData;
            var route = context.Route;
            

            if (blockSupplierActionData == null)
                return InvalidActionData("actionData is null or it is not of type BlockSuppliersRouteActionData");
            if (blockSupplierActionData.BlockedOptions == null)
                return InvalidActionData("BlockSuppliersRouteActionData.BlockedOptions is null");
            List<RouteSupplierOption> options = null;
            if (context.Route.Options != null)
                options = context.Route.Options.SupplierOptions;
            RouteActionResult rslt = new RouteActionResult();
            if (options != null)
            {
                foreach (RouteSupplierOption option in options)
                {
                    if (option.Setting == null || !option.Setting.IsBlocked)
                    {
                        if (blockSupplierActionData.BlockedOptions.Contains(option.SupplierId))
                        {
                            if (option.Setting == null)
                                option.Setting = new OptionSetting();
                            option.Setting.IsBlocked = true;
                            rslt.NextActionData = typeof(GetTopOptionsRouteAction);
                        }
                    }
                }
            }

            return rslt;
        }
    }
}
