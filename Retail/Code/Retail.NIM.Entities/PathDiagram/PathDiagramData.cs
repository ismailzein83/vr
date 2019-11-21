using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.NIM.Entities
{
    public class PathDiagramData
    {
        public long PathId { get; set; }
        public long NodeId { get; set; }
        public string NodeNumber { get; set; }
        public Guid NodeTypeId { get; set; }
    }
}
