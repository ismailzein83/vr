using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Routing.Entities
{
    public class RouteRuleQualityConfiguration
    {
        public Guid QualityConfigurationId { get; set; }

        public Guid QualityConfigurationDefinitionId { get; set; }

        public string Name { get; set; }

        public bool IsActive { get; set; }

        public bool IsDefault { get; set; }

        public RouteRuleQualityConfigurationSettings Settings { get; set; }
    }

    public abstract class RouteRuleQualityConfigurationSettings
    {
        public abstract Guid ConfigId { get; }

        public abstract List<RouteRuleQualityConfigurationData> GetRouteRuleQualityConfigurationData(IGetRouteRuleQualityConfigurationDataContext context);

        public virtual bool ValidateRouteRuleQualityConfigurationSettings(IValidateQualityConfigurationDataContext context)
        {
            return true;
        }
    }

    public interface IGetRouteRuleQualityConfigurationDataContext
    {
        Guid QualityConfigurationId { get; }

        Guid QualityConfigurationDefinitionId { get; }

        string QualityConfigurationName { get; } 

        InitializedQualityConfiguration InitializedQualityConfiguration { get; }

        RoutingProcessType RoutingProcessType { get; }

        DateTime EffectiveDate { get; }
    }

    public class GetRouteRuleQualityConfigurationDataContext : IGetRouteRuleQualityConfigurationDataContext
    {
        public Guid QualityConfigurationId { get; set; }

        public Guid QualityConfigurationDefinitionId { get; set; }

        public string QualityConfigurationName { get; set; } 

        public InitializedQualityConfiguration InitializedQualityConfiguration { get; set; }

        public RoutingProcessType RoutingProcessType { get; set; }

        public DateTime EffectiveDate { get; set; }
    }

    public interface IValidateQualityConfigurationDataContext
    {
        Guid QualityConfigurationDefinitionId { get; }

        string QualityConfigurationName { get; } 
    }

    public class ValidateQualityConfigurationDataContext : IValidateQualityConfigurationDataContext
    {
        public Guid QualityConfigurationDefinitionId { get; set; }

        public string QualityConfigurationName { get; set; } 
    }
}