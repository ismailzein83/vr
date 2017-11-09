using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess.Entities;

namespace TOne.WhS.CodePreparation.Entities
{
    public class AllNewZones : IRuleTarget
    {
        public IEnumerable<NewZone> Zones { get; set; }
        public object Key
        {
            get { return null; }
        }

        public string TargetType
        {
            get { return "AllNewZones"; }
        }
    }
}
