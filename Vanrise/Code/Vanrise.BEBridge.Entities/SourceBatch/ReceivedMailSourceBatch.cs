using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Business;
using Vanrise.Entities;

namespace Vanrise.BEBridge.Entities
{
    public class ReceivedMailSourceBatch : SourceBEBatch
    {
        public override string BatchName
        {
            get { return "ReceivedMessage"; }
        }
        public List<VRReceivedMailMessage> Messages { get; set; }
    }
}
