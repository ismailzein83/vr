using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.Invoice.MainExtensions
{
    public class InvoiceObjectType : VRObjectType
    {
        public override Guid ConfigId
        {
            get { return new Guid("D74A64B6-FDFA-4095-B6CD-6FE0E31E0BE1"); }
        }
        public Guid InvoiceTypeId { get; set; }
    }
}
