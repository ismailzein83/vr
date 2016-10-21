using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Data.SQL
{
    public class StateBackupSaleEntityBehavior : StateBackupTypeBehavior
    {
        public override string GetBackupCommands(long stateBackupId)
        {
            StateBackupCustomer backupSaleEntityData = base.Data as StateBackupCustomer;
            int ownerId = backupSaleEntityData.OwnerId;
            int ownerType = (int)backupSaleEntityData.OwnerType;

            SalePriceListDataManager salePriceListDataManager = new SalePriceListDataManager();
            string result = salePriceListDataManager.BackupAllDataByOwner(stateBackupId, base.BackupDatabaseName, ownerId, ownerType);

            SaleRateDataManager saleRateDataManager = new SaleRateDataManager();
            result = string.Concat(result, saleRateDataManager.BackupAllDataByOwner(stateBackupId, base.BackupDatabaseName, ownerId, ownerType));

            SaleEntityServiceDataManager saleEntityServiceDataManager = new SaleEntityServiceDataManager();

            result = string.Concat(result, saleEntityServiceDataManager.BackupAllSaleEntityZoneServiceDataByOwner(stateBackupId, base.BackupDatabaseName, ownerId, ownerType));

            result = string.Concat(result, saleEntityServiceDataManager.BackupAllSaleEntityRoutingProductDataByByOwner(stateBackupId, base.BackupDatabaseName, ownerId, ownerType));

            return result;
        }

        public override string GetRestoreCommands(long stateBackupId)
        {
            throw new NotImplementedException();
        }
    }
}
