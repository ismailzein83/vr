using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.NIM.Entities
{
    public class NodePart
    {
        public long NodePartId { get; set; }
        public string Number { get; set; }
        public long? ParentPartId { get; set; }
        public long NodeId { get; set; }
        public Guid NodePartTypeId { get; set; }
        public int Model { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedTime { get; set; }
        public int LastModifiedBy { get; set; }
        public DateTime LastModifiedTime { get; set; }

    }
}
