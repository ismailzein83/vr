using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.LCR.Entities;

namespace TOne.LCR.Business
{
    public class OverrideRouteAction : BaseRouteAction
    {
        public override Type GetActionDataType()
        {
            return typeof(OverrideRouteActionData);
        }

        public override RouteActionResult Execute(IRouteBuildContext context, object actionData)
        {
            OverrideRouteActionData overrideActionData = actionData as OverrideRouteActionData;
            
            if (overrideActionData == null)
                return InvalidActionData("actionData is null or it is not of type OverrideRouteActionData");
            if (overrideActionData.Options == null)
                return InvalidActionData("overrideActionData.Options is null");

            RouteActionResult rslt = new RouteActionResult();

            BuildRouteFromOverrideOptions(context, overrideActionData.Options, false);
            context.ExecuteOptionsActions(false, null);
            if(context.Route.Options.SupplierOptions.Count == 0)
            {
                switch(overrideActionData.NoOptionAction)
                {
                    case OverrideRouteNoOptionAction.None:
                        break;
                    case OverrideRouteNoOptionAction.SwitchToLCR:
                        rslt.NextActionType = typeof(PriorityRouteActionData);
                        break;
                    case OverrideRouteNoOptionAction.BackupRoute:
                        if (overrideActionData.BackupOptions != null)
                        {
                            BuildRouteFromOverrideOptions(context, overrideActionData.BackupOptions, true);
                            context.ExecuteOptionsActions(false, null);
                        }
                        break;
                }
            }
            return rslt;
        }

        void BuildRouteFromOverrideOptions(IRouteBuildContext context, List<OverrideOption> overrideOptions, bool ignoreRateCheck)
        {
            var route = context.Route;
            route.Options = new RouteOptions();
            route.Options.SupplierOptions = new List<RouteSupplierOption>();
            foreach (var overrideOption in overrideOptions)
            {
                RouteSupplierOption routeOption;
                if (context.TryBuildSupplierOption(overrideOption.SupplierId, overrideOption.Percentage, out routeOption))
                {
                    if(ignoreRateCheck)
                    {
                        if (routeOption.Setting == null)
                            routeOption.Setting = new OptionSetting();
                        routeOption.Setting.IgnoreRateCheck = true;
                    }
                    route.Options.SupplierOptions.Add(routeOption);
                }
            }
        }
    }
}