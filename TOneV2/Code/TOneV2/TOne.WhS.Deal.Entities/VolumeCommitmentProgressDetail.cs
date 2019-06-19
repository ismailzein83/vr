using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Deal.Entities
{
    public class VolumeCommitmentProgressDetail
    {
        public string GroupName { get; set; }
        public decimal Reached { get; set; }
        public int LastTierReached { get; set; }
        public int ZoneGroupNumber { get; set; }
        public bool IsSale { get; set; }
    }
}
