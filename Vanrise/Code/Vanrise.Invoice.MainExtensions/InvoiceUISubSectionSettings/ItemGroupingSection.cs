using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Invoice.Entities;

namespace Vanrise.Invoice.MainExtensions
{
    public class ItemGroupingSection : InvoiceSubSectionSettings
    {
        public override Guid ConfigId { get { return new Guid("8A958396-18C2-4913-BABB-FF31683C6A17"); } }
        public Guid ItemGroupingId { get; set; }
        public ItemGroupingSectionSettings Settings { get; set; }
    }
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
