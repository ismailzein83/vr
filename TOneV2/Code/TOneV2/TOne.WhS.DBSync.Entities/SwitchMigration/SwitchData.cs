using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.RouteSync.Entities;
using Vanrise.GenericData.Transformation.Entities;

namespace TOne.WhS.DBSync.Entities
{
    public class SwitchData
    {
        public SwitchRouteSynchronizer SwitchRouteSynchronizer { get; set; }
        public List<MappingRule> MappingRules { get; set; }
    }
}
