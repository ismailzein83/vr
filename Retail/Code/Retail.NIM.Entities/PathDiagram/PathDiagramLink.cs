using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.NIM.Entities
{
    public class PathDiagramLink
    {
        public long LinkId { get; set; }
        public string SourcePortId { get; set; }
        public string DestinationPortId { get; set; }
    }
}
