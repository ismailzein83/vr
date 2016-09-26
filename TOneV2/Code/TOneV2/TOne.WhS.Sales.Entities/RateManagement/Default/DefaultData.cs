using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.BusinessProcess.Entities;

namespace TOne.WhS.Sales.Entities
{
    public class DefaultData : IRuleTarget
    {
        public object Key
        {
            get { return OwnerType;  }
        }
        public string TargetType
        {
            get { return "DefaultData"; }
        }

        public SalePriceListOwnerType OwnerType { get; set; }
        public IEnumerable<ZoneService> CurrentServices { get; set; }
        public DefaultServiceToAdd DefaultServiceToAdd { get; set; }
    }
}
