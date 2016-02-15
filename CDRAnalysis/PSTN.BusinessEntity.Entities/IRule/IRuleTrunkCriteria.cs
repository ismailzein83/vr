using System.Collections.Generic;

namespace PSTN.BusinessEntity.Entities
{
    public interface IRuleTrunkCriteria
    {
        IEnumerable<int> TrunkIds { get; }
    }
}
