using Vanrise.Data.RDB;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Data.RDB
{
    class StateBackupSupplierBehavior : StateBackupTypeBehavior
    {
        public override void GetBackupQueryContext(RDBQueryContext queryContext, long stateBackupId)
        {
            if (base.Data is StateBackupSupplier backupSupplierData)
            {
                int supplierId = backupSupplierData.SupplierId;

                var supplierPriceListDataManager = new SupplierPriceListDataManager();
                supplierPriceListDataManager.BackupBySupplierId(queryContext, stateBackupId, this.BackupDatabaseName, supplierId);

                var supplierZoneDataManager = new SupplierZoneDataManager();
                supplierZoneDataManager.BackupBySupplierId(queryContext, stateBackupId, this.BackupDatabaseName, supplierId);

                var supplierCodeDataManager = new SupplierCodeDataManager();
                supplierCodeDataManager.BackupBySupplierId(queryContext, stateBackupId, this.BackupDatabaseName, supplierId);

                var supplierRateDataManager = new SupplierRateDataManager();
                supplierRateDataManager.BackupBySupplierId(queryContext, stateBackupId, this.BackupDatabaseName, supplierId);

                var supplierZoneServiceDataManager = new SupplierZoneServiceDataManager();
                supplierZoneServiceDataManager.BackupBySupplierId(queryContext, stateBackupId, this.BackupDatabaseName, supplierId);
            }
        }

        public override void GetRestoreCommands(RDBQueryContext queryContext, long stateBackupId)
        {
            if (base.Data is StateBackupSupplier backupSupplierData)
            {
                int supplierId = backupSupplierData.SupplierId;

                var supplierPriceListDataManager = new SupplierPriceListDataManager();
                var supplierZoneDataManager = new SupplierZoneDataManager();
                var supplierCodeDataManager = new SupplierCodeDataManager();
                var supplierRateDataManager = new SupplierRateDataManager();
                var supplierZoneServiceDataManager = new SupplierZoneServiceDataManager();

                supplierRateDataManager.SetDeleteQueryBySupplierId(queryContext, supplierId);
                supplierCodeDataManager.SetDeleteQueryBySupplierId(queryContext, supplierId);
                supplierZoneServiceDataManager.SetDeleteQueryBySupplierId(queryContext, supplierId);
                supplierZoneDataManager.SetDeleteQueryBySupplierId(queryContext, supplierId);
                supplierPriceListDataManager.SetDeleteQueryBySupplierId(queryContext, supplierId);

                supplierPriceListDataManager.GetRestoreQuery(queryContext, stateBackupId, this.BackupDatabaseName);
                supplierZoneDataManager.GetRestoreQuery(queryContext, stateBackupId, this.BackupDatabaseName);
                supplierCodeDataManager.GetRestoreQuery(queryContext, stateBackupId, this.BackupDatabaseName);
                supplierRateDataManager.GetRestoreQuery(queryContext, stateBackupId, this.BackupDatabaseName);
                supplierZoneServiceDataManager.GetRestoreQuery(queryContext, stateBackupId, this.BackupDatabaseName);
            }
        }
    }
}
