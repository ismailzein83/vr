using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDRComparison.Entities
{
    public class ExcelFileCDRSample : CDRSample
    {
        public int ColumnCount { get; set; }
        public IEnumerable<ExcelFileDataRow> Rows { get; set; }
        public string ErrorMessage { get; set; }
    }

    public class ExcelFileDataRow
    {
        public IEnumerable<string> Data { get; set; }
    }
}
