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
    public class ApplyNewRatesToDB : CodeActivity
    {
        #region Arguments

        [RequiredArgument]
        public InArgument<IEnumerable<NewRate>> NewRates { get; set; }

        #endregion

        protected override void Execute(CodeActivityContext context)
        {
            IEnumerable<NewRate> newRates = this.NewRates.Get(context);

            if (newRates != null && newRates.Count() > 0)
            {
                INewSaleRateDataManager dataManager = SalesDataManagerFactory.GetDataManager<INewSaleRateDataManager>();
                dataManager.ProcessInstanceId = context.GetSharedInstanceData().InstanceInfo.ProcessInstanceID;
                dataManager.ApplyNewRatesToDB(newRates);
            }
        }
    }
}
