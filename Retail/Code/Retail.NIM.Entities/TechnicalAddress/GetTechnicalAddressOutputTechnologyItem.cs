using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.NIM.Entities
{
    public class GetTechnicalAddressOutputTechnologyItem
    {
        public Guid Technology { get; set; }
        public bool TechnologyAvailable { get; set; }
        public bool SubscriptionFeasible { get; set; }
        public bool TelephonyFeasible { get; set; }
        public bool DataFeasible { get; set; }
        public string PanelNumber { get; set; }
        public List<GetTechnicalAddressOutputTechnologyItemNetworkElement> NetworkElements { get; set; }
        public List<GetTechnicalAddressOutputTechnologyItemConnection> Connections { get; set; }
    }
    public class GetTechnicalAddressOutputTechnologyItemConnection
    {
        public long ConnectionId { get; set; }
        public long Port1Id { get; set; }
        public long Port2Id { get; set; }
    }
}
