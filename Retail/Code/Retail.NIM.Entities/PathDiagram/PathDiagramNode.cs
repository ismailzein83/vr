using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.NIM.Entities
{

    public class PathDiagramNode
    {
        public string NodeId { get; set; }
        public string ParentId { get; set; }
        public string Number { get; set; }
        public Guid ElementTypeId { get; set; }
    }
}
