using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.BusinessProcess.Entities
{
    public class BPInstanceUpdateOutput
    {
        public List<BPInstanceDetail> ListBPInstanceDetails { get; set; }
        public byte[] MaxTimeStamp { get; set; }
    }
}
