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
using Vanrise.Invoice.Entities;

namespace TOne.WhS.Invoice.Business.Extensions
{
    public enum RDLCParameter
    {
        Carrier = 0,
        Currency = 1,
        Address = 2,
        Phone = 3,
        Phone1 = 4,
        Fax = 5,
        Image = 6,
        CustomerRegNo = 7,
        RegNo = 8,
        RegAddress = 9,
        Name = 10,
        VatID = 11,
        CustomerVatID = 12,
        CarrierCompanyName = 13,
        Email = 14,
        Attention = 15
    }
    public class CarrierPartnerSettings : InvoicePartnerManager
    {
        public override string PartnerFilterSelector
        {
            get
            {
                return "whs-invoice-invoiceaccount-selector";
            }
        }
        public override string PartnerSelector
        {
            get
            {
                return "whs-invoice-invoiceaccount-selector";
            }
        }
        public override string PartnerInvoiceSettingFilterFQTN
        {
            get
            {
                return "TOne.WhS.Invoice.Business.AssignedPartnerInvoiceSettingFilter, TOne.WhS.Invoice.Business";
            }
        }
        public bool UseMaskInfo { get; set; }
        public override dynamic GetPartnerInfo(IPartnerManagerInfoContext context)
        {
            WHSFinancialAccountManager financialAccountManager = new WHSFinancialAccountManager();
            var financialAccount = financialAccountManager.GetFinancialAccount(Convert.ToInt32(context.PartnerId));

            switch (context.InfoType)
            {
                case "InvoiceRDLCReport":
                    {
                        #region InvoiceRDLCReport
                        Dictionary<string, VRRdlcReportParameter> rdlcReportParameters = new Dictionary<string, VRRdlcReportParameter>();
                        CurrencyManager currencyManager = new CurrencyManager();
                        CarrierProfileManager carrierProfileManager = new CarrierProfileManager();
                        CarrierProfile carrierProfile = null;
                        string carrierName = null;
                        string currencySymbol = null;
                        string carrierCompanyName = null;
                        CompanySetting companySetting = null;

                        string pricingContact = null;
                        string pricingEmail = null;
                        if (financialAccount.CarrierProfileId.HasValue)
                        {
                            companySetting = carrierProfileManager.GetCompanySetting(financialAccount.CarrierProfileId.Value);
                            carrierProfile = carrierProfileManager.GetCarrierProfile(financialAccount.CarrierProfileId.Value);
                            carrierName = carrierProfileManager.GetCarrierProfileName(carrierProfile.CarrierProfileId);
                            carrierProfile.Settings.ThrowIfNull("carrierProfile.Settings", carrierProfile.CarrierProfileId);
                            var pricingContactObject = carrierProfile.Settings.Contacts.FindRecord(x => x.Type == CarrierContactType.PricingContactPerson);
                            if (pricingContactObject != null)
                                pricingContact = pricingContactObject.Description;

                            var pricingEmailObject = carrierProfile.Settings.Contacts.FindRecord(x => x.Type == CarrierContactType.PricingEmail);
                            if (pricingEmailObject != null)
                                pricingEmail = pricingEmailObject.Description;

                            carrierCompanyName = carrierProfile.Settings.Company;
                            currencySymbol = currencyManager.GetCurrencySymbol(carrierProfile.Settings.CurrencyId);
                        }
                        else
                        {
                            CarrierAccountManager carrierAccountManager = new CarrierAccountManager();
                            companySetting = carrierAccountManager.GetCompanySetting(financialAccount.CarrierAccountId.Value);
                            var carrierAccount = carrierAccountManager.GetCarrierAccount(Convert.ToInt32(financialAccount.CarrierAccountId.Value));
                            carrierProfile = carrierProfileManager.GetCarrierProfile(carrierAccount.CarrierProfileId);
                            carrierProfile.Settings.ThrowIfNull("carrierProfile.Settings", carrierAccount.CarrierProfileId);
                            carrierCompanyName = carrierProfile.Settings.Company;
                            carrierName = carrierAccountManager.GetCarrierAccountName(carrierAccount.CarrierAccountId);
                            var pricingContactObject = carrierProfile.Settings.Contacts.FindRecord(x => x.Type == CarrierContactType.PricingContactPerson);
                            if (pricingContactObject != null)
                                pricingContact = pricingContactObject.Description;

                            var pricingEmailObject = carrierProfile.Settings.Contacts.FindRecord(x => x.Type == CarrierContactType.PricingEmail);
                            if (pricingEmailObject != null)
                                pricingEmail = pricingEmailObject.Description;
                            currencySymbol = currencyManager.GetCurrencySymbol(carrierAccount.CarrierAccountSettings.CurrencyId);
                        }
                        AddRDLCParameter(rdlcReportParameters, RDLCParameter.Email, pricingEmail, true);
                        AddRDLCParameter(rdlcReportParameters, RDLCParameter.Attention, pricingContact, true);
                        AddRDLCParameter(rdlcReportParameters, RDLCParameter.CarrierCompanyName,carrierCompanyName , true);
                        AddRDLCParameter(rdlcReportParameters, RDLCParameter.Carrier, carrierName, true);
                        AddRDLCParameter(rdlcReportParameters, RDLCParameter.Currency, currencySymbol, true);
                        if (carrierProfile != null)
                        {

                            if (carrierProfile.Settings.Address != null)
                                AddRDLCParameter(rdlcReportParameters, RDLCParameter.Address, carrierProfile.Settings.Address, true);
                            if (carrierProfile.Settings.PhoneNumbers != null)
                            {
                                AddRDLCParameter(rdlcReportParameters, RDLCParameter.Phone, carrierProfile.Settings.PhoneNumbers.ElementAtOrDefault(0), true);
                                AddRDLCParameter(rdlcReportParameters, RDLCParameter.Phone1, carrierProfile.Settings.PhoneNumbers.ElementAtOrDefault(1), true);
                            }
                            if (carrierProfile.Settings.Faxes != null)
                            {
                                AddRDLCParameter(rdlcReportParameters, RDLCParameter.Fax, carrierProfile.Settings.Faxes.ElementAtOrDefault(0), true);
                            }
                            if (carrierProfile.Settings.RegistrationNumber != null)
                            {
                                AddRDLCParameter(rdlcReportParameters, RDLCParameter.CustomerRegNo, carrierProfile.Settings.RegistrationNumber, true);
                            }
                            if (carrierProfile.Settings.TaxSetting != null && carrierProfile.Settings.TaxSetting.VATId != null)
                            {
                                AddRDLCParameter(rdlcReportParameters, RDLCParameter.CustomerVatID, carrierProfile.Settings.TaxSetting.VATId, true);
                            }
                        }
                        if (companySetting != null)
                        {
                            VRFileManager fileManager = new VRFileManager();
                            var logo = fileManager.GetFile(companySetting.CompanyLogo);
                            if (logo != null)
                            {
                                AddRDLCParameter(rdlcReportParameters, RDLCParameter.Image, Convert.ToBase64String(logo.Content), true);
                            }
                            AddRDLCParameter(rdlcReportParameters, RDLCParameter.RegNo, companySetting.RegistrationNumber, true);
                            AddRDLCParameter(rdlcReportParameters, RDLCParameter.RegAddress, companySetting.RegistrationAddress, true);
                            AddRDLCParameter(rdlcReportParameters, RDLCParameter.Name, companySetting.CompanyName, true);
                            AddRDLCParameter(rdlcReportParameters, RDLCParameter.VatID, companySetting.VatId, true);

                        }
                        return rdlcReportParameters;
                        #endregion
                    }


            }
            return null;
        }
        public override string GetFullPartnerName(IPartnerNameManagerContext context)
        {
            WHSFinancialAccountManager financialAccountManager = new WHSFinancialAccountManager();
            var financialAccount = financialAccountManager.GetFinancialAccount(Convert.ToInt32(context.PartnerId));
            string fullName;
            if (financialAccount.CarrierProfileId.HasValue)
            {
                CarrierProfileManager carrierProfileManager = new CarrierProfileManager();
                string carrierProfileName = carrierProfileManager.GetCarrierProfileName(financialAccount.CarrierProfileId.Value);
                string profilePrefix = Utilities.GetEnumDescription<WHSFinancialAccountCarrierType>(WHSFinancialAccountCarrierType.Profile);
                fullName = string.Format("{1} ({0})", profilePrefix, carrierProfileName);
            }
            else
            {
                CarrierAccountManager carrierAccountManager = new CarrierAccountManager();
                string carrierAccountName = carrierAccountManager.GetCarrierAccountName(financialAccount.CarrierAccountId.Value);
                string accountPrefix = Utilities.GetEnumDescription<WHSFinancialAccountCarrierType>(WHSFinancialAccountCarrierType.Account);
                fullName = string.Format("{1} ({0})", accountPrefix, carrierAccountName);
            }
            return fullName;
        }

