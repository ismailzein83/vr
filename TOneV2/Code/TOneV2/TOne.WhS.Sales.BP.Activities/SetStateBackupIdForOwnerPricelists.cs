using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.BusinessProcess;
using TOne.WhS.Sales.Business;

namespace TOne.WhS.Sales.BP.Activities
{
    public class SetStateBackupIdForOwnerPricelists : CodeActivity
    {
        [RequiredArgument]
        public InArgument<SalePriceListOwnerType> OwnerType { get; set; }

        [RequiredArgument]
        public InArgument<int> OwnerId { get; set; }

        [RequiredArgument]
        public InArgument<long> StateBackupId { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            SalePriceListOwnerType ownerType = OwnerType.Get(context);
            int ownerId = OwnerId.Get(context);
            long stateBackupId = StateBackupId.Get(context);
            long processInstanceId = context.GetSharedInstanceData().InstanceInfo.ProcessInstanceID;

            var ratePlanManager = new RatePlanManager();
            ratePlanManager.SetStateBackupIdForOwnerPricelists(processInstanceId, ownerType, ownerId, stateBackupId);
        }
    }
}
