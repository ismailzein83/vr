using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SIPVoipSDK;

namespace CallGeneratorServiceCLI
{
    public class SIP
    {
        public CAbtoPhone phone { get; set; }
        public int SipId { get; set; }
        public int ConfigId { get; set; }
    }
}
