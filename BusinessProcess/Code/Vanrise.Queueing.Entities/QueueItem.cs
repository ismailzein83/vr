using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Queueing.Entities
{
    public class QueueItem
    {
        public Guid ItemId { get; set; }
        
        public byte[] Content { get; set; }
    }
}