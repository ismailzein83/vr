using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;
using Vanrise.GenericData.Entities;

namespace TOne.WhS.SupplierPriceList.MainExtensions
{
    public class Pop3SupplierPricelistMessageFilter : VRPop3MessageFilter
    {
        public override Guid ConfigId
        {
            get { return new Guid("05382832-5CBB-46C0-8214-B3B81769FB80"); }
        }

        public override bool IsApplicableFunction(VRPop3MailMessageHeader receivedMailMessageHeader)
        {
            var carrierAccountManager = new CarrierAccountManager();
            Dictionary<string, CarrierAccount> supplierAccounts = carrierAccountManager.GetCachedSupplierAccountsByAutoImportEmail();
            var supplierAccount= supplierAccounts.GetRecord(receivedMailMessageHeader.From);
            if (supplierAccount != null && supplierAccount.CarrierAccountSettings != null && supplierAccount.CarrierAccountSettings.PriceListSettings!=null)
            {
                if (supplierAccount.CarrierAccountSettings.PriceListSettings.SubjectCode == null)
                    return true;
                if (receivedMailMessageHeader.Subject.ToLower().Contains(supplierAccount.CarrierAccountSettings.PriceListSettings.SubjectCode.ToLower()))
                    return true;
            }
            return false;
        }
    }
}
