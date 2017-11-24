using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Sales.Business;
using Vanrise.BusinessProcess;
using Vanrise.Common.Business;
using Vanrise.Entities;

namespace TOne.WhS.Sales.BP.Activities
{
    public class SyncImportedDataWithDB : CodeActivity
    {
        #region Input Arguments

        [RequiredArgument]
        public InArgument<int?> RerservedSalePriceListId { get; set; }

        [RequiredArgument]
        public InArgument<SalePriceListOwnerType> OwnerType { get; set; }

        [RequiredArgument]
        public InArgument<int> OwnerId { get; set; }

        [RequiredArgument]
        public InArgument<int> CurrencyId { get; set; }

        [RequiredArgument]
        public InArgument<DateTime> EffectiveOn { get; set; }

        [RequiredArgument]
        public InArgument<long> StateBackupId { get; set; }

        [RequiredArgument]
        public InArgument<IEnumerable<NewCustomerPriceListChange>> CustomerPriceListChanges { get; set; }

        #endregion

        protected override void Execute(CodeActivityContext context)
        {
            int? reservedSalePriceListId = RerservedSalePriceListId.Get(context);
            SalePriceListOwnerType ownerType = OwnerType.Get(context);
            int ownerId = OwnerId.Get(context);
            int currencyId = CurrencyId.Get(context);
            DateTime effectiveOn = EffectiveOn.Get(context);
            long stateBackupId = StateBackupId.Get(context);
            IEnumerable<NewCustomerPriceListChange> customerPriceListChanges = this.CustomerPriceListChanges.Get(context);
            VRFileManager fileManager = new VRFileManager();
            var ratePlanManager = new RatePlanManager();

            foreach (var customerPricelistChange in customerPriceListChanges)
            {
                foreach (var pricelistChange in customerPricelistChange.PriceLists)
                {
                    if (!fileManager.SetFileUsed(pricelistChange.PriceList.FileId))
                        throw new VRBusinessException("Pricelist files have been removed, Process must be restarted.");
                }
            }

            long processInstanceId = context.GetSharedInstanceData().InstanceInfo.ProcessInstanceID;
            ratePlanManager.SyncImportedDataWithDB(processInstanceId, reservedSalePriceListId, ownerType, ownerId, currencyId, effectiveOn, stateBackupId);
        }
    }
}
