using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Common.Business;

namespace TOne.WhS.BusinessEntity.Business
{
    public class SaleEntityStateBackupBehavior : StateBackupBehavior
    {
        public override string GetDescription(IStateBackupContext context)
        {
            if (context.Data == null)
                return string.Empty;

            string restoreText = base.GetDescription(context);

            if (restoreText != null)
                return restoreText;

            StateBackupSaleEntity backupData = context.Data as StateBackupSaleEntity;
            if (backupData.OwnerType == SalePriceListOwnerType.Customer)
            {
                CarrierAccountManager carrierAccountManager = new CarrierAccountManager();
                var description = String.Format("Backup for customer {0}", carrierAccountManager.GetCarrierAccountName(backupData.OwnerId));
                if (backupData.MasterOwnerId.HasValue)
                {
                    string test = String.Format(" copy from master customer {0}", carrierAccountManager.GetCarrierAccountName(backupData.MasterOwnerId.Value));
                    description = string.Concat(description, test);
                }
                return description;
            }
            else
            {
                SellingProductManager sellingProductManager = new SellingProductManager();
                return string.Format("Backup for selling product {0}", sellingProductManager.GetSellingProductName(backupData.OwnerId));
            }
        }

        public override bool IsMatch(IStateBackupContext context, object filter)
        {
            if (context.Data == null)
                return false;

            StateBackupSaleEntity backupData = context.Data as StateBackupSaleEntity;
            SaleEntityStateBackupFilter filterData = filter as SaleEntityStateBackupFilter;

            if (filterData.OwnerIds == null && filterData.OwnerType.HasValue)
                return filterData.OwnerType == backupData.OwnerType;

            else if (!filterData.OwnerType.HasValue)
                return true;

            return filterData.OwnerIds.Contains(backupData.OwnerId);
        }

        public override bool CanRestore(IStateBackupCanRestoreContext context)
        {
            StateBackupSaleEntity backupData = context.StateBackupType as StateBackupSaleEntity;

            if (backupData == null)
                throw new ArgumentException("StateBackupType must be of type StateBackupSaleEntity");

            if (backupData.OwnerType == SalePriceListOwnerType.Customer)
                return true;

            string errorMessage = "Cannot restore this selling product, data might be lost";

            StateBackupManager manager = new StateBackupManager();
            IEnumerable<StateBackup> stateBackups = manager.GetStateBackupsAfterId(context.StateBackupId);

            var sellingProductManager = new SellingProductManager();
            var sellingProduct = sellingProductManager.GetSellingProduct(backupData.OwnerId);

            foreach (var stateBackup in stateBackups)
            {
                if (stateBackup.Info.OnRestoreStateBackupId.HasValue || stateBackup.RestoreDate.HasValue)
                    continue;

                if (stateBackup.Info is StateBackupSaleEntity)
                {
                    var backupType = stateBackup.Info as StateBackupSaleEntity;
                    
                    if (backupData.SellingProductCustomerIds.Contains(backupType.OwnerId))
                    {
                        context.ErrorMessage = errorMessage;
                        return false;
                    }
                }
                else if (stateBackup.Info is StateBackupAllSaleEntities)
                {
                    var backupType = stateBackup.Info as StateBackupAllSaleEntities;

                    if (backupType.SellingNumberPlanId == sellingProduct.SellingNumberPlanId)
                    {
                        context.ErrorMessage = errorMessage;
                        return false;
                    }
                }
            }
            return true;
        }
    }
}
