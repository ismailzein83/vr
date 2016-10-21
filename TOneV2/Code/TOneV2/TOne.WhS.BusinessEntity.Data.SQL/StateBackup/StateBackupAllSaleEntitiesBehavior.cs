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

            SalePriceListDataManager salePriceListDataManager = new SalePriceListDataManager();
            string result = salePriceListDataManager.BackupAllDataBySellingNumberingPlanId(stateBackupId, base.BackupDatabaseName, sellingNumberPlanId);

            SaleZoneDataManager saleZoneDataManager = new SaleZoneDataManager();
            result = string.Concat(result, saleZoneDataManager.BackupAllDataBySellingNumberingPlanId(stateBackupId, base.BackupDatabaseName, sellingNumberPlanId));

            SaleCodeDataManager saleCodeDataManager = new SaleCodeDataManager();
            result = string.Concat(result, saleCodeDataManager.BackupAllDataBySellingNumberingPlanId(stateBackupId, base.BackupDatabaseName, sellingNumberPlanId));

            SaleRateDataManager saleRateDataManager = new SaleRateDataManager();
            result = string.Concat(result, saleRateDataManager.BackupAllDataBySellingNumberingPlanId(stateBackupId, base.BackupDatabaseName, sellingNumberPlanId));

            SaleEntityServiceDataManager saleEntityServiceDataManager = new SaleEntityServiceDataManager();
            result = string.Concat(result, saleEntityServiceDataManager.BackupAllSaleEntityZoneServiceDataBySellingNumberPlanId(stateBackupId, base.BackupDatabaseName, sellingNumberPlanId));

            result = string.Concat(result, saleEntityServiceDataManager.BackupAllSaleEntityRoutingProductDataBySellingNumberPlanId(stateBackupId, base.BackupDatabaseName, sellingNumberPlanId));
            
            return result;
        }

        public override string GetRestoreCommands(long stateBackupId)
        {
            throw new NotImplementedException();
        }
    }
}
