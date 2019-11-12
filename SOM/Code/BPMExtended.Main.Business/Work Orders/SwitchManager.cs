using BPMExtended.Main.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMExtended.Main.Business
{
    public class SwitchManager : WorkOrdersManager
    {
        public List<ServiceItem> GetNetworkServices (string servicesList)
        {
            List<ServiceItem> items = new List<ServiceItem>();
            NetworkServices services = JsonConvert.DeserializeObject<NetworkServices>(servicesList);
            if (services != null && services.ListNetworkServices != null)
            {
                items = services.ListNetworkServices;
            }
            return items;
        }
        public List<ServiceItem> GetNetworkServicesWithoutProvisioning(string servicesList)
        {
            List<ServiceItem> items = new List<ServiceItem>();
            NetworkServices services = JsonConvert.DeserializeObject<NetworkServices>(servicesList);
            if(services!=null && services.ListNetworkServicesWithoutProvisioning!=null)
            {
                items = services.ListNetworkServicesWithoutProvisioning;
            }
            return items;
        }
        public class NetworkServices
        {
            public List<ServiceItem> ListNetworkServices { get; set; }
            public List<ServiceItem> ListNetworkServicesWithoutProvisioning { get; set; }
        }
    }
}
