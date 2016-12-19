using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Invoice.MainExtensions
{
    public class ItemGroupingSectionSettings
    {
        public List<GridDimesionItemGrouping> GridDimesions { get; set; }
        public List<GridMeasureItemGrouping> GridMeasures { get; set; }
        public List<ItemGroupingSubSection> SubSections { get; set; }
    }
    public class ItemGroupingSubSection
    {
        public Guid InvoiceSubSectionId { get; set; }
        public string SectionTitle { get; set; }
        public ItemGroupingSectionSettings Settings { get; set; }
        public Vanrise.GenericData.Entities.RecordFilterGroup SubSectionFilter { get; set; }

    }
    public class GridDimesionItemGrouping
    {
        public Guid DimensionId { get; set; }
        public string Header { get; set; }
        public string WidthFactor { get; set; }
    }
    public class GridMeasureItemGrouping
    {
        public Guid MeasureId { get; set; }
        public string Header { get; set; }
        public string WidthFactor { get; set; }
    }
}
