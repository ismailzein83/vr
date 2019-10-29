using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.NIM.Entities
{
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
}
