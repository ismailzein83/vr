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
        public abstract Guid ConfigId { get; }
    }

    public class QualityRoutingOptimizerItemSettings : RoutingOptimizerItemSettings
    {
        public override Guid ConfigId { get { return new Guid("7df74212-379a-4092-b025-701345ffdce9"); } }
        public int PeriodLength { get; set; }

        public TimeUnit PeriodTimeUnit { get; set; }

        public string QualityFormula { get; set; }
    }

    public class MarginRoutingOptimizerItemSettings : RoutingOptimizerItemSettings
    {
        public override Guid ConfigId { get { return new Guid("0d42afb5-92e2-43a4-85ee-6a51079aedc3"); } }
    }

    public class DealCompletionProgressRoutingOptimizerItemSettings : RoutingOptimizerItemSettings
    {
        public override Guid ConfigId { get { return new Guid("7a030e29-1fd6-42e6-92ad-66ccb566883c"); } }
    }

    public class DealProfitRoutingOptimizerItemSettings : RoutingOptimizerItemSettings
    {
        public override Guid ConfigId { get { return new Guid("405e0be1-f785-453e-a6c2-7df82de38210"); } }
    }
}
