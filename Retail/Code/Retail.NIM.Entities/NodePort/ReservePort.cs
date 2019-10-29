using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.NIM.Entities
{
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
