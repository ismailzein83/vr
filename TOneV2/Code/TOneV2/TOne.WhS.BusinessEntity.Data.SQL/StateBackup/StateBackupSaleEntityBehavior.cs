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
            var ownerIds = new List<int> { ownerId };

            SalePriceListDataManager salePriceListDataManager = new SalePriceListDataManager();
            backupCommand.AppendLine(salePriceListDataManager.BackupAllDataByOwner(stateBackupId, base.BackupDatabaseName, ownerIds, ownerType));

            SaleRateDataManager saleRateDataManager = new SaleRateDataManager();
            backupCommand.AppendLine(saleRateDataManager.BackupAllDataByOwner(stateBackupId, base.BackupDatabaseName, ownerIds, ownerType));

            SaleEntityServiceDataManager saleEntityServiceDataManager = new SaleEntityServiceDataManager();
            backupCommand.AppendLine(saleEntityServiceDataManager.BackupAllSaleEntityZoneServiceDataByOwner(stateBackupId, base.BackupDatabaseName, ownerId, ownerType));

            SaleEntityRoutingProductDataManager saleEntityRoutingProductDataManager = new SaleEntityRoutingProductDataManager();
            backupCommand.AppendLine(saleEntityRoutingProductDataManager.BackupAllSaleEntityRoutingProductDataByByOwner(stateBackupId, base.BackupDatabaseName, ownerId, ownerType));

            CustomerCountryDataManager customerCountryDataManager = new CustomerCountryDataManager();

            if (backupSaleEntityData.OwnerType == SalePriceListOwnerType.SellingProduct && backupSaleEntityData.SellingProductCustomerIds.Any())
            {
                var customerIds = backupSaleEntityData.SellingProductCustomerIds;
                backupCommand.AppendLine(customerCountryDataManager.BackupSaleEntityCustomerCountryByOwner(stateBackupId, BackupDatabaseName, customerIds));
                backupCommand.AppendLine(salePriceListDataManager.BackupAllDataByOwner(stateBackupId, base.BackupDatabaseName, customerIds, (int)SalePriceListOwnerType.Customer));
                backupCommand.AppendLine(saleRateDataManager.BackupAllDataByOwner(stateBackupId, base.BackupDatabaseName, customerIds, (int)SalePriceListOwnerType.Customer));
            }
            else
                backupCommand.AppendLine(customerCountryDataManager.BackupSaleEntityCustomerCountryByOwner(stateBackupId, BackupDatabaseName, ownerIds));

            return backupCommand.ToString();
        }

        public override string GetRestoreCommands(long stateBackupId)
        {
            StateBackupSaleEntity backupSaleEntityData = base.Data as StateBackupSaleEntity;
            int ownerId = backupSaleEntityData.OwnerId;
            int ownerType = (int)backupSaleEntityData.OwnerType;

            StringBuilder restoreCommands = new StringBuilder();
            var ownerIds = new List<int> { ownerId };

            SalePriceListDataManager salePriceListDataManager = new SalePriceListDataManager();
            SaleRateDataManager saleRateDataManager = new SaleRateDataManager();
            SaleEntityServiceDataManager saleEntityServiceDataManager = new SaleEntityServiceDataManager();
            SaleEntityRoutingProductDataManager saleEntityRoutingProductDataManager = new SaleEntityRoutingProductDataManager();
            CustomerCountryDataManager customerCountryDataManager = new CustomerCountryDataManager();

            restoreCommands.AppendLine(saleRateDataManager.GetDeleteCommandsByOwner(ownerIds, ownerType));
            restoreCommands.AppendLine(saleEntityServiceDataManager.GetDeleteCommandsByOwner(ownerId, ownerType));
            restoreCommands.AppendLine(saleEntityRoutingProductDataManager.GetDeleteCommandsByOwner(ownerId, ownerType));
            restoreCommands.AppendLine(salePriceListDataManager.GetDeleteCommandsByOwner(ownerIds, ownerType));

            if (backupSaleEntityData.OwnerType == SalePriceListOwnerType.SellingProduct)
            {
                var customerIds = backupSaleEntityData.SellingProductCustomerIds;
                restoreCommands.AppendLine(customerCountryDataManager.GetDeleteCommandsByOwner(customerIds));
                restoreCommands.AppendLine(saleRateDataManager.GetDeleteCommandsByOwner(customerIds, (int)SalePriceListOwnerType.Customer));
                restoreCommands.AppendLine(salePriceListDataManager.GetDeleteCommandsByOwner(customerIds, (int)SalePriceListOwnerType.Customer));
            }
            else
                restoreCommands.AppendLine(customerCountryDataManager.GetDeleteCommandsByOwner(ownerIds));

            restoreCommands.AppendLine(salePriceListDataManager.GetRestoreCommands(stateBackupId, base.BackupDatabaseName));
            restoreCommands.AppendLine(saleRateDataManager.GetRestoreCommands(stateBackupId, base.BackupDatabaseName));
            restoreCommands.AppendLine(saleEntityServiceDataManager.GetRestoreCommands(stateBackupId, base.BackupDatabaseName));
            restoreCommands.AppendLine(saleEntityRoutingProductDataManager.GetRestoreCommands(stateBackupId, base.BackupDatabaseName));
            restoreCommands.AppendLine(customerCountryDataManager.GetRestoreCommands(stateBackupId, BackupDatabaseName));

            return restoreCommands.ToString();
        }
    }
}
