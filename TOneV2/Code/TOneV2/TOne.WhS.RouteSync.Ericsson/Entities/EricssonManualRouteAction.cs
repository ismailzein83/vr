using System;
using System.Collections.Generic;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
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
            TechnicalCodeManager technicalCodeManager = new TechnicalCodeManager();

            foreach (var code in context.Codes)
            {
                foreach (var customer in customersForManualRoutes)
                {
                    TechnicalCodePrefix technicalCodePrefix = technicalCodeManager.GetTechnicalCodeByNumberPrefix(code.ToString());
                    if (technicalCodePrefix == null)
                        throw new NullReferenceException($"No Technical Code Match is found for the following Code: '{code.ToString()}'");
                    var trd = technicalCodePrefix.ZoneID;
                    manualRoutes.Add(new EricssonConvertedRoute() { BO = customer, Code = code.ToString(), RouteType = EricssonRouteType.ANumber, RCNumber = context.BlockRCNumber, TRD = trd });
                }
            }
            context.ManualConvertedRoutes = manualRoutes;
        }
    }

    public interface IEricssonManualRouteActionContext
    {
        List<EricssonConvertedRoute> ManualConvertedRoutes { get; set; }
        List<int> Codes { get; set; }
        List<int> Customers { get; set; }
        List<int> AllCustomers { get; set; }
        int BlockRCNumber { get; set; }
    }

    public class EricssonManualRouteActionContext : IEricssonManualRouteActionContext
    {
        public List<EricssonConvertedRoute> ManualConvertedRoutes { get; set; }
        public List<int> Codes { get; set; }
        public List<int> Customers { get; set; }
        public List<int> AllCustomers { get; set; }
        public int BlockRCNumber { get; set; }
    }

    public class EricssonManualRouteActionConfig : ExtensionConfiguration
    {
        public const string EXTENSION_TYPE = "WhS_RouteSync_EricssonManualRouteActionConfig";

        public string Editor { get; set; }
    }
}