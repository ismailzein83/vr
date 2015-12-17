using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.CodePreparation.Entities.CP
{
    public class Changes
    {
        public List<ChangedCode> CodeChanges { get; set; }
        public List<NewCode> NewCodes { get; set; }
        public List<NewZone> NewZones { get; set; }
    }
}
