using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Invoice.Data
{
    public interface IInvoiceSequenceDataManager:IDataManager
    {
        long GetNextSequenceValue(Guid invoiceTypeId,string sequenceKey,long initialValue);
    }
}
