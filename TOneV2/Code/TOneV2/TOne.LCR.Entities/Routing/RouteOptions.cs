using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.LCR.Entities
{
    public class RouteOptions
    {
        public bool IsBlock { get; set; }

        public List<SupplierRoute> Options { get; set; }
    }
}
