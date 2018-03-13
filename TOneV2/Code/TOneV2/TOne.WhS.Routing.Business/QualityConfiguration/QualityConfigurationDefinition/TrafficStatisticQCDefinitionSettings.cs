using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Routing.Entities;

namespace TOne.WhS.Routing.Business
{
    public class TrafficStatisticQCDefinitionSettings : TOne.WhS.Routing.Entities.QualityConfigurationDefinitionExtendedSettings 
    {
        public override Guid ConfigId { get { return new Guid("E670425F-2F91-4C9A-BB03-8B85CD77BBD2"); } }

        public override string RuntimeEditor { get { return ""; } }

        //public Guid AnalyticTableId { get; set; }

        //public List<string> IncludedMeasures { get; set; }

        //public List<string> CustomerRouteDimensions { get; set; }

        //public List<string> ProductRouteDimensions { get; set; } 

        //public override Dictionary<Guid, InitializedQualityConfiguration> InitializeQualityConfigurations(List<RouteRuleQualityConfiguration> qualityConfigurations)
        //{
        //    return new TrafficStatisticQualityConfigurationManager().InitializeTrafficStatisticQualityConfigurations(qualityConfigurations);
        //}
    }

    //public class InitializedTrafficStatisticQualityConfiguration : InitializedQualityConfiguration
    //{
    //    public IRouteRuleQualityConfigurationEvaluator Evaluator { get; set; }
    //}
}