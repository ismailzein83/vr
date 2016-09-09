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
    public class OverallInvoiceCounterSerialNumberPart : VRConcatenatedPartSettings<IInvoiceSerialNumberConcatenatedPartContext>
    {
        public bool UsePartnerCount { get; set; }
        public int OverAllStartUpCounter { get; set; }
        public override string GetPartText(IInvoiceSerialNumberConcatenatedPartContext context)
        {
            InvoiceManager invoiceManager = new InvoiceManager();
            string partnerId = null;
            if(this.UsePartnerCount)
                partnerId = context.Invoice.PartnerId;
            var counter = invoiceManager.GetOverAllInvoiceCount(context.InvoiceTypeId,partnerId);
            return (counter + OverAllStartUpCounter).ToString();
        }
    }
}
