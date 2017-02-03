using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.Ringo.Entities
{
    public class RingoMessageFilter
    {
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
        public string RecipientNetwork { get; set; }
        public string SenderNetwork { get; set; }
        public List<int> MessageTypes { get; set; }
        public List<int> StateRequests { get; set; }
        public string Sender { get; set; }
        public string Recipient { get; set; }
    }
}
