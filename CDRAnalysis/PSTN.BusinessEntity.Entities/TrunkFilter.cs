using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSTN.BusinessEntity.Entities
{
    public class TrunkFilter
    {
        public List<int> SwitchIds { get; set; }

        public string TrunkNameFilter { get; set; }
    }
}
