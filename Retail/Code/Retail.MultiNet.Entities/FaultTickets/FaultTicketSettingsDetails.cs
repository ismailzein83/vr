using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.MultiNet.Entities
{
    public class FaultTicketSettingsDetailsCollection : List<FaultTicketDescriptionSettingDetails>
    {

    }
    public class FaultTicketSettingsDetails
    {
        public List<FaultTicketDescriptionSettingDetails> DescriptionSettings { get; set; }
    }
    public class FaultTicketDescriptionSettingDetails
    {
        public Guid TicketReasonId { get; set; }
        public string TicketReasonDescription { get; set; }
        public string Type { get; set; }
        public string Note { get; set; }
    }
}
