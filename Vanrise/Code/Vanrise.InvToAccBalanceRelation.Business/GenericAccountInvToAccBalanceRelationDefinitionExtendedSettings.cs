using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.InvToAccBalanceRelation.Entities;

namespace Vanrise.InvToAccBalanceRelation.Business
{
    public class GenericAccountInvToAccBalanceRelationDefinitionExtendedSettings : InvToAccBalanceRelationDefinitionExtendedSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("58946185-98C0-46B3-B0F8-C1068FFFE58D"); }
        }
        public List<Guid> InvoiceTypeIds { get; set; }
        public Guid? BalanceAccountTypeId { get; set; }
        public override List<InvoiceAccountInfo> GetBalanceInvoiceAccounts(IInvToAccBalanceRelGetBalanceInvoiceAccountsContext context)
        {
            List<InvoiceAccountInfo> invoiceAccountInfo = new List<InvoiceAccountInfo>();
            if (InvoiceTypeIds != null)
            {
                foreach (var invoiceTypeId in InvoiceTypeIds)
                {
                    invoiceAccountInfo.Add(new InvoiceAccountInfo
                    {
                        InvoiceTypeId = invoiceTypeId,
                        PartnerId = context.AccountId
                    });
                }
            }
            return invoiceAccountInfo;
        }

        public override List<BalanceAccountInfo> GetInvoiceBalanceAccounts(IInvToAccBalanceRelGetInvoiceBalanceAccountsContext context)
        {
            List<BalanceAccountInfo> balanceAccountInfo = new List<BalanceAccountInfo>();
            if (BalanceAccountTypeId.HasValue)
            {
                balanceAccountInfo.Add(new BalanceAccountInfo
                {
                    AccountTypeId = BalanceAccountTypeId.Value,
                    AccountId = context.PartnerId
                });
            }
            return balanceAccountInfo;
        }

    }
}
