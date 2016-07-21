using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Entities.Status
{
    public class StatusChargingSetQuery
    {
        public string Name { get; set; }

        public List<EntityType> EntityTypes { get; set; }
    }
}
