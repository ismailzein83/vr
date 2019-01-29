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
        public InvoiceSubSectionFilter Filter { get; set; }
        public InvoiceSubSectionSettings Settings { get; set; }

    }
    public abstract class InvoiceSubSectionFilter
    {
        public abstract Guid ConfigId { get; }
        public abstract bool IsFilterMatch(IInvoiceSubSectionFilterContext context);
    }
    public interface IInvoiceSubSectionFilterContext
    {
        InvoiceType InvoiceType { get; }
    }
    public class InvoiceSubSectionFilterContext : IInvoiceSubSectionFilterContext
    {
        public InvoiceType InvoiceType { get; set; }
    }
    public abstract class InvoiceSubSectionSettings
    {
        public abstract Guid ConfigId { get; }
        public virtual Dictionary<Guid, string> GetSubsectionHeaders()
        {
            return null;
        }

    }
}
