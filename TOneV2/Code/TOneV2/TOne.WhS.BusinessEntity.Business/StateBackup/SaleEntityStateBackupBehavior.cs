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
                return String.Format("Backup from Selling Rates for {0} customer",carrierAccountManager.GetCarrierAccountName(backupData.OwnerId));
            }
            else
            {
                SellingProductManager sellingProductManager = new SellingProductManager();
                return string.Format("Backup from Selling Rates for {0} selling product", sellingProductManager.GetSellingProductName(backupData.OwnerId));
            }
        }

        public override bool IsMatch(IStateBackupContext context, object filter)
        {
            throw new NotImplementedException();
        }
    }
}
