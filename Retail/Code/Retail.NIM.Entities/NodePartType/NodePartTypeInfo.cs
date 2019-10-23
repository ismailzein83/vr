using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.NIM.Entities
{
    public class NodePartTypeInfo
    {
        public Guid NodePartTypeId { get; set; }
        public string Name { get; set; }
        public Guid BusinessEntitityDefinitionId { get; set; }
    }
}
