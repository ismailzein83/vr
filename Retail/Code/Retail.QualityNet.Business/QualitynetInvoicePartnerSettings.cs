using Retail.BusinessEntity.Business;
using Retail.BusinessEntity.Entities;
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
                return "Retail.QualityNet.Business.AssignedFinancialAccountToInvoiceSettingFilter,Retail.QualityNet.Business";
            }
        }

        public QualityNetInvoicePartnerSettings(Guid acountBEDefinitionId)
        {
            this._acountBEDefinitionId = acountBEDefinitionId;
        }

        public override dynamic GetActualPartnerId(IActualPartnerContext context)
        {
            return Convert.ToInt64(context.PartnerId);
        }

        public override VRInvoiceAccountData GetInvoiceAccountData(IInvoiceAccountDataContext context)
        {
            return new VRInvoiceAccountData
            {
                BED = null,
                EED = null,
            };
        }

        public override dynamic GetPartnerInfo(IPartnerManagerInfoContext context)
        {
            switch (context.InfoType)
            {
                case "Account":
                    long accountId = long.Parse(context.PartnerId);
                    return new AccountBEManager().GetAccount(_acountBEDefinitionId, accountId);

                case "InvoiceRDLCReport":
                    Dictionary<string, VRRdlcReportParameter> rdlcReportParameters = new Dictionary<string, VRRdlcReportParameter>();
                    long partnerId = Convert.ToInt64(context.PartnerId);
                    AccountBEManager accountBEManager = new AccountBEManager();

                    var account = accountBEManager.GetAccount(this._acountBEDefinitionId, partnerId);
                    IAccountPayment accountPayment;
                    if (!accountBEManager.HasAccountPayment(this._acountBEDefinitionId, account.AccountId, false, out accountPayment))
                        throw new NullReferenceException($"AccountId {context.PartnerId} for AccoutnBEDefinitionId {_acountBEDefinitionId}  doesn't have account payment");

                    string currencySymbol = new CurrencyManager().GetCurrencySymbol(accountPayment.CurrencyId);
                    AddQualityNetInvoiceRDLCParameter(rdlcReportParameters, QualityNetInvoiceRDLCParameter.Currency, currencySymbol, true);

                    string accountName = accountBEManager.GetAccountName(this._acountBEDefinitionId, account.AccountId);
                    AddQualityNetInvoiceRDLCParameter(rdlcReportParameters, QualityNetInvoiceRDLCParameter.CustomerName, accountName, true);

                    CompanySetting companySetting = accountBEManager.GetCompanySetting(this._acountBEDefinitionId, partnerId);
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
            return new AccountBEManager().GetAccountName(this._acountBEDefinitionId, Convert.ToInt64(context.PartnerId));
        }

        private void AddQualityNetInvoiceRDLCParameter(Dictionary<string, VRRdlcReportParameter> rdlcReportParameters, QualityNetInvoiceRDLCParameter key, string value, bool isVisible)
        {
            rdlcReportParameters.Add(key.ToString(), new VRRdlcReportParameter { Value = value, IsVisible = isVisible });
        }
    }
}