using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XBooster.PriceListConversion.Entities
{
    public class OutputTable
    {
        public int SheetIndex { get; set; }
        public int RowIndex { get; set; }
        public List<OutputFieldMapping> FieldsMapping { get; set; }
    }
}