        public override string GetPartnerName(IPartnerNameManagerContext context)
        {
            WHSFinancialAccountManager financialAccountManager = new WHSFinancialAccountManager();
            var financialAccount = financialAccountManager.GetFinancialAccount(Convert.ToInt32(context.PartnerId));

            if (financialAccount.CarrierProfileId.HasValue)
            {
                CarrierProfileManager carrierProfileManager = new CarrierProfileManager();
                return carrierProfileManager.GetCarrierProfileName(financialAccount.CarrierProfileId.Value);
            }
            else
            {
                CarrierAccountManager carrierAccountManager = new CarrierAccountManager();
                return carrierAccountManager.GetCarrierAccountName(financialAccount.CarrierAccountId.Value);
            }
        }
        public override dynamic GetActualPartnerId(IActualPartnerContext context)
        {
            WHSFinancialAccountManager financialAccountManager = new WHSFinancialAccountManager();
            var financialAccount = financialAccountManager.GetFinancialAccount(Convert.ToInt32(context.PartnerId));

            if (financialAccount.CarrierProfileId.HasValue)
            {
                return financialAccount.CarrierProfileId.Value;
            }
            else
            {
                return financialAccount.CarrierAccountId.Value;
            }
        }
        private void AddRDLCParameter(Dictionary<string, VRRdlcReportParameter> rdlcReportParameters, RDLCParameter key, string value, bool isVisible)
        {
            if (rdlcReportParameters == null)
                rdlcReportParameters = new Dictionary<string, VRRdlcReportParameter>();
            rdlcReportParameters.Add(key.ToString(), new VRRdlcReportParameter { Value = value, IsVisible = isVisible });
        }

        public override VRInvoiceAccountData GetInvoiceAccountData(IInvoiceAccountDataContext context)
        {
            WHSFinancialAccountManager financialAccountManager = new WHSFinancialAccountManager();
            var financialAccount = financialAccountManager.GetFinancialAccount(Convert.ToInt32(context.PartnerId));
            VRAccountStatus status = financialAccountManager.IsFinancialAccountActive(financialAccount, context.InvoiceTypeId) ? VRAccountStatus.Active : VRAccountStatus.InActive;
            return new VRInvoiceAccountData
            {
                BED = financialAccount.BED,
                EED = financialAccount.EED,
                Status = status
            };
        }
    }

}
