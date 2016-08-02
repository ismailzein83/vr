using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using TOne.WhS.Routing.Entities;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.Routing.Data;
using Vanrise.BusinessProcess;
using Vanrise.Entities;

namespace TOne.WhS.Routing.BP.Activities
{
    public sealed class GetActiveCustomerInfos : CodeActivity
    {
        [RequiredArgument]
        public OutArgument<IEnumerable<RoutingCustomerInfo>> ActiveRoutingCustomerInfos { get; set; }
        protected override void Execute(CodeActivityContext context)
        {
            CarrierAccountManager carrierAccountManager = new CarrierAccountManager();
            IEnumerable<RoutingCustomerInfo> customerInfos = carrierAccountManager.GetRoutingActiveCustomers();
            ActiveRoutingCustomerInfos.Set(context, customerInfos);
            context.GetSharedInstanceData().WriteTrackingMessage(LogEntryType.Information, "Getting Active Customer Infos is done", null);
        }
    }
}
