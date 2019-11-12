using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.NIM.Entities
{
    public class NodePortInfo
    {
        public long PortId { get; set; }
        public Guid PortTypeId { get; set; }
        public string PortNumber { get; set; }
        public long NodeId { get; set; }
        public Guid NodeTypeId { get; set; }
        public string NodeNumber { get; set; }
        public List<NodePortPartInfo> NodeParts { get; set; }

    }

    public class NodePortPartInfo
    {
        public Guid NodePartTypeId { get; set; }
        public long NodePartId { get; set; }
        public string NodePartNumber { get; set; }
    }
}
