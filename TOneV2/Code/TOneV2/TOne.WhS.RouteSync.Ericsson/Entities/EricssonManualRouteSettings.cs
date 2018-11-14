using System;
using System.Collections.Generic;
using Vanrise.Common;

namespace TOne.WhS.RouteSync.Ericsson.Entities
{
    public class EricssonManualRouteSettings
    {
        public List<EricssonManualRoute> EricssonManualRoutes { get; set; }
        public List<EricssonSpecialRoute> EricssonSpecialRoutes { get; set; }
    }

    public class EricssonManualRoute
    {
        public List<string> Customers { get; set; }
        public EricssonManualRouteOriginations ManualRouteOriginations { get; set; }
        public EricssonManualRouteDestinations ManualRouteDestinations { get; set; }
        public EricssonManualRouteAction ManualRouteAction { get; set; }

    }

    public class EricssonSpecialRoute
    {
        public string TargetBO { get; set; }
        public string SourceBO { get; set; }
        public EricssonSpecialRoutingSetting Settings { get; set; }
        public List<EricssonConvertedRoute> GetSpecialRoutes(IGetSpecialRoutesContext context)
        {
            context.ThrowIfNull("context");
            this.Settings.ThrowIfNull("Settings");

            return Settings.Execute(new EricssonSpecialRoutingSettingContext() { SourceRoutes = context.SourceRoutes, TargetBO = TargetBO });
        }
    }

    public interface IGetSpecialRoutesContext
    {
        List<EricssonConvertedRoute> SourceRoutes { get; set; }
    }

    public class GetSpecialRoutesContext : IGetSpecialRoutesContext
    {
        public List<EricssonConvertedRoute> SourceRoutes { get; set; }
    }
}
