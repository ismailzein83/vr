using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;
using Vanrise.Invoice.Entities;

namespace Vanrise.Invoice.MainExtensions.VRConcatenatedPart.SerialNumberParts
{
    public enum InvoiceDate { FromDate = 0, ToDate = 1,IssueDate = 2, DueDate = 3,} 
    public class InvoiceDateSerialNumberPart : VRConcatenatedPartSettings<IInvoiceSerialNumberConcatenatedPartContext>
    {
        public override Guid ConfigId { get { return  new Guid("B9CB6032-438E-42FD-9520-E3451FAD6A71"); } }
        public InvoiceDate InvoiceDate { get; set; }
        public string DateFormat { get; set; }
        public override string GetPartText(IInvoiceSerialNumberConcatenatedPartContext context)
        {
            DateTime dateValue = DateTime.MinValue;
            switch (this.InvoiceDate)
            {
                case InvoiceDate.FromDate: dateValue = context.Invoice.FromDate; break;
                case InvoiceDate.ToDate: dateValue = context.Invoice.ToDate; break;
                case InvoiceDate.IssueDate: dateValue = context.Invoice.IssueDate; break;
                case InvoiceDate.DueDate: dateValue = context.Invoice.DueDate; break;
            }
            return dateValue.ToString(DateFormat);
        }
    }
}
