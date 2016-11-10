using System;

namespace TOne.WhS.Routing.Entities
{
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