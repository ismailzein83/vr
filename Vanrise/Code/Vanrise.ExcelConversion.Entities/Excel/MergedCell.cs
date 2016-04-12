using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.ExcelConversion.Entities
{
    public class MergedCell
    {
        public int row { get; set; }
        public int col { get; set; }
        public int rowspan { get; set; }

        public int colspan { get; set; }

    }
}
