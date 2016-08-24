using Vanrise.Entities;

namespace TOne.WhS.Routing.Entities
{
    public class RouteSettingsData : SettingData
    {
        public RouteDatabasesToKeep RouteDatabasesToKeep { get; set; }

        public SubProcessSettings SubProcessSettings { get; set; }

        public RouteBuildConfiguration RouteBuildConfiguration { get; set; }
    }
}
