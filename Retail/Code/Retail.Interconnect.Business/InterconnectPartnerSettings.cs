using Retail.BusinessEntity.Business;
using Retail.BusinessEntity.Entities;
using Retail.BusinessEntity.MainExtensions.AccountParts;
using Retail.Interconnect.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;
using Vanrise.Invoice.Entities;

namespace Retail.Interconnect.Business
{
    #region Enums

    public enum RDLCParameter
    {
        AccountName = 0,
        Image = 1,
        RegNo = 2,
        RegAddress = 3,
        Name = 4,
        VatID = 5,
        Currency = 6,
        Address = 7,
        Phone = 8,
        Fax = 9,
        CompanyName = 10,
        Email = 11,
        NTN = 12,
        AccountType = 13,
        AccountNumber = 14,
        Attn = 15,
        AssignedNumber = 16,
        BillingCompanyEmail = 17,
        BillingCompanyPhone = 18,
        SMSServiceTypes = 19,
        BillingCompanyContact = 20,
        AccountBillingContact = 21,
        OriginalAmount = 22,
        CompanyProfileName = 23,
        CompanyFax = 24
    }

    public enum InterconnectPartnerType { Customer = 0, Supplier = 1, Settlement = 2 }

    #endregion

    public class InterconnectPartnerSettings : InvoicePartnerManager
    {
        Guid _acountBEDefinitionId;
        InterconnectPartnerType _interconnectPartnerType;

        public override string PartnerFilterSelector { get { return "retail-be-account-invoice-selector"; } }

        public override string PartnerSelector { get { return "retail-be-account-invoice-selector"; } }

        public override string PartnerInvoiceSettingFilterFQTN { get { return "Retail.Interconnect.Business.AssignedFinancialAccountToInvoiceSettingFilter, Retail.Interconnect.Business"; } }

        #region Ctor

        public InterconnectPartnerSettings(Guid acountBEDefinitionId, InterconnectPartnerType interconnectPartnerType)
        {
            _acountBEDefinitionId = acountBEDefinitionId;
            _interconnectPartnerType = interconnectPartnerType;
        }

        #endregion

        #region Public Methods 

        public override dynamic GetActualPartnerId(IActualPartnerContext context)
        {
            var account = GetAccountByPartnerId(context.PartnerId);
            return account.AccountId;
        }

        public override VRInvoiceAccountData GetInvoiceAccountData(IInvoiceAccountDataContext context)
        {
            var financialAccountData = GetFinancialAccountData(context.PartnerId);
            financialAccountData.FinancialAccount.ThrowIfNull("financialAccountData.FinancialAccount", context.PartnerId);
            financialAccountData.Account.ThrowIfNull("financialAccountData.Account", context.PartnerId);

            return new VRInvoiceAccountData
            {
                BED = financialAccountData.FinancialAccount.BED,
                EED = financialAccountData.FinancialAccount.EED,
                Status = new AccountBEManager().IsAccountInvoiceActive(financialAccountData.Account) ? VRAccountStatus.Active : VRAccountStatus.InActive
            };
        }

