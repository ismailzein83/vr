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
    }
}
