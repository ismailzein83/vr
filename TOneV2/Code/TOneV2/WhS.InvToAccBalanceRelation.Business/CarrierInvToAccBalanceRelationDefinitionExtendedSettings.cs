using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.InvToAccBalanceRelation.Entities;

namespace WhS.InvToAccBalanceRelation.Business
{
    public class CarrierInvToAccBalanceRelationDefinitionExtendedSettings : InvToAccBalanceRelationDefinitionExtendedSettings
    {
        public override Guid ConfigId
        {
            get { throw new NotImplementedException(); }
        }

        public override List<InvoiceAccountInfo> GetBalanceInvoiceAccounts(IInvToAccBalanceRelGetBalanceInvoiceAccountsContext context)
        {
            throw new NotImplementedException();
        }

        public override List<BalanceAccountInfo> GetInvoiceBalanceAccounts(IInvToAccBalanceRelGetInvoiceBalanceAccountsContext context)
        {
            throw new NotImplementedException();
        }
    }
}
