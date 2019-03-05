using Vanrise.Data.RDB;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Data.RDB
{
    public class StateBackupAllSaleEntitiesBehavior : StateBackupTypeBehavior
    {
        public override void GetBackupQueryContext(RDBQueryContext queryContext, long stateBackupId)
        {
            if (base.Data is StateBackupAllSaleEntities backupAllSaleEntities)
            {
                int sellingNumberPlanId = backupAllSaleEntities.SellingNumberPlanId;

                var salePriceListDataManager = new SalePriceListDataManager();
                salePriceListDataManager.BackupBySNPId(queryContext, stateBackupId, this.BackupDatabaseName, sellingNumberPlanId);

                var saleZoneDataManager = new SaleZoneDataManager();
                saleZoneDataManager.BackupBySNPId(queryContext, stateBackupId, this.BackupDatabaseName, sellingNumberPlanId);

                var saleCodeDataManager = new SaleCodeDataManager();
                saleCodeDataManager.BackupBySNPId(queryContext, stateBackupId, this.BackupDatabaseName, sellingNumberPlanId);

                var saleRateDataManager = new SaleRateDataManager();
                //  saleRateDataManager.BackupBySNPId(queryContext, stateBackupId, this.BackupDatabaseName, sellingNumberPlanId);

                var saleEntityServiceDataManager = new SaleEntityServiceDataManager();
                saleEntityServiceDataManager.BackupBySNPId(queryContext, stateBackupId, this.BackupDatabaseName, sellingNumberPlanId);

                var saleEntityRoutingProductDataManager = new SaleEntityRoutingProductDataManager();
                saleEntityRoutingProductDataManager.BackupBySNPId(queryContext, stateBackupId, this.BackupDatabaseName, sellingNumberPlanId);

                var customerCountryDataManager = new CustomerCountryDataManager();
                customerCountryDataManager.BackupBySNPId(queryContext, stateBackupId, this.BackupDatabaseName, sellingNumberPlanId);
            }
        }

        public override void GetRestoreCommands(RDBQueryContext queryContext, long stateBackupId)
        {
            if (base.Data is StateBackupAllSaleEntities backupAllSaleEntities)
            {
                int sellingNumberPlanId = backupAllSaleEntities.SellingNumberPlanId;

                var saleZoneDataManager = new SaleZoneDataManager();
                var saleRateDataManager = new SaleRateDataManager();
                var saleCodeDataManager = new SaleCodeDataManager();
                var salePriceListDataManager = new SalePriceListDataManager();
                var customerCountryDataManager = new CustomerCountryDataManager();
                var saleEntityServiceDataManager = new SaleEntityServiceDataManager();
                var saleEntityRoutingProductDataManager = new SaleEntityRoutingProductDataManager();

                // saleRateDataManager.SetDeleteQueryBySNPId(queryContext, sellingNumberPlanId);
                saleCodeDataManager.SetDeleteQueryBySNPId(queryContext, sellingNumberPlanId);
                saleEntityServiceDataManager.SetDeleteQueryBySNPId(queryContext, sellingNumberPlanId);
                saleEntityRoutingProductDataManager.SetDeleteQueryBySNPId(queryContext, sellingNumberPlanId);
                saleZoneDataManager.SetDeleteQueryBySNPId(queryContext, sellingNumberPlanId);
                salePriceListDataManager.SetDeleteQueryBySNPId(queryContext, sellingNumberPlanId);
                customerCountryDataManager.SetDeleteQueryBySNPId(queryContext, sellingNumberPlanId);



                salePriceListDataManager.SetRestoreQuery(queryContext, stateBackupId, this.BackupDatabaseName);
                saleZoneDataManager.SetRestoreQuery(queryContext, stateBackupId, this.BackupDatabaseName);
                saleCodeDataManager.SetRestoreQuery(queryContext, stateBackupId, this.BackupDatabaseName);
                //  saleRateDataManager.SetRestoreQuery(queryContext, stateBackupId, this.BackupDatabaseName);
                saleEntityServiceDataManager.SetRestoreQuery(queryContext, stateBackupId, this.BackupDatabaseName);
                saleEntityRoutingProductDataManager.SetRestoreQuery(queryContext, stateBackupId, this.BackupDatabaseName);
                customerCountryDataManager.SetRestoreQuery(queryContext, stateBackupId, this.BackupDatabaseName);
            }
        }
    }
}
