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
    public class ApplyChangedSaleZoneRoutingProductsToDB : CodeActivity
    {
        #region Input Arguments
        
        [RequiredArgument]
        public InArgument<IEnumerable<ChangedSaleZoneRoutingProduct>> ChangedSaleZoneRoutingProducts { get; set; }
        
        #endregion

        protected override void Execute(CodeActivityContext context)
        {
            IEnumerable<ChangedSaleZoneRoutingProduct> changedSaleZoneRoutingProducts = ChangedSaleZoneRoutingProducts.Get(context);

            if (changedSaleZoneRoutingProducts == null)
                return;

            var dataManager = SalesDataManagerFactory.GetDataManager<IChangedSaleZoneRoutingProductDataManager>();
            dataManager.ProcessInstanceId = context.GetSharedInstanceData().InstanceInfo.ProcessInstanceID;

            dataManager.ApplyChangedSaleZoneRoutingProductsToDB(changedSaleZoneRoutingProducts);
        }
    }
}
