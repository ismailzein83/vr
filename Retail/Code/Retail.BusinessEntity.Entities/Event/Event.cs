using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Entities
{
    public class Event
    {

        public int EventId { get; set; }
        public EventSetting Settings { get; set; }
    }

    public class EventSetting
    {
        public int EventIdMvno { get; set; }
        public string Status { get; set; }
        public string MSISDN { get; set; }
        public List<object> Parameters { get; set; }
    }
}
