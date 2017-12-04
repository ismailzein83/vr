using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.CodePreparation.Data;
using Vanrise.BusinessProcess;
using Vanrise.Common.Business;
using Vanrise.Entities;
namespace TOne.WhS.CodePreparation.BP.Activities
{
    public class SyncImportedDataWithDB : CodeActivity
    {
        [RequiredArgument]
        public InArgument<int> SellingNumberPlanId { get; set; }

        [RequiredArgument]
        public InArgument<long> StateBackupId { get; set; }

        [RequiredArgument]
        public InArgument<IEnumerable<NewCustomerPriceListChange>> CustomerPriceListChanges { get; set; }

        protected override void Execute(CodeActivityContext context)
        {

            int sellingNumberPlanId = this.SellingNumberPlanId.Get(context);
            long processInstanceID = context.GetSharedInstanceData().InstanceInfo.ProcessInstanceID;
            long stateBackupId = this.StateBackupId.Get(context);
            IEnumerable<NewCustomerPriceListChange> customerPriceListChanges = this.CustomerPriceListChanges.Get(context);

            VRFileManager fileManager = new VRFileManager();

            if (customerPriceListChanges!=null)    
            {
                    foreach (var customerPricelistChange in customerPriceListChanges)
                    {
                        foreach (var pricelistChange in customerPricelistChange.PriceLists)
                        {
                            if (!fileManager.SetFileUsed(pricelistChange.PriceList.FileId))
                                throw new VRBusinessException("Pricelist files have been removed, Process must be restarted.");
                        }
                    }
            }
           
            ICodePreparationDataManager dataManager = CodePrepDataManagerFactory.GetDataManager<ICodePreparationDataManager>();
            dataManager.AddPriceListAndSyncImportedDataWithDB(processInstanceID, sellingNumberPlanId, stateBackupId);
        }
    }
}
