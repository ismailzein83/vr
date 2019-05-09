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
        public void ApplyTranslation(InvoiceTranslationContext context)
        {
            VRLocalizationManager vrLocalizationManager = new VRLocalizationManager();

            if (GridDimesions != null && GridDimesions.Count > 0)
            {
                foreach (var gridDimension in GridDimesions)
                {
                    if (gridDimension.TextResourceKey != null)
                        gridDimension.Header = vrLocalizationManager.GetTranslatedTextResourceValue(gridDimension.TextResourceKey, gridDimension.Header, context.LanguageId);
                }
            }
            if (GridMeasures != null && GridMeasures.Count > 0)
            {
                foreach (var gridMeasure in GridMeasures)
                {
                    if (gridMeasure.TextResourceKey != null)
                        gridMeasure.Header = vrLocalizationManager.GetTranslatedTextResourceValue(gridMeasure.TextResourceKey, gridMeasure.Header, context.LanguageId);
                }
            }
            if (SubSections != null && SubSections.Count > 0)
            {
                foreach (var subSection in SubSections)
                {
                    if (subSection.TextResourceKey != null)
                        subSection.SectionTitle = vrLocalizationManager.GetTranslatedTextResourceValue(subSection.TextResourceKey, subSection.SectionTitle, context.LanguageId);
                    subSection.Settings.ApplyTranslation(context);
                }
            }
        }
    }
    public class ItemGroupingSubSection
    {
        public Guid InvoiceSubSectionId { get; set; }
        public string SectionTitle { get; set; }
        public string TextResourceKey { get; set; }
        public ItemGroupingSectionSettings Settings { get; set; }
        public Vanrise.GenericData.Entities.RecordFilterGroup SubSectionFilter { get; set; }

    }
    public class GridDimesionItemGrouping
    {
        public Guid DimensionId { get; set; }
        public string Header { get; set; }
        public string TextResourceKey { get; set; }
        public GridColumnSettings GridColumnSettings { get; set; }
    }
    public class GridMeasureItemGrouping
    {
        public Guid MeasureId { get; set; }
        public string Header { get; set; }
        public string TextResourceKey { get; set; }
        public GridColumnSettings GridColumnSettings { get; set; }
    }
}
