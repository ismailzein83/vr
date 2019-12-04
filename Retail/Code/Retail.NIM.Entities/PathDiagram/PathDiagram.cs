using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.NIM.Entities
{
    public class PathDiagram
    {
        public List<PathDiagramNode> PathDiagramNodes { get; set; }
        public List<PathDiagramLink> PathDiagramLinks { get; set; }
    }
}
