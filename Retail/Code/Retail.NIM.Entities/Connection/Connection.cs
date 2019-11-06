using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.NIM.Entities
{
    public class ConnectionInput
    {
        public int Model { get; set; }
        public long Port1 { get; set; }
        public long Port2 { get; set; }
        public Guid ConnectionType { get; set; }
    }

    public class ConnectionOutput
    {
        public long ConnectionId { get; set; }
    }
}
