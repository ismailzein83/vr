using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Business
{
    public class SupplierStateBackupBehavior : StateBackupBehavior
    {
        public override string GetDescription(IStateBackupContext context)
        {
            if (context.Data == null)
                return string.Empty;

            string restoreText = base.GetDescription(context);

            if (restoreText != null)
                return restoreText;

            StateBackupSupplier backupData = context.Data as StateBackupSupplier;

            CarrierAccountManager carrierAccountManager = new CarrierAccountManager();
            return String.Format("Backup for Supplier {0}", carrierAccountManager.GetCarrierAccountName(backupData.SupplierId));
        }

        public override bool IsMatch(IStateBackupContext context, object filter)
        {
            if (context.Data == null)
                return false;

            StateBackupSupplier backupData = context.Data as StateBackupSupplier;
            SupplierStateBackupFilter filterData = filter as SupplierStateBackupFilter;

            if (filterData.SupplierIds == null)
                return true;

            return filterData.SupplierIds.Contains(backupData.SupplierId);
        }

        public override bool CanRestore(IStateBackupCanRestoreContext context)
        {
            return true;
        }
    }
}
