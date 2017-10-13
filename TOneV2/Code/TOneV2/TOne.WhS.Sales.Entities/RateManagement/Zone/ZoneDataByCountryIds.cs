using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Sales.Entities
{
    public class ZoneDataByCountryIds : Dictionary<int, List<DataByZone>>, Vanrise.BusinessProcess.Entities.IRuleTarget
    {
        public object Key
        {
            get { return null; }
        }

        public string TargetType
        {
            get { return "Zones"; }
        }
    }
}
