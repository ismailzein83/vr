using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace TOne.WhS.Routing.Entities
{
    public class RoutingOptimizerSettings : SettingData
    {
        public const string SETTING_TYPE = "WhS_Routing_RoutingOptimizerSettings";
        public List<RoutingOptimizerItemSettings> Items { get; set; }
    }

    public abstract class RoutingOptimizerItemSettings
    {
        public int ConfigId { get; set; }
    }

    public class QualityRoutingOptimizerItemSettings : RoutingOptimizerItemSettings
    {
        public int PeriodLength { get; set; }

        public TimeUnit PeriodTimeUnit { get; set; }

        public string QualityFormula { get; set; }
    }

    public class MarginRoutingOptimizerItemSettings : RoutingOptimizerItemSettings
    {

    }

    public class DealCompletionProgressRoutingOptimizerItemSettings : RoutingOptimizerItemSettings
    {

    }

    public class DealProfitRoutingOptimizerItemSettings : RoutingOptimizerItemSettings
    {

    }
}
