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
        public RenamedZone Zone { get; set; }
        public ValidationOutput Result { get; set; }
    }
}
