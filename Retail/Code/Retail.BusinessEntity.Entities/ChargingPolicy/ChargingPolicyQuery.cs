using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Entities
{
    public class ChargingPolicyQuery
    {
        public string Name { get; set; }

        public IEnumerable<Guid> ServiceTypeIds { get; set; }
    }
}
