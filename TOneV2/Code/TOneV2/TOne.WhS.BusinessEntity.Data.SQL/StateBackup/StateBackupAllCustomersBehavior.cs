using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Data.SQL
{
    public class StateBackupAllCustomersBehavior : StateBackupTypeBehavior
    {
        public override string GetBackupCommand(int stateBackupId)
        {
            SalePriceListDataManager salePriceListManager = new SalePriceListDataManager();
            return salePriceListManager.BackupAllData(stateBackupId, base.BackupDatabaseName);
        }

        public override string GetRestoreCommand(int stateBackupId)
        {
            throw new NotImplementedException();
        }
    }
}
