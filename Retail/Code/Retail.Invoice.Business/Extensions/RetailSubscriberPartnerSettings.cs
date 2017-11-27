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

namespace Retail.Invoice.Business
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
    }

    public class RetailSubscriberPartnerSettings : InvoicePartnerManager
    {
        Guid _acountBEDefinitionId;

        public RetailSubscriberPartnerSettings(Guid acountBEDefinitionId)
        {
            this._acountBEDefinitionId = acountBEDefinitionId;
        }

        public override string PartnerFilterSelector
        {
            get
            {
                return "retail-invoice-financialaccount-selector";
            }
        }

        public override string PartnerSelector
        {
            get
            {
                return "retail-invoice-financialaccount-selector";
            }
        }
        public override string PartnerInvoiceSettingFilterFQTN
        {
            get
            {
                return "Retail.Invoice.Business.AssignedFinancialAccountPartnerInvoiceSettingFilter, Retail.Invoice.Business";
            }
        }
        public override dynamic GetActualPartnerId(IActualPartnerContext context)
        {
            return Convert.ToInt32(context.PartnerId);
        }

        //public override int GetPartnerDuePeriod(IPartnerDuePeriodContext context)
        //{
        //    AccountBEManager accountBEManager = new AccountBEManager();
        //    return accountBEManager.GetAccountDuePeriod(Convert.ToInt32(context.PartnerId));
        //}

        public override dynamic GetPartnerInfo(IPartnerManagerInfoContext context)
        {
            switch (context.InfoType)
            {
                case "Account":
                    AccountBEManager accountBeManager = new AccountBEManager();
                    long accountId = 0;
                    long.TryParse(context.PartnerId, out accountId);
                    return accountBeManager.GetAccount(_acountBEDefinitionId, accountId);
                case "InvoiceRDLCReport":
                    Dictionary<string, VRRdlcReportParameter> rdlcReportParameters = new Dictionary<string, VRRdlcReportParameter>();
                    long partnerId = Convert.ToInt32(context.PartnerId);
                    CurrencyManager currencyManager = new CurrencyManager();

                    AccountBEManager accountBEManager = new AccountBEManager();
                    CompanySetting companySetting = accountBEManager.GetCompanySetting(partnerId);
                    var account = accountBEManager.GetAccount(this._acountBEDefinitionId, partnerId);
                    string accountName = accountBEManager.GetAccountName(this._acountBEDefinitionId, account.AccountId);

                    IAccountPayment accountPayment;
                    accountBEManager.HasAccountPayment(this._acountBEDefinitionId, account.AccountId, false, out accountPayment);
                    string currencySymbol = currencyManager.GetCurrencySymbol(accountPayment.CurrencyId);

                    AddRDLCParameter(rdlcReportParameters, RDLCParameter.AccountName, accountName, true);
                    AddRDLCParameter(rdlcReportParameters, RDLCParameter.Currency, currencySymbol, true);

                    IAccountProfile accountProfile;
                    accountBEManager.HasAccountProfile(this._acountBEDefinitionId, account.AccountId, false, out accountProfile);

                    if (accountProfile != null)
                    {

                        if (accountProfile.Address != null)
                            AddRDLCParameter(rdlcReportParameters, RDLCParameter.Address, accountProfile.Address, true);
                        if (accountProfile.PhoneNumbers != null)
                        {
                            string phoneNumbers = string.Join(",", accountProfile.PhoneNumbers);
                            AddRDLCParameter(rdlcReportParameters, RDLCParameter.Phone, phoneNumbers, true);
                        }
                        if (accountProfile.Faxes != null)
                        {
                            string faxes = string.Join(",", accountProfile.Faxes);
                            AddRDLCParameter(rdlcReportParameters, RDLCParameter.Fax, faxes, true);
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
            }
            return null;
        }

        public override string GetPartnerName(IPartnerNameManagerContext context)
        {
            AccountBEManager accountBEManager = new AccountBEManager();
            return accountBEManager.GetAccountName(this._acountBEDefinitionId, Convert.ToInt64(context.PartnerId));
        }

        private void AddRDLCParameter(Dictionary<string, VRRdlcReportParameter> rdlcReportParameters, RDLCParameter key, string value, bool isVisible)
        {
            if (rdlcReportParameters == null)
                rdlcReportParameters = new Dictionary<string, VRRdlcReportParameter>();
            rdlcReportParameters.Add(key.ToString(), new VRRdlcReportParameter { Value = value, IsVisible = isVisible });
        }

        //public override string GetPartnerSerialNumberPattern(IPartnerSerialNumberPatternContext context)
        //{
        //   return "CONT-#Year#-#YearSequence#";
        //}


        public override VRInvoiceAccountData GetInvoiceAccountData(IInvoiceAccountDataContext context)
        {
            return new VRInvoiceAccountData();
        }
    }
}
