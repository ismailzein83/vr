using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class SalePriceListTemplateTableCell
    {
        public int RowIndex { get; set; }

        public int ColumnIndex { get; set; }

        public object Value { get; set; }
    }
}
