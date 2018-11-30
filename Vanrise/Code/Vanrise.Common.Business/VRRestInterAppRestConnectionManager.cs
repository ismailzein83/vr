using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Common.Business
{
	public class VRRestInterAppRestConnectionManager
	{
		#region Public Methods
		public static VRInterAppRestConnection GetVRInterAppRestConnection(Guid connectionId)
		{
			VRConnectionManager connectionManager = new VRConnectionManager();
			var vrConnection = connectionManager.GetVRConnection<VRInterAppRestConnection>(connectionId);
			return vrConnection.Settings as VRInterAppRestConnection;
		}
		#endregion

	}

}
