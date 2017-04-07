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
    public class CustomerInvoiceSettings : InvoiceSettings
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

            InvoiceAccountManager invoiceAccountManager = new Business.InvoiceAccountManager();
            var invoiceAccount = invoiceAccountManager.GetInvoiceAccount(Convert.ToInt32(context.Invoice.PartnerId));
            CarrierProfileManager carrierProfileManager = new CarrierProfileManager();
            
            switch(context.InfoType)
            {
                case "MailTemplate":
                    {
                        Dictionary<string, dynamic> objects = new Dictionary<string, dynamic>();
                        objects.Add("CustomerInvoice", context.Invoice);
                        CarrierProfile carrierProfile = null;
                        int carrierProfileId;
                        if (invoiceAccount.CarrierProfileId.HasValue)
                        {
                            carrierProfileId = invoiceAccount.CarrierProfileId.Value;
                        }
                        else
                        {
                            CarrierAccountManager carrierAccountManager = new CarrierAccountManager();
                            var account = carrierAccountManager.GetCarrierAccount(invoiceAccount.CarrierAccountId.Value);
                            carrierProfileId = account.CarrierProfileId;
                        }
                        carrierProfile = carrierProfileManager.GetCarrierProfile(carrierProfileId);
                        if (carrierProfile != null)
                        {
                            objects.Add("Profile", carrierProfile);
                        }
                        return objects;
                    }
                case "Taxes":
                    {
                        #region Taxes
                        IEnumerable<VRTaxItemDetail> taxItemDetails = null;

                        if (invoiceAccount.CarrierProfileId.HasValue)
                        {
                            taxItemDetails = carrierProfileManager.GetTaxItemDetails(invoiceAccount.CarrierProfileId.Value);
                        }
                        else
                        {
                            CarrierAccountManager carrierAccountManager = new CarrierAccountManager();
                            var carrierAccount = carrierAccountManager.GetCarrierAccount(invoiceAccount.CarrierAccountId.Value);
                            taxItemDetails = carrierProfileManager.GetTaxItemDetails(carrierAccount.CarrierProfileId);
                        }
                        return taxItemDetails;
                        #endregion
                    }
                case "BankDetails":
                    {
                        #region BankDetails
                        IEnumerable<Guid> bankDetails = null;
                        if (invoiceAccount.CarrierProfileId.HasValue)
                        {
                            bankDetails = carrierProfileManager.GetBankDetails(invoiceAccount.CarrierProfileId.Value);
                        }
                        else
                        {
                             CarrierAccountManager carrierAccountManager = new CarrierAccountManager();
                             bankDetails = carrierAccountManager.GetBankDetails(invoiceAccount.CarrierAccountId.Value);
                        }
                        return bankDetails;
                        #endregion
                    }
            }
            return null;
        }
        public override void GetInitialPeriodInfo(IInitialPeriodInfoContext context)
        {
            InvoiceAccountManager invoiceAccountManager = new Business.InvoiceAccountManager();
            var invoiceAccount = invoiceAccountManager.GetInvoiceAccount(Convert.ToInt32(context.PartnerId));
            CarrierProfile carrierProfile = null;
            CarrierProfileManager carrierProfileManager = new CarrierProfileManager();
            if (invoiceAccount.CarrierProfileId.HasValue)
            {
                carrierProfile = carrierProfileManager.GetCarrierProfile(Convert.ToInt32(invoiceAccount.CarrierProfileId.Value));
                context.PartnerCreationDate = carrierProfile.CreatedTime;
            }
            else
            {
                CarrierAccountManager carrierAccountManager = new CarrierAccountManager();
                var account = carrierAccountManager.GetCarrierAccount(Convert.ToInt32(invoiceAccount.CarrierAccountId.Value));
                context.PartnerCreationDate = account.CreatedTime;
            }
        }
        public override IEnumerable<string> GetPartnerIds(IExtendedSettingsPartnerIdsContext context)
        {
            InvoiceCarrierFilter invoiceCarrierFilter = new InvoiceCarrierFilter{ GetCustomers = true};
            switch (context.PartnerRetrievalType)
            {
                case PartnerRetrievalType.GetAll:
                  break;
                case PartnerRetrievalType.GetActive:
                  invoiceCarrierFilter.ActivationStatus = ActivationStatus.Active;
                  break;
                case PartnerRetrievalType.GetInactive:
                  invoiceCarrierFilter.ActivationStatus = ActivationStatus.Inactive;
                    break;
                default : throw new NotSupportedException(string.Format("PartnerRetrievalType {0} not supported.",context.PartnerRetrievalType));
            }
             var carriers = new InvoiceManager().GetInvoiceCarriers(invoiceCarrierFilter);
             if (carriers == null)
                        return null;
            return carriers.Select(x => x.InvoiceCarrierId);
        }

        public override bool IsApplicableToCustomer
        {
            get { return true; }
        }
        public override bool IsApplicableToSupplier
        {
            get { return false; }
        }
        public override string RuntimeEditor
        {
            get
            {
                return "whs-invoicetype-runtime-customerinvoicesettings";
            }
        }
    }
}
