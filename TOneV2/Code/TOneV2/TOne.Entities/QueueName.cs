using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.Entities
{    
    public enum QueueType { CDR}
    public class QueueNameAttribute : Attribute
    {
        public QueueType QueueType { get; set; }

        public string QueueTitle { get; set; }
    }

    public enum QueueName
    {
        [QueueName(QueueTitle = "CDR Raw From Switch {0}", QueueType = QueueType.CDR)]
        CDRRaw,
        [QueueName(QueueTitle = "CDR Raw for Billing From Switch {0}", QueueType = QueueType.CDR)]
        CDRRawForBilling,
        [QueueName(QueueTitle = "CDR Billing From Switch {0}", QueueType = QueueType.CDR)]
        CDRBilling,
        [QueueName(QueueTitle = "CDR Billing for Statistics From Switch {0}", QueueType = QueueType.CDR)]
        CDRBillingForStats,
        [QueueName(QueueTitle = "CDR Billing for Statistics Daily From Switch {0}", QueueType = QueueType.CDR)]
        CDRBillingForStatsDaily,
        [QueueName(QueueTitle = "CDR Billing Main From Switch {0}", QueueType = QueueType.CDR)]
        CDRMain,
        [QueueName(QueueTitle = "CDR Invalid From Switch {0}", QueueType = QueueType.CDR)]
        CDRInvalid,
        [QueueName(QueueTitle = "Traffic Statistcs From Switch {0}", QueueType = QueueType.CDR)]
        TrafficStats,
        [QueueName(QueueTitle = "Traffic Statistcs Daily From Switch {0}", QueueType = QueueType.CDR)]
        TrafficStatsDaily
    }
}
