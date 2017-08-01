using System;
using System.Activities;
using System.Collections.Generic;
using TOne.WhS.Routing.Entities;

namespace TOne.WhS.Routing.BP.Activities
{
    public sealed class CheckAffectedRulesImpact : CodeActivity
    {
        [RequiredArgument]
        public InArgument<AffectedRouteRules> AffectedRouteRules { get; set; }

        [RequiredArgument]
        public InArgument<AffectedRouteOptionRules> AffectedRouteOptionRules { get; set; }

        [RequiredArgument]
        public OutArgument<bool> ShouldTriggerFullRouteProcess { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            AffectedRouteOptionRules affectedRouteRules = this.AffectedRouteOptionRules.Get(context);
            AffectedRouteOptionRules affectedRouteOptionRules = this.AffectedRouteOptionRules.Get(context);

            bool shouldTriggerFullRouteProcess = false;

            this.ShouldTriggerFullRouteProcess.Set(context, shouldTriggerFullRouteProcess);
        }
    }
}