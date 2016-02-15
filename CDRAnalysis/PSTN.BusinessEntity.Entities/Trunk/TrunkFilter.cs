using System.Collections.Generic;

namespace PSTN.BusinessEntity.Entities
{
    public class TrunkFilter
    {
        public List<int> SwitchIds { get; set; }

        public string TrunkNameFilter { get; set; }
    }
}
