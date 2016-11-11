using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NP.IVSwitch.Entities
{
    public class CodecDefInfo
    {
        public int CodecId { get; set; }
        public string FsName { get; set; }
        public string Description { get; set; }
        public int ClockRate { get; set; }
        public int DefaultMsPerPacket { get; set; }
        public bool PassThru { get; set; }


    }
}
