using System;
using System.Collections.Generic;

namespace TOne.WhS.Deal.Entities
{
    public interface IRuleDealCriteria
    {
        IEnumerable<int> DealIds { get; }
    }
}
