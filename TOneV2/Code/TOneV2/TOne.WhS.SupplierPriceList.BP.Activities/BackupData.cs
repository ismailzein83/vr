using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using TOne.WhS.SupplierPriceList.Entities.SPL;
using TOne.WhS.SupplierPriceList.Entities;
using TOne.WhS.SupplierPriceList.Business;
using Vanrise.Queueing;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.BusinessEntity.Business;

namespace TOne.WhS.SupplierPriceList.BP.Activities
{

    public sealed class BackupData : CodeActivity
    {
        [RequiredArgument]
        public InArgument<int> SupplierId { get; set; }

        [RequiredArgument]
        public InArgument<int> UserId { get; set; }

        [RequiredArgument]
        public OutArgument<long> StateBackupId { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            int supplierId = this.SupplierId.Get(context);
            int userId = this.UserId.Get(context);

            StateBackupSupplier backupSupplier = new StateBackupSupplier()
            {
                SupplierId = supplierId,
                UserId = userId
            };

            StateBackupManager manager = new StateBackupManager();
            object stateBackupId;
            stateBackupId = manager.BackupData(backupSupplier);
            if (stateBackupId != null)
                StateBackupId.Set(context, (long)stateBackupId);

        }
    }
}
