using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Invoice.Entities
{
    public class InvoiceTypeUISettings
    {
        public PartnerSettings PartnerSettings { get; set; }
        public List<InvoiceUIGridColumn> MainGridColumns { get; set; }
        public List<InvoiceGridAction> InvoiceGridActions { get; set; }
        public List<InvoiceUISubSection> SubSections { get; set; }

        public string GenerationCustomSectionDirective { get; set; }
    }
    public abstract class PartnerSettings
    {
        public virtual Guid ConfigId { get; set; }
        public virtual string PartnerSelector { get; set; }
        public virtual IPartnerManager PartnerManagerFQTN { get; set; }
    }
}
