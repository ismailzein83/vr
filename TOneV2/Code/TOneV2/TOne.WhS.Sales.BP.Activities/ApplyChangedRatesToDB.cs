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
    public class ApplyChangedRatesToDB : CodeActivity
    {
        #region Arguments

        [RequiredArgument]
        public InArgument<IEnumerable<ChangedRate>> ChangedRates { get; set; }

        #endregion

        protected override void Execute(CodeActivityContext context)
        {
            IEnumerable<ChangedRate> changedRates = this.ChangedRates.Get(context);

            if (changedRates != null && changedRates.Count() > 0)
            {
                IChangedSaleRateDataManager dataManager = SalesDataManagerFactory.GetDataManager<IChangedSaleRateDataManager>();
                dataManager.ProcessInstanceId = context.GetSharedInstanceData().InstanceInfo.ProcessInstanceID;
                dataManager.ApplyChangedRatesToDB(changedRates);
            }
        }
    }
}
