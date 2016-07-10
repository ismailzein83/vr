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
    public class ApplyRatePreviewsToDB : CodeActivity
    {
        #region Input Arguments
        
        [RequiredArgument]
        public InArgument<IEnumerable<RatePreview>> RatePreviews { get; set; }

        #endregion

        protected override void Execute(CodeActivityContext context)
        {
            IEnumerable<RatePreview> ratePreviews = RatePreviews.Get(context);

            if (ratePreviews == null)
                return;

            var dataManager = SalesDataManagerFactory.GetDataManager<IRatePreviewDataManager>();
            dataManager.ProcessInstanceId = context.GetSharedInstanceData().InstanceInfo.ProcessInstanceID;
            dataManager.ApplyRatePreviewsToDB(ratePreviews);
        }
    }
}
