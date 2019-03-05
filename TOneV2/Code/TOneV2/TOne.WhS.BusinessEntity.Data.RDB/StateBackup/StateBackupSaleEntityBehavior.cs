using System.Linq;
using Vanrise.Data.RDB;
using System.Collections.Generic;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Data.RDB
{
    public class StateBackupSaleEntityBehavior : StateBackupTypeBehavior
    {
        public override void GetBackupQueryContext(RDBQueryContext queryContext, long stateBackupId)
        {
            if (base.Data is StateBackupSaleEntity backupSaleEntityData)
            {
                int ownerId = backupSaleEntityData.OwnerId;
                int ownerType = (int)backupSaleEntityData.OwnerType;
                var ownerIds = new List<int> { ownerId };

                var salePriceListDataManager = new SalePriceListDataManager();
                salePriceListDataManager.BackupByOwner(queryContext, stateBackupId, this.BackupDatabaseName, ownerIds, ownerType);

                var saleRateDataManager = new SaleRateDataManager();
                saleRateDataManager.BackupByOwner(queryContext, stateBackupId, this.BackupDatabaseName, ownerIds, ownerType);

                var saleEntityServiceDataManager = new SaleEntityServiceDataManager();
                saleEntityServiceDataManager.BackupByOwner(queryContext, stateBackupId, this.BackupDatabaseName, ownerId, ownerType);

                var saleEntityRoutingProductDataManager = new SaleEntityRoutingProductDataManager();
                saleEntityRoutingProductDataManager.BackupByOwner(queryContext, stateBackupId, this.BackupDatabaseName, ownerId, ownerType);

                var customerCountryDataManager = new CustomerCountryDataManager();
                if (backupSaleEntityData.OwnerType == SalePriceListOwnerType.Customer)
                {
                    customerCountryDataManager.BackupByOwner(queryContext, stateBackupId, this.BackupDatabaseName, ownerIds);
                }
                else if (backupSaleEntityData.SellingProductCustomerIds.Any())
                {
                    var customerIds = backupSaleEntityData.SellingProductCustomerIds;
                    customerCountryDataManager.BackupByOwner(queryContext, stateBackupId, this.BackupDatabaseName, customerIds);
                    salePriceListDataManager.BackupByOwner(queryContext, stateBackupId, this.BackupDatabaseName, customerIds, (int)SalePriceListOwnerType.Customer);
                    saleRateDataManager.BackupByOwner(queryContext, stateBackupId, this.BackupDatabaseName, customerIds, (int)SalePriceListOwnerType.Customer);
                }
            }
        }

        public override void GetRestoreCommands(RDBQueryContext queryContext, long stateBackupId)
        {
            if (base.Data is StateBackupSaleEntity backupSaleEntityData)
            {
                int ownerId = backupSaleEntityData.OwnerId;
                int ownerType = (int)backupSaleEntityData.OwnerType;

                var ownerIds = new List<int> { ownerId };

                var saleRateDataManager = new SaleRateDataManager();
                var salePriceListDataManager = new SalePriceListDataManager();
                var customerCountryDataManager = new CustomerCountryDataManager();
                var saleEntityServiceDataManager = new SaleEntityServiceDataManager();
                var saleEntityRoutingProductDataManager = new SaleEntityRoutingProductDataManager();

                saleRateDataManager.SetDeleteQueryByOwner(queryContext, ownerIds, ownerType);
                saleEntityServiceDataManager.SetDeleteQueryByOwner(queryContext, ownerId, ownerType);
                saleEntityRoutingProductDataManager.SetDeleteQueryByOwner(queryContext, ownerId, ownerType);
                salePriceListDataManager.SetDeleteQueryByOwner(queryContext, ownerIds, ownerType);
            
                if (backupSaleEntityData.OwnerType == SalePriceListOwnerType.SellingProduct)
                {
                    var customerIds = backupSaleEntityData.SellingProductCustomerIds;
                    customerCountryDataManager.SetDeleteQueryByOwner(queryContext, customerIds);
                    saleRateDataManager.SetDeleteQueryByOwner(queryContext, customerIds, (int)SalePriceListOwnerType.Customer);
                    salePriceListDataManager.SetDeleteQueryByOwner(queryContext, customerIds, (int)SalePriceListOwnerType.Customer);
                }
                else
                    customerCountryDataManager.SetDeleteQueryByOwner(queryContext, ownerIds);

                salePriceListDataManager.SetRestoreQuery(queryContext, stateBackupId, this.BackupDatabaseName);
                saleRateDataManager.SetRestoreQuery(queryContext, stateBackupId, this.BackupDatabaseName);
                saleEntityServiceDataManager.SetRestoreQuery(queryContext, stateBackupId, this.BackupDatabaseName);
                saleEntityRoutingProductDataManager.SetRestoreQuery(queryContext, stateBackupId, this.BackupDatabaseName);
                customerCountryDataManager.SetRestoreQuery(queryContext, stateBackupId, this.BackupDatabaseName);
            }
        }
    }
}
