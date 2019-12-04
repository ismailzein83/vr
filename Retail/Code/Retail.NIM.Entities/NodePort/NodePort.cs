using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.NIM.Entities
{
    public enum ConnectionDirection { Output = 1, Input = 2 }
    public class NodePort
    {
        public long NodePortId { get; set; }
        public string Number { get; set; }
        public Guid NodePortTypeId { get; set; }
        public int ModelId { get; set; }
        public Guid StatusId { get; set; }
        public long NodeId { get; set; }
        public Guid NodeTypeId { get; set; }
        public long? PartId { get; set; }
        public Guid? PartTypeId { get; set; }
        public ConnectionDirection? ConnectionDirection { get; set; }
        public string Notes { get; set; }
    }
}
