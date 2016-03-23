using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities.SummaryTransformation;

namespace Vanrise.Queueing.Entities
{
    public class QueueSettings
    {
        public int? MaximumConcurrentReaders { get; set; }

        public QueueActivator Activator { get; set; }
    }
}
