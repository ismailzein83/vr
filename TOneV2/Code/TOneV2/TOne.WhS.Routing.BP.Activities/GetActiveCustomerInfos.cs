using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using TOne.WhS.Routing.Entities;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.Routing.Data;

namespace TOne.WhS.Routing.BP.Activities
{
    public sealed class GetActiveCustomerInfos : CodeActivity
    {
        [RequiredArgument]
        public InArgument<int> RoutingDatabaseId { get; set; }
        [RequiredArgument]
        public OutArgument<IEnumerable<RoutingCustomerInfo>> ActiveRoutingCustomerInfos { get; set; }
        protected override void Execute(CodeActivityContext context)
        {
            CarrierAccountManager carrierAccountManager = new CarrierAccountManager();
            IEnumerable<RoutingCustomerInfo> customerInfos = carrierAccountManager.GetRoutingActiveCustomers();
            ICarrierAccountInfoDataManager dataManager = RoutingDataManagerFactory.GetDataManager<ICarrierAccountInfoDataManager>();
            dataManager.DatabaseId = RoutingDatabaseId.Get(context);
            dataManager.SaveRoutingCustomerInfo(customerInfos);
            ActiveRoutingCustomerInfos.Set(context, customerInfos);
        }
    }
}
