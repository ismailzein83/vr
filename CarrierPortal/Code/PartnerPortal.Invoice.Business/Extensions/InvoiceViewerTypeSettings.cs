using PartnerPortal.Invoice.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Invoice.Entities;

namespace PartnerPortal.Invoice.Business.Extensions
{
    public class InvoiceViewerTypeSettings : Vanrise.Entities.VRComponentTypeSettings
    {
        public override Guid VRComponentTypeConfigId
        {
            get { return new Guid("3A02EEEA-6F38-4277-BAC4-9D8F88F71851"); }
        }
        public Guid VRConnectionId { get; set; }
        public Guid InvoiceTypeId { get; set; }
        public InvoiceViewerTypeGridSettings GridSettings { get; set; }
        public InvoiceQueryInterceptor InvoiceQueryInterceptor { get; set; }

    }
    public class InvoiceViewerTypeGridSettings
    {
        public List<InvoiceViewerTypeGridField> InvoiceGridFields { get; set; }
    }
    public class InvoiceViewerTypeGridField
    {
        public string Header { get; set; }
        public InvoiceField Field { get; set; }
        public string CustomFieldName { get; set; }
    }
}
