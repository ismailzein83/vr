using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Business;

namespace CP.WhS.Business
{
    public class PortalConnectionManager
    {
        VRConnectionManager _connectionManager = new VRConnectionManager();
        static Guid WhSConnectionId = new Guid("B8058F6A-6545-465A-9DCA-6A4157ECFECB");
        public VRInterAppRestConnection GetWhSConnectionSettings()
        {
            var vrConnection = _connectionManager.GetVRConnection<VRInterAppRestConnection>(WhSConnectionId);
            return vrConnection.Settings as VRInterAppRestConnection;
        }

        public VRInterAppRestConnection GetConnectionSettings(Guid connectionId)
        {
            var vrConnection = _connectionManager.GetVRConnection<VRInterAppRestConnection>(connectionId);
            return vrConnection.Settings as VRInterAppRestConnection;
        }

        public Guid GetWhSConnectionId()
        {
            return WhSConnectionId;
        }

    }
}
