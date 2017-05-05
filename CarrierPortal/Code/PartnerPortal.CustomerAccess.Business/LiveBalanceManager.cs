using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.AccountBalance.Entities;
using Vanrise.Common.Business;
using Vanrise.Security.Business;

namespace PartnerPortal.CustomerAccess.Business
{
    public class LiveBalanceManager
    {
        public LiveBalance GetCurrentAccountBalance(Guid connectionId, Guid accountTypeId)
        {
            int userId = SecurityContext.Current.GetLoggedInUserId();
            RetailAccountUserManager manager = new RetailAccountUserManager();
            string accountId = manager.GetRetailAccountId(userId).ToString();
          
            VRConnectionManager connectionManager = new VRConnectionManager();
            var vrConnection = connectionManager.GetVRConnection<VRInterAppRestConnection>(connectionId);
            VRInterAppRestConnection connectionSettings = vrConnection.Settings as VRInterAppRestConnection;

            return connectionSettings.Get<LiveBalance>(string.Format("/api/VR_AccountBalance/LiveBalance/GetCurrentAccountBalance?accountId={0}&accountTypeId={1}", accountId, accountTypeId));
        }
    }
}
