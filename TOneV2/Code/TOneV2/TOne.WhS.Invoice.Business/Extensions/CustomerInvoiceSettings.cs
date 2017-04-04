﻿using System;
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
    public class CustomerInvoiceSettings : InvoiceSetting
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
                        objects.Add("CustomerInvoice", context.Invoice);
                        CarrierProfileManager carrierProfileManager = new CarrierProfileManager();
                        CarrierProfile carrierProfile = null;
                        switch (partner[0])
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
                        if (carrierProfile != null)
                        {
                            objects.Add("Profile", carrierProfile);
                        }
                        return objects;
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
