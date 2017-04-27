using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Entities.SummaryTransformation
{
    public class SummaryBatch<T> : ISummaryBatch<T>
        where T : ISummaryItem
    {
        public IEnumerable<T> Items { get; set; }

        public DateTime BatchStart { get; set; }

        public DateTime BatchEnd { get; set; }
    }
}
