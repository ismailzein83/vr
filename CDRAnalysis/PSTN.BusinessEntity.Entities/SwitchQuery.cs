using System.Collections.Generic;

namespace PSTN.BusinessEntity.Entities
{
    public class SwitchQuery
    {
        public string Name { get; set; }

        public List<int> SelectedTypeIDs { get; set; }

        public string AreaCode { get; set; }

        public List<int> SelectedDataSourceIDs { get; set; }
    }
}
