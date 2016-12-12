using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Invoice.Data;

namespace Vanrise.Invoice.Business
{
    public class InvoiceSequenceManager
    {
        public long GetNextSequenceValue(Guid invoiceTypeId, string sequenceKey, long initialValue)
        {
            IInvoiceSequenceDataManager dataManager = InvoiceDataManagerFactory.GetDataManager<IInvoiceSequenceDataManager>();
            return dataManager.GetNextSequenceValue(invoiceTypeId, sequenceKey, initialValue);
        }
    }
}
