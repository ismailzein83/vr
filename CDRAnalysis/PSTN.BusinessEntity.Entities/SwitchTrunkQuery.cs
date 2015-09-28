using System.Collections.Generic;

namespace PSTN.BusinessEntity.Entities
{
    public class SwitchTrunkQuery
    {
        public string Name { get; set; }

        public string Symbol { get; set; }

        public List<int> SelectedSwitchIDs { get; set; }

        public List<SwitchTrunkType> SelectedTypes { get; set; }

        public List<SwitchTrunkDirection> SelectedDirections { get; set; }

        public bool? IsLinkedToTrunk { get; set; }
    }
}
