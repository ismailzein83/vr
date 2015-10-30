using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Queueing.Entities;

namespace TOne.WhS.CDRProcessing.Entities
{
    public class CDR 
    {

        static CDR()
        {
            Vanrise.Common.ProtoBufSerializer.AddSerializableType(typeof(CDR), "ID", "Attempt", "IN_Carrier", "IN_Trunk", "CDPN", "OUT_Carrier", "OUT_Trunk");
        }
        public DateTime Attempt { get; set; }
        public int ID { get; set; }
        public string IN_Carrier { get; set; }
        public string IN_Trunk { get; set; }

        public string OUT_Carrier { get; set; }
        public string OUT_Trunk { get; set; }
        public string CDPN { get; set; }
    }
}
