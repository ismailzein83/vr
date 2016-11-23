using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Entities
{
    public class FixedCharging
    {
        public int FixedChargingId { get; set; }

        public string Name { get; set; }

        public FixedChargingSettings Settings { get; set; }
    }
}
