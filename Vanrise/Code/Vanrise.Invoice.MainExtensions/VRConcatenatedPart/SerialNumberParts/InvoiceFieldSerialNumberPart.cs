using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;
using Vanrise.Invoice.Business;
using Vanrise.Invoice.Entities;

namespace Vanrise.Invoice.MainExtensions.VRConcatenatedPart.SerialNumberParts
{
    public class InvoiceFieldSerialNumberPart : VRConcatenatedPartSettings<IInvoiceSerialNumberConcatenatedPartContext>
    {
        public override Guid ConfigId { get { return  new Guid("5C47A8C8-240C-41DA-9A3C-C671BC03D478"); } }
        public Entities.InvoiceField Field { get; set; }
        public string FieldName { get; set; }
        public override string GetPartText(IInvoiceSerialNumberConcatenatedPartContext context)
        {
            switch (this.Field)
            {
                case Entities.InvoiceField.DueDate: return context.Invoice.DueDate.ToString();
                case Entities.InvoiceField.FromDate: return context.Invoice.FromDate.ToString();
                case Entities.InvoiceField.IssueDate: return context.Invoice.IssueDate.ToString();
                case Entities.InvoiceField.Partner: 
                    PartnerManager partnerManager = new PartnerManager();
                    return partnerManager.GetActualPartnerId(context.InvoiceTypeId, context.Invoice.PartnerId);
                case Entities.InvoiceField.SerialNumber: return context.Invoice.SerialNumber.ToString();
                case Entities.InvoiceField.ToDate: return context.Invoice.ToDate.ToString();
                case Entities.InvoiceField.CustomField: return context.Invoice.Details != null ?
                    context.Invoice.Details.GetType().GetProperty(this.FieldName).GetValue(context.Invoice.Details, null)
               : null;
                       //  Vanrise.Common.Utilities.GetPropValueReader(this.FieldName).GetPropertyValue(context.Invoice.Details) 
            }
            return null;
        }
    }
}
