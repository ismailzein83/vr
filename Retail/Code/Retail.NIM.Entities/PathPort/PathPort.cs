using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.NIM.Entities
{
    public class PathPort
    {
        public long PathPortId { get; set; }
        public long PortId { get; set; }
        public long PathId { get; set; }
        public long PortNodeId { get; set; }
        public string PortNodeNumber { get; set; }
        public Guid PortNodeTypeId { get; set; }
        public long? PortNodePartId { get; set; }
        public Guid? PortNodePartTypeId { get; set; }
        public long? AreaId { get; set; }
        public long? SiteId { get; set; }
    }
    public class PathPortInput
    {
        public long PathId { get; set; }
        public long PortId { get; set; }
    }

    public class PathPortOutput
    {
        public long PathPortId { get; set; }
    }

    public class RemovePathPortInput
    {
        public long PathPortId { get; set; }
    }

}
