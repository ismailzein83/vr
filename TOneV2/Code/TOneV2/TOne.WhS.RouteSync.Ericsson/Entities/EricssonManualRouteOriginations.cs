using System;
using System.Collections.Generic;
using System.Linq;
using Vanrise.Entities;

namespace TOne.WhS.RouteSync.Ericsson.Entities
{
    public abstract class EricssonManualRouteOriginations
    {
        public abstract Guid ConfigId { get; }

        public abstract List<int> GetOriginationCodes(IEricssonManualRouteGetOriginationsContext context);
    }

    public class EricssonManualRouteOriginationCodes : EricssonManualRouteOriginations
    {
        public override Guid ConfigId { get { return new Guid("44F41AFD-47A9-4F86-A40D-8997B7C2D896"); } }
        public List<int> OriginationCodes { get; set; }
        public override List<int> GetOriginationCodes(IEricssonManualRouteGetOriginationsContext context)
        {
            return OriginationCodes;
        }
    }

    public class EricssonManualRouteOriginationRange : EricssonManualRouteOriginations
    {
        public override Guid ConfigId { get { return new Guid("B53B18B1-3B53-4A2D-882A-CEC022E85320"); } }
        public int FromCode { get; set; }
        public int ToCode { get; set; }
        public override List<int> GetOriginationCodes(IEricssonManualRouteGetOriginationsContext context)
        {
            return Enumerable.Range(FromCode, (ToCode - FromCode) + 1).ToList();
        }
    }

    public interface IEricssonManualRouteGetOriginationsContext
    {

    }
    public class EricssonManualRouteGetOriginationsContext : IEricssonManualRouteGetOriginationsContext
    {

    }

    public class EricssonManualRouteOriginationsConfig : ExtensionConfiguration
    {
        public const string EXTENSION_TYPE = "WhS_RouteSync_EricssonManualRouteOriginationsConfig";

        public string Editor { get; set; }
    }
}
