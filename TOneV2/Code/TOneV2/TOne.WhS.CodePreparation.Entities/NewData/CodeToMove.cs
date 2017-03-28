using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.CodePreparation.Entities.Processing
{
    public class CodeToMove : CodeToAdd
    {
        public string OldZoneName { get; set; }

        public DateTime OldCodeBED { get; set; }

        public bool HasOverlapedCodesInOtherZone { get; set; }
    }
}
