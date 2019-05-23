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

        public override string RuntimeEditor { get { return "vr-whs-routing-qc-trafficstatistic-settings"; } }

        public Guid AnalyticTableId { get; set; }

        public List<string> IncludedMeasures { get; set; }

        public string SaleZoneFieldName { get; set; }

        public string SupplierFieldName { get; set; }

        public string SupplierZoneFieldName { get; set; }

        public override void InitializeQualityConfigurations(IInitializeQualityConfigurationsContext context)
        {
            var qcManager = new TrafficStatisticQualityConfigurationManager();
            context.InitializedQualityConfigurationsById = qcManager.InitializeTrafficStatisticQualityConfigurations(context.QualityConfigurationDefinitionId, 
                context.QualityConfigurations);
        }
    }

    public class InitializedTrafficStatisticQualityConfiguration : InitializedQualityConfiguration
    {
        public IRouteRuleQualityConfigurationEvaluator Evaluator { get; set; }
    }
}