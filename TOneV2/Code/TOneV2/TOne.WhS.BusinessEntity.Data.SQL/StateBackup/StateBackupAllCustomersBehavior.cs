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
        public override string GetBackupCommands(int stateBackupId)
        {
            //It should return all tables tht should have backup for this type all customers
            SalePriceListDataManager salePriceListManager = new SalePriceListDataManager();
            return salePriceListManager.BackupAllData(stateBackupId, base.BackupDatabaseName);
        }

        public override string GetRestoreCommands(int stateBackupId)
        {
            throw new NotImplementedException();
        }
    }
}
