using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.CodePreparation.Entities;
using Vanrise.BusinessProcess.Entities;

namespace TOne.WhS.CodePreparation.Entities.Processing
{
    public class AllZonesToProcess : IRuleTarget
    {
        public IEnumerable<ZoneToProcess> Zones { get; set; }
        public object Key
        {
            get { return null; }
        }

        public string TargetType
        {
            get { return "AllZonesToProcess"; }
        }
    }
}
