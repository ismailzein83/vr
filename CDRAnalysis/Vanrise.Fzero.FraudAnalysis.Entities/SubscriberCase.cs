using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Fzero.FraudAnalysis.Entities
{
    public class SubscriberCase
    {
        public string SubscriberNumber { get; set; }

        public int StatusID { get; set; }

        public DateTime? ValidTill { get; set; }

    }
}
