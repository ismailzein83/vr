using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Data.SQL
{
    public class StateBackupAllSaleEntitiesBehavior : StateBackupTypeBehavior
    {
        public override string GetBackupCommands(long stateBackupId)
        {
            StateBackupAllSaleEntities backupAllSaleEntities = base.Data as StateBackupAllSaleEntities;
            int sellingNumberPlanId = backupAllSaleEntities.SellingNumberPlanId;

            StringBuilder backupCommand = new StringBuilder();

            SalePriceListDataManager salePriceListDataManager = new SalePriceListDataManager();
            backupCommand.AppendLine(salePriceListDataManager.BackupAllDataBySellingNumberingPlanId(stateBackupId, base.BackupDatabaseName, sellingNumberPlanId));

            SaleZoneDataManager saleZoneDataManager = new SaleZoneDataManager();
            backupCommand.AppendLine(saleZoneDataManager.BackupAllDataBySellingNumberingPlanId(stateBackupId, base.BackupDatabaseName, sellingNumberPlanId));

            SaleCodeDataManager saleCodeDataManager = new SaleCodeDataManager();
            backupCommand.AppendLine(saleCodeDataManager.BackupAllDataBySellingNumberingPlanId(stateBackupId, base.BackupDatabaseName, sellingNumberPlanId));

            SaleRateDataManager saleRateDataManager = new SaleRateDataManager();
            backupCommand.AppendLine(saleRateDataManager.BackupAllDataBySellingNumberingPlanId(stateBackupId, base.BackupDatabaseName, sellingNumberPlanId));

            SaleEntityServiceDataManager saleEntityServiceDataManager = new SaleEntityServiceDataManager();
            backupCommand.AppendLine(saleEntityServiceDataManager.BackupAllSaleEntityZoneServiceDataBySellingNumberPlanId(stateBackupId, base.BackupDatabaseName, sellingNumberPlanId));

            SaleEntityRoutingProductDataManager saleEntityRoutingProductDataManager = new SaleEntityRoutingProductDataManager();
            backupCommand.AppendLine(saleEntityRoutingProductDataManager.BackupAllSaleEntityRoutingProductDataBySellingNumberPlanId(stateBackupId, base.BackupDatabaseName, sellingNumberPlanId));
            
            return backupCommand.ToString();
        }

        public override string GetRestoreCommands(long stateBackupId)
        {
            StateBackupAllSaleEntities backupAllSaleEntities = base.Data as StateBackupAllSaleEntities;
            int sellingNumberPlanId = backupAllSaleEntities.SellingNumberPlanId;

            StringBuilder restoreCommands = new StringBuilder();

            SalePriceListDataManager salePriceListDataManager = new SalePriceListDataManager();
            SaleZoneDataManager saleZoneDataManager = new SaleZoneDataManager();
            SaleCodeDataManager saleCodeDataManager = new SaleCodeDataManager();
            SaleRateDataManager saleRateDataManager = new SaleRateDataManager();
            SaleEntityServiceDataManager saleEntityServiceDataManager = new SaleEntityServiceDataManager();
            SaleEntityRoutingProductDataManager saleEntityRoutingProductDataManager = new SaleEntityRoutingProductDataManager();

            restoreCommands.AppendLine(saleRateDataManager.GetDeleteCommandsBySellingNumberPlanId(sellingNumberPlanId));
            restoreCommands.AppendLine(saleCodeDataManager.GetDeleteCommandsBySellingNumberPlanId(sellingNumberPlanId));
            restoreCommands.AppendLine(saleEntityServiceDataManager.GetDeleteCommandsBySellingNumberPlanId(sellingNumberPlanId));
            restoreCommands.AppendLine(saleEntityRoutingProductDataManager.GetDeleteCommandsBySellingNumberPlanId(sellingNumberPlanId));
            restoreCommands.AppendLine(saleZoneDataManager.GetDeleteCommandsBySellingNumberPlanId(sellingNumberPlanId));
            restoreCommands.AppendLine(salePriceListDataManager.GetDeleteCommandsBySellingNumberPlanId(sellingNumberPlanId));


            restoreCommands.AppendLine(salePriceListDataManager.GetRestoreCommands(stateBackupId, base.BackupDatabaseName));
            restoreCommands.AppendLine(saleZoneDataManager.GetRestoreCommands(stateBackupId, base.BackupDatabaseName));
            restoreCommands.AppendLine(saleCodeDataManager.GetRestoreCommands(stateBackupId, base.BackupDatabaseName));
            restoreCommands.AppendLine(saleRateDataManager.GetRestoreCommands(stateBackupId, base.BackupDatabaseName));
            restoreCommands.AppendLine(saleEntityServiceDataManager.GetRestoreCommands(stateBackupId, base.BackupDatabaseName));
            restoreCommands.AppendLine(saleEntityRoutingProductDataManager.GetRestoreCommands(stateBackupId, base.BackupDatabaseName));

            return restoreCommands.ToString();
        }
 
    }
}
