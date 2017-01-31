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
        public int PaddingLeft { get; set; }
        public override string GetPartText(IInvoiceSerialNumberConcatenatedPartContext context)
        {
            StringBuilder sequenceKey = new StringBuilder();
            StringBuilder sequenceGroup = new StringBuilder();
            sequenceGroup.Append("OVERALL");
            long initialSequenceValue = new PartnerManager().GetInitialSequenceValue(context.InvoiceTypeId, context.Invoice.PartnerId);

            if (this.IncludePartnerId)
            {
                sequenceKey.Append(context.Invoice.PartnerId);
                sequenceGroup.Append("_");
                sequenceGroup.Append(context.Invoice.PartnerId);
            }
            if (this.DateCounterType.HasValue)
            {
                if (sequenceKey.Length > 0)
                    sequenceKey.Append("_");
                sequenceGroup.Append("_");
                sequenceGroup.Append(Common.Utilities.GetEnumDescription(this.DateCounterType.Value));
                switch (this.DateCounterType)
                {
                    case SerialNumberParts.DateCounterType.Yearly:
                        sequenceKey.Append(string.Format("{0}_{1}", context.Invoice.IssueDate.Year, context.Invoice.IssueDate.Year + 1));
                        break;
                }
            }
            InvoiceSequenceManager manager = new InvoiceSequenceManager();
            var sequenceNumber = manager.GetNextSequenceValue(sequenceGroup.ToString(), context.InvoiceTypeId, sequenceKey.ToString(), initialSequenceValue);
            return sequenceNumber.ToString().PadLeft(this.PaddingLeft, '0');
        }
    }
}
