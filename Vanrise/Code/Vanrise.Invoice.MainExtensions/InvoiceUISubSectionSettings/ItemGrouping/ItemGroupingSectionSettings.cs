using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Business;
using Vanrise.Entities;
using Vanrise.Invoice.Entities;

namespace Vanrise.Invoice.MainExtensions
{
    public class ItemGroupingSectionSettings
    {
        public List<GridDimesionItemGrouping> GridDimesions { get; set; }
        public List<GridMeasureItemGrouping> GridMeasures { get; set; }
        public List<ItemGroupingSubSection> SubSections { get; set; }
        public void ApplyTranslation(IInvoiceTranslationContext context)
        {
            VRLocalizationManager vrLocalizationManager = new VRLocalizationManager();

            if (GridDimesions != null && GridDimesions.Count > 0)
            {
                foreach (var gridDimension in GridDimesions)
                {
                    if (gridDimension.HeaderResourceKey != null)
                        gridDimension.Header = vrLocalizationManager.GetTranslatedTextResourceValue(gridDimension.HeaderResourceKey, gridDimension.Header, context.LanguageId);
                }
            }
            if (GridMeasures != null && GridMeasures.Count > 0)
            {
                foreach (var gridMeasure in GridMeasures)
                {
                    if (gridMeasure.HeaderResourceKey != null)
                        gridMeasure.Header = vrLocalizationManager.GetTranslatedTextResourceValue(gridMeasure.HeaderResourceKey, gridMeasure.Header, context.LanguageId);
                }
            }
            if (SubSections != null && SubSections.Count > 0)
            {
                foreach (var subSection in SubSections)
                {
                    if (subSection.SectionTitleResourceKey != null)
                        subSection.SectionTitle = vrLocalizationManager.GetTranslatedTextResourceValue(subSection.SectionTitleResourceKey, subSection.SectionTitle, context.LanguageId);
                    subSection.Settings.ApplyTranslation(context);
                }
            }
        }
    }
    public class ItemGroupingSubSection
    {
        public Guid InvoiceSubSectionId { get; set; }
        public string SectionTitle { get; set; }
        public string SectionTitleResourceKey { get; set; }
        public ItemGroupingSectionSettings Settings { get; set; }
        public Vanrise.GenericData.Entities.RecordFilterGroup SubSectionFilter { get; set; }

    }
    public class GridDimesionItemGrouping
    {
        public Guid DimensionId { get; set; }
        public string Header { get; set; }
        public string HeaderResourceKey { get; set; }
        public GridColumnSettings GridColumnSettings { get; set; }
    }
    public class GridMeasureItemGrouping
    {
        public Guid MeasureId { get; set; }
        public string Header { get; set; }
        public string HeaderResourceKey { get; set; }
        public GridColumnSettings GridColumnSettings { get; set; }
    }
}
