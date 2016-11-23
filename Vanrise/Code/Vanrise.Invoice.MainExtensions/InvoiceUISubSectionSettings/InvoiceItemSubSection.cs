using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;
using Vanrise.Invoice.Entities;

namespace Vanrise.Invoice.MainExtensions
{
    public class InvoiceItemSubSection : InvoiceSubSectionSettings
    {
        public override Guid ConfigId { get { return  new Guid("E46CBB79-5448-460E-A94A-3C6405C5BB5F"); } }
        public string ItemSetName { get; set; }
        public List<InvoiceSubSectionGridColumn> GridColumns { get; set; }
        public List<InvoiceItemSubSectionOfSubSuction> SubSections { get; set; }
    }

}
