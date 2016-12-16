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

    public class RetailSubscriberPartnerSettings : InvoicePartnerSettings
    {
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

        public override dynamic GetActualPartnerId(IActualPartnerContext context)
        {
            return Convert.ToInt32(context.PartnerId);
        }

        public override int GetPartnerDuePeriod(IPartnerDuePeriodContext context)
        {
            AccountManager accountManager = new AccountManager();
            return accountManager.GetAccountDuePeriod(Convert.ToInt32(context.PartnerId));
        }

        public override dynamic GetPartnerInfo(IPartnerManagerInfoContext context)
        {
            switch (context.InfoType)
            {
                case "InvoiceRDLCReport":
                    Dictionary<string, VRRdlcReportParameter> rdlcReportParameters = new Dictionary<string, VRRdlcReportParameter>();
                    long partnerId = Convert.ToInt32(context.PartnerId);
                    CurrencyManager currencyManager = new CurrencyManager();

                    AccountManager accountManager = new AccountManager();
                    CompanySetting companySetting = accountManager.GetCompanySetting(partnerId);
                    var account = accountManager.GetAccount(partnerId);
                    string accountName = accountManager.GetAccountName(account.AccountId);

                    IAccountPayment accountPayment;
                    accountManager.HasAccountPayment(account.AccountId, false, out accountPayment);
                    string currencySymbol = currencyManager.GetCurrencySymbol(accountPayment.CurrencyId);

                    AddRDLCParameter(rdlcReportParameters, RDLCParameter.AccountName, accountName, true);
                    AddRDLCParameter(rdlcReportParameters, RDLCParameter.Currency, currencySymbol, true);

                    IAccountProfile accountProfile;
                    accountManager.HasAccountProfile(account.AccountId, false, out accountProfile);

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
            AccountManager accountManager = new AccountManager();
            return accountManager.GetAccountName(Convert.ToInt32(context.PartnerId));
        }

        private void AddRDLCParameter(Dictionary<string, VRRdlcReportParameter> rdlcReportParameters, RDLCParameter key, string value, bool isVisible)
        {
            if (rdlcReportParameters == null)
                rdlcReportParameters = new Dictionary<string, VRRdlcReportParameter>();
            rdlcReportParameters.Add(key.ToString(), new VRRdlcReportParameter { Value = value, IsVisible = isVisible });
        }
    }
}
