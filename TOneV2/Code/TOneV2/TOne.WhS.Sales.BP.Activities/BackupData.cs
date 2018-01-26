using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Sales.Data;
using TOne.WhS.Sales.Entities;
using Vanrise.BusinessProcess;

namespace TOne.WhS.Sales.BP.Activities
{

    public sealed class BackupData : CodeActivity
    {
        [RequiredArgument]
        public InArgument<int> OwnerId { get; set; }

        [RequiredArgument]
        public InArgument<SalePriceListOwnerType> OwnerType { get; set; }

        [RequiredArgument]
        public InArgument<int> UserId { get; set; }

        public InArgument<int?> MasterOwnerId { get; set; }

        [RequiredArgument]
        public OutArgument<long> StateBackupId { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            int ownerId = this.OwnerId.Get(context);
            int userId = this.UserId.Get(context);
            int? masterOwnerId = this.MasterOwnerId.Get(context);

            SalePriceListOwnerType ownerType = this.OwnerType.Get(context);

            StateBackupSaleEntity backupCustomer = new StateBackupSaleEntity
            {
                OwnerId = ownerId,
                OwnerType = ownerType,
                UserId = userId,
                MasterOwnerId = masterOwnerId,
            };
            if (ownerType == SalePriceListOwnerType.SellingProduct)
            {
                CarrierAccountManager carrierAccountManager = new CarrierAccountManager();
                backupCustomer.SellingProductCustomerIds = carrierAccountManager.GetCarrierAccountIdsAssignedToSellingProduct(ownerId);
            }

            StateBackupManager manager = new StateBackupManager();
            object stateBackupId;
            stateBackupId = manager.BackupData(backupCustomer);
            if (stateBackupId != null)
                StateBackupId.Set(context, (long)stateBackupId);
        }
    }
}
