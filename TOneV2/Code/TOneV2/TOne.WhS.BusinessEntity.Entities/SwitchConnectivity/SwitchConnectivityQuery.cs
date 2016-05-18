using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class SwitchConnectivityQuery
    {
        public string Name { get; set; }

        public IEnumerable<int> CarrierAccountIds { get; set; }

        public IEnumerable<int> SwitchIds { get; set; }

        public IEnumerable<SwitchConnectivityType> ConnectionTypes { get; set; }
    }
}
