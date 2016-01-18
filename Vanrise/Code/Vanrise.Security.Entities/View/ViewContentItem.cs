using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Security.Entities
{
    public enum ColumnWidthEnum { QuarterRow = 0, FullRow = 1, HalfRow = 2, OneThirdRow = 3 }
    public class ViewContentItem
    {
        public int WidgetId { get; set; }
        public ColumnWidthEnum NumberOfColumns { get; set; }
        public string SectionTitle { get; set; }
        public int? DefaultGrouping { get; set; }
        public int? DefaultPeriod { get; set; }

    }
}
