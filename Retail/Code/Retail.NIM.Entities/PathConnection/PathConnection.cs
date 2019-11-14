using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.NIM.Entities
{
    public class PathConnection
    {
        public long PathConnectionId { get; set; }
        public long ConnectionId { get; set; }
        public Guid ConnectionTypeId { get; set; }
        public long PathId { get; set; }
        public long Port1Id { get; set; }
        public long Port1NodeId { get; set; }
        public Guid Port1NodeTypeId { get; set; }
        public long? Port1NodePartId { get; set; }
        public Guid? Port1NodePartTypeId { get; set; }

        public long Port2Id { get; set; }
        public long Port2NodeId { get; set; }
        public Guid Port2NodeTypeId { get; set; }
        public long? Port2NodePartId { get; set; }
        public Guid? Port2NodePartTypeId { get; set; }

    }
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
