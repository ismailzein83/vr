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

            StateBackupAllSaleEntities backupData = context.Data as StateBackupAllSaleEntities;
            SellingNumberPlanManager sellingNumberPlanManager = new SellingNumberPlanManager();

            return string.Format("Backup from Numbering Plan for {0} selling number plan", sellingNumberPlanManager.GetSellingNumberPlanName(backupData.SellingNumberPlanId));
        }

        public override bool IsMatch(IStateBackupContext context, object filter)
        {
            throw new NotImplementedException();
        }
    }
}
