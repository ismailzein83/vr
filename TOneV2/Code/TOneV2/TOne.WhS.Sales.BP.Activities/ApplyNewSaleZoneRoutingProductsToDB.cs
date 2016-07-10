using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Sales.Data;
using TOne.WhS.Sales.Entities;
using Vanrise.BusinessProcess;

namespace TOne.WhS.Sales.BP.Activities
{
    public class ApplyNewSaleZoneRoutingProductsToDB : CodeActivity
    {
        #region Input Arguments
        
        [RequiredArgument]
        public InArgument<IEnumerable<NewSaleZoneRoutingProduct>> NewSaleZoneRoutingProducts { get; set; }
        
        #endregion

        protected override void Execute(CodeActivityContext context)
        {
            IEnumerable<NewSaleZoneRoutingProduct> newSaleZoneRoutingProducts = NewSaleZoneRoutingProducts.Get(context);

            if (newSaleZoneRoutingProducts == null)
                return;

            var dataManager = SalesDataManagerFactory.GetDataManager<INewSaleZoneRoutingProductDataManager>();
            dataManager.ProcessInstanceId = context.GetSharedInstanceData().InstanceInfo.ProcessInstanceID;

            dataManager.ApplyNewSaleZoneRoutingProductsToDB(newSaleZoneRoutingProducts);
        }
    }
}
