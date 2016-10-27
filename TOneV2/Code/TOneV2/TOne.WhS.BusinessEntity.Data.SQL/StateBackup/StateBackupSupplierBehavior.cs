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
            StringBuilder backupCommand = new StringBuilder();
            backupCommand.AppendLine(supplierPriceListDataManager.BackupAllDataBySupplierId(stateBackupId, base.BackupDatabaseName, supplierId));

            SupplierZoneDataManager supplierZoneDataManager = new SupplierZoneDataManager();
            backupCommand.AppendLine(supplierZoneDataManager.BackupAllDataBySupplierId(stateBackupId, base.BackupDatabaseName, supplierId));

            SupplierCodeDataManager supplierCodeDataManager = new SupplierCodeDataManager();
            backupCommand.AppendLine(supplierCodeDataManager.BackupAllDataBySupplierId(stateBackupId, base.BackupDatabaseName, supplierId));

            SupplierRateDataManager supplierRateDataManager = new SupplierRateDataManager();
            backupCommand.AppendLine(supplierRateDataManager.BackupAllDataBySupplierId(stateBackupId, base.BackupDatabaseName, supplierId));

            SupplierZoneServiceDataManager supplierZoneServiceDataManager = new SupplierZoneServiceDataManager();
            backupCommand.AppendLine(supplierZoneServiceDataManager.BackupAllDataBySupplierId(stateBackupId, base.BackupDatabaseName, supplierId));

            return backupCommand.ToString();
        }

        public override string GetRestoreCommands(long stateBackupId)
        {
            StateBackupSupplier backupSupplierData = base.Data as StateBackupSupplier;
            int supplierId = backupSupplierData.SupplierId;


            StringBuilder restoreCommands = new StringBuilder();

            SupplierPriceListDataManager supplierPriceListDataManager = new SupplierPriceListDataManager();
            SupplierZoneDataManager supplierZoneDataManager = new SupplierZoneDataManager();
            SupplierCodeDataManager supplierCodeDataManager = new SupplierCodeDataManager();
            SupplierRateDataManager supplierRateDataManager = new SupplierRateDataManager();
            SupplierZoneServiceDataManager supplierZoneServiceDataManager = new SupplierZoneServiceDataManager();

            restoreCommands.AppendLine(supplierRateDataManager.GetDeleteCommandsBySupplierId(supplierId));
            restoreCommands.AppendLine(supplierCodeDataManager.GetDeleteCommandsBySupplierId(supplierId));
            restoreCommands.AppendLine(supplierZoneServiceDataManager.GetDeleteCommandsBySupplierId(supplierId));
            restoreCommands.AppendLine(supplierZoneDataManager.GetDeleteCommandsBySupplierId(supplierId));
            restoreCommands.AppendLine(supplierPriceListDataManager.GetDeleteCommandsBySupplierId(supplierId));


            restoreCommands.AppendLine(supplierPriceListDataManager.GetRestoreCommands(stateBackupId, base.BackupDatabaseName));
            restoreCommands.AppendLine(supplierZoneDataManager.GetRestoreCommands(stateBackupId, base.BackupDatabaseName));
            restoreCommands.AppendLine(supplierCodeDataManager.GetRestoreCommands(stateBackupId, base.BackupDatabaseName));
            restoreCommands.AppendLine(supplierRateDataManager.GetRestoreCommands(stateBackupId, base.BackupDatabaseName));
            restoreCommands.AppendLine(supplierZoneServiceDataManager.GetRestoreCommands(stateBackupId, base.BackupDatabaseName));

            return restoreCommands.ToString();
        }
    }
}
