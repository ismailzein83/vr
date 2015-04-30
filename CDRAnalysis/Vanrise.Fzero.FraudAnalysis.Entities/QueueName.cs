using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Fzero.FraudAnalysis.Entities
{
    public class QueueNameAttribute : Attribute
    {

        public string QueueTitle { get; set; }
    }

    public enum QueueName
    {
        [QueueName(QueueTitle = "Profile Numbers From Data Source {0}")]
        ProfileNumbers,
        [QueueName(QueueTitle = "Criteria Values From Data Source {0}")]
        CriteriaValues,
        [QueueName(QueueTitle = "Suspicisous Numbers From Data Source {0}")]
        SuspiciousNumbers
    }

}
