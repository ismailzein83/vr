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
            : base(GetConnectionStringName("InvoiceTypeDBConnStringKey", "InvoiceTypeDBConnString"))
        {

        }
        #endregion


        #region Public Methods
        public List<Entities.InvoiceDetail> GetInvoices(DataRetrievalInput<InvoiceQuery> input)
        {

            return null;
           // return GetItemsSP("VR_Invoice.sp_Invoice_GetAll", InvoiceMapper);
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
                //Details = GetReaderValue<Guid>(reader,"ID"),
                //FromDate = Vanrise.Common.Serializer.Deserialize<>(reader["Settings"] as string),
                //InvoiceId =,
                //InvoiceTypeId=,
                //IssueDate=,
                //PartnerId=,
                //SerialNumber=,
                //ToDate=,
            };
            return invoice;
        }
        #endregion

    }
}
