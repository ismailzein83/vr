using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Data.SQL
{
    public class StateBackupSupplierBehavior : StateBackupTypeBehavior
    {
        public override string GetBackupCommands(long stateBackupId)
        {
            StateBackupSupplier backupSupplierData = base.Data as StateBackupSupplier;
            int supplierId = backupSupplierData.SupplierId;

            SupplierPriceListDataManager supplierPriceListDataManager = new SupplierPriceListDataManager();
            string result = supplierPriceListDataManager.BackupAllDataBySupplierId(stateBackupId, base.BackupDatabaseName, supplierId);

            SupplierZoneDataManager supplierZoneDataManager = new SupplierZoneDataManager();
            result = string.Concat(result, supplierZoneDataManager.BackupAllDataBySupplierId(stateBackupId, base.BackupDatabaseName, supplierId));

            SupplierCodeDataManager supplierCodeDataManager = new SupplierCodeDataManager();
            result = string.Concat(result, supplierCodeDataManager.BackupAllDataBySupplierId(stateBackupId, base.BackupDatabaseName, supplierId));

            SupplierRateDataManager supplierRateDataManager = new SupplierRateDataManager();
            result = string.Concat(result, supplierRateDataManager.BackupAllDataBySupplierId(stateBackupId, base.BackupDatabaseName, supplierId));

            SupplierZoneServiceDataManager supplierZoneServiceDataManager = new SupplierZoneServiceDataManager();
            result = string.Concat(result, supplierZoneServiceDataManager.BackupAllDataBySupplierId(stateBackupId, base.BackupDatabaseName, supplierId));

            return result;
        }

        public override string GetRestoreCommands(long stateBackupId)
        {
            throw new NotImplementedException();
        }
    }
}
