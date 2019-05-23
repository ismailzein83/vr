using System;
using System.Collections.Generic;

namespace TOne.WhS.Routing.Entities
{
    public abstract class QualityConfigurationDefinitionExtendedSettings
    {
        public abstract Guid ConfigId { get; }

        public virtual string RuntimeEditor { get; set; }

        public virtual void InitializeQualityConfigurations(IInitializeQualityConfigurationsContext context)
        {
        }
    }

    public abstract class InitializedQualityConfiguration
    {

    }

    public interface IInitializeQualityConfigurationsContext
    {
        Guid QualityConfigurationDefinitionId { get; }
        List<RouteRuleQualityConfiguration> QualityConfigurations { get; }
        Dictionary<Guid, InitializedQualityConfiguration> InitializedQualityConfigurationsById { set; }
    }

    public class InitializeQualityConfigurationsContext : IInitializeQualityConfigurationsContext
    {
        public Guid QualityConfigurationDefinitionId { get; set; }
        public List<RouteRuleQualityConfiguration> QualityConfigurations { get; set; }
        public Dictionary<Guid, InitializedQualityConfiguration> InitializedQualityConfigurationsById { get; set; }
    }
}