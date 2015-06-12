using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using TOne.LCR.Entities;
using Vanrise.BusinessProcess;
using TOne.LCR.Data;

namespace TOne.LCRProcess.Activities
{

    public sealed class GetDifferentialRouteRules : CodeActivity
    {

        [RequiredArgument]
        public InArgument<DateTime> LastRun { get; set; }

        [RequiredArgument]
        public OutArgument<List<RouteRule>> RouteRules { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            IRouteRulesDataManager dataManager = LCRDataManagerFactory.GetDataManager<IRouteRulesDataManager>();
            List<RouteRule> routeRules = dataManager.GetDifferentialRouteRules(this.LastRun.Get(context));
            this.RouteRules.Set(context, routeRules);
        }
    }
}
