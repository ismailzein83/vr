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
                if (backupData.PublisherOwnerId.HasValue)
                {
                    string test = String.Format(" copy from publisher customer {0}", carrierAccountManager.GetCarrierAccountName(backupData.PublisherOwnerId.Value));
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

            StateBackupManager manager = new StateBackupManager();
            IEnumerable<StateBackup> stateBackups = manager.GetStateBackupsAfterId(context.StateBackupId);

            int sellingNumberPlanId;
            string errorMessage = "Cannot restore this checkpoint, data might be lost";

            if (backupData.OwnerType == SalePriceListOwnerType.Customer)
            {
                var carrierAccountManager = new CarrierAccountManager();
                sellingNumberPlanId = carrierAccountManager.GetSellingNumberPlanId(backupData.OwnerId);
            }
            else
            {
                var sellingProductManager = new SellingProductManager();
                sellingNumberPlanId = sellingProductManager.GetSellingProduct(backupData.OwnerId).SellingNumberPlanId;
            }

            foreach (var stateBackup in stateBackups)
            {
                if (stateBackup.Info.OnRestoreStateBackupId.HasValue || stateBackup.RestoreDate.HasValue)
                    continue;

                if (stateBackup.Info is StateBackupSaleEntity && backupData.OwnerType == SalePriceListOwnerType.SellingProduct)
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

                    if (backupType.SellingNumberPlanId == sellingNumberPlanId)
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
