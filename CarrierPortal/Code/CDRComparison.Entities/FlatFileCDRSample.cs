using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDRComparison.Entities
{
    public class FlatFileCDRSample : CDRSample
    {
        public int ColumnCount { get; set; }

        public IEnumerable<FlatFileDataRow> Rows { get; set; }
    }

    public class FlatFileDataRow
    {
        public IEnumerable<string> Data { get; set; }
    }
}
