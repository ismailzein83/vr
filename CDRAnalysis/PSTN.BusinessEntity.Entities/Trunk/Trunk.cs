using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSTN.BusinessEntity.Entities
{
    public class Trunk
    {
        public int TrunkId { get; set; }

        public string Name { get; set; }

        public string Symbol { get; set; }

        public int SwitchId { get; set; }

        public TrunkType Type { get; set; }

        public TrunkDirection Direction { get; set; }

        public int? LinkedToTrunkId { get; set; }
    }
}
