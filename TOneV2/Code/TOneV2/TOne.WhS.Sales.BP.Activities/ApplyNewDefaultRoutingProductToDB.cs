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
    public class ApplyNewDefaultRoutingProductToDB : CodeActivity
    {
        #region Input Arguments
        
        [RequiredArgument]
        public InArgument<NewDefaultRoutingProduct> NewDefaultRoutingProduct { get; set; }
        
        #endregion

        protected override void Execute(CodeActivityContext context)
        {
            NewDefaultRoutingProduct newDefaultRoutingProduct = NewDefaultRoutingProduct.Get(context);

            if (newDefaultRoutingProduct != null)
            {
                var dataManager = SalesDataManagerFactory.GetDataManager<INewDefaultRoutingProductDataManager>();
                dataManager.ProcessInstanceId = context.GetSharedInstanceData().InstanceInfo.ProcessInstanceID;
                dataManager.ApplyNewDefaultRoutingProductsToDB(new List<NewDefaultRoutingProduct>() { newDefaultRoutingProduct });
            }
        }
    }
}
