using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using Vanrise.Data.SQL;
using Vanrise.Entities;
using Vanrise.Invoice.Entities;

namespace Vanrise.Invoice.Data.SQL
{
    public class InvoiceDataManager:BaseSQLDataManager,IInvoiceDataManager
    {
        
        #region ctor
        public InvoiceDataManager()
            : base(GetConnectionStringName("InvoiceDBConnStringKey", "InvoiceDBConnString"))
        {

        }
        #endregion

        #region Public Methods

        public Entities.Invoice GetInvoice(long invoiceId)
        {
            return GetItemSP("VR_Invoice.sp_Invoice_Get", InvoiceMapper, invoiceId);
        }
        public IEnumerable<Entities.Invoice> GetGetFilteredInvoices(DataRetrievalInput<InvoiceQuery> input)
        {
            return GetItemsSP("VR_Invoice.sp_Invoice_GetFiltered", InvoiceMapper, input.Query.PartnerId, input.Query.FromTime, input.Query.ToTime);
        }
        public void SaveInvoices(Entities.GenerateInvoiceInput createInvoiceInput, Entities.GeneratedInvoice invoice)
        {
            var options = new TransactionOptions
            {
                IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted,
                Timeout = TransactionManager.DefaultTimeout
            };

            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, options))
            {
                object invoiceId;
                ExecuteNonQuerySP("[VR_Invoice].[sp_Invoice_Save]", out invoiceId, createInvoiceInput.InvoiceTypeId, createInvoiceInput.PartnerId, "", createInvoiceInput.FromDate, createInvoiceInput.ToDate, DateTime.Now, DateTime.Now, Vanrise.Common.Serializer.Serialize(invoice.InvoiceDetails));

                InvoiceItemDataManager dataManager = new InvoiceItemDataManager();
                dataManager.SaveInvoiceItems((long)invoiceId,invoice.InvoiceItemSets);
                scope.Complete();
            }
        }
        public bool AreInvoicesUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("VR_Invoice.Invoice", ref updateHandle);
        }
        #endregion
        
        #region Mappers
        public Entities.Invoice InvoiceMapper(IDataReader reader)
        {
            Entities.Invoice invoice = new Entities.Invoice
            {
                Details = Vanrise.Common.Serializer.Deserialize(reader["Details"] as string),
                FromDate = GetReaderValue<DateTime>(reader,"FromDate"),
                InvoiceId = GetReaderValue<long>(reader,"ID"),
                InvoiceTypeId=GetReaderValue<Guid>(reader,"InvoiceTypeId"),
                IssueDate = GetReaderValue<DateTime>(reader, "IssueDate"),
                PartnerId=  reader["PartnerId"] as string,
                SerialNumber= reader["SerialNumber"] as string,
                ToDate=  GetReaderValue<DateTime>(reader,"ToDate"),
            };
            return invoice;
        }
        #endregion

    }
}
