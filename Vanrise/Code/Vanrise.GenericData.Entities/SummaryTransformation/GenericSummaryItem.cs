using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities.SummaryTransformation;

namespace Vanrise.GenericData.Entities
{
    public class GenericSummaryItem : ISummaryItem
    {
        public long SummaryItemId { get; set; }

        public DateTime BatchStart { get; set; }

        public DateTime BatchEnd { get; set; }

        public dynamic DataRecord { get; set; }
    }

    //public class SummaryTransformationBatch : ISummaryBatch<GenericSummaryItem>
    //{
    //    public IEnumerable<GenericSummaryItem> Items { get; set; }

    //    public DateTime BatchStart { get; set; }
    //}

}
