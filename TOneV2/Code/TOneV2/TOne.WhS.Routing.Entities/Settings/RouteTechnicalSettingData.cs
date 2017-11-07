using Vanrise.Entities;

namespace TOne.WhS.Routing.Entities
{
    public class RouteTechnicalSettingData : SettingData
    {
        public RouteRuleDataTransformation RouteRuleDataTransformation { get; set; }

        public TechnicalPartialRouting TechnicalPartialRouting { get; set; }

        public TechnicalQualityConfiguration TechnicalQualityConfiguration { get; set; }
    }
}
