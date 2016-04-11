using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.CodePreparation.Entities
{
    public class CloseZoneOutput
    {
        public string Message { get; set; }
        public DeletedZone ClosedZone { get; set; }

        public CodePreparationOutputResult Result { get; set; }
    }
}
