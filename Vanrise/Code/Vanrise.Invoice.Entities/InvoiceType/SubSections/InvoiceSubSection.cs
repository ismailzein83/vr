using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Invoice.Entities
{
    public class InvoiceSubSection
    {
        public Guid InvoiceSubSectionId { get; set; }
        public string SectionTitle { get; set; }
        public Vanrise.GenericData.Entities.RecordFilterGroup SubSectionFilter { get; set; }
        public InvoiceSubSectionSettings Settings { get; set; }
       
    }
    public abstract class InvoiceSubSectionSettings
    {
        public abstract Guid ConfigId { get; }
    }
}
