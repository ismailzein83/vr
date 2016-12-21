using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NP.IVSwitch.Entities
{
    public class AccessList
    {
        public int RouteTableId { get; set; }
        public int TariffId { get; set; }
        public int UserId { get; set; }
    }
}
