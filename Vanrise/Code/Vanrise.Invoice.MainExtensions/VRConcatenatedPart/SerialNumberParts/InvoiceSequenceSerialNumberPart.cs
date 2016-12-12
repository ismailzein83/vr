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
    public enum DateCounterType { Yearly = 0 }
    public class InvoiceSequenceSerialNumberPart : VRConcatenatedPartSettings<IInvoiceSerialNumberConcatenatedPartContext>
    {
        public override Guid ConfigId { get { return new Guid("034D0103-916C-48DC-8A7F-986DEB09FE3F"); } }
        public bool IncludePartnerId { get; set; }
        public DateCounterType? DateCounterType { get; set; }
        public long InitialSequenceValue { get; set; }

        public override string GetPartText(IInvoiceSerialNumberConcatenatedPartContext context)
        {
            StringBuilder sequenceKey = new StringBuilder();
            if (this.IncludePartnerId)
               sequenceKey.Append(context.Invoice.PartnerId);
            if (this.DateCounterType != null)
            {
                switch (this.DateCounterType)
                {
                    case SerialNumberParts.DateCounterType.Yearly:
                        sequenceKey.Append(new DateTime(context.Invoice.IssueDate.Year, 1, 1, 0, 0, 0, 0));
                        sequenceKey.Append(new DateTime(context.Invoice.IssueDate.Year + 1, 1, 1, 0, 0, 0, 0));
                        break;
                }
            }
           InvoiceSequenceManager manager = new InvoiceSequenceManager();
            return manager.GetNextSequenceValue(context.InvoiceTypeId, sequenceKey.ToString(), this.InitialSequenceValue).ToString();
        }
    }
}
