using System.Collections.Generic;

namespace PSTN.BusinessEntity.Entities
{
    public interface IRuleSwitchCriteria
    {
        IEnumerable<int> SwitchIds { get; }
    }
}
