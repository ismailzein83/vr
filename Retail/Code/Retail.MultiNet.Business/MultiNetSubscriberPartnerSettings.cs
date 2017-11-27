using Retail.BusinessEntity.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;
using Vanrise.Invoice.Entities;
using Vanrise.Common.Business;
using Retail.BusinessEntity.Entities;
using Retail.BusinessEntity.MainExtensions.AccountParts;
using Vanrise.Common;
using Retail.MultiNet.Entities;
namespace Retail.MultiNet.Business
{
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
        AccountNumber= 14,
        Attn = 15,
        AssignedNumber = 16
    }

    public class MultiNetSubscriberPartnerSettings : InvoicePartnerManager
    {
        Guid _acountBEDefinitionId;
        Guid _companyTypeId;
        Guid _branchTypeId;
        public MultiNetSubscriberPartnerSettings(Guid acountBEDefinitionId,Guid companyTypeId,Guid branchTypeId)
        {
            this._acountBEDefinitionId = acountBEDefinitionId;
            this._companyTypeId = companyTypeId;
            this._branchTypeId = branchTypeId;
        }

        public override string PartnerFilterSelector
        {
            get
            {
                return "retail-multinet-account-invoice-selector";
            }
        }
        public override string PartnerSelector
        {
            get
            {
                return "retail-multinet-account-invoice-selector";
            }
        }
        public override string PartnerInvoiceSettingFilterFQTN
        {
            get
            {
                return "Retail.MultiNet.Business.AssignedFinancialAccountToInvoiceSettingFilter, Retail.MultiNet.Business";
            }
        }

        public override dynamic GetActualPartnerId(IActualPartnerContext context)
        {
            FinancialAccountManager financialAccountManager = new FinancialAccountManager();
            var financialAccountData = financialAccountManager.GetFinancialAccountData(_acountBEDefinitionId, context.PartnerId);
            return financialAccountData.Account.AccountId;
        }
        public override dynamic GetPartnerInfo(IPartnerManagerInfoContext context)
        {
            FinancialAccountManager financialAccountManager = new FinancialAccountManager();
            var financialAccountData = financialAccountManager.GetFinancialAccountData(_acountBEDefinitionId, context.PartnerId);
            switch (context.InfoType)
            {
                case "Account":
                       return financialAccountData.Account;
                case "InvoiceRDLCReport":
                    Dictionary<string, VRRdlcReportParameter> rdlcReportParameters = new Dictionary<string, VRRdlcReportParameter>();
                    CurrencyManager currencyManager = new CurrencyManager();
                    AccountBEManager accountBEManager = new AccountBEManager();
                    CompanySetting companySetting = accountBEManager.GetCompanySetting(financialAccountData.Account.AccountId);
                    string accountName = accountBEManager.GetAccountName(this._acountBEDefinitionId, financialAccountData.Account.AccountId);
                    int currencyId = accountBEManager.GetCurrencyId(this._acountBEDefinitionId, financialAccountData.Account.AccountId);
                    string currencySymbol = currencyManager.GetCurrencySymbol(currencyId);

                    AddRDLCParameter(rdlcReportParameters, RDLCParameter.AccountName, accountName, true);
                    AddRDLCParameter(rdlcReportParameters, RDLCParameter.Currency, currencySymbol, true);
                    CityManager cityManager = new CityManager();

                    IAccountProfile accountProfile;
                    string address = null;
   
                    if(financialAccountData.Account.TypeId ==_companyTypeId ) 
                    {
                        foreach (var accountPart in financialAccountData.Account.Settings.Parts)
                        {
                            var multiNetCompanyExtendedInfo = accountPart.Value.Settings as MultiNetCompanyExtendedInfo;
                            if (multiNetCompanyExtendedInfo != null)
                            {
                                AddRDLCParameter(rdlcReportParameters, RDLCParameter.NTN, multiNetCompanyExtendedInfo.NTN, true);
                                if (multiNetCompanyExtendedInfo.AccountType.HasValue)
                                {
                                    var accountType = Utilities.GetEnumDescription<MultiNetAccountType>(multiNetCompanyExtendedInfo.AccountType.Value);
                                    AddRDLCParameter(rdlcReportParameters, RDLCParameter.AccountType, accountType, true);
                                }
                                AddRDLCParameter(rdlcReportParameters, RDLCParameter.AssignedNumber, multiNetCompanyExtendedInfo.AssignedNumber, true);
                                break;
                            }
                        }
                    }else if(financialAccountData.Account.TypeId ==_branchTypeId )
                    {
                        foreach (var accountPart in financialAccountData.Account.Settings.Parts)
                        {
                            var multiNetBranchExtendedInfo = accountPart.Value.Settings as MultiNetBranchExtendedInfo;
                            if (multiNetBranchExtendedInfo != null)
                            {
                                AddRDLCParameter(rdlcReportParameters, RDLCParameter.NTN, multiNetBranchExtendedInfo.NTN, true);
                                if (multiNetBranchExtendedInfo.AccountType.HasValue)
                                {
                                    var accountType = Utilities.GetEnumDescription<MultiNetAccountType>(multiNetBranchExtendedInfo.AccountType.Value);
                                    AddRDLCParameter(rdlcReportParameters, RDLCParameter.AccountType, accountType, true);
                                }
                                address = multiNetBranchExtendedInfo.BillingAddress;
                                AddRDLCParameter(rdlcReportParameters, RDLCParameter.AssignedNumber, multiNetBranchExtendedInfo.AssignedNumber, true);
                                break;
                            }
                        }
                    }

                    if (accountBEManager.HasAccountProfile(this._acountBEDefinitionId, financialAccountData.Account.AccountId, true, out accountProfile))
                    {
                        var companyProfile = accountProfile.CastWithValidate<AccountPartCompanyProfile>("accountProfile", financialAccountData.Account.AccountId);

                        if (companyProfile != null)
                        {
                            if (financialAccountData.Account.TypeId == _companyTypeId)
                            {
                                address = companyProfile.Address;
                            }
                            string phoneNumbers = null;
                            if (companyProfile.PhoneNumbers != null && companyProfile.PhoneNumbers.Count > 0)
                            {
                                phoneNumbers = string.Join(",", companyProfile.PhoneNumbers);
                            }
                            if (companyProfile.MobileNumbers != null && companyProfile.MobileNumbers.Count > 0)
                            {
                                if (phoneNumbers != null)
                                    phoneNumbers += ",";
                                phoneNumbers += string.Join(",", companyProfile.MobileNumbers);
                            }
                            AddRDLCParameter(rdlcReportParameters, RDLCParameter.Phone, phoneNumbers, true);

                            if (accountProfile.Faxes != null)
                            {
                                string faxes = string.Join(",", accountProfile.Faxes);
                                AddRDLCParameter(rdlcReportParameters, RDLCParameter.Fax, faxes, true);
                            }
                            if (companyProfile.Contacts != null)
                            {
                                AccountCompanyContact accountCompanyContact;
                                if (companyProfile.Contacts.TryGetValue("Main", out accountCompanyContact))
                                {
                                    AddRDLCParameter(rdlcReportParameters, RDLCParameter.Email, accountCompanyContact.Email, true);
                                    if(accountCompanyContact.PhoneNumbers != null && accountCompanyContact.PhoneNumbers.Count > 0)
                                    {
                                        string accountNumbers = string.Join(",", accountCompanyContact.PhoneNumbers);
                                        AddRDLCParameter(rdlcReportParameters, RDLCParameter.AccountNumber, accountNumbers, true);
                                    }
                                    if (accountCompanyContact.ContactName != null)
                                    {
                                        string contactName = "";
                                        if (accountCompanyContact.Salutation.HasValue)
                                        {
                                            contactName = string.Format("{0} ", Utilities.GetEnumDescription(accountCompanyContact.Salutation.Value));
                                        }
                                        contactName += accountCompanyContact.ContactName;
                                        AddRDLCParameter(rdlcReportParameters, RDLCParameter.Attn, contactName, true);
                                    }
                                }
                            }
                        }
                    }
                    AddRDLCParameter(rdlcReportParameters, RDLCParameter.Address, address, true);
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
                        AddRDLCParameter(rdlcReportParameters, RDLCParameter.CompanyName, companySetting.CompanyName, true);
                        AddRDLCParameter(rdlcReportParameters, RDLCParameter.VatID, companySetting.VatId, true);

                    }
                    return rdlcReportParameters;
            }
            return null;
        }
        public override string GetPartnerName(IPartnerNameManagerContext context)
        {
            FinancialAccountManager financialAccountManager = new FinancialAccountManager();
            var financialAccountData = financialAccountManager.GetFinancialAccountData(_acountBEDefinitionId, context.PartnerId);
            AccountBEManager accountBEManager = new AccountBEManager();
            return accountBEManager.GetAccountName(this._acountBEDefinitionId, financialAccountData.Account.AccountId);
        }
        private void AddRDLCParameter(Dictionary<string, VRRdlcReportParameter> rdlcReportParameters, RDLCParameter key, string value, bool isVisible)
        {
            if (rdlcReportParameters == null)
                rdlcReportParameters = new Dictionary<string, VRRdlcReportParameter>();
            rdlcReportParameters.Add(key.ToString(), new VRRdlcReportParameter { Value = value, IsVisible = isVisible });
        }
       
        public override VRInvoiceAccountData GetInvoiceAccountData(IInvoiceAccountDataContext context)
        {
            
            FinancialAccountManager financialAccountManager = new FinancialAccountManager();
            var financialAccountData = financialAccountManager.GetFinancialAccountData(_acountBEDefinitionId, context.PartnerId);
            AccountBEManager accountBEManager = new AccountBEManager();
            return new VRInvoiceAccountData
            {
                BED = financialAccountData.FinancialAccount.BED,
                EED = financialAccountData.FinancialAccount.EED,
                Status = accountBEManager.IsAccountInvoiceActive(financialAccountData.Account)? VRAccountStatus.Active :VRAccountStatus.InActive
            };
        }
    }
}
