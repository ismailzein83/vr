﻿using System;
using System.Collections.Generic;
using TOne.WhS.Routing.Entities;

namespace TOne.WhS.Routing.BP.Arguments
{
    public class RoutingProcessInput : Vanrise.BusinessProcess.Entities.BaseProcessInputArgument
    {
        public DateTime? EffectiveTime { get; set; }
        public bool IsFuture { get; set; }
        public RoutingProcessMode RoutingProcessMode { get; set; }
        public RoutingProcessType RoutingProcessType { get; set; }
        public RoutingDatabaseType RoutingDatabaseType { get; set; }
        public bool DivideProcessIntoSubProcesses { get; set; }
        public int EffectiveAfterInMinutes { get; set; }
        public List<string> Switches { get; set; }
        public List<Guid> RiskyMarginCategories { get; set; }

        public override string GetTitle()
        {
            if (EffectiveTime.HasValue)
                return String.Format("#BPDefinitionTitle# for Effective Time {0}", EffectiveTime.Value.ToString("yyyy-MM-dd HH:mm:ss"));
            else
                return String.Format("#BPDefinitionTitle#");
        }

        public override void MapExpressionValues(Dictionary<string, object> evaluatedExpressions)
        {
            if (!IsFuture && evaluatedExpressions.ContainsKey("ScheduleTime"))
                EffectiveTime = DateTime.Now.AddMinutes(EffectiveAfterInMinutes);
        }
    }
}