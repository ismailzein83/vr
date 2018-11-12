using System;
using System.Collections.Generic;
using System.Linq;
using Vanrise.Entities;

namespace TOne.WhS.RouteSync.Ericsson.Entities
{
    public abstract class EricssonManualRouteDestinations
    {
        public abstract Guid ConfigId { get; }

        public abstract List<int> GetDestinationCodes(IEricssonManualRouteGetDestinationsContext context);
    }

    public class EricssonManualRouteDestinationCodes : EricssonManualRouteDestinations
    {
        public override Guid ConfigId { get { return new Guid("DD43F4F5-6908-4335-92AF-E2A7CC557BE4"); } }
        public List<int> DestinationCodes { get; set; }
        public override List<int> GetDestinationCodes(IEricssonManualRouteGetDestinationsContext context)
        {
            return DestinationCodes;
        }
    }

    public class EricssonManualRouteDestinationCodeRange : EricssonManualRouteDestinations
    {
        public override Guid ConfigId { get { return new Guid("5FD9FE6B-BC13-4C82-9244-3CB5976BF448"); } }
        public int FromCode { get; set; }
        public int ToCode { get; set; }
        public override List<int> GetDestinationCodes(IEricssonManualRouteGetDestinationsContext context)
        {
            return Enumerable.Range(FromCode, (ToCode - FromCode) + 1).ToList();
        }
    }

    public interface IEricssonManualRouteGetDestinationsContext
    {

    }
    public class EricssonManualRouteGetDestinationsContext : IEricssonManualRouteGetDestinationsContext
    {

    }

    public class EricssonManualRouteDestinationsConfig : ExtensionConfiguration
    {
        public const string EXTENSION_TYPE = "WhS_RouteSync_EricssonManualRouteDestinationsConfig";

        public string Editor { get; set; }
    }
}
