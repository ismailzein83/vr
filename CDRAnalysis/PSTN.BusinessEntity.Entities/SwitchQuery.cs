using System.Collections.Generic;

namespace PSTN.BusinessEntity.Entities
{
    public class SwitchQuery
    {
        public string Name { get; set; }

        public List<int> SelectedBrandIds { get; set; }

        public string AreaCode { get; set; }
    }
}
