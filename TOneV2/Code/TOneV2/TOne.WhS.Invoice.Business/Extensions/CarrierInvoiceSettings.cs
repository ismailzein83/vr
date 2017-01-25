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
        public override InvoiceGenerator GetInvoiceGenerator()
        {
            return new CustomerInvoiceGenerator();
        }

        public override InvoicePartnerManager GetPartnerManager()
        {
            return new CarrierPartnerSettings();
        }

        public override dynamic GetInfo(IInvoiceTypeExtendedSettingsInfoContext context)
        {
            string[] partner = context.Invoice.PartnerId.Split('_');
            int partnerId = Convert.ToInt32(partner[1]);
            switch(context.InfoType)
            {
                case "MailTemplate":
                    {

                        Dictionary<string, dynamic> objects = new Dictionary<string, dynamic>();
                        var invoiceDetails = context.Invoice.Details as CustomerInvoiceDetails;
                        objects.Add("CustomerInvoice", context.Invoice);
                        CarrierProfileManager carrierProfileManager = new CarrierProfileManager();
                        CarrierProfile carrierProfile = null;
                        Guid invoiceTemplateId = Guid.Empty;
                        switch (partner[0])
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
                        if (carrierProfile != null)
                        {
                            objects.Add("Profile", carrierProfile);
                        }
                        VRMailManager vrMailManager = new VRMailManager();
                        VRMailEvaluatedTemplate template = vrMailManager.EvaluateMailTemplate(invoiceTemplateId, objects);
                        return template;
                    }
                case "Taxes":
                    {
                        #region Taxes
                        CarrierProfileManager carrierProfileManager = new CarrierProfileManager();
                        IEnumerable<VRTaxItemDetail> taxItemDetails = null;
                        if (partner[0].Equals("Profile"))
                        {
                            taxItemDetails = carrierProfileManager.GetTaxItemDetails(partnerId);
                        }
                        else if (partner[0].Equals("Account"))
                        {
                            CarrierAccountManager carrierAccountManager = new CarrierAccountManager();
                            var carrierAccount = carrierAccountManager.GetCarrierAccount(Convert.ToInt32(partnerId));
                            taxItemDetails = carrierProfileManager.GetTaxItemDetails(carrierAccount.CarrierProfileId);
                        }
                        return taxItemDetails;
                        #endregion
                    }
                case "BankDetails":
                    {
                        #region BankDetails
                        IEnumerable<Guid> bankDetails = null;
                        if (partner[0].Equals("Profile"))
                        {
                            CarrierProfileManager carrierProfileManager = new CarrierProfileManager();
                            bankDetails = carrierProfileManager.GetBankDetails(partnerId);
                        }
                        else if (partner[0].Equals("Account"))
                        {
                            CarrierAccountManager carrierAccountManager = new CarrierAccountManager();
                            bankDetails = carrierAccountManager.GetBankDetails(partnerId);
                        }
                        return bankDetails;
                        #endregion
                    }
            }
            return null;
        }

        public override BillingPeriod GetBillingPeriod(IExtendedSettingsBillingPeriodContext context)
        {
            string[] partner = context.PartnerId.Split('_');
            int partnerId = Convert.ToInt32(partner[1]);
            switch (partner[0])
            {
                case "Account":
                    CarrierAccountManager carrierAccountManager = new CarrierAccountManager();
                    return carrierAccountManager.GetBillingPeriod(Convert.ToInt32(partner[1]));
                case "Profile":
                    CarrierProfileManager carrierProfileManager = new CarrierProfileManager();
                    return carrierProfileManager.GetBillingPeriod(Convert.ToInt32(partner[1]));
            }
            return null;
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
                    context.PartnerCreationDate = account.CreatedTime;
                    break;
                case "Profile":
                    carrierProfile = carrierProfileManager.GetCarrierProfile(Convert.ToInt32(partner[1]));
                    context.PartnerCreationDate = carrierProfile.CreatedTime;
                    break;
            }
        }

        public override IEnumerable<string> GetPartnerIds(IExtendedSettingsPartnerIdsContext context)
        {
            var carriers = new InvoiceManager().GetInvoiceCarriers(new InvoiceCarrierFilter
            {
                GetCustomers = true
            });
            if (carriers == null)
                return null;
            return carriers.Select(x => x.InvoiceCarrierId);
        }
    }
}
