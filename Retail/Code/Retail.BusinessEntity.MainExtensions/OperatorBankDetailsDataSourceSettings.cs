using Retail.BusinessEntity.Business;
using Retail.BusinessEntity.Entities;
using Retail.BusinessEntity.MainExtensions.AccountParts;
using System;
using System.Collections.Generic;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;
using Vanrise.Invoice.Business;
using Vanrise.Invoice.Entities;

namespace Retail.BusinessEntity.MainExtensions
{
    public class OperatorBankDetailsDataSourceSettings : InvoiceDataSourceSettings
    {
        public override Guid ConfigId { get { return new Guid("F0871B89-11BC-42D0-BA88-244DE96F581A"); } }

        public override IEnumerable<dynamic> GetDataSourceItems(IInvoiceDataSourceSettingsContext context)
        {
            var invoice = context.InvoiceActionContext.GetInvoice();
            FinancialAccountManager financialAccountManager = new FinancialAccountManager();
            AccountBEManager accountBEManager = new AccountBEManager();

            var invoiceTypeExtendedSettings = new InvoiceTypeManager().GetInvoiceTypeExtendedSettings(invoice.InvoiceTypeId);
            var invoiceSettings = invoiceTypeExtendedSettings.CastWithValidate<BaseRetailInvoiceTypeSettings>("invoiceTypeExtendedSettings", invoice.InvoiceTypeId);

            var financialAccountMananger = new FinancialAccountManager();
            var financialAccountData = financialAccountMananger.GetFinancialAccountData(invoiceSettings.AccountBEDefinitionId, invoice.PartnerId);

            IEnumerable<BankDetail> bankDetails = null;

            if (financialAccountData != null && financialAccountData.Account != null)
            {
                AccountPart accountPart;
                if (accountBEManager.TryGetAccountPart<AccountPartFinancial>(invoiceSettings.AccountBEDefinitionId, financialAccountData.Account, false, out accountPart))
                {
                    var accountPartSettings = accountPart.Settings as AccountPartFinancial;
                    if (accountPartSettings != null && accountPartSettings.OperatorBanks != null)
                    {
                        bankDetails = accountPartSettings.OperatorBanks;
                    }
                }
            }

            List<BankDetailsDetail> bankDetailsList = new List<BankDetailsDetail>();
            if (bankDetails != null)
            {
                CurrencyManager currencyManager = new CurrencyManager();
                foreach (var bankDetail in bankDetails)
                {
                    bankDetailsList.Add(new BankDetailsDetail
                    {
                        AccountCode = bankDetail.AccountCode,
                        SortCode = bankDetail.SortCode,
                        SwiftCode = bankDetail.SwiftCode,
                        AccountHolder = bankDetail.AccountHolder,
                        AccountNumber = bankDetail.AccountNumber,
                        Address = bankDetail.Address,
                        Bank = bankDetail.Bank,
                        IBAN = bankDetail.IBAN,
                        CurrencyId = bankDetail.CurrencyId,
                        CurrencyName = currencyManager.GetCurrencySymbol(bankDetail.CurrencyId),
                        ChannelName = bankDetail.ChannelName,
                        CorrespondentBank = bankDetail.CorrespondentBank,
                        CorrespondentBankSwiftCode = bankDetail.CorrespondentBankSwiftCode,
                        ACH = bankDetail.ACH,
                        ABARoutingNumber = bankDetail.ABARoutingNumber
                    });
                }
            }
            return bankDetailsList;
        }
    }
}
