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
        public CompareOperator CompareOperator { get; set; }
        public List<InvoiceSubSectionGridColumn> GridColumns { get; set; }
        public List<InvoiceItemSubSectionOfSubSuction> SubSections { get; set; }
        public override List<InvoiceSubSectionGridColumn> GetSubsectionGridColumns(InvoiceType invoiceType, Guid uniqueSectionID)
        {
            List<InvoiceSubSectionGridColumn> gridColumns = null;
            foreach (var subsection in invoiceType.Settings.SubSections)
            {
                var invoiceItemSubSection = subsection.Settings as InvoiceItemSubSection;
                if (invoiceItemSubSection != null)
                {
                    if (subsection.InvoiceSubSectionId == uniqueSectionID)
                    {
                        gridColumns = invoiceItemSubSection.GridColumns;
                        break;
                    }
                    else
                    {
                        gridColumns = GetInvoiceSubSectionGridColumn(invoiceItemSubSection.SubSections, uniqueSectionID);
                        if (gridColumns != null)
                            break;
                    }
                }


            }
            return gridColumns;
        }
        public List<InvoiceSubSectionGridColumn> GetInvoiceSubSectionGridColumn(List<InvoiceItemSubSectionOfSubSuction> subSections, Guid uniqueSectionID)
        {
            if (subSections == null || subSections.Count == 0)
                return null;
            List<InvoiceSubSectionGridColumn> gridColumns = null;

            foreach (var subsection in subSections)
            {
                if (subsection.UniqueSectionID == uniqueSectionID)
                {
                    gridColumns = subsection.Settings.GridColumns;
                    break;
                }
                else
                {
                    gridColumns = GetInvoiceSubSectionGridColumn(subsection.Settings.SubSections, uniqueSectionID);
                    if (gridColumns != null)
                        break;
                }
            }
            return gridColumns;
        }
    }

}
