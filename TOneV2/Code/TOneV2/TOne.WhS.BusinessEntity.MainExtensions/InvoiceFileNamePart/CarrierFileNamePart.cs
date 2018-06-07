using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using Vanrise.Entities;
using Vanrise.Invoice.Entities;
using Vanrise.Common;
using TOne.WhS.BusinessEntity.Entities;
namespace TOne.WhS.BusinessEntity.MainExtensions.InvoiceFileNamePart
{
    public enum CarrierFileNamePartEnum { CompanyProfileName = 0 }
    public class CarrierFileNamePart : VRConcatenatedPartSettings<IInvoiceFileNamePartContext>
    {
        public CarrierFileNamePartEnum PartName { get; set; }
        public override Guid ConfigId
        {
            get { return new Guid("67dd74aa-33ac-4d75-93f6-0bd1adacaf41"); }
        }

        public override string GetPartText(IInvoiceFileNamePartContext context)
        {
            context.Invoice.ThrowIfNull("invoiceRecordObject");
            WHSFinancialAccountManager financialAccountManager = new WHSFinancialAccountManager();
            var financialAccount = financialAccountManager.GetFinancialAccount(Convert.ToInt32(context.Invoice.PartnerId));
            CarrierProfileManager carrierProfileManager = new CarrierProfileManager();
            CarrierProfile carrierProfile = null;
            switch (this.PartName)
            {           
                case CarrierFileNamePartEnum.CompanyProfileName :
                    if(financialAccount.CarrierAccountId.HasValue)
                    {
                        CarrierAccountManager carrierAccountManager = new CarrierAccountManager();
                        var carrierProfileId = carrierAccountManager.GetCarrierProfileId(financialAccount.CarrierAccountId.Value);
                        if (carrierProfileId.HasValue)
                        {
                            carrierProfile = carrierProfileManager.GetCarrierProfile(carrierProfileId.Value);
                            return carrierProfile.Settings.Company; 
                        }
                    }else
                    {
                         carrierProfile = carrierProfileManager.GetCarrierProfile(financialAccount.CarrierProfileId.Value);
                         carrierProfile.ThrowIfNull("carrierProfile", financialAccount.CarrierProfileId.Value);
                         carrierProfile.Settings.ThrowIfNull("carrierProfile.Settings", financialAccount.CarrierProfileId.Value);
                         return carrierProfile.Settings.Company; 
                    }
                    break;
            }
            return null;
        }
    }
}
