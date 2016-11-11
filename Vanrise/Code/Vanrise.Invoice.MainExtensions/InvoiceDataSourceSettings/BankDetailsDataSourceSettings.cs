using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Business;
using Vanrise.Invoice.Entities;

namespace Vanrise.Invoice.MainExtensions
{
    public class BankDetailsDataSourceSettings : InvoiceDataSourceSettings
    {
        public override Guid ConfigId { get { return  new Guid("DE6F2641-A4A8-4F56-AEB4-2A0A25000408"); } }
        public override IEnumerable<dynamic> GetDataSourceItems(IInvoiceDataSourceSettingsContext context)
        {
            Vanrise.Common.Business.ConfigManager configManager = new Vanrise.Common.Business.ConfigManager();
            var bankDetails =  configManager.GetBankDetails();
            List<DataSourceBankDetails> dataSourceBankDetails = new List<DataSourceBankDetails>();
            CurrencyManager currencyManager = new CurrencyManager();
            foreach(var item in bankDetails)
            {
                dataSourceBankDetails.Add(new DataSourceBankDetails
                {
                    AccountCode = item.AccountCode,
                    SortCode = item.SortCode,
                    SwiftCode = item.SwiftCode,
                    AccountHolder = item.AccountHolder,
                    AccountNumber = item.AccountNumber,
                    Address = item.Address,
                    Bank = item.Bank,
                    IBAN = item.IBAN,
                    CurrencyId = item.CurrencyId,
                    CurrencyName = currencyManager.GetCurrencyName(item.CurrencyId)
                });
            }
            return dataSourceBankDetails;
        }
    }
}
