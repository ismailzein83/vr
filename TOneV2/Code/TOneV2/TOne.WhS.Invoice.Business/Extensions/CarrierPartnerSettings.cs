using System;
using System.Collections.Generic;
using System.Linq;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Invoice.Entities;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;
using Vanrise.Invoice.Entities;

namespace TOne.WhS.Invoice.Business.Extensions
{
    #region enums
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
        Attention = 15,
        BillingCompanyEmail = 16,
        SMSServiceTypes = 17,
        BillingCompanyPhone = 18,
        CustomerSMSServiceTypes = 19,
        SupplierSMSServiceTypes = 20,
        BillingCompanyContact = 21,
        CarrierCompanyBillingContact = 22,
        OriginalAmount = 23,
        CompanyFax = 24,
        CompanyProfileName = 25,
        Note = 26,
        PostalCode = 27
    }

    public enum CarrierInvoiceType { Customer = 1, Supplier = 2, Settlement = 3 }

    #endregion

    public class CarrierPartnerSettings : InvoicePartnerManager
    {
        CurrencyExchangeRateManager _currencyExchangeRateManager = new CurrencyExchangeRateManager();
        CarrierInvoiceType _accountType;

        public override string PartnerFilterSelector { get { return "whs-invoice-invoiceaccount-selector"; } }

        public override string PartnerSelector { get { return "whs-invoice-invoiceaccount-selector"; } }

        public override string PartnerInvoiceSettingFilterFQTN { get { return "TOne.WhS.Invoice.Business.AssignedPartnerInvoiceSettingFilter, TOne.WhS.Invoice.Business"; } }

        public bool UseMaskInfo { get; set; }

        #region Public Methods

        public CarrierPartnerSettings(CarrierInvoiceType accountType)
        {
            _accountType = accountType;
        }

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

                        CarrierProfileManager carrierProfileManager = new CarrierProfileManager();
                        CurrencyManager currencyManager = new CurrencyManager();

                        CarrierProfile carrierProfile;
                        string carrierName;
                        int currencyId;

                        if (financialAccount.CarrierProfileId.HasValue)
                        {
                            AddCompanySettingsParameters(rdlcReportParameters, carrierProfileManager.GetCompanySetting(financialAccount.CarrierProfileId.Value));

                            carrierProfile = carrierProfileManager.GetCarrierProfile(financialAccount.CarrierProfileId.Value);
                            carrierName = carrierProfileManager.GetCarrierProfileName(carrierProfile.CarrierProfileId);
                            carrierProfile.Settings.ThrowIfNull("carrierProfile.Settings", carrierProfile.CarrierProfileId);

                            currencyId = carrierProfile.Settings.CurrencyId;
                        }
                        else
                        {
                            CarrierAccountManager carrierAccountManager = new CarrierAccountManager();
                            AddCompanySettingsParameters(rdlcReportParameters, carrierAccountManager.GetCompanySetting(financialAccount.CarrierAccountId.Value));

                            var carrierAccount = carrierAccountManager.GetCarrierAccount(Convert.ToInt32(financialAccount.CarrierAccountId.Value));
                            carrierProfile = carrierProfileManager.GetCarrierProfile(carrierAccount.CarrierProfileId);
                            carrierProfile.Settings.ThrowIfNull("carrierProfile.Settings", carrierAccount.CarrierProfileId);

                            carrierName = carrierAccountManager.GetCarrierAccountName(carrierAccount.CarrierAccountId);
                            currencyId = carrierAccount.CarrierAccountSettings.CurrencyId;
                        }

                        AddRDLCParameter(rdlcReportParameters, RDLCParameter.Carrier, carrierName, true);

                        string currencySymbol = currencyManager.GetCurrencySymbol(currencyId);
                        AddRDLCParameter(rdlcReportParameters, RDLCParameter.Currency, currencySymbol, true);

                        switch (_accountType)
                        {
                            case CarrierInvoiceType.Customer:
                                AddAccountSMSServiceTypeParameter(rdlcReportParameters, financialAccount, RDLCParameter.SMSServiceTypes, financialAccountManager.GetFinancialAccountCustomerSMSServiceTypes);
                                break;

                            case CarrierInvoiceType.Supplier:
                                AddAccountSMSServiceTypeParameter(rdlcReportParameters, financialAccount, RDLCParameter.SMSServiceTypes, financialAccountManager.GetFinancialAccountSupplierSMSServiceTypes);

                                var supplierInvoiceDetails = context.Invoice.Details as SupplierInvoiceDetails;

                                decimal? originalAmount = null;
                                if (supplierInvoiceDetails.OriginalAmountByCurrency != null && supplierInvoiceDetails.OriginalAmountByCurrency.Count > 0)
                                {
                                    foreach (var originalAmountByCurrency in supplierInvoiceDetails.OriginalAmountByCurrency)
                                    {
                                        var totalAmount = originalAmountByCurrency.Value.TrafficAmount.HasValue ? originalAmountByCurrency.Value.TrafficAmount.Value : 0;
                                        if (originalAmountByCurrency.Value.SMSAmount.HasValue)
                                            totalAmount += originalAmountByCurrency.Value.SMSAmount.Value;
                                        if (originalAmountByCurrency.Value.DealAmount.HasValue)
                                            totalAmount += originalAmountByCurrency.Value.DealAmount.Value;
                                        if (originalAmountByCurrency.Value.RecurringChargeAmount.HasValue)
                                            totalAmount += originalAmountByCurrency.Value.RecurringChargeAmount.Value;

                                        if (totalAmount > 0)
                                        {
                                            if (!originalAmount.HasValue)
                                                originalAmount = 0;
                                            originalAmount += _currencyExchangeRateManager.ConvertValueToCurrency(totalAmount, originalAmountByCurrency.Key, currencyId, context.Invoice.IssueDate);
                                        }
                                    }
                                }
                                if (originalAmount.HasValue)
                                    AddRDLCParameter(rdlcReportParameters, RDLCParameter.OriginalAmount, originalAmount.Value.ToString(), true);
                                break;

                            case CarrierInvoiceType.Settlement:
                                AddAccountSMSServiceTypeParameter(rdlcReportParameters, financialAccount, RDLCParameter.CustomerSMSServiceTypes, financialAccountManager.GetFinancialAccountCustomerSMSServiceTypes);
                                AddAccountSMSServiceTypeParameter(rdlcReportParameters, financialAccount, RDLCParameter.SupplierSMSServiceTypes, financialAccountManager.GetFinancialAccountSupplierSMSServiceTypes);
                                break;
                        }

                        if (carrierProfile != null)
                        {
                            AddContactPartParameter(rdlcReportParameters, RDLCParameter.Attention, carrierProfile, CarrierContactType.PricingContactPerson);
                            AddContactPartParameter(rdlcReportParameters, RDLCParameter.Email, carrierProfile, CarrierContactType.BillingEmail);
                            AddContactPartParameter(rdlcReportParameters, RDLCParameter.CarrierCompanyBillingContact, carrierProfile, CarrierContactType.BillingContactPerson);

                            var carrierCompanyName = carrierProfile.Settings.Company;
                            AddRDLCParameter(rdlcReportParameters, RDLCParameter.CarrierCompanyName, carrierCompanyName, true);

                            if (carrierProfile.Settings.Address != null)
                                AddRDLCParameter(rdlcReportParameters, RDLCParameter.Address, carrierProfile.Settings.Address, true);

                            if (carrierProfile.Settings.PhoneNumbers != null)
                            {
                                AddRDLCParameter(rdlcReportParameters, RDLCParameter.Phone, carrierProfile.Settings.PhoneNumbers.ElementAtOrDefault(0), true);
                                AddRDLCParameter(rdlcReportParameters, RDLCParameter.Phone1, carrierProfile.Settings.PhoneNumbers.ElementAtOrDefault(1), true);
                            }

                            if (carrierProfile.Settings.Faxes != null)
                                AddRDLCParameter(rdlcReportParameters, RDLCParameter.Fax, carrierProfile.Settings.Faxes.ElementAtOrDefault(0), true);

                            if (carrierProfile.Settings.RegistrationNumber != null)
                                AddRDLCParameter(rdlcReportParameters, RDLCParameter.CustomerRegNo, carrierProfile.Settings.RegistrationNumber, true);

                            if (carrierProfile.Settings.PostalCode != null)
                                AddRDLCParameter(rdlcReportParameters, RDLCParameter.PostalCode, carrierProfile.Settings.PostalCode, true);

                            if (carrierProfile.Settings.TaxSetting != null && carrierProfile.Settings.TaxSetting.VATId != null)
                                AddRDLCParameter(rdlcReportParameters, RDLCParameter.CustomerVatID, carrierProfile.Settings.TaxSetting.VATId, true);
                        }

                        AddRDLCParameter(rdlcReportParameters, RDLCParameter.Note, context.Invoice.Note, true);

                        return rdlcReportParameters;

                        #endregion
                    }
            }
            return null;
        }

        public override string GetFullPartnerName(IPartnerNameManagerContext context)
        {
            var financialAccount = new WHSFinancialAccountManager().GetFinancialAccount(Convert.ToInt32(context.PartnerId));

            if (financialAccount.CarrierProfileId.HasValue)
            {
                string carrierProfileName = new CarrierProfileManager().GetCarrierProfileName(financialAccount.CarrierProfileId.Value);
                string profilePrefix = Utilities.GetEnumDescription<WHSFinancialAccountCarrierType>(WHSFinancialAccountCarrierType.Profile);
                return string.Format("{1} ({0})", profilePrefix, carrierProfileName);
            }
            else
            {
                CarrierAccountManager carrierAccountManager = new CarrierAccountManager();
                string carrierAccountName = carrierAccountManager.GetCarrierAccountName(financialAccount.CarrierAccountId.Value);
                string accountPrefix = Utilities.GetEnumDescription<WHSFinancialAccountCarrierType>(WHSFinancialAccountCarrierType.Account);
                return string.Format("{1} ({0})", accountPrefix, carrierAccountName);
            }
        }

        public override string GetPartnerName(IPartnerNameManagerContext context)
        {
            var financialAccount = new WHSFinancialAccountManager().GetFinancialAccount(Convert.ToInt32(context.PartnerId));

            if (financialAccount.CarrierProfileId.HasValue)
                return new CarrierProfileManager().GetCarrierProfileName(financialAccount.CarrierProfileId.Value);
            else
                return new CarrierAccountManager().GetCarrierAccountName(financialAccount.CarrierAccountId.Value);
        }

        public override dynamic GetActualPartnerId(IActualPartnerContext context)
        {
            var financialAccount = new WHSFinancialAccountManager().GetFinancialAccount(Convert.ToInt32(context.PartnerId));

            if (financialAccount.CarrierProfileId.HasValue)
                return financialAccount.CarrierProfileId.Value;
            else
                return financialAccount.CarrierAccountId.Value;
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

        #endregion

        #region Private Methods
        private void AddRDLCParameter(Dictionary<string, VRRdlcReportParameter> rdlcReportParameters, RDLCParameter key, string value, bool isVisible)
        {
            if (rdlcReportParameters == null)
                rdlcReportParameters = new Dictionary<string, VRRdlcReportParameter>();

            rdlcReportParameters.Add(key.ToString(), new VRRdlcReportParameter { Value = value, IsVisible = isVisible });
        }

        private string GetContactDescription(CarrierProfile carrierProfile, CarrierContactType carrierContactType)
        {
            var contactObject = carrierProfile.Settings.Contacts.FindRecord(x => x.Type == carrierContactType);
            if (contactObject == null)
                return null;

            return contactObject.Description;
        }

        private void AddAccountSMSServiceTypeParameter(Dictionary<string, VRRdlcReportParameter> rdlcReportParameters, WHSFinancialAccount financialAccount, RDLCParameter rdlcParameter, Func<int, List<SMSServiceType>> getFinancialAccountSMSServiceTypes)
        {
            List<SMSServiceType> smsServiceTypes = getFinancialAccountSMSServiceTypes(financialAccount.FinancialAccountId);
            AddRDLCParameter(rdlcReportParameters, rdlcParameter, string.Join(",", smsServiceTypes.Select(x => x.Symbol)), true);
        }

        private void AddCompanySettingsParameters(Dictionary<string, VRRdlcReportParameter> rdlcReportParameters, CompanySetting companySetting)
        {
            if (companySetting == null)
                return;

            CompanyContact companyContact;
            if (companySetting.Contacts.TryGetValue("Billing", out companyContact))
            {
                AddRDLCParameter(rdlcReportParameters, RDLCParameter.BillingCompanyPhone, companyContact.Phone, true);
                AddRDLCParameter(rdlcReportParameters, RDLCParameter.BillingCompanyContact, $"{companyContact.Title} {companyContact.ContactName}", true);
            }

            var logo = new VRFileManager().GetFile(companySetting.CompanyLogo);
            if (logo != null)
                AddRDLCParameter(rdlcReportParameters, RDLCParameter.Image, Convert.ToBase64String(logo.Content), true);

            AddRDLCParameter(rdlcReportParameters, RDLCParameter.RegNo, companySetting.RegistrationNumber, true);
            AddRDLCParameter(rdlcReportParameters, RDLCParameter.RegAddress, companySetting.RegistrationAddress, true);
            AddRDLCParameter(rdlcReportParameters, RDLCParameter.Name, companySetting.CompanyName, true);
            AddRDLCParameter(rdlcReportParameters, RDLCParameter.CompanyProfileName, companySetting.ProfileName, true);
            AddRDLCParameter(rdlcReportParameters, RDLCParameter.VatID, companySetting.VatId, true);
            AddRDLCParameter(rdlcReportParameters, RDLCParameter.BillingCompanyEmail, companySetting.BillingEmails, true);

            if (companySetting.Faxes != null && companySetting.Faxes.Count > 0)
                AddRDLCParameter(rdlcReportParameters, RDLCParameter.CompanyFax, string.Join(",", companySetting.Faxes), true);
        }

        private void AddContactPartParameter(Dictionary<string, VRRdlcReportParameter> rdlcReportParameters, RDLCParameter rdlcParameter, CarrierProfile carrierProfile, CarrierContactType carrierContactType)
        {
            var contactPart = GetContactDescription(carrierProfile, carrierContactType);
            AddRDLCParameter(rdlcReportParameters, rdlcParameter, contactPart, true);
        }

        #endregion
    }
}