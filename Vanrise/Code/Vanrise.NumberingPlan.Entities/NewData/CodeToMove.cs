using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.NumberingPlan.Entities
{
    public class CodeToMove : CodeToAdd
    {
        public string OldZoneName { get; set; }

        public bool HasOverlapedCodesInOtherZone { get; set; }
    }
}