        public override dynamic GetPartnerInfo(IPartnerManagerInfoContext context)
        {
            var account = GetAccountByPartnerId(context.PartnerId);

            switch (context.InfoType)
            {
                case "Account":
                    return account;

                case "InvoiceRDLCReport":
                    Dictionary<string, VRRdlcReportParameter> rdlcReportParameters = new Dictionary<string, VRRdlcReportParameter>();
                    AccountBEManager accountBEManager = new AccountBEManager();
                    CurrencyExchangeRateManager currencyExchangeRateManager = new CurrencyExchangeRateManager();

                    string accountName = accountBEManager.GetAccountName(_acountBEDefinitionId, account.AccountId);
                    AddRDLCParameter(rdlcReportParameters, RDLCParameter.AccountName, accountName, true);

                    int accountCurrencyId = accountBEManager.GetCurrencyId(_acountBEDefinitionId, account.AccountId);
                    string accountCurrencySymbol = new CurrencyManager().GetCurrencySymbol(accountCurrencyId);
                    AddRDLCParameter(rdlcReportParameters, RDLCParameter.Currency, accountCurrencySymbol, true);

                    if (accountBEManager.HasAccountProfile(_acountBEDefinitionId, account.AccountId, true, out IAccountProfile accountProfile))
                        AddCompanyProfileParameters(rdlcReportParameters, accountProfile, accountProfile.CastWithValidate<AccountPartCompanyProfile>("accountProfile", account.AccountId));

                    AddCompanySettingsParameters(rdlcReportParameters, accountBEManager.GetCompanySetting(_acountBEDefinitionId, account.AccountId));

                    if (accountBEManager.TryGetAccountPart<AccountPartInterconnectSetting>(_acountBEDefinitionId, account, false, out AccountPart accountPart))
                    {
                        var accountPartSettings = accountPart.Settings as AccountPartInterconnectSetting;
                        if (accountPartSettings != null && accountPartSettings.SMSServiceTypes != null)
                            AddRDLCParameter(rdlcReportParameters, RDLCParameter.SMSServiceTypes, new SMSServiceTypeManager().GetSMSServiceTypesConcatenatedSymbolsByIds(accountPartSettings.SMSServiceTypes.MapRecords(sMSServiceType => sMSServiceType.SMSServiceTypeId)), true);
                    }

                    switch (_interconnectPartnerType)
                    {
                        case InterconnectPartnerType.Customer:
                            break;

                        case InterconnectPartnerType.Supplier:
                            var supplierInvoiceDetails = context.Invoice.Details as InterconnectInvoiceDetails;

                            if (supplierInvoiceDetails.OriginalAmountByCurrency == null || supplierInvoiceDetails.OriginalAmountByCurrency.Count == 0)
                                break;

                            decimal? originalAmount = null;
                            foreach (var originalAmountByCurrency in supplierInvoiceDetails.OriginalAmountByCurrency)
                            {
                                int currencyId = originalAmountByCurrency.Key;
                                OriginalDataCurrrency originalDataCurrrency = originalAmountByCurrency.Value;

                                if (originalDataCurrrency.OriginalAmount.HasValue)
                                {
                                    if (!originalAmount.HasValue)
                                        originalAmount = 0;

                                    originalAmount += currencyExchangeRateManager.ConvertValueToCurrency(originalDataCurrrency.OriginalAmount.Value, currencyId, accountCurrencyId, context.Invoice.IssueDate);
                                }
                            }

                            if (originalAmount.HasValue)
                                AddRDLCParameter(rdlcReportParameters, RDLCParameter.OriginalAmount, originalAmount.Value.ToString(), true);

                            break;

                        case InterconnectPartnerType.Settlement:
                            break;
                    }
                    return rdlcReportParameters;
            }
            return null;
        }

        public override string GetPartnerName(IPartnerNameManagerContext context)
        {
            var account = GetAccountByPartnerId(context.PartnerId);
            return new AccountBEManager().GetAccountName(_acountBEDefinitionId, account.AccountId);
        }

        #endregion

        #region Private Methods

        private void AddCompanyProfileParameters(Dictionary<string, VRRdlcReportParameter> rdlcReportParameters, IAccountProfile accountProfile, AccountPartCompanyProfile companyProfile)
        {
            if (companyProfile == null)
                return;

            AddRDLCParameter(rdlcReportParameters, RDLCParameter.Address, companyProfile.Address, true);
            AddCompanyProfilePhoneNumbers(rdlcReportParameters, companyProfile);

            if (accountProfile.Faxes != null && accountProfile.Faxes.Count > 0)
            {
                string faxes = string.Join(", ", accountProfile.Faxes);
                AddRDLCParameter(rdlcReportParameters, RDLCParameter.Fax, faxes, true);
            }

            AddCompanyProfileContacts(rdlcReportParameters, companyProfile);
        }

