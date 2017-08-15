using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Invoice.Business;
using Vanrise.Invoice.Entities;
using Vanrise.Common;
using Vanrise.Invoice.Business.Context;
namespace Vanrise.Invoice.MainExtensions
{
    public class ConditionalInvoiceFile : InvoiceFileConverter
    {
        public List<InvoiceFileCondition> ConditionalAttachments { get; set; }

        public override Guid ConfigId
        {
            get { return new Guid("C24CE2DE-DDD2-44BD-A2B0-A9133113D7C0"); }
        }

        public override InvoiceFile ConvertToInvoiceFile(IInvoiceRDLCFileConverterContext context)
        {
            if(this.ConditionalAttachments != null)
            {
                var invoice = new InvoiceManager().GetInvoice(context.InvoiceId);
                invoice.ThrowIfNull("invoice",context.InvoiceId);
                InvoiceTypeManager invoiceTypeManager = new InvoiceTypeManager();
                var invoiceType = invoiceTypeManager.GetInvoiceType(invoice.InvoiceTypeId);
                foreach(var conditionalAttachment in this.ConditionalAttachments)
                {
                   
                    InvoiceGridActionFilterConditionContext invoiceFilterConditionContext = new InvoiceGridActionFilterConditionContext
                    {
                        Invoice = invoice,
                        InvoiceType = invoiceType
                    };
                    conditionalAttachment.Condition.ThrowIfNull("conditionalAttachment.Condition");
                    if (conditionalAttachment.Condition.IsFilterMatch(invoiceFilterConditionContext))
                    {
                        var invoiceAttachment = invoiceTypeManager.GeInvoiceTypeAttachment(invoice.InvoiceTypeId, conditionalAttachment.InvoiceAttachmentId);
                        invoiceAttachment.ThrowIfNull("invoiceAttachment", conditionalAttachment.InvoiceAttachmentId);
                        invoiceAttachment.InvoiceFileConverter.ThrowIfNull("invoiceAttachment.InvoiceFileConverter");
                        InvoiceRDLCFileConverterContext invoiceRDLCFileConverterContext = new InvoiceRDLCFileConverterContext
                        {
                            InvoiceId = context.InvoiceId
                        };
                        return invoiceAttachment.InvoiceFileConverter.ConvertToInvoiceFile(invoiceRDLCFileConverterContext);
                    }
                }
            }
            return null;
        }
    }
    public class InvoiceFileCondition
    {
        public string Name { get; set; }
        public Guid InvoiceAttachmentId { get; set; }
        public InvoiceGridActionFilterCondition Condition { get; set; }
    }
}
