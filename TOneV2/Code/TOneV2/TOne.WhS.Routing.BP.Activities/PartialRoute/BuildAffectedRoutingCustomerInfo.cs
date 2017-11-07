using System;
using System.Linq;
using System.Activities;
using System.Collections.Generic;
using Vanrise.Common;
using TOne.WhS.Routing.Entities;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.BusinessEntity.Business;
using Vanrise.BusinessProcess;

namespace TOne.WhS.Routing.BP.Activities
{
    public class BuildAffectedRoutingCustomerInfoInput
    {
        public List<CustomerRoute> AffectedCustomerRoutes { get; set; }
    }

    public class BuildAffectedRoutingCustomerInfoOutput
    {
        public IEnumerable<RoutingCustomerInfo> ActiveRoutingCustomerInfos { get; set; }
    }

    public sealed class BuildAffectedRoutingCustomerInfo : BaseAsyncActivity<BuildAffectedRoutingCustomerInfoInput, BuildAffectedRoutingCustomerInfoOutput>
    {

        [RequiredArgument]
        public InArgument<List<CustomerRoute>> AffectedCustomerRoutes { get; set; }

        [RequiredArgument]
        public OutArgument<IEnumerable<RoutingCustomerInfo>> ActiveRoutingCustomerInfos { get; set; }

        protected override BuildAffectedRoutingCustomerInfoOutput DoWorkWithResult(BuildAffectedRoutingCustomerInfoInput inputArgument, AsyncActivityHandle handle)
        {
            List<CustomerRoute> affectedCustomerRoutes = inputArgument.AffectedCustomerRoutes;
            HashSet<int> customerIds = affectedCustomerRoutes.Select(itm => itm.CustomerId).ToHashSet();

            return new BuildAffectedRoutingCustomerInfoOutput() { ActiveRoutingCustomerInfos = new CarrierAccountManager().GetRoutingCustomerInfos(customerIds) };
        }

        protected override BuildAffectedRoutingCustomerInfoInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new BuildAffectedRoutingCustomerInfoInput()
            {
                AffectedCustomerRoutes = this.AffectedCustomerRoutes.Get(context)
            };
        }

        protected override void OnWorkComplete(AsyncCodeActivityContext context, BuildAffectedRoutingCustomerInfoOutput result)
        {
            this.ActiveRoutingCustomerInfos.Set(context, result.ActiveRoutingCustomerInfos);
        }
    }
}