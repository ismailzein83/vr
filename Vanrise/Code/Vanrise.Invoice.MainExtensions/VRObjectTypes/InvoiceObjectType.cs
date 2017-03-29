using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;
using Vanrise.Invoice.Business;

namespace Vanrise.Invoice.MainExtensions
{
    public class InvoiceObjectType : VRObjectType
    {
        public override Guid ConfigId
        {
            get { return new Guid("D74A64B6-FDFA-4095-B6CD-6FE0E31E0BE1"); }
        }
        public Guid InvoiceTypeId { get; set; }

        public override object CreateObject(IVRObjectTypeCreateObjectContext context)
        {
            InvoiceManager invoiceManager = new InvoiceManager();
            Vanrise.Invoice.Entities.Invoice invoice = invoiceManager.GetInvoice((long)context.ObjectId);

            if (invoice == null)
                throw new DataIntegrityValidationException(string.Format("Invoice not found for ID: '{0}'", context.ObjectId));

            return invoice;
        }
    }
}
