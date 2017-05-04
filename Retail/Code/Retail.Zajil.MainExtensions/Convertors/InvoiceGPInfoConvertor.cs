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
                            },
                            SourceId = invoiceId.ToString()
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
            newInvoice.CreatedTime = existingInvoice.CreatedTime;
            var gpRefNumber = (newInvoice.Details as InvoiceDetails).GPReferenceNumber;
            newInvoice.Details = GetInvoiceDetails(existingInvoice.Details as InvoiceDetails);
            (newInvoice.Details as InvoiceDetails).GPReferenceNumber = gpRefNumber;
            newInvoice.DueDate = existingInvoice.DueDate;
            newInvoice.FromDate = existingInvoice.FromDate;
            newInvoice.InvoiceTypeId = existingInvoice.InvoiceTypeId;
            newInvoice.IssueDate = existingInvoice.IssueDate;
            newInvoice.LockDate = existingInvoice.LockDate;
            newInvoice.Note = existingInvoice.Note;
            newInvoice.PaidDate = existingInvoice.PaidDate;
            newInvoice.PartnerId = existingInvoice.PartnerId;
            newInvoice.SerialNumber = existingInvoice.SerialNumber;
            newInvoice.TimeZoneId = existingInvoice.TimeZoneId;
            newInvoice.TimeZoneOffset = existingInvoice.TimeZoneOffset;
            newInvoice.ToDate = existingInvoice.ToDate;
            newInvoice.UserId = existingInvoice.UserId;
            return newInvoice;
        }

        private dynamic GetInvoiceDetails(InvoiceDetails invoiceDetails)
        {
            InvoiceDetails newDetails = new InvoiceDetails();
            newDetails.GPReferenceNumber = invoiceDetails.GPReferenceNumber;
            newDetails.CountCDRs = invoiceDetails.CountCDRs;
            newDetails.CurrencyId = invoiceDetails.CurrencyId;
            newDetails.CustomerPO = invoiceDetails.CustomerPO;
            newDetails.DuePeriod = invoiceDetails.DuePeriod;
            newDetails.SalesAgent = invoiceDetails.SalesAgent;
            newDetails.TotalAmount = invoiceDetails.TotalAmount;
            newDetails.TotalDuration = invoiceDetails.TotalDuration;
            newDetails.VoiceCustomerNo = invoiceDetails.VoiceCustomerNo;
            return newDetails;
        }
    }
}
