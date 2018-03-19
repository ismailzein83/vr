using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Routing.Entities
{
    public abstract class QualityConfigurationDefinitionExtendedSettings
    {
        public abstract Guid ConfigId { get; }

        public virtual string RuntimeEditor { get; set; }

        public virtual Dictionary<Guid, InitializedQualityConfiguration> InitializeQualityConfigurations(List<RouteRuleQualityConfiguration> qualityConfigurations)
        {
            return null;
        }
    }

    public abstract class InitializedQualityConfiguration
    {

    }
}