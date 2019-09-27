using Retail.BusinessEntity.Business;
using Retail.QualityNet.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Business;
using Vanrise.Entities;
using Vanrise.Invoice.Entities;

namespace Retail.QualityNet.Business
{
    public class QualitynetInvoicePartnerSettings : InvoicePartnerManager
    {
        Guid _acountBEDefinitionId;

        public override string PartnerFilterSelector
        {
            get
            {
                return "retail-be-account-invoice-selector";
            }
        }
        public override string PartnerSelector
        {
            get
            {
                return "retail-be-account-invoice-selector";
            }
        }
        public override string PartnerInvoiceSettingFilterFQTN
        {
            get
            {
                return "";
            }
        }

        public QualitynetInvoicePartnerSettings(Guid acountBEDefinitionId)
        {
            this._acountBEDefinitionId = acountBEDefinitionId;
        }

        public override dynamic GetActualPartnerId(IActualPartnerContext context)
        {
            var financialAccountData = new FinancialAccountManager().GetFinancialAccountData(_acountBEDefinitionId, context.PartnerId);
            return financialAccountData.Account.AccountId;
        }

        public override VRInvoiceAccountData GetInvoiceAccountData(IInvoiceAccountDataContext context)
        {
            var financialAccountData = new FinancialAccountManager().GetFinancialAccountData(_acountBEDefinitionId, context.PartnerId);
            return new VRInvoiceAccountData
            {
                BED = financialAccountData.FinancialAccount.BED,
                EED = financialAccountData.FinancialAccount.EED,
                Status = new AccountBEManager().IsAccountInvoiceActive(financialAccountData.Account) ? VRAccountStatus.Active : VRAccountStatus.InActive
            };
        }

        public override dynamic GetPartnerInfo(IPartnerManagerInfoContext context)
        {
            var financialAccountData = new FinancialAccountManager().GetFinancialAccountData(_acountBEDefinitionId, context.PartnerId);
            switch (context.InfoType)
            {
                case "Account":
                    return financialAccountData.Account;
                case "InvoiceRDLCReport":
                    Dictionary<string, VRRdlcReportParameter> rdlcReportParameters = new Dictionary<string, VRRdlcReportParameter>();
                    AccountBEManager accountBEManager = new AccountBEManager();

                    CompanySetting companySetting = accountBEManager.GetCompanySetting(this._acountBEDefinitionId, financialAccountData.Account.AccountId);
                    string accountName = accountBEManager.GetAccountName(this._acountBEDefinitionId, financialAccountData.Account.AccountId);
                    // int currencyId = accountBEManager.GetCurrencyId(this._acountBEDefinitionId, financialAccountData.Account.AccountId);

                    CurrencyManager currencyManager = new CurrencyManager();
                    int currencyId = currencyManager.GetSystemCurrency().CurrencyId;
                    string currencySymbol = currencyManager.GetCurrencySymbol(currencyId);
                    AddQualitynetInvoiceRDLCParameter(rdlcReportParameters, QualitynetInvoiceRDLCParameter.Currency, currencySymbol, true);

                    if (companySetting != null)
                    {
                        var logo = new VRFileManager().GetFile(companySetting.CompanyLogo);
                        if (logo != null)
                            AddQualitynetInvoiceRDLCParameter(rdlcReportParameters, QualitynetInvoiceRDLCParameter.Image, Convert.ToBase64String(logo.Content), true);

                        AddQualitynetInvoiceRDLCParameter(rdlcReportParameters, QualitynetInvoiceRDLCParameter.Name, companySetting.CompanyName, true);
                    }

                    return rdlcReportParameters;
            }
            return null;
        }

        public override string GetPartnerName(IPartnerNameManagerContext context)
        {
            var financialAccountData = new FinancialAccountManager().GetFinancialAccountData(_acountBEDefinitionId, context.PartnerId);
            return new AccountBEManager().GetAccountName(this._acountBEDefinitionId, financialAccountData.Account.AccountId);
        }

        private void AddQualitynetInvoiceRDLCParameter(Dictionary<string, VRRdlcReportParameter> rdlcReportParameters, QualitynetInvoiceRDLCParameter key, string value, bool isVisible)
        {
            if (rdlcReportParameters == null)
                rdlcReportParameters = new Dictionary<string, VRRdlcReportParameter>();
            rdlcReportParameters.Add(key.ToString(), new VRRdlcReportParameter { Value = value, IsVisible = isVisible });
        }

    }
}