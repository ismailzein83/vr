using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Queueing.Entities
{
    public class QueueSettings
    {
        public bool SingleConcurrentReader { get; set; }

        public string QueueActivatorFQTN { get; set; }
    }    
}
