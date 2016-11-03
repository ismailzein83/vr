using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Business
{
    public class TelestSwitchManager
    {
        public object GetDomains()
        {
            RequestManager manager = new RequestManager();
            string url = string.Format("https://c5-iot2-prov.teles.de/SIPManagement/rest/v1/domain/vr.rest.ws.de/sub");
            string data = manager.GetRequest(url,null);
            return data;
        }
        public object GetGateWays(string domain)
        {
            RequestManager manager = new RequestManager();
            string url = string.Format("https://c5-iot2-prov.teles.de/SIPManagement/rest/v1/domain/{0}/gateway", domain);
            string data = manager.GetRequest(url, null);
            return data;
        }
    }
}
