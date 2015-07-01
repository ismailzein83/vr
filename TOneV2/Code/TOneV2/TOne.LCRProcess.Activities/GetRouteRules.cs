using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Vanrise.BusinessProcess;
using TOne.LCR.Entities;
using TOne.LCR.Data;

namespace TOne.LCRProcess.Activities
{
    public class GetRouteRulesInput
    {
        public bool IsFuture { get; set; }
        public DateTime EffectiveOn { get; set; }
        public string CodePrefix { get; set; }
        public HashSet<Int32> CustomerZoneIds { get; set; }

        public HashSet<Int32> SupplierZoneIds { get; set; }

    }

    public class GetRouteRulesOutput
    {
        public List<RouteRule> RouteRules { get; set; }
    }

    public sealed class GetRouteRules : DependentAsyncActivity<GetRouteRulesInput, GetRouteRulesOutput>
    {
        [RequiredArgument]
        public InArgument<bool> IsFuture { get; set; }

        [RequiredArgument]
        public InArgument<DateTime> EffectiveOn { get; set; }

        [RequiredArgument]
        public InArgument<string> CodePrefix { get; set; }

        [RequiredArgument]
        public InArgument<HashSet<int>> CustomerZoneIds { get; set; }

        [RequiredArgument]
        public InArgument<HashSet<int>> SupplierZoneIds { get; set; }

        public OutArgument<List<RouteRule>> RouteRules { get; set; }

        protected override GetRouteRulesInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new GetRouteRulesInput()
            {
                CodePrefix = this.CodePrefix.Get(context),
                EffectiveOn = this.EffectiveOn.Get(context),
                IsFuture = this.IsFuture.Get(context),
                CustomerZoneIds = this.CustomerZoneIds.Get(context),
                SupplierZoneIds = this.SupplierZoneIds.Get(context)
            };
        }

        protected override GetRouteRulesOutput DoWorkWithResult(GetRouteRulesInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            IRouteRulesDataManager dataManager = LCRDataManagerFactory.GetDataManager<IRouteRulesDataManager>();
            List<RouteRule> routeRules = dataManager.GetAllRouteRule(); //dataManager.GetRouteRules(inputArgument.EffectiveOn, inputArgument.IsFuture, inputArgument.CodePrefix, inputArgument.CustomerZoneIds, inputArgument.SupplierZoneIds);
            return new GetRouteRulesOutput()
            {
                RouteRules = routeRules
            };
        }

        protected override void OnWorkComplete(AsyncCodeActivityContext context, GetRouteRulesOutput result)
        {
            this.RouteRules.Set(context, result.RouteRules);
        }
    }
}
