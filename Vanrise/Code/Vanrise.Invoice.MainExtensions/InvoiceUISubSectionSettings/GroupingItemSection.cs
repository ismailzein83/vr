using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Invoice.Entities;

namespace Vanrise.Invoice.MainExtensions
{
    public class GroupingItemSection : InvoiceSubSectionSettings
    {
        public override Guid ConfigId { get { return new Guid("8A958396-18C2-4913-BABB-FF31683C6A17"); } }
        public Guid GroupingItemId { get; set; }
        public List<GridDimesionGroupingItem> GridDimesions { get; set; }
        public List<GridMeasureGroupingItem> GridMeasures { get; set; }
    }
    public class GridDimesionGroupingItem
    {
        public Guid DimensionId { get; set; }
        public string Header { get; set; }
        public string WidthFactor { get; set; }
    }
    public class GridMeasureGroupingItem
    {
        public Guid MeasureId { get; set; }
        public string Header { get; set; }
        public string WidthFactor { get; set; }
    }
}
