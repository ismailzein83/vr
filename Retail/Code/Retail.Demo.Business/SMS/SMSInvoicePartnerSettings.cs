//using Retail.BusinessEntity.Business;
//using Retail.BusinessEntity.Entities;
//using Retail.BusinessEntity.MainExtensions.AccountParts;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Vanrise.Common;
//using Vanrise.Common.Business;
//using Vanrise.Entities;
//using Vanrise.Invoice.Entities;

//namespace Retail.Demo.Business
//{
//    public enum SMSInvoiceRDLCParameter
//    {
//        AccountName = 0,
//        Image = 1,
//        RegNo = 2,
//        RegAddress = 3,
//        Name = 4,
//        VatID = 5,
//        Currency = 6,
//        Address = 7,
//        Phone = 8,
//        Fax = 9,
//        CompanyName = 10,
//        Email = 11,
//        NTN = 12,
//        AccountType = 13,
//        AccountNumber = 14,
//        Attn = 15,
//        AssignedNumber = 16
//    }
//    public class SMSInvoicePartnerSettings : InvoicePartnerManager
//    {
//        Guid _acountBEDefinitionId;
//        public SMSInvoicePartnerSettings(Guid acountBEDefinitionId)
//        {
//            this._acountBEDefinitionId = acountBEDefinitionId;
//        }

//        public override string PartnerFilterSelector
//        {
//            get
//            {
//                return "retail-be-account-invoice-selector";
//            }
//        }
//        public override string PartnerSelector
//        {
//            get
//            {
//                return "retail-be-account-invoice-selector";
//            }
//        }
//        public override string PartnerInvoiceSettingFilterFQTN
//        {
//            get
//            {
//                return "Retail.Demo.Business.AssignedFinancialAccountToInvoiceSettingFilter, Retail.Demo.Business";
//            }
//        }
//        public override dynamic GetActualPartnerId(IActualPartnerContext context)
//        {
//            FinancialAccountManager financialAccountManager = new FinancialAccountManager();
//            var financialAccountData = financialAccountManager.GetFinancialAccountData(_acountBEDefinitionId, context.PartnerId);
//            return financialAccountData.Account.AccountId;
//        }

//        public override VRInvoiceAccountData GetInvoiceAccountData(IInvoiceAccountDataContext context)
//        {
//            FinancialAccountManager financialAccountManager = new FinancialAccountManager();
//            var financialAccountData = financialAccountManager.GetFinancialAccountData(_acountBEDefinitionId, context.PartnerId);
//            AccountBEManager accountBEManager = new AccountBEManager();
//            return new VRInvoiceAccountData
//            {
//                BED = financialAccountData.FinancialAccount.BED,
//                EED = financialAccountData.FinancialAccount.EED,
//                Status = accountBEManager.IsAccountInvoiceActive(financialAccountData.Account) ? VRAccountStatus.Active : VRAccountStatus.InActive
//            };
//        }

//        public override dynamic GetPartnerInfo(IPartnerManagerInfoContext context)
//        {
//            FinancialAccountManager financialAccountManager = new FinancialAccountManager();
//            var financialAccountData = financialAccountManager.GetFinancialAccountData(_acountBEDefinitionId, context.PartnerId);
//            switch (context.InfoType)
//            {
//                case "Account":
//                    return financialAccountData.Account;
//                case "InvoiceRDLCReport":
//                    Dictionary<string, VRRdlcReportParameter> rdlcReportParameters = new Dictionary<string, VRRdlcReportParameter>();
//                    CurrencyManager currencyManager = new CurrencyManager();
//                    AccountBEManager accountBEManager = new AccountBEManager();
//                    CompanySetting companySetting = accountBEManager.GetCompanySetting(financialAccountData.Account.AccountId);
//                    string accountName = accountBEManager.GetAccountName(this._acountBEDefinitionId, financialAccountData.Account.AccountId);
//                    // int currencyId = accountBEManager.GetCurrencyId(this._acountBEDefinitionId, financialAccountData.Account.AccountId);
//                    int currencyId = currencyManager.GetSystemCurrency().CurrencyId;

//                    string currencySymbol = currencyManager.GetCurrencySymbol(currencyId);

//                    AddSMSInvoiceRDLCParameter(rdlcReportParameters, SMSInvoiceRDLCParameter.AccountName, accountName, true);
//                    AddSMSInvoiceRDLCParameter(rdlcReportParameters, SMSInvoiceRDLCParameter.Currency, currencySymbol, true);
//                    CityManager cityManager = new CityManager();

//                    IAccountProfile accountProfile;
//                    string address = null;
//                    if (accountBEManager.HasAccountProfile(this._acountBEDefinitionId, financialAccountData.Account.AccountId, true, out accountProfile))
//                    {
//                        var companyProfile = accountProfile.CastWithValidate<AccountPartCompanyProfile>("accountProfile", financialAccountData.Account.AccountId);

