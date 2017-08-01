using System;
using System.Activities;
using System.Collections.Generic;
using TOne.WhS.Routing.Entities;

namespace TOne.WhS.Routing.BP.Activities
{
    public sealed class LoadAffectedRules : CodeActivity
    {
        [RequiredArgument]
        public InArgument<PartialRouteProcessState> PartialRouteProcessState { get; set; }

        [RequiredArgument]
        public OutArgument<AffectedRouteRules> AffectedRouteRules { get; set; }

        [RequiredArgument]
        public OutArgument<AffectedRouteOptionRules> AffectedRouteOptionRules { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            PartialRouteProcessState partialRouteProcessState = this.PartialRouteProcessState.Get(context);
            AffectedRouteOptionRules affectedRouteRules = null;
            AffectedRouteOptionRules affectedRouteOptionRules = null;


            this.AffectedRouteRules.Set(context, affectedRouteRules);
            this.AffectedRouteOptionRules.Set(context, affectedRouteOptionRules);
        }
    }
}