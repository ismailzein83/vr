using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.CodePreparation.Entities
{
    public class RenamedZoneOutput
    {

        public string Message { get; set; }
        public RenamedZone Zones { get; set; }
        public CodePreparationOutputResult Result { get; set; }
    }
}
