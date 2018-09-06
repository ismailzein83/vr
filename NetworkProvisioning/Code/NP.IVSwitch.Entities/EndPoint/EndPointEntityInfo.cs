using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NP.IVSwitch.Entities 
{
    public class EndPointEntityInfo
    {

        public int EndPointId { get; set; }
        public string Description { get; set; }
        public int AccountId { get; set; }
        public Int16 CliRouting { get; set; }
        public int? DstRouting { get; set; }

    }
}
