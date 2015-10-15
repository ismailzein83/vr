using System.Collections.Generic;

namespace PSTN.BusinessEntity.Entities
{
    public class TrunkQuery
    {
        public string Name { get; set; }

        public string Symbol { get; set; }

        public List<int> SelectedSwitchIds { get; set; }

        public List<TrunkType> SelectedTypes { get; set; }

        public List<TrunkDirection> SelectedDirections { get; set; }

        public bool? IsLinkedToTrunk { get; set; }
    }
}
