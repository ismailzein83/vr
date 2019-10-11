using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.RouteSync.Entities
{
    public class CodeData
    {
        public string Code { get; set; }

        public int MinCodeLength { get; set; }

        public int MaxCodeLength { get; set; }

        public string CodeCharge { get; set; }

    }
}
