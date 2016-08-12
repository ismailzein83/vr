using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Vanrise.Queueing;
using Vanrise.BusinessProcess;
using TOne.WhS.CodePreparation.Entities;
using TOne.WhS.CodePreparation.Data;
using TOne.WhS.CodePreparation.Entities.Processing;
using TOne.WhS.CodePreparation.Business;

namespace TOne.WhS.CodePreparation.BP.Activities
{

    public class ApplyNewSalePriceListsToDBInput
    {
        public SalePriceListsByOwner SalePriceListsByOwner { get; set; }
    }

    public sealed class ApplyNewSalePriceListsToDB : DependentAsyncActivity<ApplyNewSalePriceListsToDBInput>
    {

        [RequiredArgument]
        public InArgument<SalePriceListsByOwner> SalePriceListsByOwner { get; set; }

        protected override void DoWork(ApplyNewSalePriceListsToDBInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            INewSalePriceListDataManager dataManager = CodePrepDataManagerFactory.GetDataManager<INewSalePriceListDataManager>();
            dataManager.ProcessInstanceId = handle.SharedInstanceData.InstanceInfo.ProcessInstanceID;
            dataManager.SaveSalePriceListsToDB(inputArgument.SalePriceListsByOwner.GetSalePriceLists());
        }

        protected override ApplyNewSalePriceListsToDBInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new ApplyNewSalePriceListsToDBInput()
            {
                SalePriceListsByOwner = this.SalePriceListsByOwner.Get(context)
            };
        }

        protected override void OnBeforeExecute(AsyncCodeActivityContext context, AsyncActivityHandle handle)
        {
            base.OnBeforeExecute(context, handle);
        }
    }
}