        private void AddCompanyProfileContacts(Dictionary<string, VRRdlcReportParameter> rdlcReportParameters, AccountPartCompanyProfile companyProfile)
        {
            if (companyProfile.Contacts == null)
                return;

            if (companyProfile.Contacts.TryGetValue("Billing", out AccountCompanyContact billingCompanyContact))
            {
                AddRDLCParameter(rdlcReportParameters, RDLCParameter.Email, billingCompanyContact.Email, true);
                AddRDLCParameter(rdlcReportParameters, RDLCParameter.AccountBillingContact, billingCompanyContact.ContactName, true);

                if (billingCompanyContact.PhoneNumbers != null && billingCompanyContact.PhoneNumbers.Count > 0)
                {
                    string accountNumbers = string.Join(", ", billingCompanyContact.PhoneNumbers);
                    AddRDLCParameter(rdlcReportParameters, RDLCParameter.AccountNumber, accountNumbers, true);
                }
            }

            if (companyProfile.Contacts.TryGetValue("Main", out AccountCompanyContact accountCompanyContact))
            {
                if (!string.IsNullOrEmpty(accountCompanyContact.ContactName))
                {
                    string salutation = accountCompanyContact.Salutation.HasValue ? Utilities.GetEnumDescription(accountCompanyContact.Salutation.Value) : string.Empty;
                    AddRDLCParameter(rdlcReportParameters, RDLCParameter.Attn, $"{salutation} {accountCompanyContact.ContactName}", true);
                }
            }
        }

        private void AddCompanyProfilePhoneNumbers(Dictionary<string, VRRdlcReportParameter> rdlcReportParameters, AccountPartCompanyProfile companyProfile)
        {
            List<string> allPhoneNumbers = new List<string>();

            if (companyProfile.PhoneNumbers != null)
                allPhoneNumbers.AddRange(companyProfile.PhoneNumbers);

            if (companyProfile.MobileNumbers != null)
                allPhoneNumbers.AddRange(companyProfile.MobileNumbers);

            string concatenatedPhoneNumbers = string.Join(", ", allPhoneNumbers);

            AddRDLCParameter(rdlcReportParameters, RDLCParameter.Phone, concatenatedPhoneNumbers, true);
        }

        private void AddCompanySettingsParameters(Dictionary<string, VRRdlcReportParameter> rdlcReportParameters, CompanySetting companySetting)
        {
            if (companySetting == null)
                return;

            if (companySetting.Contacts.TryGetValue("Billing", out CompanyContact companyContact))
            {
                AddRDLCParameter(rdlcReportParameters, RDLCParameter.BillingCompanyPhone, companyContact.Phone, true);
                AddRDLCParameter(rdlcReportParameters, RDLCParameter.BillingCompanyContact, $"{companyContact.Title} {companyContact.ContactName}", true);
            }

            var logo = new VRFileManager().GetFile(companySetting.CompanyLogo);
            if (logo != null)
                AddRDLCParameter(rdlcReportParameters, RDLCParameter.Image, Convert.ToBase64String(logo.Content), true);

            AddRDLCParameter(rdlcReportParameters, RDLCParameter.RegNo, companySetting.RegistrationNumber, true);
            AddRDLCParameter(rdlcReportParameters, RDLCParameter.RegAddress, companySetting.RegistrationAddress, true);
            AddRDLCParameter(rdlcReportParameters, RDLCParameter.CompanyName, companySetting.CompanyName, true);
            AddRDLCParameter(rdlcReportParameters, RDLCParameter.CompanyProfileName, companySetting.ProfileName, true);
            AddRDLCParameter(rdlcReportParameters, RDLCParameter.VatID, companySetting.VatId, true);
            AddRDLCParameter(rdlcReportParameters, RDLCParameter.BillingCompanyEmail, companySetting.BillingEmails, true);

            if (companySetting.Faxes != null && companySetting.Faxes.Count > 0)
                AddRDLCParameter(rdlcReportParameters, RDLCParameter.CompanyFax, string.Join(", ", companySetting.Faxes), true);
        }

        private void AddRDLCParameter(Dictionary<string, VRRdlcReportParameter> rdlcReportParameters, RDLCParameter key, string value, bool isVisible)
        {
            rdlcReportParameters.Add(key.ToString(), new VRRdlcReportParameter { Value = value, IsVisible = isVisible });
        }

        private FinancialAccountData GetFinancialAccountData(string partnerId)
        {
            var financialAccountData = new FinancialAccountManager().GetFinancialAccountData(_acountBEDefinitionId, partnerId);
            financialAccountData.ThrowIfNull("financialAccountData", partnerId);

            return financialAccountData;
        }

        private Account GetAccountByPartnerId(string partnerId)
        {
            var financialAccountData = GetFinancialAccountData(partnerId);
            financialAccountData.Account.ThrowIfNull("financialAccountData.Account", partnerId);

            return financialAccountData.Account;
        }

        #endregion
    }
}