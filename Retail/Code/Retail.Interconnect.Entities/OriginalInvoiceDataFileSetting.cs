using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Retail.Interconnect.Entities
{
    public class OriginalInvoiceDataFileSetting : VRFileExtendedSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("0F9111CF-EA2D-40EF-A1E4-C403FF1C48E0"); }
        }
        public long InvoiceId { get; set; }
        public Guid InvoiceTypeId { get; set; }
        public override bool DoesUserHaveViewAccess(IVRFileDoesUserHaveViewAccessContext context)
        {
            return true;
        }
    }
}
