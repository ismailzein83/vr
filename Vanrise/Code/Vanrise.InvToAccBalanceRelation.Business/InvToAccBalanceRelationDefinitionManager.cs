using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.InvToAccBalanceRelation.Entities;
using Vanrise.Common;

namespace Vanrise.InvToAccBalanceRelation.Business
{
    public class InvToAccBalanceRelationDefinitionManager
    {
        #region Public Methods

        public List<InvoiceAccountInfo> GetBalanceInvoiceAccounts(Guid relationId, Guid accountTypeId, string accountId, DateTime effectiveOn)
        {
            InvToAccBalanceRelationDefinitionExtendedSettings relationSettings = GetRelationExtendedSettings(relationId);
            relationSettings.ThrowIfNull("relationSettings", relationId);
            var context = new InvToAccBalanceRelGetBalanceInvoiceAccountsContext { AccountTypeId = accountTypeId, AccountId = accountId, EffectiveOn = effectiveOn };
            return relationSettings.GetBalanceInvoiceAccounts(context);
        }

        public List<BalanceAccountInfo> GetInvoiceBalanceAccounts(Guid relationId, Guid invoiceTypeId, string PartnerId, DateTime effectiveOn)
        {
            InvToAccBalanceRelationDefinitionExtendedSettings relationSettings = GetRelationExtendedSettings(relationId);
            relationSettings.ThrowIfNull("relationSettings", relationId);
            var context = new InvToAccBalanceRelGetInvoiceBalanceAccountsContext { InvoiceTypeId = invoiceTypeId, PartnerId = PartnerId, EffectiveOn = effectiveOn };
            return relationSettings.GetInvoiceBalanceAccounts(context);
        }

        public InvToAccBalanceRelationDefinitionExtendedSettings GetRelationExtendedSettings(Guid relationId)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Private Classes

        private class InvToAccBalanceRelGetBalanceInvoiceAccountsContext: IInvToAccBalanceRelGetBalanceInvoiceAccountsContext
        {
            public Guid AccountTypeId { get; set; }

            public string AccountId { get; set; }

            public DateTime EffectiveOn { get; set; }
        }

        private class InvToAccBalanceRelGetInvoiceBalanceAccountsContext : IInvToAccBalanceRelGetInvoiceBalanceAccountsContext
        {
            public Guid InvoiceTypeId { get; set; }

            public string PartnerId { get; set; }

            public DateTime EffectiveOn { get; set; }
        }

        #endregion
    }
}
