using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.InvToAccBalanceRelation.Entities;

namespace Vanrise.InvToAccBalanceRelation.Business
{
    public class InvToAccBalanceRelGetInvoiceBalanceAccountsContext : IInvToAccBalanceRelGetInvoiceBalanceAccountsContext
    {
        public Guid InvoiceTypeId { get; set; }

        public string PartnerId { get; set; }

        public DateTime EffectiveOn { get; set; }
    }
}
