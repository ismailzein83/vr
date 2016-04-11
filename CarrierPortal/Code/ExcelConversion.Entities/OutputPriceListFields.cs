using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExcelConversion.Entities
{
    public class OutputPriceListFields
    {
        public int SheetIndex { get; set; }
        public int FirstRowIndex { get; set; }
        public int CodeCellIndex { get; set; }
        public int ZoneCellIndex { get; set; }
        public int RateCellIndex { get; set; }
        public int EffectiveDateCellIndex { get; set; }
    }
}
