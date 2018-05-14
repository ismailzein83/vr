using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Invoice.Entities;

namespace Retail.Interconnect.Business
{
    public class InterconnectInvoiceGenerator : InvoiceGenerator
    {
        
        #region Constructors
         Guid _acountBEDefinitionId;
         Guid _invoiceTransactionTypeId;
         List<Guid> _usageTransactionTypeIds;
         public InterconnectInvoiceGenerator(Guid acountBEDefinitionId, Guid invoiceTransactionTypeId, List<Guid> usageTransactionTypeIds)
         {
             this._acountBEDefinitionId = acountBEDefinitionId;
             this._invoiceTransactionTypeId = invoiceTransactionTypeId;
             this._usageTransactionTypeIds = usageTransactionTypeIds;
         }

        #endregion

        public override void GenerateInvoice(IInvoiceGenerationContext context)
        {
            throw new NotImplementedException();
        }
    }
}
