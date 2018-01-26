using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

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
    }
}
