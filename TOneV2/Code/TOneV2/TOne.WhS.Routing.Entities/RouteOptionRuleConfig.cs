using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace TOne.WhS.Routing.Entities
{
    public class RouteOptionRuleConfig : ExtensionConfiguration
    {
        public const string EXTENSION_TYPE = "WhS_Routing_RouteOptionRuleSettingsType";
        public string Editor { get; set; }
        public int? Priority { get; set; }
        public string CssClass { get; set; }
        public bool CanExcludeFromRouteBuildProcess { get; set; }
        public bool CanExcludeFromProductCostProcess { get; set; }
    }

    public abstract class ProcessRuleConfig
    {
        public Guid ExtensionConfigurationId { get; set; }

        public string Title { get; set; }

        public bool CanExclude { get; set; }
    }

    public class ProcessRouteOptionRuleConfig : ProcessRuleConfig
    {

    }
}
