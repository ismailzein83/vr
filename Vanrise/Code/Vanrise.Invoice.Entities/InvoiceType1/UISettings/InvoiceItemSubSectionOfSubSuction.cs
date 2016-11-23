using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.Invoice.Entities
{
    public class InvoiceItemSubSectionOfSubSuction
    {
        public Guid UniqueSectionID { get; set; }
        public string SectionTitle { get; set; }
        public InvoiceItemSubSectionOfSubSuctionSettings Settings { get; set; }
    }
    public class InvoiceItemSubSectionOfSubSuctionSettings
    {
        public List<InvoiceItemConcatenatedPart> ItemSetNameParts { get; set; }
        public List<InvoiceSubSectionGridColumn> GridColumns { get; set; }
        public List<InvoiceItemSubSectionOfSubSuction> SubSections { get; set; }
    }
    public class InvoiceItemConcatenatedPart : VRConcatenatedPart<IInvoiceItemConcatenatedPartContext>
    {

    }
    public interface IInvoiceItemConcatenatedPartContext
    {
        dynamic InvoiceItemDetails { get;}
        string CurrentItemSetName { get;  }
    }
}
