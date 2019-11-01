using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.NIM.Entities
{
    public class PathConnectionInput
    {
        public long PathId { get; set; }
        public long ConnectionId { get; set; }
    }

    public class PathConnectionOutput
    {
        public long PathConnectionId { get; set; }
    }
}
