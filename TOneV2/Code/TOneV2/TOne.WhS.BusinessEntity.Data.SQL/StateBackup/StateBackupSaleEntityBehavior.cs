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
            StateBackupSaleEntity backupSaleEntityData = base.Data as StateBackupSaleEntity;
            int ownerId = backupSaleEntityData.OwnerId;
            int ownerType = (int)backupSaleEntityData.OwnerType;

            StringBuilder backupCommand = new StringBuilder();

            SalePriceListDataManager salePriceListDataManager = new SalePriceListDataManager();
            backupCommand.AppendLine(salePriceListDataManager.BackupAllDataByOwner(stateBackupId, base.BackupDatabaseName, ownerId, ownerType));

            SaleRateDataManager saleRateDataManager = new SaleRateDataManager();
            backupCommand.AppendLine(saleRateDataManager.BackupAllDataByOwner(stateBackupId, base.BackupDatabaseName, ownerId, ownerType));

            SaleEntityServiceDataManager saleEntityServiceDataManager = new SaleEntityServiceDataManager();
            backupCommand.AppendLine(saleEntityServiceDataManager.BackupAllSaleEntityZoneServiceDataByOwner(stateBackupId, base.BackupDatabaseName, ownerId, ownerType));

            SaleEntityRoutingProductDataManager saleEntityRoutingProductDataManager = new SaleEntityRoutingProductDataManager();
            backupCommand.AppendLine(saleEntityRoutingProductDataManager.BackupAllSaleEntityRoutingProductDataByByOwner(stateBackupId, base.BackupDatabaseName, ownerId, ownerType));

            if (backupSaleEntityData.OwnerType == SalePriceListOwnerType.Customer)
            {
                CustomerCountryDataManager customerCountryDataManager = new CustomerCountryDataManager();
                backupCommand.AppendLine(customerCountryDataManager.BackupSaleEntityCustomerCountryByOwner(stateBackupId,
                    BackupDatabaseName, ownerId));
            }

            return backupCommand.ToString();
        }

        public override string GetRestoreCommands(long stateBackupId)
        {
            StateBackupSaleEntity backupSaleEntityData = base.Data as StateBackupSaleEntity;
            int ownerId = backupSaleEntityData.OwnerId;
            int ownerType = (int)backupSaleEntityData.OwnerType;

            StringBuilder restoreCommands = new StringBuilder();

            SalePriceListDataManager salePriceListDataManager = new SalePriceListDataManager();
            SaleRateDataManager saleRateDataManager = new SaleRateDataManager();
            SaleEntityServiceDataManager saleEntityServiceDataManager = new SaleEntityServiceDataManager();
            SaleEntityRoutingProductDataManager saleEntityRoutingProductDataManager = new SaleEntityRoutingProductDataManager();
            CustomerCountryDataManager customerCountryDataManager = new CustomerCountryDataManager();

            restoreCommands.AppendLine(saleRateDataManager.GetDeleteCommandsByOwner(ownerId, ownerType));
            restoreCommands.AppendLine(saleEntityServiceDataManager.GetDeleteCommandsByOwner(ownerId, ownerType));
            restoreCommands.AppendLine(saleEntityRoutingProductDataManager.GetDeleteCommandsByOwner(ownerId, ownerType));
            restoreCommands.AppendLine(salePriceListDataManager.GetDeleteCommandsByOwner(ownerId, ownerType));
            if (backupSaleEntityData.OwnerType == SalePriceListOwnerType.Customer)
                restoreCommands.AppendLine(customerCountryDataManager.GetDeleteCommandsByOwner(ownerId));

            restoreCommands.AppendLine(salePriceListDataManager.GetRestoreCommands(stateBackupId, base.BackupDatabaseName));
            restoreCommands.AppendLine(saleRateDataManager.GetRestoreCommands(stateBackupId, base.BackupDatabaseName));
            restoreCommands.AppendLine(saleEntityServiceDataManager.GetRestoreCommands(stateBackupId, base.BackupDatabaseName));
            restoreCommands.AppendLine(saleEntityRoutingProductDataManager.GetRestoreCommands(stateBackupId, base.BackupDatabaseName));
            if (backupSaleEntityData.OwnerType == SalePriceListOwnerType.Customer)
                restoreCommands.AppendLine(customerCountryDataManager.GetRestoreCommands(stateBackupId,
                    BackupDatabaseName));

            return restoreCommands.ToString();
        }
    }
}
