using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.SQL;

namespace Vanrise.Invoice.Data.SQL
{
    public class InvoiceSequenceDataManager:BaseSQLDataManager,IInvoiceSequenceDataManager
    {
        #region ctor
        public InvoiceSequenceDataManager()
            : base(GetConnectionStringName("InvoiceDBConnStringKey", "InvoiceDBConnString"))
        {

        }

        #endregion
        public long GetNextSequenceValue(string sequenceGroup, Guid invoiceTypeId, string sequenceKey, long initialValue)
        {
            return (long)ExecuteScalarSP("VR_Invoice.sp_InvoiceSequence_GetNext", sequenceGroup,invoiceTypeId, sequenceKey, initialValue);
        }
    }
}
