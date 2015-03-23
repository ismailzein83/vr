using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.LCR.Entities;

namespace TOne.LCR.Business
{
    public class PriorityRouteAction : BaseRouteAction
    {
        public override Type GetActionDataType()
        {
            return typeof(PriorityRouteActionData);
        }

        public override RouteActionResult Execute(IRouteBuildContext context, object actionData)
        {
            PriorityRouteActionData priorityActionData = actionData as PriorityRouteActionData;

            if (priorityActionData == null)
                return InvalidActionData("actionData is null or it is not of type PriorityRouteActionData");
            if (priorityActionData.Options == null)
                return InvalidActionData("priorityActionData.Options is null");

            var route = context.Route;
            bool hasOptions = route.Options != null && route.Options.SupplierOptions != null;
            if (!hasOptions)
            {
                route.Options = new RouteOptions();
                route.Options.SupplierOptions = new List<RouteSupplierOption>();
            }

            foreach(var priorityOption in priorityActionData.Options.OrderBy(itm => itm.Priority))
            {
                RouteSupplierOption routeOption;
                if (!TrySetOptionOrderFromRoute(context, priorityOption, out routeOption))
                    TryAddOptionFromLCR(context, priorityOption, out routeOption);
                if(routeOption != null)
                {
                    if (routeOption.Setting == null)
                        routeOption.Setting = new OptionSetting();
                    routeOption.Setting.Priority = priorityOption.Priority;
                    if (priorityOption.Percentage.HasValue)
                        routeOption.Setting.Percentage = priorityOption.Percentage;
                }
            }

            return null;
        }

        private bool TrySetOptionOrderFromRoute(IRouteBuildContext context, PriorityOption priorityOption, out RouteSupplierOption routeOption)
        {
            var routeOptions = context.Route.Options.SupplierOptions;
            routeOption = routeOptions.FirstOrDefault(itm => itm.SupplierId == priorityOption.SupplierId);
            if (routeOption != null)
            {
                if (routeOptions.IndexOf(routeOption) != priorityOption.Priority)
                {
                    routeOptions.Remove(routeOption);
                    InsertOptionAtPosition(priorityOption.Priority, routeOptions, routeOption);
                }

                return true;
            }
            else
                return false;
        }

        private bool TryAddOptionFromLCR(IRouteBuildContext context, PriorityOption priorityOption, out RouteSupplierOption routeOption)
        {
            routeOption = null;
            var routeOptions = context.Route.Options.SupplierOptions;
            RouteSupplierOption current = context.GetNextOptionInLCR();
            while(current != null)
            {
                if (current.SupplierId == priorityOption.SupplierId)//match option found
                {
                    if (priorityOption.Force || context.Route.Rate >= current.Rate)
                    {
                        routeOption = current;
                        InsertOptionAtPosition(priorityOption.Priority, context.Route.Options.SupplierOptions, routeOption);
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else//if option not match with priority option, add it to the end of the list
                {
                    routeOptions.Add(current);
                }
                current = context.GetNextOptionInLCR();
            }

            return false;
        }

        private static void InsertOptionAtPosition(int position, List<RouteSupplierOption> routeOptions, RouteSupplierOption option)
        {
            if (position >= routeOptions.Count)
                routeOptions.Add(option);
            else
                routeOptions.Insert(position, option);
        }
    }
}
