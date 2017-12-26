using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;
using Vanrise.Invoice.Business;
using Vanrise.Invoice.Business.Context;
using Vanrise.Invoice.Entities;

namespace Vanrise.Invoice.MainExtensions.InvoiceFileNamePart
{
    public class InvoiceFieldFileNamePart : VRConcatenatedPartSettings<IInvoiceFileNamePartContext>
    {
        public override Guid ConfigId { get { return new Guid("203D36DD-F867-48D9-A748-0CAC483B5407"); } }
        public Entities.InvoiceField Field { get; set; }
        public string FieldName { get; set; }
        public string DateTimeFormat { get; set; }
        public override string GetPartText(IInvoiceFileNamePartContext context)
        {
            switch (this.Field)
            {
                case Entities.InvoiceField.DueDate: return context.Invoice.DueDate.ToString(DateTimeFormat);
                case Entities.InvoiceField.FromDate: return context.Invoice.FromDate.ToString(DateTimeFormat);
                case Entities.InvoiceField.IssueDate: return context.Invoice.IssueDate.ToString(DateTimeFormat);
                case InvoiceField.IsAutomatic: return context.Invoice.IsAutomatic.ToString();
                case InvoiceField.IsSent: return context.Invoice.SentDate.HasValue.ToString();
                case Entities.InvoiceField.Partner:
                    var partnerManager = new PartnerManager();
                    return partnerManager.GetPartnerName(context.InvoiceTypeId, context.Invoice.PartnerId);
                case Entities.InvoiceField.SerialNumber: return context.Invoice.SerialNumber;
                case Entities.InvoiceField.ToDate: return context.Invoice.ToDate.ToString(DateTimeFormat);
                case Entities.InvoiceField.CustomField: return context.Invoice.Details != null ? context.Invoice.Details.GetType().GetProperty(this.FieldName).GetValue(context.Invoice.Details, null)
               : null;
            }
            return null;
        }
    }
}
