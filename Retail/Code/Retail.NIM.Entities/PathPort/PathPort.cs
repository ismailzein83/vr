using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.NIM.Entities
{
    public class PathPortInput
    {
        public long PathId { get; set; }
        public long PortId { get; set; }
    }

    public class PathPortOutput
    {
        public long PathPortId { get; set; }
    }
}
