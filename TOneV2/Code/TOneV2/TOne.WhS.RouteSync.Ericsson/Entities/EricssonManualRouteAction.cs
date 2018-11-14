using System;
using System.Collections.Generic;
using Vanrise.Entities;

namespace TOne.WhS.RouteSync.Ericsson.Entities
{
    public abstract class EricssonManualRouteAction
    {
        public abstract Guid ConfigId { get; }
        public abstract void Execute(IEricssonManualRouteActionContext context);
    }

    public class EricssonManualRouteBlockAction : EricssonManualRouteAction
    {
        public override Guid ConfigId { get { return new Guid("58B8076D-C69A-4019-B2AE-B32456F9560B"); } }

        public override void Execute(IEricssonManualRouteActionContext context)
        {
            var manualRoutes = new List<EricssonConvertedRoute>();
            var customersForManualRoutes = (context.Customers != null && context.Customers.Count > 0) ? context.Customers : context.AllCustomers;
            foreach (var code in context.Codes)
            {
                foreach (var customer in customersForManualRoutes)
                {
                    manualRoutes.Add(new EricssonConvertedRoute() { BO = customer, Code = code.ToString(), RouteType = EricssonRouteType.ANumber, RCNumber = context.BlockRCNumber });
                }
            }
            context.ManualConvertedRoutes = manualRoutes;
        }
    }

    public interface IEricssonManualRouteActionContext
    {
        List<EricssonConvertedRoute> ManualConvertedRoutes { get; set; }
        List<int> Codes { get; set; }
        List<string> Customers { get; set; }
        List<string> AllCustomers { get; set; }
        int BlockRCNumber { get; set; }
    }

    public class EricssonManualRouteActionContext : IEricssonManualRouteActionContext
    {
        public List<EricssonConvertedRoute> ManualConvertedRoutes { get; set; }
        public List<int> Codes { get; set; }
        public List<string> Customers { get; set; }
        public List<string> AllCustomers { get; set; }
        public int BlockRCNumber { get; set; }
    }

    public class EricssonManualRouteActionConfig : ExtensionConfiguration
    {
        public const string EXTENSION_TYPE = "WhS_RouteSync_EricssonManualRouteActionConfig";

        public string Editor { get; set; }
    }
}