using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSTN.BusinessEntity.Entities
{
    public class SwitchTrunk
    {
        public int ID { get; set; }

        public string Name { get; set; }

        public string Symbol { get; set; }

        public int SwitchID { get; set; }

        public SwitchTrunkType Type { get; set; }

        public SwitchTrunkDirection Direction { get; set; }

        public int? LinkedToTrunkID { get; set; }
    }
}
