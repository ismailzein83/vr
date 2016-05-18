using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class SwitchConnectivitySettings
    {
        public SwitchConnectivityType ConnectionType { get; set; }

        public int InChannelCount { get; set; }

        public int OutChannelCount { get; set; }

        public int SharedChannelCount { get; set; }

        public decimal MaxMargin { get; set; }

        public List<SwitchConnectivityTrunk> Trunks { get; set; }
    }

    public enum SwitchConnectivityType { TDM = 0, VoIP = 1 }

    public class SwitchConnectivityTrunk
    {
        public string Name { get; set; }
    }
}
