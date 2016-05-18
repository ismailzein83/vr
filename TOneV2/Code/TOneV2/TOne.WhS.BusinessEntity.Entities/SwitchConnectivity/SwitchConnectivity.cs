using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class SwitchConnectivity
    {
        public int SwitchConnectivityId { get; set; }

        public string Name { get; set; }

        public int CarrierAccountId { get; set; }

        public int SwitchId { get; set; }

        public SwitchConnectivitySettings Settings { get; set; }

        public DateTime BED { get; set; }

        public DateTime? EED { get; set; }
    }
}
