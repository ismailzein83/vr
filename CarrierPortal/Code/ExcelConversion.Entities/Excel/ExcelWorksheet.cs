using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExcelConversion.Entities
{
    public class ExcelWorksheet
    {
        public string Name { get; set; }

        public int NumberOfColumns { get; set; }

        public List<ExcelRow> Rows { get; set; }
    }
}
