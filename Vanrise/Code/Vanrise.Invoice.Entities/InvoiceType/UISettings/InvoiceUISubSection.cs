using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Invoice.Entities
{
    public class InvoiceUISubSection
    {
        public string SectionTitle { get; set; }
        public Vanrise.GenericData.Entities.RecordFilterGroup FilterGroup { get; set; }
        public InvoiceUISubSectionSettings Settings { get; set; }
    }
    public abstract class InvoiceUISubSectionSettings
    {
        public int ConfigId { get; set; }
    }
}
