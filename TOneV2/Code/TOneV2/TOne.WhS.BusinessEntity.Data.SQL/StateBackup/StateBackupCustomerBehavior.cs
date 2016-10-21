﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Data.SQL
{
    public class StateBackupCustomerBehavior : StateBackupTypeBehavior
    {
        public override string GetBackupCommands(int stateBackupId)
        {
            StateBackupCustomer backupCustomerData = base.Data as StateBackupCustomer;
            int customerId = backupCustomerData.CustomerId;
            return null;
            //SalePriceListDataManager salePriceListManager = new SalePriceListDataManager();
            //return salePriceListManager.BackupAllDataByCustomerId(stateBackupId, base.BackupDatabaseName, customerId);
        }

        public override string GetRestoreCommands(int stateBackupId)
        {
            throw new NotImplementedException();
        }
    }
}
