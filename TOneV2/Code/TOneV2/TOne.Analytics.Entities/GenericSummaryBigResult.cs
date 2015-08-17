using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.Analytics.Entities
{
    public class GenericSummaryBigResult<T> : Vanrise.Entities.BigResult<GroupSummary<T>>
    {
        public T Summary { get; set; }
    }
}
