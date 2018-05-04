using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.AccountBalance.Business;
using Vanrise.Common.Business;
using Vanrise.Invoice.Business;
using Vanrise.Invoice.Entities;
using Vanrise.InvToAccBalanceRelation.Entities;
using Vanrise.GenericData.Business;
using Vanrise.Common;

namespace Vanrise.InvToAccBalanceRelation.Business
{
    public class AccountBalanceInvoicesManager
    {
        public Vanrise.Entities.IDataRetrievalResult<AccountInvoiceDetail> GetFilteredAccountInvoices(Vanrise.Entities.DataRetrievalInput<AccountInvoicesQuery> input)
        {
            Vanrise.Entities.BigResult<AccountInvoiceDetail> finalResults = new Vanrise.Entities.BigResult<AccountInvoiceDetail>
            {
                ResultKey = input.ResultKey,
            };
            AccountTypeManager accountTypeManager = new AccountTypeManager();
            var accountTypeSettings = accountTypeManager.GetAccountTypeSettings(input.Query.AccountTypeId);
            if (accountTypeSettings != null && accountTypeSettings.InvToAccBalanceRelationId.HasValue)
            {
                var invToAccBalanceRelationDefinitionExtendedSettings = new InvToAccBalanceRelationDefinitionManager().GetRelationExtendedSettings(accountTypeSettings.InvToAccBalanceRelationId.Value);
                InvToAccBalanceRelGetBalanceInvoiceAccountsContext context = new InvToAccBalanceRelGetBalanceInvoiceAccountsContext
                {
                    AccountId = input.Query.AccountId,
                    AccountTypeId = input.Query.AccountTypeId,
                    EffectiveOn = DateTime.Now
                };
                var balanceInvoiceAccounts = invToAccBalanceRelationDefinitionExtendedSettings.GetBalanceInvoiceAccounts(context);
                if (balanceInvoiceAccounts != null)
                {
                    List<PartnerInvoiceType> partnerInvoiceTypes = new List<PartnerInvoiceType>();
                    InvoiceManager invoiceManager = new InvoiceManager();
                    foreach (var balanceInvoiceAccount in balanceInvoiceAccounts)
                    {
                        partnerInvoiceTypes.Add(new PartnerInvoiceType
                        {
                            InvoiceTypeId = balanceInvoiceAccount.InvoiceTypeId,
                            PartnerId = balanceInvoiceAccount.PartnerId
                        });
                    }
                    List<AccountInvoiceDetail> accountInvoiceDetails = new List<AccountInvoiceDetail>();

                    var unpaidInvoices = invoiceManager.GetUnPaidPartnerInvoices(partnerInvoiceTypes);
                    if (unpaidInvoices != null)
                    {
                        InvoiceTypeManager invoiceTypeManager = new InvoiceTypeManager();
                        foreach (var unpaidInvoice in unpaidInvoices)
                        {
                            var invoiceType = invoiceTypeManager.GetInvoiceType(unpaidInvoice.InvoiceTypeId);
                            InvoiceRecordObject invoiceRecordObject = new InvoiceRecordObject(unpaidInvoice);
                            invoiceRecordObject.ThrowIfNull("invoiceRecordObject");
                            invoiceRecordObject.InvoiceDataRecordObject.ThrowIfNull("invoiceRecordObject.InvoiceDataRecordObject");
                            accountInvoiceDetails.Add(AccountInvoiceDetailMapper(
                                new AccountInvoice
                                {
                                    SerialNumber = unpaidInvoice.SerialNumber,
                                    InvoiceId = unpaidInvoice.InvoiceId,
                                    FromDate = unpaidInvoice.FromDate,
                                    IssueDate = unpaidInvoice.IssueDate,
                                    Note = unpaidInvoice.Note,
                                    ToDate = unpaidInvoice.ToDate,
                                    DueDate = unpaidInvoice.DueDate,
                                    Amount = invoiceRecordObject.InvoiceDataRecordObject.GetFieldValue(invoiceType.Settings.AmountFieldName),
                                    CurrencyId = invoiceRecordObject.InvoiceDataRecordObject.GetFieldValue(invoiceType.Settings.CurrencyFieldName)
                                }));
                        }
                    }
                   
                    finalResults.Data = accountInvoiceDetails;
                    finalResults.TotalCount = accountInvoiceDetails.Count;
                }
            }


            return finalResults;
        }
        private AccountInvoiceDetail AccountInvoiceDetailMapper(AccountInvoice accountInvoice)
        {
            return new AccountInvoiceDetail
            {
                Entity = accountInvoice,
                CurrencyName = new CurrencyManager().GetCurrencySymbol(accountInvoice.CurrencyId)
            };
        }


    }
}
