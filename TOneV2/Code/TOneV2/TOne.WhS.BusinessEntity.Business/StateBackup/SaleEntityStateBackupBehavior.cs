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

            StateBackupSaleEntity backupData = context.Data as StateBackupSaleEntity;
            if (backupData.OwnerType == SalePriceListOwnerType.Customer)
            {
                CarrierAccountManager carrierAccountManager = new CarrierAccountManager();
                return String.Format("Backup for customer {0}", carrierAccountManager.GetCarrierAccountName(backupData.OwnerId));
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
