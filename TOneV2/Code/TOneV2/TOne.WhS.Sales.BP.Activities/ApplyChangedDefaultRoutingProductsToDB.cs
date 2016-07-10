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
    public class ApplyChangedDefaultRoutingProductsToDB : CodeActivity
    {
        #region Input Arguments

        [RequiredArgument]
        public InArgument<IEnumerable<ChangedDefaultRoutingProduct>> ChangedDefaultRoutingProducts { get; set; }

        #endregion

        protected override void Execute(CodeActivityContext context)
        {
            IEnumerable<ChangedDefaultRoutingProduct> changedDefaultRoutingProducts = ChangedDefaultRoutingProducts.Get(context);

            if (changedDefaultRoutingProducts != null && changedDefaultRoutingProducts.Count() > 0)
            {
                var dataManager = SalesDataManagerFactory.GetDataManager<IChangedDefaultRoutingProductDataManager>();
                dataManager.ProcessInstanceId = context.GetSharedInstanceData().InstanceInfo.ProcessInstanceID;
                dataManager.ApplyChangedDefaultRoutingProductsToDB(changedDefaultRoutingProducts);
            }
        }
    }
}
