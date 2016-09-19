using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;
using Vanrise.GenericData.Business;
using Vanrise.Invoice.Business;
using Vanrise.Invoice.Entities;

namespace Vanrise.Invoice.MainExtensions.VRConcatenatedPart.SerialNumberParts
{
    public enum CounterType {Yearly = 0 }
    public class InvoiceCounterSerialNumberPart : VRConcatenatedPartSettings<IInvoiceSerialNumberConcatenatedPartContext>
    {
        Guid _configId;
        public override Guid ConfigId { get { return _configId; } set { _configId = new Guid("034D0103-916C-48DC-8A7F-986DEB09FE3F"); } }
        public bool UsePartnerCount { get; set; }
        public int OverAllStartUpCounter { get; set; }
        public CounterType? Type { get; set; }

        public override string GetPartText(IInvoiceSerialNumberConcatenatedPartContext context)
        {
            InvoiceManager invoiceManager = new InvoiceManager();
            string partnerId = null;
            if(this.UsePartnerCount)
                partnerId = context.Invoice.PartnerId;

            DateTime? fromDate = null;
            DateTime? toDate = null;
            if(this.Type != null)
            {
                switch(this.Type)
                {
                    case CounterType.Yearly: fromDate = new DateTime(context.Invoice.IssueDate.Year, 1, 1,0,0,0,0); toDate = new DateTime(context.Invoice.IssueDate.Year+1, 1, 1,0,0,0,0); break;
                }
            }

            var counter = invoiceManager.GetInvoiceCount(context.InvoiceTypeId, partnerId, fromDate, toDate);
            return (counter + OverAllStartUpCounter).ToString();
        }
    }
}
