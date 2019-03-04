using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Invoice.Entities;
using Vanrise.Common;

namespace Vanrise.Invoice.MainExtensions
{
    public class ItemGroupingSection : InvoiceSubSectionSettings
    {
        public override Guid ConfigId { get { return new Guid("8A958396-18C2-4913-BABB-FF31683C6A17"); } }
        public Guid ItemGroupingId { get; set; }
        public ItemGroupingSectionSettings Settings { get; set; }
        public override List<InvoiceSubSectionGridColumn> GetSubsectionGridColumns(InvoiceType invoiceType, Guid uniqueSectionID)
        {
            List<InvoiceSubSectionGridColumn> gridColumns = null;
            foreach (var subsection in invoiceType.Settings.SubSections)
            {
                var invoiceItemSubSection = subsection.Settings as ItemGroupingSection;
                if (invoiceItemSubSection != null)
                {
                    if (subsection.InvoiceSubSectionId == uniqueSectionID)
                    {
                        gridColumns = GetGridColumns(invoiceItemSubSection.Settings.GridDimesions, invoiceItemSubSection.Settings.GridMeasures, invoiceType.Settings.ItemGroupings);
                        break;
                    }
                    else
                    {
                        gridColumns = GetInvoiceSubSectionGridColumn(invoiceItemSubSection.Settings.SubSections, uniqueSectionID, invoiceType.Settings.ItemGroupings);
                        if (gridColumns != null)
                            break;
                    }
                }
            }
            return gridColumns;
        }
        public List<InvoiceSubSectionGridColumn> GetInvoiceSubSectionGridColumn(List<ItemGroupingSubSection> subSections, Guid uniqueSectionID, List<ItemGrouping> itemGroupings)
        {
            if (subSections == null || subSections.Count == 0)
                return null;
            List<InvoiceSubSectionGridColumn> gridColumns = null;

            foreach (var subsection in subSections)
            {
                if (subsection.InvoiceSubSectionId == uniqueSectionID)
                {
                    gridColumns = GetGridColumns(subsection.Settings.GridDimesions, subsection.Settings.GridMeasures, itemGroupings);
                    break;
                }
                else
                {
                    gridColumns = GetInvoiceSubSectionGridColumn(subsection.Settings.SubSections, uniqueSectionID, itemGroupings);
                    if (gridColumns != null)
                        break;
                }
            }
            return gridColumns;
        }
        private List<InvoiceSubSectionGridColumn> GetGridColumns(List<GridDimesionItemGrouping> dimensions, List<GridMeasureItemGrouping> measures, List<ItemGrouping> itemGroupings)
        {
            if (itemGroupings == null || itemGroupings.Count == 0)
                return null;
            List<InvoiceSubSectionGridColumn> gridColumns = new List<InvoiceSubSectionGridColumn>();
            ItemGrouping itemGrouping = itemGroupings.FindRecord(x => x.ItemGroupingId == ItemGroupingId);
            if (itemGrouping != null)
            {
                if (itemGrouping.DimensionItemFields != null && itemGrouping.DimensionItemFields.Count>0 && dimensions != null && dimensions.Count > 0)
                {
                    foreach (var dim in dimensions)
                    {
                        var dimensionInfo = itemGrouping.DimensionItemFields.FindRecord(x => x.DimensionItemFieldId == dim.DimensionId);
                        if (dimensionInfo != null)
                        {
                            gridColumns.Add(new InvoiceSubSectionGridColumn()
                            {
                                FieldName = dimensionInfo.FieldName,
                                FieldType = dimensionInfo.FieldType,
                                Header = dim.Header
                            });
                        }
                    }
                }
                if (itemGrouping.AggregateItemFields != null && itemGrouping.AggregateItemFields.Count>0 && measures != null && measures.Count > 0)
                {
                    foreach (var measure in measures)
                    {
                        var measureInfo = itemGrouping.AggregateItemFields.FindRecord(x => x.AggregateItemFieldId == measure.MeasureId);
                        if (measureInfo != null)
                        {
                            gridColumns.Add(new InvoiceSubSectionGridColumn()
                            {
                                FieldName = measureInfo.FieldName,
                                FieldType = measureInfo.FieldType,
                                Header = measure.Header
                            });
                        }
                    }
                }
            }
            return gridColumns;
        }
    }
}
