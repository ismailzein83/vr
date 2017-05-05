using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace PartnerPortal.Invoice.Business
{
    public class InvoiceTileDefinitionSettings : VRTileExtendedSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("A8A6C730-9EF7-41E3-B875-C9FF7B6696FC"); }
        }

        public override string RuntimeEditor
        {
            get { return "partnerportal-invoice-invoicetileruntimesettings"; }
        }
        public Guid VRConnectionId { get; set; }
        public Guid InvoiceTypeId { get; set; }
    }
}