//                        if (companyProfile != null)
//                        {
//                            address = companyProfile.Address;
//                            string phoneNumbers = null;
//                            if (companyProfile.PhoneNumbers != null && companyProfile.PhoneNumbers.Count > 0)
//                            {
//                                phoneNumbers = string.Join(",", companyProfile.PhoneNumbers);
//                            }
//                            if (companyProfile.MobileNumbers != null && companyProfile.MobileNumbers.Count > 0)
//                            {
//                                if (phoneNumbers != null)
//                                    phoneNumbers += ",";
//                                phoneNumbers += string.Join(",", companyProfile.MobileNumbers);
//                            }
//                            AddSMSInvoiceRDLCParameter(rdlcReportParameters, SMSInvoiceRDLCParameter.Phone, phoneNumbers, true);

//                            if (accountProfile.Faxes != null)
//                            {
//                                string faxes = string.Join(",", accountProfile.Faxes);
//                                AddSMSInvoiceRDLCParameter(rdlcReportParameters, SMSInvoiceRDLCParameter.Fax, faxes, true);
//                            }
//                            if (companyProfile.Contacts != null)
//                            {
//                                AccountCompanyContact accountCompanyContact;
//                                if (companyProfile.Contacts.TryGetValue("Main", out accountCompanyContact))
//                                {
//                                    AddSMSInvoiceRDLCParameter(rdlcReportParameters, SMSInvoiceRDLCParameter.Email, accountCompanyContact.Email, true);
//                                    if (accountCompanyContact.PhoneNumbers != null && accountCompanyContact.PhoneNumbers.Count > 0)
//                                    {
//                                        string accountNumbers = string.Join(",", accountCompanyContact.PhoneNumbers);
//                                        AddSMSInvoiceRDLCParameter(rdlcReportParameters, SMSInvoiceRDLCParameter.AccountNumber, accountNumbers, true);
//                                    }
//                                    if (accountCompanyContact.ContactName != null)
//                                    {
//                                        string contactName = "";
//                                        if (accountCompanyContact.Salutation.HasValue)
//                                        {
//                                            contactName = string.Format("{0} ", Utilities.GetEnumDescription(accountCompanyContact.Salutation.Value));
//                                        }
//                                        contactName += accountCompanyContact.ContactName;
//                                        AddSMSInvoiceRDLCParameter(rdlcReportParameters, SMSInvoiceRDLCParameter.Attn, contactName, true);
//                                    }
//                                }
//                            }
//                        }
//                    }
//                    AddSMSInvoiceRDLCParameter(rdlcReportParameters, SMSInvoiceRDLCParameter.Address, address, true);
//                    if (companySetting != null)
//                    {
//                        VRFileManager fileManager = new VRFileManager();
//                        var logo = fileManager.GetFile(companySetting.CompanyLogo);
//                        if (logo != null)
//                        {
//                            AddSMSInvoiceRDLCParameter(rdlcReportParameters, SMSInvoiceRDLCParameter.Image, Convert.ToBase64String(logo.Content), true);
//                        }
//                        AddSMSInvoiceRDLCParameter(rdlcReportParameters, SMSInvoiceRDLCParameter.RegNo, companySetting.RegistrationNumber, true);
//                        AddSMSInvoiceRDLCParameter(rdlcReportParameters, SMSInvoiceRDLCParameter.RegAddress, companySetting.RegistrationAddress, true);
//                        AddSMSInvoiceRDLCParameter(rdlcReportParameters, SMSInvoiceRDLCParameter.CompanyName, companySetting.CompanyName, true);
//                        AddSMSInvoiceRDLCParameter(rdlcReportParameters, SMSInvoiceRDLCParameter.VatID, companySetting.VatId, true);

//                    }
//                    return rdlcReportParameters;
//            }
//            return null;
//        }

//        public override string GetPartnerName(IPartnerNameManagerContext context)
//        {
//            FinancialAccountManager financialAccountManager = new FinancialAccountManager();
//            var financialAccountData = financialAccountManager.GetFinancialAccountData(_acountBEDefinitionId, context.PartnerId);
//            AccountBEManager accountBEManager = new AccountBEManager();
//            return accountBEManager.GetAccountName(this._acountBEDefinitionId, financialAccountData.Account.AccountId);
//        }

//        private void AddSMSInvoiceRDLCParameter(Dictionary<string, VRRdlcReportParameter> rdlcReportParameters, SMSInvoiceRDLCParameter key, string value, bool isVisible)
//        {
//            if (rdlcReportParameters == null)
//                rdlcReportParameters = new Dictionary<string, VRRdlcReportParameter>();
//            rdlcReportParameters.Add(key.ToString(), new VRRdlcReportParameter { Value = value, IsVisible = isVisible });
//        }

//    }
//}
