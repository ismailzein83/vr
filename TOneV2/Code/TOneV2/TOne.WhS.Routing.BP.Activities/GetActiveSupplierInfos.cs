using System.Collections.Generic;
using System.Activities;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.BusinessEntity.Business;
using Vanrise.BusinessProcess;
using Vanrise.Entities;

namespace TOne.WhS.Routing.BP.Activities
{
    public sealed class GetActiveSupplierInfos : CodeActivity
    {
        [RequiredArgument]
        public OutArgument<IEnumerable<RoutingSupplierInfo>> ActiveRoutingSupplierInfos { get; set; }
        
        protected override void Execute(CodeActivityContext context)
        {
            CarrierAccountManager carrierAccountManager = new CarrierAccountManager();
            IEnumerable<RoutingSupplierInfo> supplierInfos = carrierAccountManager.GetRoutingActiveSuppliers();
            ActiveRoutingSupplierInfos.Set(context, supplierInfos);
            context.GetSharedInstanceData().WriteTrackingMessage(LogEntryType.Information, "Getting Active Supplier Infos is done", null);
        }
    }
}
