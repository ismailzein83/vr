using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Sales.Entities
{
    public class AllDataByZone : Vanrise.BusinessProcess.Entities.IRuleTarget
    {
        public IEnumerable<DataByZone> DataByZoneList { get; set; }
        public object Key
        {
            get { return null; }
        }
        public string TargetType
        {
            get { return "AllDataByZone"; }
        }
    }
}
