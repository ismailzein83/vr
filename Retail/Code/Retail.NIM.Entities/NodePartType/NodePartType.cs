using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.NIM.Entities
{
    public class NodePartType
    {
        public Guid NodePartTypeId { get; set; }
        public string Name { get; set; }
        public Guid BusinessEntitityDefinitionId { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedTime { get; set; }
        public int LastModifiedBy { get; set; }
        public DateTime LastModifiedTime { get; set; }
    }
}
