using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace TOne.WhS.Invoice.Entities
{
    public class OriginalInvoiceDataFileSetting : VRFileExtendedSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("A8F04E03-B628-4A2F-ABF5-CCAEF3D205C6"); }
        }
        public long InvoiceId { get; set; }
        public Guid InvoiceTypeId { get; set; }
        public override bool DoesUserHaveViewAccess(IVRFileDoesUserHaveViewAccessContext context)
        {
            return true;
        }
    }
}
