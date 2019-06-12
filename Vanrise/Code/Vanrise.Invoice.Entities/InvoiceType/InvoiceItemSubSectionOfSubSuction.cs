using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Business;
using Vanrise.Entities;

namespace Vanrise.Invoice.Entities
{
    public class InvoiceItemSubSectionOfSubSuction
    {
        public Guid UniqueSectionID { get; set; }
        public string SectionTitle { get; set; }
        public string SectionTitleResourceKey { get; set; }
        public InvoiceItemSubSectionOfSubSuctionSettings Settings { get; set; }
        public void ApplyTranslation(IInvoiceTranslationContext context)
        {
            VRLocalizationManager vrLocalizationManager = new VRLocalizationManager();
            if (SectionTitleResourceKey != null)
                SectionTitle = vrLocalizationManager.GetTranslatedTextResourceValue(SectionTitleResourceKey, SectionTitle, context.LanguageId);
            if (Settings != null)
            {
                if (Settings.GridColumns != null && Settings.GridColumns.Count > 0)
                {
                    foreach (var gridColumn in Settings.GridColumns)
                    {
                        if (gridColumn.HeaderResourceKey != null)
                            gridColumn.Header = vrLocalizationManager.GetTranslatedTextResourceValue(gridColumn.HeaderResourceKey, gridColumn.Header, context.LanguageId);
                    }
                }
                if (Settings.SubSections != null && Settings.SubSections.Count > 0)
                {
                    foreach (var subSection in Settings.SubSections)
                    {
                        if (subSection.SectionTitleResourceKey != null)
                            subSection.SectionTitle = vrLocalizationManager.GetTranslatedTextResourceValue(subSection.SectionTitleResourceKey, subSection.SectionTitle, context.LanguageId);
                        subSection.ApplyTranslation(context);
                    }
                }
            }
        }
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
        dynamic InvoiceItemDetails { get; }
        string CurrentItemSetName { get; }
    }
}
