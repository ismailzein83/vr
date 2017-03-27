using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.AccountBalance.Entities;
using Vanrise.GenericData.MainExtensions.DataRecordFields;
using Vanrise.Invoice.Business;
using Vanrise.Invoice.Entities;
using Vanrise.InvToAccBalanceRelation.Entities;
using Vanrise.Common;
namespace Vanrise.InvToAccBalanceRelation.Business
{
    public class InvoiceSummaryFieldSourceSetting: AccountBalanceFieldSourceSetting
    {
        public override Guid ConfigId
        {
            get { return new Guid("1FD21C4E-0A0E-4C0D-B567-D65CE039910F"); }
        }

        public override List<AccountBalanceFieldDefinition> GetFieldDefinitions(IAccountBalanceFieldSourceGetFieldDefinitionsContext context)
        {
            List<AccountBalanceFieldDefinition> invoiceFieldDefinitions = new List<AccountBalanceFieldDefinition> {
                new AccountBalanceFieldDefinition{
                    Name = "DuePeriod",
                    Title = "Due Period",
                    FieldType = new FieldNumberType { DataType = FieldNumberDataType.Int },

                }, 
                new AccountBalanceFieldDefinition { 
                    Name = "DueAmount",
                    Title = "Due Amount",
                    FieldType = new FieldNumberType { DataType = FieldNumberDataType.Decimal, DataPrecision = FieldNumberPrecision.Normal },
                }, 
                new AccountBalanceFieldDefinition { 
                    Name = "DueDate",
                    Title = "Due Date",
                    FieldType = new FieldDateTimeType { DataType = FieldDateTimeDataType.Date },
                }
            };
            return invoiceFieldDefinitions;
        }

        public override object PrepareSourceData(IAccountBalanceFieldSourcePrepareSourceDataContext context)
        {
            InvoiceManager invoiceManager = new InvoiceManager();
            InvToAccBalanceRelationDefinitionManager invToAccBalanceRelationDefinitionManager = new InvToAccBalanceRelationDefinitionManager();
            List<AccountBalanceInvoiceInfo> accountBalanceInvoicesInfo = new List<AccountBalanceInvoiceInfo>();
            List<PartnerInvoiceType> partnerInvoiceTypes = new List<PartnerInvoiceType>();
            foreach(var accountBalance in context.AccountBalances)
            {
                if(context.AccountTypeSettings.InvToAccBalanceRelationId.HasValue)
                {
                    var balanceInvoiceAccounts = invToAccBalanceRelationDefinitionManager.GetBalanceInvoiceAccounts(context.AccountTypeId, accountBalance.AccountId, DateTime.Now);
                    if(balanceInvoiceAccounts != null && balanceInvoiceAccounts.Count > 0)
                    {
                        AccountBalanceInvoiceInfo accountBalanceInvoiceInfo = new AccountBalanceInvoiceInfo
                        {
                            AccountBalance = accountBalance,
                            InvoiceByPartnerInvoiceTypes = new List<InvoiceByPartnerInvoiceType>()
                        };
                        foreach(var balanceInvoiceAccount in balanceInvoiceAccounts )
                        {
                            accountBalanceInvoiceInfo.InvoiceByPartnerInvoiceTypes.Add(new InvoiceByPartnerInvoiceType
                            {
                                InvoiceTypeId = balanceInvoiceAccount.InvoiceTypeId,
                                PartnerId = balanceInvoiceAccount.PartnerId
                            });
                            partnerInvoiceTypes.Add(new PartnerInvoiceType{ InvoiceTypeId = balanceInvoiceAccount.InvoiceTypeId, PartnerId = balanceInvoiceAccount.PartnerId });
                        }
                        accountBalanceInvoicesInfo.Add(accountBalanceInvoiceInfo);
                    }
                }
            }
           
            Dictionary<InvoiceByPartnerInvoiceType,List<Invoice.Entities.Invoice>> invoices = invoiceManager.GetUnPaidPartnerInvoices(partnerInvoiceTypes);

            Dictionary<string,InvoiceFieldsData> invoiceFieldsByAccountId = new Dictionary<string,InvoiceFieldsData>();

            if (invoices != null && invoices.Count() > 0)
            {
                foreach(var accountBalanceInvoiceInfo in accountBalanceInvoicesInfo)
                {
                    List<Invoice.Entities.Invoice> accountInvoices = new List<Invoice.Entities.Invoice>();

                    foreach(var item in  accountBalanceInvoiceInfo.InvoiceByPartnerInvoiceTypes)
                    {
                        List<Invoice.Entities.Invoice> invoicesPerType;
                        if(invoices.TryGetValue(item,out invoicesPerType))
                        {
                            accountInvoices.AddRange(invoicesPerType);
                        }
                    }
                    if (accountInvoices.Count > 0)
                    {
                        var invoice = accountInvoices.OrderBy(x => x.ToDate).Last();
                        invoiceFieldsByAccountId.Add(accountBalanceInvoiceInfo.AccountBalance.AccountId, new InvoiceFieldsData
                        {
                            DueDate = invoice.DueDate,
                            ToDate = invoice.ToDate,
                            DuePeriod = Convert.ToInt32((invoice.DueDate - invoice.IssueDate).TotalDays)
                        });
                    }
                }
            }
            return invoiceFieldsByAccountId;
        }

        public override object GetFieldValue(IAccountBalanceFieldSourceGetFieldValueContext context)
        {
            Dictionary<string, InvoiceFieldsData> invoiceFieldsByAccountId = context.PreparedData as Dictionary<string, InvoiceFieldsData>;
            var invoiceFieldsData = invoiceFieldsByAccountId.GetRecord(context.AccountBalance.AccountId);
            if (invoiceFieldsData != null)
            {
                switch (context.FieldName)
                {
                    case "DuePeriod": return invoiceFieldsData.DuePeriod;
                    case "DueDate": return invoiceFieldsData.DueDate;
                    case "DueAmount": return invoiceFieldsData.DueAmount;
                    default: return null;
                }
            }
            return null;
        }
        private class InvoiceFieldsData
        {
            public int DuePeriod { get; set; }
            public DateTime DueDate { get; set; }
            public decimal DueAmount { get; set; }
            public DateTime ToDate { get; set; }
        }
        private class AccountBalanceInvoiceInfo
        {

            public AccountBalance.Entities.AccountBalance AccountBalance { get; set; }
            public List<InvoiceByPartnerInvoiceType> InvoiceByPartnerInvoiceTypes { get; set; }
        }
    }

}
