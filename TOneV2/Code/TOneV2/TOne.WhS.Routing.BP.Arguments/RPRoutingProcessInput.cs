using System;
using System.Collections.Generic;
using TOne.WhS.Routing.Entities;

namespace TOne.WhS.Routing.BP.Arguments
{
    public class RPRoutingProcessInput : Vanrise.BusinessProcess.Entities.BaseProcessInputArgument
    {
        public DateTime? EffectiveOn { get; set; }
        public RoutingDatabaseType RoutingDatabaseType { get; set; }
        public RoutingProcessType RoutingProcessType { get; set; }
        public bool IsFuture { get; set; }
        public int SaleZoneRange { get; set; }
        public List<SupplierZoneToRPOptionPolicy> SupplierZoneRPOptionPolicies { get; set; }
        public int EffectiveAfterInMinutes { get; set; }

        public override string GetTitle()
        {
            if (EffectiveOn.HasValue)
                return String.Format("#BPDefinitionTitle# for Effective Time {0}", EffectiveOn.Value.ToString("yyyy-MM-dd HH:mm:ss"));
            else
                return String.Format("#BPDefinitionTitle#");
        }

        public override void MapExpressionValues(Dictionary<string, object> evaluatedExpressions)
        {
            if (!IsFuture && evaluatedExpressions.ContainsKey("ScheduleTime"))
            {
                EffectiveOn = DateTime.Now.AddMinutes(EffectiveAfterInMinutes);
            }
        }
    }
}