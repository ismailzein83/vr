using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Business;

namespace CP.WhS.Business
{
    public class WhSConnectionManager
    {
        public VRInterAppRestConnection GetWhSConnectionSettings()
        {
            VRConnectionManager connectionManager = new VRConnectionManager();
            var vrConnection = connectionManager.GetVRConnection<VRInterAppRestConnection>(new Guid("B8058F6A-6545-465A-9DCA-6A4157ECFECB"));
            return vrConnection.Settings as VRInterAppRestConnection;
        }
    }
}
