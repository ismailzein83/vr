using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Data;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Sales.Business;
using Vanrise.BusinessProcess;

namespace TOne.WhS.Sales.BP.Activities
{
    public class SaveRoutingProductChanges : CodeActivity
    {
        public InArgument<IEnumerable<SalePricelistRPChange>> AllSalePricelistRPChanges { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            IEnumerable<SalePricelistRPChange> allRoutingProductChanges = AllSalePricelistRPChanges.Get(context);
            var dataManager = BEDataManagerFactory.GetDataManager<ISalePriceListChangeDataManager>();
            var processInstanceId = context.GetRatePlanContext().RootProcessInstanceId;
            dataManager.SaveCustomerRoutingProductChangesToDb(allRoutingProductChanges, processInstanceId);
        }
    }
}
