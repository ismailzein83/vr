using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.NIM.Entities
{
    public class NodePortType
    {
        public Guid NodePortTypeId { get; set; }
        public string Name { get; set; }
        public Guid BusinessEntitityDefinitionId { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedTime { get; set; }
        public int LastModifiedBy { get; set; }
        public DateTime LastModifiedTime { get; set; }
    }

    public class ReserveConnectionOutput
    {
        public ReservePortOutput Port1 { get; set; }
        public ReservePortOutput Port2 { get; set; }
    }
    public class ReserveConnectionInput
    {
        public Guid ConnectionTypeId { get; set; }
        public long Port1NodeId { get; set; }
        public long Port2NodeId { get; set; }
        public Guid? Port1PartTypeId { get; set; }
        public Guid? Port2PartTypeId { get; set; }

    }
    public class ReservePortInput
    {
        public long NodeId { get; set; }
        public Guid? PartTypeId { get; set; }
    }
    public class ReservePortOutput
    {
        public long PortId { get; set; }
        public string Number { get; set; }
    }
}
