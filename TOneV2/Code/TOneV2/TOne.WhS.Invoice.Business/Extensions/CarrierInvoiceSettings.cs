using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Invoice.Entities;
using Vanrise.Common.Business;
using Vanrise.Entities;
using Vanrise.Invoice.Entities;

namespace TOne.WhS.Invoice.Business.Extensions
{
    public  enum InvoiceType  { Customer = 0, Supplier = 1 }
    public class CarrierInvoiceSettings : InvoiceTypeExtendedSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("FAD2C45F-FB61-4D65-9896-4CCADC2A656F"); }
        }
        public InvoiceType InvoiceType { get; set; }
        public override InvoiceGenerator GetInvoiceGenerator()
        {
            return new CustomerInvoiceGenerator();
        }

        public override InvoicePartnerSettings GetPartnerSettings()
        {
            return new CarrierPartnerSettings { InvoiceType = this.InvoiceType };
        }

        public override dynamic GetInfo(IInvoiceTypeExtendedSettingsInfoContext context)
        {

           
            switch(context.InfoType)
            {
                case "CustomerMailTemplate":
                 
                    
                    Dictionary<string, dynamic> objects = new Dictionary<string, dynamic>();
                    var invoiceDetails = context.Invoice.Details as CustomerInvoiceDetails;
                    objects.Add("CustomerInvoice", context.Invoice);
                    string[] partner = context.Invoice.PartnerId.Split('_');
                    CarrierProfileManager carrierProfileManager = new CarrierProfileManager();
                    CarrierProfile carrierProfile = null;
                    Guid  invoiceTemplateId = Guid.Empty;
                    switch(partner[0])
                    {
                        case "Account":
                            CarrierAccountManager carrierAccountManager = new CarrierAccountManager();
                            var account = carrierAccountManager.GetCarrierAccount(Convert.ToInt32(partner[1]));
                            carrierProfile = carrierProfileManager.GetCarrierProfile(account.CarrierProfileId);
                            invoiceTemplateId = carrierAccountManager.GetDefaultInvoiceEmailId(Convert.ToInt32(partner[1]));
                            break;
                        case "Profile":
                            carrierProfile = carrierProfileManager.GetCarrierProfile(Convert.ToInt32(partner[1]));
                            invoiceTemplateId = carrierProfileManager.GetDefaultInvoiceEmailId(Convert.ToInt32(partner[1]));
                               break;
                    }
                    if(carrierProfile != null)
                    {
                         objects.Add("Profile", carrierProfile);
                    }
                    VRMailManager vrMailManager = new VRMailManager();
                    VRMailEvaluatedTemplate template = vrMailManager.EvaluateMailTemplate(invoiceTemplateId, objects);
                    return template;
            }
            return null;
        }

        public override BillingPeriod GetBillingPeriod(IExtendedSettingsBillingPeriodContext context)
        {
            throw new NotImplementedException();
        }

        public override void GetInitialPeriodInfo(IInitialPeriodInfoContext context)
        {
            string[] partner = context.PartnerId.Split('_');
            CarrierProfileManager carrierProfileManager = new CarrierProfileManager();
            CarrierProfile carrierProfile = null;
            switch(partner[0])
            {
                case "Account":
                    CarrierAccountManager carrierAccountManager = new CarrierAccountManager();
                    var account = carrierAccountManager.GetCarrierAccount(Convert.ToInt32(partner[1]));
                    carrierProfile = carrierProfileManager.GetCarrierProfile(account.CarrierProfileId);
                    break;
                case "Profile":
                    carrierProfile = carrierProfileManager.GetCarrierProfile(Convert.ToInt32(partner[1]));
                    break;
            }
        }
    }
}
