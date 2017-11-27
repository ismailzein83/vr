using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.Invoice.Business
{
    public class InvoiceFileExtendedSetting : VRFileExtendedSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("2C65C41E-541F-40C7-A015-7BC098C5ED54"); }
        }
        public long InvoiceId { get; set; }
        public Guid InvoiceTypeId { get; set; }
        public override bool DoesUserHaveViewAccess(IVRFileDoesUserHaveViewAccessContext context)
        {
            return true;
        }
    }
}
