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
            IEnumerable<Guid> bankDetailsIds = null;
            if (financialAccount.CarrierProfileId.HasValue)
            {
                CarrierProfileManager carrierProfileManager = new CarrierProfileManager();
                bankDetailsIds = carrierProfileManager.GetBankDetails(financialAccount.CarrierProfileId.Value);
            }
            else
            {
                CarrierAccountManager carrierAccountManager = new CarrierAccountManager();
                bankDetailsIds = carrierAccountManager.GetBankDetails(financialAccount.CarrierAccountId.Value);
            }
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
                            CorrespondentBankSwiftCode = bankDetail.CorrespondentBankSwiftCode,
                            ACH = bankDetail.ACH,
                            ABARoutingNumber = bankDetail.ABARoutingNumber
                        });
                    }
                }
            }
            return bankDetailsList;
        }
    }
}
