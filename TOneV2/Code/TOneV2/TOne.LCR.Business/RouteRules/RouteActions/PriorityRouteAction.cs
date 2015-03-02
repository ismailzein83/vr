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
                return InvalidActionData("priorityActionData.Options");

            var route = context.Route;
            if (route.Options == null || route.Options.SupplierOptions == null)
            {
                route.Options = new RouteOptions();
                route.Options.SupplierOptions = new List<RouteSupplierOption>();
            }

            foreach(var priorityOption in priorityActionData.Options.OrderBy(itm => itm.Order))
            {
                if (priorityOption.Force)
                {
                    if (!TrySetOptionOrderFromRoute(context, priorityOption))
                        TryAddOptionFromCodeMatches(context, priorityOption);
                }
                else
                {
                    TrySetOptionOrderFromRoute(context, priorityOption);
                }
            }

            return null;
        }

        private bool TryAddOptionFromCodeMatches(IRouteBuildContext context, PriorityOption priorityOption)
        {
            RouteSupplierOption routeOption;
            if (context.TryBuildSupplierOption(priorityOption.SupplierId, priorityOption.Percentage, out routeOption))
            {
                InsertOptionAtPosition(priorityOption.Order, context.Route.Options.SupplierOptions, routeOption);
                return true;
            }
            return false;
        }

        private bool TrySetOptionOrderFromRoute(IRouteBuildContext context, PriorityOption priorityOption)
        {
            var routeOptions = context.Route.Options.SupplierOptions;
            var option = routeOptions.FirstOrDefault(itm => itm.SupplierId == priorityOption.SupplierId);
            if (option != null)
            {
                if (routeOptions.IndexOf(option) != priorityOption.Order)
                {
                    routeOptions.Remove(option);
                    InsertOptionAtPosition(priorityOption.Order, routeOptions, option);
                }

                return true;
            }
            else
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
