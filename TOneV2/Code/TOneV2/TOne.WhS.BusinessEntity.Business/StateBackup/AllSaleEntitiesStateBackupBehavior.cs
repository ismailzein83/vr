using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Business
{
    public class AllSaleEntitiesStateBackupBehavior : StateBackupBehavior
    {
        public override string GetDescription(IStateBackupContext context)
        {
            if (context.Data == null)
                return string.Empty;

            string restoreText = base.GetDescription(context);

            if (restoreText != null)
                return restoreText;

            StateBackupAllSaleEntities backupData = context.Data as StateBackupAllSaleEntities;
            SellingNumberPlanManager sellingNumberPlanManager = new SellingNumberPlanManager();

            return string.Format("Backup for Numbering Plan {0}", sellingNumberPlanManager.GetSellingNumberPlanName(backupData.SellingNumberPlanId));
        }

        public override bool IsMatch(IStateBackupContext context, object filter)
        {
            if (context.Data == null)
                return false;

            StateBackupAllSaleEntities backupData = context.Data as StateBackupAllSaleEntities;
            AllSaleEntitiesStateBackupFilter filterData = filter as AllSaleEntitiesStateBackupFilter;

            if (filterData.SellingNumberPlanIds == null)
                return true;

            return filterData.SellingNumberPlanIds.Contains(backupData.SellingNumberPlanId);
        }

        public override bool CanRestore(IStateBackupCanRestoreContext context)
        {
            return true;
        }
    }
}
