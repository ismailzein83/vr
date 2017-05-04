using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Retail.Zajil.Entities;
using Vanrise.BEBridge.Entities;
using Vanrise.Common;
using Vanrise.Entities;
using Vanrise.Invoice.Business;
using Vanrise.Invoice.Entities;

namespace Retail.Zajil.MainExtensions.Convertors
{
    public class InvoiceGPInfoConvertor : TargetBEConvertor
    {
        public string GPReferenceNumberColumn { get; set; }
        public string SourceIdColumn { get; set; }
        public override string Name
        {
            get
            {
                return "Zajil Invoice GP Info Convertor";
            }
        }
        public override void ConvertSourceBEs(ITargetBEConvertorConvertSourceBEsContext context)
        {
            SqlSourceBatch sourceBatch = context.SourceBEBatch as SqlSourceBatch;
            List<ITargetBE> transactionTargetBEs = new List<ITargetBE>();
            foreach (DataRow row in sourceBatch.Data.Rows)
            {
                long invoiceId = (long)row[this.SourceIdColumn];
                try
                {
                    InvoiceManager invoiceManager = new InvoiceManager();
                    if (invoiceId <= 0)
                    {
                        context.WriteBusinessTrackingMsg(LogEntryType.Error, "Failed to import Invoice GP Info (SourceId: '{0}')", invoiceId);
                        continue;
                    }

                    SourceInvoice sourceInvoice = new SourceInvoice
                    {
                        Invoice = new Invoice
                        {
                            InvoiceId = invoiceId,
                            Details = new InvoiceDetails
                            {
                                GPReferenceNumber = row[GPReferenceNumberColumn] as string
                            }
                        }
                    };
                    transactionTargetBEs.Add(sourceInvoice);
                }
                catch (Exception ex)
                {
                    var finalException = Utilities.WrapException(ex, String.Format("Failed to import Invoice GP Info (SourceId: '{0}') due to conversion error", invoiceId));
                    context.WriteBusinessHandledException(finalException);
                }
            }
            context.TargetBEs = transactionTargetBEs;
        }

        public override void MergeTargetBEs(ITargetBEConvertorMergeTargetBEsContext context)
        {
            SourceInvoice existingBe = context.ExistingBE as SourceInvoice;
            SourceInvoice newBe = context.NewBE as SourceInvoice;

            SourceInvoice finalBe = new SourceInvoice
            {
                Invoice = GetInvoice(newBe.Invoice, existingBe.Invoice)
            };

            context.FinalBE = finalBe;
        }

        private Invoice GetInvoice(Invoice newInvoice, Invoice existingInvoice)
        {
            return newInvoice;
        }
    }
}
