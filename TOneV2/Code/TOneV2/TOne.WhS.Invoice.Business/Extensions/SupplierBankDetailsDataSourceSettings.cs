using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using Vanrise.Common.Business;
using Vanrise.Entities;
using Vanrise.Invoice.Business;
using Vanrise.Invoice.Entities;

namespace TOne.WhS.Invoice.Business.Extensions
{
    public class SupplierBankDetailsDataSourceSettings : InvoiceDataSourceSettings
    {
        public override Guid ConfigId { get { return new Guid("2CD52A5A-5C91-4D58-AB31-AF94D4CC8AF0"); } }

        public override IEnumerable<dynamic> GetDataSourceItems(IInvoiceDataSourceSettingsContext context)
        {
            var invoice = context.InvoiceActionContext.GetInvoice();
            WHSFinancialAccountManager financialAccountManager = new WHSFinancialAccountManager();
            var financialAccount = financialAccountManager.GetFinancialAccount(Convert.ToInt32(invoice.PartnerId));
            IEnumerable<BankDetail> bankDetails = null;
            if (financialAccount.CarrierProfileId.HasValue)
            {
                CarrierProfileManager carrierProfileManager = new CarrierProfileManager();
                bankDetails = carrierProfileManager.GetSupplierBankDetails(financialAccount.CarrierProfileId.Value);
            }
            else
            {
                CarrierAccountManager carrierAccountManager = new CarrierAccountManager();
                bankDetails = carrierAccountManager.GetSupplierBankDetails(financialAccount.CarrierAccountId.Value);
            }
            List<BankDetailsDetail> bankDetailsList = new List<BankDetailsDetail>();
            if (bankDetails != null)
            {
                CurrencyManager currencyManager = new CurrencyManager();

                foreach (var bankDetail in bankDetails)
                {
                    StringBuilder secondaryAccounts = new StringBuilder();
                    var mainAccountCurrency = currencyManager.GetCurrencySymbol(bankDetail.CurrencyId);
                    secondaryAccounts.AppendLine($"{mainAccountCurrency} N° {bankDetail.AccountNumber} ");

                    if (bankDetail.SecondaryAccounts != null && bankDetail.SecondaryAccounts.Count > 0)
                        foreach (var secondaryAccount in bankDetail.SecondaryAccounts)
                            secondaryAccounts.AppendLine($"{currencyManager.GetCurrencySymbol(secondaryAccount.CurrencyId)} N° {secondaryAccount.AccountNumber} ");

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
                        CurrencyName = mainAccountCurrency,
                        ChannelName = bankDetail.ChannelName,
                        CorrespondentBank = bankDetail.CorrespondentBank,
                        CorrespondentBankSwiftCode = bankDetail.CorrespondentBankSwiftCode,
                        ACH = bankDetail.ACH,
                        ABARoutingNumber = bankDetail.ABARoutingNumber,
                        MoreInfo = bankDetail.MoreInfo,
                        SecondaryAccounts = secondaryAccounts.ToString()
                    });
                }
            }
            return bankDetailsList;
        }
    }
}
