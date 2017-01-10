using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NP.IVSwitch.Entities
{
    public class CustomerRouteQuery
    {
        public int CustomerId { get; set; }
        public string CodePrefix { get; set; }
        public int Top { get; set; }

    }
}
