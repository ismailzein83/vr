using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess.Entities;

namespace Vanrise.NumberingPlan.Entities
{
    public class NewZone : IRuleTarget
    {
        public string Name { get; set; }
        public int CountryId { get; set; }
        public bool hasChanges { get; set; }

        public object Key
        {
            get { return this.Name; }
        }

        public string TargetType
        {
            get { return "Zone"; }
        }
    }
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
