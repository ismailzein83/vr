using Retail.BusinessEntity.Business;
using Retail.QualityNet.Entities;
using System;
using System.Collections.Generic;
using Vanrise.Common.Business;
using Vanrise.Entities;
using Vanrise.Invoice.Entities;

namespace Retail.QualityNet.Business
{
    public class QualityNetInvoicePartnerSettings : InvoicePartnerManager
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
                return "Retail.QualityNet.Business.AssignedFinancialAccountToInvoiceSettingFilter,Retail.QualityNet.Business";
            }
        }

        public QualityNetInvoicePartnerSettings(Guid acountBEDefinitionId)
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

                    int currencyId = accountBEManager.GetCurrencyId(this._acountBEDefinitionId, financialAccountData.Account.AccountId);
                    string currencySymbol = new CurrencyManager().GetCurrencySymbol(currencyId);
                    AddQualityNetInvoiceRDLCParameter(rdlcReportParameters, QualityNetInvoiceRDLCParameter.Currency, currencySymbol, true);

                    string accountName = accountBEManager.GetAccountName(this._acountBEDefinitionId, financialAccountData.Account.AccountId);
                    AddQualityNetInvoiceRDLCParameter(rdlcReportParameters, QualityNetInvoiceRDLCParameter.CustomerName, accountName, true);

                    CompanySetting companySetting = accountBEManager.GetCompanySetting(this._acountBEDefinitionId, financialAccountData.Account.AccountId);
                    if (companySetting != null)
                    {
                        var logo = new VRFileManager().GetFile(companySetting.CompanyLogo);
                        if (logo != null)
                            AddQualityNetInvoiceRDLCParameter(rdlcReportParameters, QualityNetInvoiceRDLCParameter.Image, Convert.ToBase64String(logo.Content), true);
                    }

                    var invoiceItemsDetails = context.Invoice.Details as InvoiceDetails;
                    AddQualityNetInvoiceRDLCParameter(rdlcReportParameters, QualityNetInvoiceRDLCParameter.GrandTotal, invoiceItemsDetails.GrandTotalAmount.ToString(), true);
                  
                    return rdlcReportParameters;
            }
            return null;
        }

        public override string GetPartnerName(IPartnerNameManagerContext context)
        {
            var financialAccountData = new FinancialAccountManager().GetFinancialAccountData(_acountBEDefinitionId, context.PartnerId);
            return new AccountBEManager().GetAccountName(this._acountBEDefinitionId, financialAccountData.Account.AccountId);
        }

        private void AddQualityNetInvoiceRDLCParameter(Dictionary<string, VRRdlcReportParameter> rdlcReportParameters, QualityNetInvoiceRDLCParameter key, string value, bool isVisible)
        {
            rdlcReportParameters.Add(key.ToString(), new VRRdlcReportParameter { Value = value, IsVisible = isVisible });
        }
    }
}