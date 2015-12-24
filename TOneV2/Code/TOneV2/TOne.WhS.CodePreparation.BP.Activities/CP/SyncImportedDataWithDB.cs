using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.CodePreparation.Data;
using Vanrise.BusinessProcess;
namespace TOne.WhS.CodePreparation.BP.Activities
{
    public class SyncImportedDataWithDB : CodeActivity
    {
        [RequiredArgument]
        public InArgument<int> SellingNumberPlanId { get; set; }


        protected override void Execute(CodeActivityContext context)
        {

            int sellingNumberPlanId = this.SellingNumberPlanId.Get(context);
            long processInstanceID = context.GetSharedInstanceData().InstanceInfo.ProcessInstanceID;
            ICodePreparationDataManager dataManager = CodePrepDataManagerFactory.GetDataManager<ICodePreparationDataManager>();
            dataManager.AddPriceListAndSyncImportedDataWithDB(processInstanceID, sellingNumberPlanId);
        }
    }
}
