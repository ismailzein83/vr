using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.NIM.Entities
{
    public class Connection
    {
        public long ConnectionId { get; set; }
        public int ModelId { get; set; }
        public long AreaId { get; set; }
        public long SiteId { get; set; }
        public long Port1Id { get; set; }
        public Guid Port1StatusId { get; set; }
        public long Port1NodeId { get; set; }
        public long? Port1PartId { get; set; }
        public Guid? Port1PartTypeId { get; set; }
        public Guid Port1NodeTypeId { get; set; }
        public long Port2Id { get; set; }
        public Guid Port2StatusId { get; set; }
        public long Port2NodeId { get; set; }
        public long? Port2PartId { get; set; }
        public Guid? Port2PartTypeId { get; set; }
        public Guid Port2NodeTypeId { get; set; }
    }
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
