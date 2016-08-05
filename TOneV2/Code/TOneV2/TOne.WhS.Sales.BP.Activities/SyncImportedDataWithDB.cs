using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Sales.Business;
using Vanrise.BusinessProcess;

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

        #endregion

        protected override void Execute(CodeActivityContext context)
        {
            int? reservedSalePriceListId = RerservedSalePriceListId.Get(context);
            SalePriceListOwnerType ownerType = OwnerType.Get(context);
            int ownerId = OwnerId.Get(context);
            int currencyId = CurrencyId.Get(context);
            DateTime effectiveOn = EffectiveOn.Get(context);

            var ratePlanManager = new RatePlanManager();
            
            long processInstanceId = context.GetSharedInstanceData().InstanceInfo.ProcessInstanceID;
            ratePlanManager.SyncImportedDataWithDB(processInstanceId, reservedSalePriceListId, ownerType, ownerId, currencyId, effectiveOn);
        }
    }
}
