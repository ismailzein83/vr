using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Business;
using Vanrise.Entities;
using Vanrise.Invoice.Business;
using Vanrise.Invoice.Business.Extensions;
using Vanrise.Invoice.Entities;

namespace Vanrise.Invoice.MainExtensions
{
    public class BankDetailsDataSourceSettings : InvoiceDataSourceSettings
    {
        public override Guid ConfigId { get { return  new Guid("DE6F2641-A4A8-4F56-AEB4-2A0A25000408"); } }
        public override IEnumerable<dynamic> GetDataSourceItems(IInvoiceDataSourceSettingsContext context)
        {
            var invoice = context.InvoiceActionContext.GetInvoice;
            var invoiceType = new InvoiceTypeManager().GetInvoiceType(invoice.InvoiceTypeId);
            InvoiceTypeExtendedSettingsInfoContext InvoiceTypeExtendedSettingsInfoContext = new InvoiceTypeExtendedSettingsInfoContext
            {
                InfoType = "BankDetails",
                Invoice = invoice
            };
            var bankDetailsIds = invoiceType.Settings.ExtendedSettings.GetInfo(InvoiceTypeExtendedSettingsInfoContext);
            Vanrise.Common.Business.ConfigManager configManager = new Vanrise.Common.Business.ConfigManager();
            var bankDetails = configManager.GetBankDetails();
            List<BankDetailsDetail> bankDetailsList = new List<BankDetailsDetail>();
            if (bankDetailsIds != null)
            {
                CurrencyManager currencyManager = new CurrencyManager();
                foreach (var item in bankDetailsIds)
                {
                    var bankDetail = bankDetails.FirstOrDefault(x => x.BankDetailId == item);
                    if (bankDetail != null)
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
                            CorrespondentBankSwiftCode = bankDetail.CorrespondentBankSwiftCode
                        });
                    }
                }
            }
            return bankDetailsList;
        }
    }
}
