using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Entities.SummaryTransformation
{
    public interface ISummaryBatch
    {
        DateTime BatchStart { get; set; }
        DateTime BatchEnd { get; set; }
    }

    public interface ISummaryBatch<T> : ISummaryBatch
       where T : ISummaryItem
    {
        IEnumerable<T> Items { get; set; }
    }
}
